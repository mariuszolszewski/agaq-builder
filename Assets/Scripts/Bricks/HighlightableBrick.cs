using UnityEngine;
using UnityEngine.EventSystems;
using AgaQ.Bricks.Tools;

namespace AgaQ.Bricks
{
    /// <summary>
    /// Brick that can be highligted by moving cursor into it.
    /// Class also handles visual effect of higligting
    /// </summary>
    public abstract class HighlightableBrick : Brick, IPointerEnterHandler, IPointerExitHandler
    {
        public bool highlighted {
            get;
            private set;
        }

        BoundBox _boundBox;
        BoundBox boundBox
        {
            get
            {
                if (_boundBox == null)
                {
                    _boundBox = GetComponent<BoundBox>();
                    if (_boundBox == null)
                        _boundBox = gameObject.AddComponent<BoundBox>();
                }

                return _boundBox;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (grouped)
                return;

            var tool = ToolsManager.instance.tool;
            if ((tool is MoveTool && !((MoveTool)tool).isDragging) ||
                tool is ColorTool || tool is MeasureTool)
            {
                SetHighlighted(true);
                ToolsManager.instance.tool.OnEneter(this);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (grouped)
                return;

            SetHighlighted(false);
            ToolsManager.instance.tool.OnExit(this);
        }

        /// <summary>
        /// Set brick highlighted.
        /// </summary>
        /// <param name="highlight">If set to <c>true</c> highlight.</param>
        public virtual void SetHighlighted(bool highlight)
        {
            highlighted = highlight;
            boundBox.enabled = highlighted;
        }
    }
}
