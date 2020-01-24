using UnityEngine;
using UnityEngine.EventSystems;
using AgaQ.Bricks.History;
using System.Collections.Generic;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Tool to clone bricks that exist in scene.
    /// </summary>
    public class CloneTool : MoveTool
    {
        public override void OnClick(SelectableBrick brick, PointerEventData pointerEventData)
        {
            if (!isDragging)
            {
                if (SelectionManager.instance.IsSelected(brick))
                    //clone selection
                    CloneSelection(brick as DragableBrick, pointerEventData);
                else if (brick is DragableBrick)
                    //clone single brick
                    Clone(brick as DragableBrick, pointerEventData);
            }
        }

        public override bool OnBeginDrag(DragableBrick brick, PointerEventData eventData)
        {
            return false;
        }

        public override bool OnEndDrag(DragableBrick brick, PointerEventData eventData)
        {
            return base.OnEndDrag(brick, eventData);
        }

        public override void OnCancel()
        {
            if (dragableBrick == null)
                return;

            var brickTodelete = dragableBrick;
            base.OnCancel();

            Object.Destroy(brickTodelete.gameObject);
            dragableBrick = null;
        }

        /// <summary>
        /// Clone the specified brick.
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="eventData">Event data.</param>
        public void Clone(DragableBrick brick, PointerEventData eventData)
        {
            var newBrick = BrickBuilder.Clone(brick);
            if (newBrick is DragableBrick)
            {
                dragableBrick = newBrick as DragableBrick;
                base.OnBeginDrag(dragableBrick, eventData);
            }
        }

        /// <summary>
        /// Clone selected bricks.
        /// </summary>
        /// <param name="brick">Clicked brick being selected</param>
        /// <param name="eventData">Event data.</param>
        public void CloneSelection(DragableBrick brick, PointerEventData eventData)
        {
            List<SelectableBrick> clonedBricks = new List<SelectableBrick>();
            var bricks = SelectionManager.instance.GetSelected();

            //clone bricks
            foreach (var brickToClone in bricks)
            {
                var newBrick = BrickBuilder.Clone(brickToClone);
                clonedBricks.Add(newBrick as SelectableBrick);
            }

            //start moving
            base.OnBeginDrag(clonedBricks, eventData);
        }

        protected override void RegisterHistory()
        {
            HistoryManager.instance.Register(new HistoryNodeAdd[] { new HistoryNodeAdd(dragableBrick.gameObject) });
        }

        /// <summary>
        /// Begin dragging. Added to provide bypas for derived classes to MoveTool.OnBeginDrag function.
        /// </summary>
        /// <param name="brick">Brick.</param>
        protected void BeginDrag(DragableBrick brick)
        {
            base.OnBeginDrag(brick, null);
        }
    }
}
