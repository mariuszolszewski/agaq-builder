using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using AgaQ.Bricks.Positioners;
using AgaQ.Bricks.History;
using AgaQ.Bricks.Joints;
using AgaQ.Camera;
using System;
using System.Collections.Generic;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Tool to move brick around and change it position freely or join it with other bricks.
    /// </summary>
    public abstract class MoveTool : BaseTool
    {
        bool _isDragging;
        public bool isDragging {
            get { return _isDragging; }
            protected set {
                _isDragging = value;
            }
        }

        public Action<DragableBrick> OnStartMove;
        public Action OnCancelMove;
        public Action OnEndMove;

        public DragableBrick brick {
            get {
                return dragableBrick;
            }
        }

        const float pointSelectAccuracy = 5f;

        bool groupBrick; //is there temporary created brick as group to drag
        bool isJoined = false;

        bool isPointJoining = false;
        Collider pointJoinBrick;
        Vector3 pointJointDragOffset;

        protected DragableBrick dragableBrick;
        Vector3 lastMousePosition = Vector3.zero;

        //positioners
        SimplePositioner _simplePositioner;
        JoinablePositioner _joinablePositioner;
        PointJointPositioner _pointJointPositioner;
        SimplePositioner simplePositioner {
            get {
                if (_simplePositioner == null)
                    _simplePositioner = new SimplePositioner();
                
                return _simplePositioner;
            }
        }
        JoinablePositioner joinablePositioner {
            get {
                if (_joinablePositioner == null)
                    _joinablePositioner = new JoinablePositioner();
                
                return _joinablePositioner;
            }
        }

        //oryginal position
        Vector3 oldPosition;
        Quaternion oldRotation;

        //last proposed position
        ProposedPosition lastProposedPosition;

        HistoryNodeTransform[] historyNodes; //nodes for history to be able undo operation

        bool LMBRelesed; //is LFM released after start drag

        #region Event handlers

        public override void OnUpdate()
        {
            if (isDragging)
            {
                HandleRotationKeys(dragableBrick);
                MoveToCursor(dragableBrick, Input.mousePosition);

                OnModelChange?.Invoke();

                if (!LMBRelesed && Input.GetMouseButtonUp((int)MouseButton.LeftMouse))
                    LMBRelesed = true;
                else if (LMBRelesed &&
                         Input.GetMouseButtonDown((int)MouseButton.LeftMouse) &&
                         CameraController.instance.IsInsideWorkArea(Input.mousePosition))
                    OnEndDrag(dragableBrick, null);
            }
            else if (isPointJoining)
            {
                MovePointToPoint();
                if (Input.GetMouseButtonUp(0))
                {
                    isPointJoining = false;

                    if (historyNodes[0].oldPosition != pointJoinBrick.transform.position)
                        RegisterHistory();
                }
            }
            else if (Input.GetKey(KeyCode.V))
                HighlightPoint();
            else if (Input.GetKeyUp(KeyCode.V))
            {
                MeasureToolVisualizer.instance.ResetHighlight();
                OnCancel();
            }
        }

        public override bool OnBeginDrag(DragableBrick brick, PointerEventData eventData)
        {
            if (Input.GetKey(KeyCode.V))
                return false;
            
            if (brick == null)
            {
                Debug.LogError("MoveTool.OnBeginDrag: brcik is null!");
                return false;
            }

            PrepareBrickToDrag(brick);
            StartDrag(eventData);

            return true;
        }

        protected bool OnBeginDrag(List<SelectableBrick> bricks, PointerEventData eventData)
        {
            PrepareBricksToDrag(bricks);
            StartDrag(eventData);

            return true;
        }

        public override bool OnEndDrag(DragableBrick brick, PointerEventData eventData)
        {
            if (dragableBrick == null)
                return true;
            
            if (dragableBrick.isTransparent || !lastProposedPosition.isValid)
            {
                AudioSource.PlayClipAtPoint(Preferences.instance.errorSound, UnityEngine.Camera.main.transform.position);
                return false;
            }

            RegisterHistory();
            EndDragging();

            return true;
        }
            
        public override void OnEneter(HighlightableBrick brick)
        {
            brick.SetHighlighted(true);
        }

        public override void OnExit(HighlightableBrick brick)
        {
            brick.SetHighlighted(false);
        }

        public override void OnCancel()
        {
            if (dragableBrick == null)
                return;

            OnCancelMove?.Invoke();

            //reset position and rotation
            dragableBrick.transform.position = oldPosition;
            dragableBrick.transform.rotation = oldRotation;

            //rebuild brick joints
            var agaqBrick = dragableBrick.GetComponent<AgaQBrick>();
            if (agaqBrick != null)
                agaqBrick.RebuildJoints();
                         
            EndDragging();

            OnModelChange?.Invoke();
        }

        #endregion

        /// <summary>
        /// Replace current dragged brick with new one/
        /// </summary>
        /// <param name="brick">Brick.</param>
        public virtual void ChangeBrick(DragableBrick brick)
        {
            var toDestroy = dragableBrick;

            OnEndMove?.Invoke();
            SelectionManager.instance.Clear();

            PrepareBrickToDrag(brick);
            dragableBrick = brick;
            var positioner = GetPositioner(brick);
            positioner.StartMoving(brick, brick.transform.position);

            OnStartMove?.Invoke(dragableBrick);

            UnityEngine.Object.DestroyImmediate(toDestroy.gameObject);
        }

        protected virtual void RegisterHistory()
        {
            HistoryManager.instance.Register(historyNodes);
        }

        #region Private functions

        /// <summary>
        /// Find mesh point under cursor and higlight it
        /// </summary>
        void HighlightPoint()
        {
            var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.SphereCast(ray, 0.1f, out hit, 1000, 255, QueryTriggerInteraction.Collide))
            {
                var obj = hit.collider.gameObject;
                var meshFilter = obj.GetComponentInChildren<MeshFilter>();
                if (meshFilter != null)
                {
                    var meshVertises = meshFilter.gameObject.GetComponent<Vertices>();
                    if (meshVertises == null)
                        meshVertises = meshFilter.gameObject.AddComponent<Vertices>();
                    
                    Vector2 currentScreenPoint;
                    Vector3 currentWorldPoint;
                    if (MeshPointsTool.GetVisiblePointUnderCursor(
                        meshVertises.uniqueVertices, meshFilter.gameObject.transform,
                        UnityEngine.Camera.main,
                        Input.mousePosition,
                        pointSelectAccuracy,
                        out currentScreenPoint,
                        out currentWorldPoint))
                    {
                        MeasureToolVisualizer.instance.HighlighVertex(currentScreenPoint);
                        if (Input.GetMouseButton(0))
                        {
                            isPointJoining = true;
                            pointJoinBrick = hit.collider;
                            pointJointDragOffset = currentWorldPoint - obj.transform.position;

                            //prepare undo nodes
                            historyNodes = HistoryTool.PrepareTransformNodes(obj);
                        }
                    }
                    else
                        MeasureToolVisualizer.instance.ResetHighlight();
                }
            }
        }

        void MovePointToPoint()
        {
            pointJoinBrick.enabled = false;

            var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.SphereCast(ray, 0.1f, out hit, 1000, 255, QueryTriggerInteraction.Collide))
            {
                var obj = hit.collider.gameObject;
                var meshFilter = obj.GetComponentInChildren<MeshFilter>();
                if (meshFilter != null)
                {
                    var meshVertises = meshFilter.gameObject.GetComponent<Vertices>();
                    if (meshVertises == null)
                        meshVertises = meshFilter.gameObject.AddComponent<Vertices>();
                    
                    Vector2 currentScreenPoint;
                    Vector3 currentWorldPoint;
                    if (MeshPointsTool.GetVisiblePointUnderCursor(
                        meshVertises.uniqueVertices, meshFilter.gameObject.transform,
                        UnityEngine.Camera.main,
                        Input.mousePosition,
                        pointSelectAccuracy,
                        out currentScreenPoint,
                        out currentWorldPoint))
                    {
                        //move brick to jointpoints
                        var pos = currentWorldPoint - pointJointDragOffset;
                        pointJoinBrick.transform.position = pos;
                    }
                }
            }

            pointJoinBrick.enabled = true;
        }

        /// <summary>
        /// Start dragging.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        void StartDrag(PointerEventData eventData)
        {
            //save start position and rotaton to be able to revert move action
            oldPosition = dragableBrick.transform.position;
            oldRotation = dragableBrick.transform.rotation;

            LMBRelesed = !Input.GetMouseButton((int)MouseButton.LeftMouse);
            isDragging = true;
            SelectionManager.instance.TurnOffSelectionEffect();

            IPositioner positioner = GetPositioner(dragableBrick);
            var dragPoint = eventData == null ? dragableBrick.transform.position : eventData.pointerPressRaycast.worldPosition;
            positioner.StartMoving(dragableBrick, dragPoint);

            if (OnStartMove != null)
                OnStartMove(dragableBrick);
        }

        /// <summary>
        /// Prepare brick or all selected bric to be dragged.
        /// </summary>
        /// <param name="brick">Brick that is dragged</param>
        void PrepareBrickToDrag(DragableBrick brick)
        {
            if (SelectionManager.instance.SelectedAmount <= 1)
            {
                dragableBrick = brick;
                dragableBrick.SetHighlighted(false);
                dragableBrick.SetLayer(Preferences.instance.movingLayer);
                groupBrick = false;

                //prepare undo nodes
                historyNodes = HistoryTool.PrepareTransformNodes(brick.gameObject);
            }
            else
            {
                var selectedBricks = SelectionManager.instance.GetSelected();
                dragableBrick = PrepereGroupToDrag(selectedBricks);
            }

            dragableBrick.isDragging = true;
            AddRigidBody(dragableBrick);
        }

        /// <summary>
        /// Prepare brics to be dragged
        /// </summary>
        /// <param name="bricks">Bricks.</param>
        void PrepareBricksToDrag(List<SelectableBrick> bricks)
        {
            if (bricks.Count == 0)
                return;

            if (bricks.Count == 1 && bricks[0] is DragableBrick)
            {
                PrepareBrickToDrag(bricks[0] as DragableBrick);
                dragableBrick = bricks[0] as DragableBrick;
            }
            else
                dragableBrick = PrepereGroupToDrag(bricks);
            AddRigidBody(dragableBrick);
        }

        /// <summary>
        /// Adds the rigid body do object if there is no any.
        /// </summary>
        /// <param name="dragableBrick">Dragable brick.</param>
        void AddRigidBody(DragableBrick dragableBrick)
        {
            if (dragableBrick.GetComponent<Rigidbody>() == null)
            {
                var rigidBody = dragableBrick.gameObject.AddComponent<Rigidbody>();
                if (rigidBody != null)
                    rigidBody.isKinematic = true;
            }
        }

        /// <summary>
        /// Prepare bricks groups to drag.
        /// </summary>
        /// <returns>The group to drag.</returns>
        /// <param name="">.</param>
        DragableBrick PrepereGroupToDrag(List<SelectableBrick> bricks)
        {
            //create parent object to handle dragging
            GameObject parentObject = new GameObject();
            var parentBrick = parentObject.AddComponent<AgaQTemporaryGroup>();
            parentBrick.transform.SetParent(bricks[0].transform.parent);

            //make parentObject parent to all selected bricks and tuen off its highlight
            foreach (var brick in bricks)
            {
                brick.transform.SetParent(parentObject.transform);
                brick.SetHighlighted(false);
                brick.SetLayer(Preferences.instance.movingLayer);
                if (brick is DragableBrick)
                    (brick as DragableBrick).isDragging = true;
            }

            groupBrick = true;

            //prepare undo nodes
            historyNodes = HistoryTool.PrepareTransformNodes(bricks);

            return parentBrick;
        }

        /// <summary>
        /// End drag operation
        /// </summary>
        void EndDragging()
        {
            SelectionManager.instance.RestoreSelectionEffect();

            OnEndMove?.Invoke();

            //remove all rigidbodies and disable reporing
            var bricks = dragableBrick.GetComponentsInChildren<TransparentBrick>();
            foreach (var brick in bricks)
            {
                var rigidBody = brick.gameObject.GetComponent<Rigidbody>();
                if (rigidBody != null)
                    UnityEngine.Object.DestroyImmediate(rigidBody);
                if (brick is DragableBrick)
                    (brick as DragableBrick).isDragging = false;
            }

            if (groupBrick)
            {
                var childBricks = dragableBrick.GetComponentsInChildren<DragableBrick>();
                foreach (var childBrick in childBricks)
                {
                    childBrick.transform.SetParent(dragableBrick.transform.parent);
                    childBrick.SetLayer(0); //set default layer
                }

                UnityEngine.Object.Destroy(dragableBrick.gameObject);
            }
            else
                dragableBrick.SetLayer(0); //set default layer

            isDragging = false;
            dragableBrick = null;
        }

        /// <summary>
        /// Check cursor keys and rotae brick if pressed
        /// </summary>
        /// <param name="brick">Brick.</param>
        void HandleRotationKeys(DragableBrick brick)
        {
            bool snapToGrid = ToolsManager.instance.snapToGridButton.isSelected;
            float rotationStep = snapToGrid ? 90 : Preferences.instance.moveToolRotationStep;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                brick.transform.rotation = Rotate(brick.transform.rotation, 0, rotationStep);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                brick.transform.rotation = Rotate(brick.transform.rotation, 0, -rotationStep);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                brick.transform.rotation = Rotate(brick.transform.rotation, rotationStep, 0);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                brick.transform.rotation = Rotate(brick.transform.rotation, -rotationStep, 0);
        }

        /// <summary>
        /// Calculate new rotation by adding x and y deltas in degrees
        /// </summary>
        /// <returns>The rotate.</returns>
        /// <param name="rotation">Rotation.</param>
        /// <param name="deltaX">Delta x in degrees.</param>
        /// <param name="deltaY">Delta y in degrees.</param>
        Quaternion Rotate(Quaternion rotation, float deltaX, float deltaY)
        {
            Vector3 rotationEuler = rotation.eulerAngles;

            if (!Mathf.Approximately(deltaX, 0))
            {
                rotationEuler.x += deltaX;
                rotationEuler.x = Mathf.Round(rotationEuler.x / deltaX) * deltaX;
            }

            if (!Mathf.Approximately(deltaY, 0))
            {
                rotationEuler.y += deltaY;
                rotationEuler.y = Mathf.Round(rotationEuler.y / deltaY) * deltaY;
            }

            return Quaternion.Euler(rotationEuler);
        }

        /// <summary>
        /// Calculate new position based on cursor in drag mode and move brick.
        /// </summary>
        void MoveToCursor(DragableBrick brick, Vector3 mousePosition)
        {
            if (lastMousePosition != mousePosition)
            {
                lastMousePosition = mousePosition;
                IPositioner positioner = GetPositioner(brick);
                lastProposedPosition = positioner.GetPosition(mousePosition, ToolsManager.instance.snapToGridButton.isSelected, 0.5f);

                if (!isJoined && lastProposedPosition.isJoined)
                {
                    AudioSource.PlayClipAtPoint(Preferences.instance.joinBrickSound, UnityEngine.Camera.main.transform.position);
                    dragableBrick.FlashBrick();
                }

                isJoined = lastProposedPosition.isJoined;

                var moveVector = lastProposedPosition.position - brick.transform.position;
                if (moveVector.magnitude > 1)
                    lastProposedPosition.position = brick.transform.position + moveVector.normalized;

                brick.transform.position = lastProposedPosition.position;
                brick.transform.rotation = lastProposedPosition.rotation;
            }
        }

        /// <summary>
        /// Gets the proper positioner for brick.
        /// </summary>
        /// <returns>The positioner.</returns>
        /// <param name="brick">Brick.</param>
        IPositioner GetPositioner(DragableBrick brick)
        {
            if (brick is AgaQBrick)
                return joinablePositioner;
    
            return simplePositioner;
        }

        #endregion
    }
}
