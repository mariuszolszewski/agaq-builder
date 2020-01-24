using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;

namespace AgaQ.Bricks.Serialization.STL
{
    /// <summary>
    /// Import methods for STL files.
    /// </summary>
    public static class STLImporter
	{
		const int MAX_FACETS_PER_MESH = 65535 / 3;
        const int SOLID = 1;
        const int FACET = 2;
        const int OUTER = 3;
        const int VERTEX = 4;
        const int ENDLOOP = 5;
        const int ENDFACET = 6;
        const int ENDSOLID = 7;
        const int EMPTY = 0;

        /// <summary>
        ///Import an STL file at path.
        /// </summary>
        /// <param name="path">Path.</param>
		public static Mesh[] Import(string path)
		{
			if( IsBinary(path) )
			{
				try
				{
					return ImportBinary(path);
				}
				catch(System.Exception e)
				{
					UnityEngine.Debug.LogWarning(string.Format("Failed importing mesh at path {0}.\n{1}", path, e.ToString()));
					return null;
				}
			}
			else
			{
				return ImportAscii(path);
			}
		}

        #region Private functions

		static Mesh[] ImportBinary(string path)
		{
			Facet[] facets;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs, new ASCIIEncoding()))
            {
                // read header
                byte[] header = br.ReadBytes(80);
                uint facetCount = br.ReadUInt32();
                facets = new Facet[facetCount];

                for (uint i = 0; i < facetCount; i++)
                    facets[i] = br.GetFacet();
            }

			return CreateMeshWithFacets(facets);
		}

        static Facet GetFacet(this BinaryReader binaryReader)
        {
            Facet facet = new Facet();
            facet.normal = binaryReader.GetVector3();

            // maintain counter-clockwise orientation of vertices:
            facet.a = binaryReader.GetVector3();
            facet.c = binaryReader.GetVector3();
            facet.b = binaryReader.GetVector3();
            binaryReader.ReadUInt16(); // padding
          
            return facet;
        }

        static Vector3 GetVector3(this BinaryReader binaryReader)
        {
            Vector3 vector3 = new Vector3();
            for (int i = 0; i < 3; i++)
                vector3[i] = binaryReader.ReadSingle();
            return vector3.UnityCoordTrafo();
        }

        static Vector3 UnityCoordTrafo(this Vector3 vector3)
        {
            return new Vector3(-vector3.y, vector3.z, vector3.x);
        }
            
		static int ReadState(string line)
		{
			if (line.StartsWith("solid"))
				return SOLID;
			else if (line.StartsWith("facet"))
				return FACET;
			else if (line.StartsWith("outer"))
				return OUTER;
			else if (line.StartsWith("vertex"))
				return VERTEX;
			else if (line.StartsWith("endloop"))
				return ENDLOOP;
			else if (line.StartsWith("endfacet"))
				return ENDFACET;
			else if (line.StartsWith("endsolid"))
				return ENDSOLID;
			else
				return EMPTY;
		}

		static Mesh[] ImportAscii(string path)
		{
			List<Facet> facets = new List<Facet>();

			using(StreamReader sr = new StreamReader(path))
			{
				string line;
				int state = EMPTY, vertex = 0;
				Facet f = null;
				bool exit = false;

				while (sr.Peek() > 0 && !exit)
				{
					line = sr.ReadLine().Trim();
					state = ReadState(line);

					switch(state)
					{
						case SOLID:
							continue;

						case FACET:
							f = new Facet();
							f.normal = StringToVec3(line.Replace("facet normal ", ""));
                            break;

						case OUTER:
							vertex = 0;
                            break;

						case VERTEX:
                            // maintain counter-clockwise orientation of vertices:
                            if (vertex == 2)
                                f.a = StringToVec3(line.Replace("vertex ", ""));							
							else if (vertex == 0)
                                f.c = StringToVec3(line.Replace("vertex ", ""));
                            else if (vertex == 1)
                                f.b = StringToVec3(line.Replace("vertex ", ""));
                            vertex++;
                            break;

						case ENDLOOP:
                            break;

						case ENDFACET:
							facets.Add(f);
						break;

						case ENDSOLID:
							exit = true;
                            break;

						case EMPTY:
						default:
                            break;

					}
				}
			}

			return CreateMeshWithFacets(facets);
		}

		static Vector3 StringToVec3(string str)
		{
			string[] split = str.Trim().Split(null);

            float x, y, z;
            float.TryParse(split[0], NumberStyles.Float, CultureInfo.InvariantCulture,  out x);
            float.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture,  out y);
            float.TryParse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture,  out z);
            Vector3 v = new Vector3(x, y, z);

            return v.UnityCoordTrafo(); 
		}

        /// <summary>
        /// Read the first 80 bytes of a file and if they are all 0x0 it's likely that this file is binary.
        /// </summary>
        /// <returns><c>true</c> if is binary the specified path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Path.</param>
		static bool IsBinary(string path)
		{
			FileInfo file = new FileInfo(path);

			if (file.Length < 130)
				return false;

			var isBinary = false;

			using (FileStream f0 = file.OpenRead())
			using (BufferedStream bs0 = new BufferedStream(f0))
			{
				for (long i = 0; i < 80; i++)
				{
				    var readByte = bs0.ReadByte();
				    if (readByte == 0x0)
				    {
				        isBinary = true;
				        break;
				    }
				}
			}

            if (!isBinary)
            {
                using (FileStream f0 = file.OpenRead())
                using (BufferedStream bs0 = new BufferedStream(f0))
                {
                    var byteArray = new byte[6];

                    for (var i = 0; i < 6; i++)
                        byteArray[i] = (byte)bs0.ReadByte();

                    var text = Encoding.UTF8.GetString(byteArray);
                    isBinary = text != "solid ";

                    if (!isBinary)
                    {
                        //read some extra string and try to search "facet" text.
                        byteArray = new byte[2000];

                        for (var i = 0; i < 72; i++)
                            byteArray[i] = (byte)bs0.ReadByte();

                        for (var i = 0; i < 2000; i++)
                            byteArray[i] = (byte)bs0.ReadByte();

                        text = Encoding.UTF8.GetString(byteArray);

                        if (!text.Contains("facet") && !text.Contains("FACET"))
                            isBinary = true;
                    }
                }
            }

			return isBinary;
		}

		static Mesh[] CreateMeshWithFacets(IList<Facet> facets)
		{
			int fl = facets.Count;
            int f = 0;
            int mvc = MAX_FACETS_PER_MESH * 3;
			Mesh[] meshes = new Mesh[fl / MAX_FACETS_PER_MESH + 1];

			for (int i = 0; i < meshes.Length; i++)
			{
				int len = System.Math.Min(mvc, (fl - f) * 3);
				Vector3[] v = new Vector3[len];
				Vector3[] n = new Vector3[len];
				int[] t = new int[len];

				for (int it = 0; it < len; it += 3)
				{
					v[it    ] = facets[f].a;
					v[it + 1] = facets[f].b;
					v[it + 2] = facets[f].c;

                    n[it    ] = facets[f].normal;
                    n[it + 1] = facets[f].normal;
                    n[it + 2] = facets[f].normal;

					t[it    ] = it;
					t[it + 1] = it + 1;
					t[it + 2] = it + 2;

					f++;
				}

				meshes[i] = new Mesh();
				meshes[i].vertices = v;
                meshes[i].triangles = t;
				meshes[i].normals = n;
			}

			return meshes;
		}

        #endregion
	}
}
