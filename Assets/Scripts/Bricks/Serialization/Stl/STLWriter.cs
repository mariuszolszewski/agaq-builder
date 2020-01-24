using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AgaQ.Bricks.Serialization.STL
{
    /// <summary>
    /// Export STL files from Unity mesh assets.
    /// </summary>
    public static class STLWriter
    {
        #region Public functions

        /// <summary>
        /// Write a mesh file to STL.
        /// </summary>
        /// <returns><c>true</c>, if file was writed, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        /// <param name="mesh">Mesh.</param>
        /// <param name="type">Type.</param>
        /// <param name="rightHandedCoordinates">If true convert to right handed coordinates.</param>
        public static bool WriteFile(string path, Mesh mesh, FileType type = FileType.Ascii, bool rightHandedCoordinates = true)
        {
            return WriteFile(path, new Mesh[] { mesh }, type, rightHandedCoordinates);
        }

        /// <summary>
        /// Write a collection of mesh assets to an STL file.
        /// No transformations are performed on meshes in this method.
        /// Eg, if you want to export a set of a meshes in a transform
        /// hierarchy the meshes should be transformed prior to this call.
        /// </summary>
        /// <returns><c>true</c>, if file was writed, <c>false</c> otherwise.</returns>
        /// <param name="path">Where to write the file</param>
        /// <param name="meshes">The mesh assets to write</param>
        /// <param name="type">How to format the file (in ASCII or binary)</param>
        /// <param name="rightHandedCoordinates">If true convert to right handled coordinates</param>
        public static bool WriteFile(string path, IList<Mesh> meshes, FileType type = FileType.Ascii, bool rightHandedCoordinates = true)
        {
            try
            {
                if (type == FileType.Binary)
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create), new ASCIIEncoding()))
                    {
                        // 80 byte header
                        writer.Write(new byte[80]);

                        uint totalTriangleCount = (uint)(meshes.Sum(x => x.triangles.Length) / 3);

                        // unsigned long facet count (4 bytes)
                        writer.Write(totalTriangleCount);

                        foreach (Mesh mesh in meshes)
                            WriteMesh(writer, mesh, rightHandedCoordinates);
                    }
                }
                else
                {
                    string model = WriteString(meshes);
                    File.WriteAllText(path, model);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write a Unity mesh to an ASCII STL string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="mesh">Mesh.</param>
        /// <param name="rightHandedCoordinates">If true convert to right handed coordinates.</param>
        public static string WriteString(Mesh mesh, bool rightHandedCoordinates = true)
        {
            return WriteString(new Mesh[] { mesh }, rightHandedCoordinates);
        }

        /// <summary>
        /// Write a set of meshes to an ASCII string in STL format.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="meshes">Meshes.</param>
        /// <param name="convertToRightHandedCoordinates">If set to <c>true</c> convert to right handed coordinates.</param>
        public static string WriteString(IList<Mesh> meshes, bool convertToRightHandedCoordinates = true)
        {
            StringBuilder sb = new StringBuilder();

            string name = meshes.Count == 1 ? meshes[0].name : "Composite Mesh";

            sb.AppendLine(string.Format("solid {0}", name));

            foreach (Mesh mesh in meshes)
            {
                Vector3[] v = convertToRightHandedCoordinates ? Left2Right(mesh.vertices) : mesh.vertices;
                Vector3[] n = convertToRightHandedCoordinates ? Left2Right(mesh.normals) : mesh.normals;
                int[] t = mesh.triangles;
                if (convertToRightHandedCoordinates)
                    System.Array.Reverse(t);
                int triLen = t.Length;

                for (int i = 0; i < triLen; i += 3)
                {
                    int a = t[i];
                    int b = t[i + 1];
                    int c = t[i + 2];

                    Vector3 nrm = AverageNorm(n[a], n[b], n[c]);

                    sb.AppendLine(string.Format("facet normal {0} {1} {2}", nrm.x, nrm.y, nrm.z));

                    sb.AppendLine("outer loop");

                    sb.AppendLine(string.Format("\tvertex {0} {1} {2}", v[a].x, v[a].y, v[a].z));
                    sb.AppendLine(string.Format("\tvertex {0} {1} {2}", v[b].x, v[b].y, v[b].z));
                    sb.AppendLine(string.Format("\tvertex {0} {1} {2}", v[c].x, v[c].y, v[c].z));

                    sb.AppendLine("endloop");

                    sb.AppendLine("endfacet");
                }
            }

            sb.AppendLine(string.Format("endsolid {0}", name));

            return sb.ToString();
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Write single mesh to file.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <param name="mesh">Mesh.</param>
        /// <param name="convertToRightHandedCoordinates">If set to <c>true</c> convert to right handed coordinates.</param>
        static void WriteMesh(BinaryWriter writer, Mesh mesh,  bool convertToRightHandedCoordinates = true)
        {
            Vector3[] v = convertToRightHandedCoordinates ? Left2Right(mesh.vertices) : mesh.vertices;
            Vector3[] n = convertToRightHandedCoordinates ? Left2Right(mesh.normals) : mesh.normals;
            int[] t = mesh.triangles;
            int triangleCount = t.Length;
            if (convertToRightHandedCoordinates)
                System.Array.Reverse(t);

            for (int i = 0; i < triangleCount; i += 3)
            {
                int a = t[i], b = t[i + 1], c = t[i + 2];

                Vector3 avg = AverageNorm(n[a], n[b], n[c]);

                writer.Write(avg.x);  writer.Write(avg.y);  writer.Write(avg.z);
                writer.Write(v[a].x); writer.Write(v[a].y); writer.Write(v[a].z);
                writer.Write(v[b].x); writer.Write(v[b].y); writer.Write(v[b].z);
                writer.Write(v[c].x); writer.Write(v[c].y); writer.Write(v[c].z);
                //writer.Write(v[a].x * 20); writer.Write(v[a].z * 20); writer.Write(v[a].y * 20);
                //writer.Write(v[b].x * 20); writer.Write(v[b].z * 20); writer.Write(v[b].y * 20);
                //writer.Write(v[c].x * 20); writer.Write(v[c].z * 20); writer.Write(v[c].y * 20);

                // specification says attribute byte count should be set to 0.
                writer.Write((ushort)0);
            }            
        }

        static Vector3[] Left2Right(Vector3[] v)
        {
            Matrix4x4 l2r = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, 1f, -1f));
            Vector3[] r = new Vector3[v.Length];

            for (int i = 0; i < v.Length; i++)
                r[i] = l2r.MultiplyPoint3x4(v[i]);

            return r;
        }

        /// <summary>
        /// Average of 3 vectors.
        /// </summary>
        /// <returns>The norm.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        static Vector3 AverageNorm(Vector3 a, Vector3 b, Vector3 c)
        {
            return new Vector3(
                (a.x + b.x + c.x) / 3f,
                (a.y + b.y + c.y) / 3f,
                (a.z + b.z + c.z) / 3f);
        }

        #endregion
    }
}
