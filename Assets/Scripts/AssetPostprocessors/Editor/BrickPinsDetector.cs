using UnityEngine;
using System.Collections.Generic;

namespace AssetPostprocessors
{
    /// <summary>
    /// Detect male pins in brick model.
    /// </summary>
    public class BricksPinDetector : IDetector
    {
        float precision = 0.0001f;
        Vector3[] vertises;
        int[] triangles;
        List<Tris> trises = new List<Tris>();

        /// <summary>
        /// Detect pins in AgaQ brick.
        /// </summary>
        /// <param name="mesh">Mesh.</param>
        public Pin[] DetectPins(Mesh mesh)
        {
            vertises = mesh.vertices;
            triangles = mesh.triangles;
            List<Pin> pins = new List<Pin>();

            //iterate over vertises
            for (var v = 0; v < vertises.Length; v++)
            {
                FindIsoscelesTriangles(trises, v);
                if (AreTrisesCircle(trises))
                {
                    //get pin position, and rotation

                    pins.Add(new Pin
                    {
                        position = vertises[triangles[trises[0].trisIndex + trises[0].refVertIndex]]
                    });
                }
            }

            // free memory
            vertises = null;
            triangles = null;
            trises.Clear();

            return pins.ToArray();
        }


        /// <summary>
        /// Check if trises are circle.
        /// Used is simplifyied method adatped to agaq brick pin. Don't check if trises are on one plane.
        /// Check if there are minnimum 4 trises with tha same arm length.
        /// </summary>
        /// <returns><c>true</c>, if trises circle was ared, <c>false</c> otherwise.</returns>
        /// <param name="trises">Trises.</param>
        bool AreTrisesCircle(List<Tris> trises)
        {
            if (trises.Count < 4)
                return false;

            var referenceLenght = trises[0].trisEgeLenght;
            for (var i = 1; i < trises.Count; i++)
            {
                if (Mathf.Abs(trises[i].trisEgeLenght - referenceLenght) > precision)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Find all isosceles tringles containg given vertex
        /// </summary>
        /// <param name="trises">Trises.</param>
        /// <param name="vertexIndex">Vertex index.</param>
        void FindIsoscelesTriangles(List<Tris> trises, int vertexIndex)
        {
            trises.Clear();

            //iterate over triangles searching triangles with current vertex
            for (var t = 0; t < triangles.Length; t += 3)
            {
                float armLength = 0;

                if (triangles[t] == vertexIndex)
                {
                    armLength = CheckTris(vertises[triangles[t]], vertises[triangles[t + 1]], vertises[triangles[t + 2]]);
                    if (armLength > 0)
                        trises.Add(new Tris { trisIndex = t, refVertIndex = 0, trisEgeLenght = armLength });
                }
                else if (triangles[t + 1] == vertexIndex)
                {
                    armLength = CheckTris(vertises[triangles[t + 1]], vertises[triangles[t]], vertises[triangles[t + 2]]);
                    if (armLength > 0)
                        trises.Add(new Tris { trisIndex = t, refVertIndex = 1, trisEgeLenght = armLength });
                }
                else if (triangles[t + 2] == vertexIndex)
                {
                    armLength = CheckTris(vertises[triangles[t + 2]], vertises[triangles[t + 1]], vertises[triangles[t]]);
                    if (armLength > 0)
                        trises.Add(new Tris { trisIndex = t, refVertIndex = 2, trisEgeLenght = armLength });
                }
            }
        }

        /// <summary>
        /// Calcualte Tris edges to detect if this is isosceles triangle.
        /// </summary>
        /// <returns>Triangle arm length</returns>
        /// <param name="firstVertex">First vertex.</param>
        /// <param name="secondVertex">Second vertex.</param>
        /// <param name="thirdVertex">Third vertex.</param>
        float CheckTris(Vector3 firstVertex, Vector3 secondVertex, Vector3 thirdVertex)
        {
            float firstEdgeLenght = (firstVertex - secondVertex).magnitude;
            float secondEdgeLenght = (firstVertex - thirdVertex).magnitude;

            return Mathf.Abs(firstEdgeLenght - secondEdgeLenght) < precision ? firstEdgeLenght : 0;
        }

        struct Tris
        {
            public int trisIndex; //triangle index
            public int refVertIndex; //refenece vertex intex in triangle (0, 1 , 2)
            public float trisEgeLenght; //lenght of edges started from reference vertex
        }
    }
}