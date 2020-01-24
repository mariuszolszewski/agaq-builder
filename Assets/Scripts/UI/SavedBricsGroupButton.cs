using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AgaQ.Bricks.Serialization;
using AgaQ.Bricks.Tools;
using System.IO;

namespace AgaQ.UI
{
    public class SavedBricsGroupButton : BaseBrickButton
    {
        protected string iconPath;
        protected string groupPath;

        /// <summary>
        /// Init component.
        /// </summary>
        /// <returns>The init.</returns>
        /// <param name="metaData">Bricks group meta data.</param>
        /// <param name="path">Path to files.</param>
        public void Init(BricksGropuMetaData metaData, string path)
        {
            PhraseName = metaData.name;
            group = metaData.category;
            iconPath = string.Concat(path, ".png");
            groupPath = string.Concat(path, ".aga");
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            ToolsManager.instance.addTool.AddSaved(groupPath, true);
        }

        public override void LoadIcon()
        {
            Texture2D texture = LoadTexture(iconPath);
            if (texture == null)
                return;

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            var image = GetComponent<Image>();
            image.sprite = sprite;
            isLoaded = true;
        }

        /// <summary>
        /// Loads the texture from png file.
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="FilePath">File path.</param>
        Texture2D LoadTexture(string FilePath)
        {
            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2);
                if (Tex2D.LoadImage(FileData))
                    return Tex2D;
            }

            return null;
        }
    }
}
