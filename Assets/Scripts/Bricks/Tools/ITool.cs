using UnityEngine.EventSystems;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Inteface for all brick tools.
    /// </summary>
    public interface ITool
    {
        void Start();
        void OnEneter(HighlightableBrick brick);
        void OnExit(HighlightableBrick brick);
        void OnClick(SelectableBrick brick, PointerEventData pointerEventData);
        void OnPointerDown(SelectableBrick brick, PointerEventData pointerEventData);
        void OnPointerUp(SelectableBrick brick, PointerEventData pointerEventData);
        bool OnBeginDrag(DragableBrick brick, PointerEventData eventData);
        void OnDrag(DragableBrick brick, PointerEventData eventData);
        bool OnEndDrag(DragableBrick brick, PointerEventData eventData);
        void OnUpdate();
        void OnCancel();
    }
}
