using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

namespace AssetPostprocessors
{
    /// <summary>
    /// Detects upper joints in AgaQ brick.
    /// </summary>
    public class BrickJointDetector : IDetector
    {
        float precision = 0.0001f;
        Vector3[] vertises;
        int[] triangles;

        public Pin[] DetectPins(Mesh mesh)
        {
            List<Pin> pins = new List<Pin>();

            vertises = mesh.vertices;
            triangles = mesh.triangles;

            int[] rectangularTriangles = FindRectangularTriangles();
			int[] rectangularTrises = FindRectangualrTrisConnections (rectangularTriangles);
			int[] cuboids = FindCuboids (rectangularTrises);

            return pins.ToArray();
        }

        /// <summary>
        /// Find all rectangular triangles
        /// </summary>
        /// <returns>The rectangular triangles. Array structer: vertex1, vertex2, vertex3, rectangualrVertex</returns>
        int[] FindRectangularTriangles()
        {
            List<int> rectagularTriangles = new List<int>();

            //interate over all triangles
            for (int i = 0; i < triangles.Length; i += 3)
            {
				//triangle vetice 0 test
                Vector3 vector1 = vertises[triangles[i + 1]] - vertises[triangles[i]];
                Vector3 vector2 = vertises[triangles[i + 2]] - vertises[triangles[i]];
                float angle1 = Vector3.Angle(vector1, vector2);
                if (Mathf.Abs(angle1 - 90) < precision)
                    AddTrianglToList(rectagularTriangles, i, i + 1);
                else
                {
					//triangle vertis 1 test
					vector1 = vertises[triangles[i]] - vertises[triangles[i + 1]];
					vector2 = vertises[triangles[i + 2]] - vertises[triangles[i + 1]];
                    float angle2 = Vector3.Angle(vector1, vector2);
                    if (Mathf.Abs(angle2 - 90) < precision)
                        AddTrianglToList(rectagularTriangles, i, i + 2);
                    else
                    {
						float angle3 = 180f - angle1 - angle2;
                        if (Mathf.Abs(angle3 - 90) < precision)
                            AddTrianglToList(rectagularTriangles, i, i);
                    }
                }
            }

            return rectagularTriangles.ToArray();
        }

        /// <summary>
        /// Add triangle to list.
        /// </summary>
        /// <param name="list">List.</param>
        /// <param name="triangle">Triangle.</param>
        /// <param name="rectangularAngle">Index of vertext with rectangular angle.</param>
        void AddTrianglToList(List<int> list, int triangle, int rectangularAngle)
        {
            list.Add(triangles[triangle]);
            list.Add(triangles[triangle + 1]);
            list.Add(triangles[triangle + 2]);
            list.Add(rectangularAngle);
        }

        /// <summary>
        /// Find pairs of triangles that has rectangular angle between.
        /// <param name="rectangularTriangles">array of rectangular triangles</param>
        /// <returns>array of pairs of triangles, structure: first triangle index, second triangle index, first common point index, second common point index</returns>
        /// </summary>
        int[] FindRectangualrTrisConnections(int[] rectangularTriangles)
        {
            List<int> cornersTriangles = new List<int>();

            //iterate over all rectangular triangles
            for (int i = 0; i < rectangularTriangles.Length; i += 4)
            {
                //iterate over all latest triangles (start with next to i)
                for (int j = i + 4; j < rectangularTriangles.Length; j += 4)
                {
                    int commonVertexIdx = -1;
                    int[] commonVertexs = new int[2];

                    //check common points
                    for (int k = 0; k < 3; k++)
                    {
                        if (CompareVertexes(rectangularTriangles, j, j, 0, k) || 
                            CompareVertexes(rectangularTriangles, i, j, 1, k) ||
                            CompareVertexes(rectangularTriangles, i, j, 2, k))
                        {
                            commonVertexs[++commonVertexIdx] = rectangularTriangles[j];
                        }
                    }

                    //if don't have two common points continue searching
                    if (commonVertexIdx < 1)
                        continue;

                    //chck if triangles makes rectangle angle
                    Vector3 v1 = vertises[commonVertexs[0]];
                    for (int k = 0; k < 3; k++)
                    {
                        if (rectangularTriangles[i + k] != commonVertexs[0] &&
                            rectangularTriangles[i + k] != commonVertexs[1])
                        {
                            v1 -= vertises[rectangularTriangles[i + k]];
                            break;
                        }
                    }
                    Vector3 v2 = vertises[commonVertexs[1]];
                    for (int k = 0; k < 3; k++)
                    {
                        if (rectangularTriangles[j + k] != commonVertexs[0] &&
                            rectangularTriangles[j + k] != commonVertexs[1])
                        {
                            v2 -= vertises[rectangularTriangles[j + k]];
                            break;
                        }
                    }
                    if (Mathf.Abs(Vector3.Angle(v1, v2) - 90f) >= precision)
                        continue;

                    //finanly we can queue our triangles pair for future processing
                    cornersTriangles.Add(rectangularTriangles[j]);
                    cornersTriangles.Add(rectangularTriangles[j + 1]);
                    cornersTriangles.Add(rectangularTriangles[j + 2]);
                    cornersTriangles.Add(rectangularTriangles[i]);
                    cornersTriangles.Add(rectangularTriangles[i + 1]);
                    cornersTriangles.Add(rectangularTriangles[i + 2]);
                }
            }

            return cornersTriangles.ToArray();
        }

		int[] FindCuboids(int[] rectangularTrises)
		{
			List<int> cuboids = new List<int> ();

			return cuboids.ToArray ();
		}

        /// <summary>
        /// Comapre triangle vertex. Check if points are common and has rectangular angle,
        /// but recatangular angle has to be in different points
        /// </summary>
        /// <returns><c>true</c>, if vertexes was compared, <c>false</c> otherwise.</returns>
        /// <param name="triangles">Triangles.</param>
        /// <param name="tidx1">index of first triangle</param>
        /// <param name="tidx2">index of second triangle</param>
        /// <param name="idx1">ponit index shift in fist triangle</param>
        /// <param name="idx2">point index shoft in second triangle</param>
        bool CompareVertexes(int[] triangles, int tidx1, int tidx2, int idx1, int idx2)
        {
            return
                (triangles[tidx1 + idx1] == triangles[tidx2 + idx2]) &&
                (triangles[tidx1 + idx1] == triangles[tidx1 + 3] || triangles[tidx2 + idx2] == triangles[tidx2 + 3]) &&
                (triangles[tidx1 + 3] != triangles[tidx2 + 3]);
        }
    }
}
