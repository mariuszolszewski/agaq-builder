using System;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Base tool for all tools
    /// </summary>
    public abstract class BaseTool : ITool
    {
        public Action OnModelChange;

        public virtual void Start() {}
        public virtual void OnEneter(HighlightableBrick brick) {}
        public virtual void OnExit(HighlightableBrick brick) {}
        public virtual void OnClick(SelectableBrick brick, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
        public virtual void OnPointerDown(SelectableBrick brick, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
        public virtual void OnPointerUp(SelectableBrick brick, UnityEngine.EventSystems.PointerEventData pointerEventData) {}
        public virtual void OnDrag(DragableBrick brick, UnityEngine.EventSystems.PointerEventData eventData) {}
        public virtual void OnUpdate() {}
        public virtual void OnCancel() {}

        public virtual bool OnBeginDrag(DragableBrick brick, UnityEngine.EventSystems.PointerEventData eventData)
        {
            return true;
        }

        public virtual bool OnEndDrag(DragableBrick brick, UnityEngine.EventSystems.PointerEventData eventData)
        {
            return true;
        }
    }
}
