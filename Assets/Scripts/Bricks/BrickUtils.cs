using UnityEngine;
using AgaQ.Bricks.Utils;

namespace AgaQ.Bricks
{
    /// <summary>
    /// Some usefull utils for bricks.
    /// </summary>
    public class BrickUtils
    {
        /// <summary>
        /// Generate icon image for brick using MeshProjector.
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="filePath">File path.</param>
        /// <param name="size">Image size.</param>
        public static void GenerateIcon(Brick brick, string filePath, int size)
        {
            //generate texture
            MeshProjector projector = new MeshProjector(new Vector3(-1000, 0, 0), 31);
            var texture = projector.Project(brick, size);

            //convert texture
            RenderTexture.active = texture;
            Texture2D texture2D = new Texture2D(texture.width, texture.height);
            texture2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            RenderTexture.active = null;

            //save texture as png image
            var image = ImageConversion.EncodeToPNG(texture2D);
            System.IO.File.WriteAllBytes(filePath, image);
        }
    }
}
