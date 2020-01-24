using UnityEngine;
using UnityEngine.EventSystems;
using AgaQ.Camera;
using AgaQ.Bricks.History;
using System.Collections;

namespace AgaQ.Bricks.Tools
{
    public class AddTool : CloneTool
    {
        Coroutine detectCursorPositionCoroutine;

        #region Event handlers

        public override bool OnEndDrag(DragableBrick brick, PointerEventData eventData)
        {
            if (brick == null)
                return true;

            bool canEndDrag = base.OnEndDrag(brick, eventData);

            //restart add tool ar next frame
            if (canEndDrag)
                ToolsManager.instance.StartCoroutine(RestartAdding(brick));

            return canEndDrag;
        }

        public override void OnCancel()
        {
            if (dragableBrick == null)
                return;

            if (detectCursorPositionCoroutine != null)
                ToolsManager.instance.StopCoroutine(detectCursorPositionCoroutine);

            OnCancelMove?.Invoke();

            isDragging = false;
            Object.Destroy(dragableBrick.gameObject);
            dragableBrick = null;

            ToolsManager.instance.SetPreviousTool();
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Add new brick to scene.
        /// </summary>
        /// <param name="resourcePath">Resource path.</param>
        /// <param name="presentFirst">If true present first brick at center of edit area and wait for cursor entering edit area.</param>
        public void Add(string resourcePath, bool presentFirst)
        {
            if (dragableBrick != null)
                OnCancel();

            var newBrick = BrickBuilder.InstansiateFromResources(resourcePath);
            if (newBrick != null)
                Add(newBrick, presentFirst);
        }

        /// <summary>
        /// Add new brick to scene froms saved file (group or add)
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="presentFirst">If true present first brick at center of edit area and wait for cursor entering edit area.</param>
        public void AddSaved(string path, bool presentFirst)
        {
            if (dragableBrick != null)
                OnCancel();

            var newBrick = BrickBuilder.InstansiateFromFile(path);
            if (newBrick != null)
                Add(newBrick, presentFirst);
       }

        /// <summary>
        /// Add new brick to scene.
        /// </summary>
        /// <param name="newBrick">New brick.</param>
        /// <param name="presentFirst">If true present forst brick at center of edit area and wait for cursor entering edit area.</param>
        void Add(Brick newBrick, bool presentFirst)
        {
            //exit if there is brick in processing
            if (dragableBrick != null || newBrick is OrdinaryBrick || newBrick is BricksGroup)
                return;

            //select this tool
            ToolsManager.instance.SetTool(ToolType.Add);

            //clear selection
            SelectionManager.instance.Clear();

            dragableBrick = newBrick as DragableBrick;

            if (presentFirst)
            {
                PresentBrick();
                detectCursorPositionCoroutine = ToolsManager.instance.StartCoroutine(DetectCursorAtEditAreaCoroutine());
            }
            else
                BeginDrag(dragableBrick as DragableBrick);
        }
                        
        /// <summary>
        /// Detect when cursor move from side panel to edit area.
        /// </summary>
        /// <returns>The cursor at edit area coroutine.</returns>
        public IEnumerator DetectCursorAtEditAreaCoroutine()
        {
            while (!CameraController.instance.IsInsideWorkArea(Input.mousePosition))
                yield return null;

            //begin brick dragging
            BeginDrag(dragableBrick as DragableBrick);
        }

        /// <summary>
        /// Add one more the same brick after one frame
        /// </summary>
        /// <returns>The adding.</returns>
        public IEnumerator RestartAdding(Brick brick)
        {
            yield return null;

            var newBrick = BrickBuilder.Clone(brick);
            if (newBrick != null)
                Add(newBrick, false);
        }

        #endregion

        protected override void RegisterHistory()
        {
            HistoryManager.instance.Register(new HistoryNodeAdd[] { new HistoryNodeAdd(dragableBrick.gameObject) });
        }

        /// <summary>
        /// position it at center of edit area in front of the camera
        /// </summary>
        void PresentBrick()
        {
            var camera = UnityEngine.Camera.main;
            var brickBounds = dragableBrick.GetBounds();
            var brickMaxDimension = Mathf.Max(
                brickBounds.extents.x,
                brickBounds.extents.y,
                brickBounds.extents.z);

            float brickDistance = brickMaxDimension * 3 / Mathf.Tan(camera.fieldOfView * Mathf.PI / 360);

            dragableBrick.transform.position = 
                camera.transform.position +
                camera.transform.forward *
                brickDistance;
        }
    }
}
