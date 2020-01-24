using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AgaQ.Bricks.Tools;

namespace AgaQ.UI
{
    /// <summary>
    /// Button in editor space where you can drop brick.
    /// </summary>
    public class DropDownButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] Color normalColor = new Color(1f, 1f, 1f, 0.39f);
        [SerializeField] Color highlightedColor = new Color(1f, 1f, 1f, 0.78f);

        Image image;

        void Start()
        {
            image = GetComponent<Image>();
        }
            
        public void OnPointerEnter(PointerEventData eventData)
        {
            var tool = ToolsManager.instance.tool;

            if (image != null && tool is DeleteTool && (tool as DeleteTool).isDragging)
                image.color = highlightedColor;
        }
            
        public void OnPointerExit(PointerEventData eventData)
        {
            if (image != null)
                image.color = normalColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var tool = ToolsManager.instance.tool;

            if (tool is DeleteTool && (tool as DeleteTool).isDragging)
                (tool as DeleteTool).DeleteSelected();

            image.color = normalColor;
        }
    }
}
