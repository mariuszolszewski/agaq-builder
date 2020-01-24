using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AgaQ.Bricks.Tools;

namespace AgaQ.UI
{
    /// <summary>
    /// Button script to use in bricks library as button with brick icon, to select new bricks.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class BrickButton : BaseBrickButton
    {
        public string brickResourcePath; //path to brick prefab in resources directory
        public string imageResourcePath; //path to button icon in resources directory

        public override void OnPointerClick(PointerEventData eventData)
        {
            ToolsManager.instance.addTool.Add(brickResourcePath, true);
        }

        public override void LoadIcon()
        {
            var sprite = Resources.Load<Sprite>(imageResourcePath);
            var image = GetComponent<Image>();
            image.sprite = sprite;
            isLoaded = true;
        }
    }
}
