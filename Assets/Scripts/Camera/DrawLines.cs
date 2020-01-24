using UnityEngine;
using System.Collections.Generic;

namespace AgaQ.Camera
{
    /// <summary>
    /// This class provides functionality of drawing GL lines in scene
    /// </summary>
    public class DrawLines : MonoBehaviour
    {
        public Material lineMaterial;

        Dictionary<int, Camera.Line[]> lines = new Dictionary<int, Line[]>();
        int lastId = 0;

		void OnPostRender()
        {
            if (lines.Count == 0)
                return;

            lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            foreach (var linesGroup in lines)
            {
                foreach (Line line in linesGroup.Value)
                {
                    GL.Color(line.color);
                    GL.Vertex(line.vertex1);
                    GL.Vertex(line.vertex2);
                }
            }
            GL.End();
        }

        /// <summary>
        /// Add lines to draw
        /// </summary>
        /// <returns>The id that can be used in RemoveLines function.</returns>
        /// <param name="newLines">New lines array.</param>
        public int AddLines(Line[] newLines)
        {
            lastId++;
            lines.Add(lastId, newLines);

            return lastId;
        }

        /// <summary>
        /// Remove lines.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void RemoveLines(int id)
        {
            lines.Remove(id);
        }

        /// <summary>
        /// Remove all lines.
        /// </summary>
        public void CleatLines()
        {
            lines.Clear();
        }
    }
}
