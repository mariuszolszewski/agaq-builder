using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using AgaQ.Bricks.Tools;
using UnityEngine.Experimental.UIElements;

namespace AgaQ.Bricks
{
    /// <summary>
    /// Brick that can be dragged around scene.
    /// </summary>
    public class DragableBrick : TransparentBrick
    {
        [NonSerialized] public bool isDragging; // true when dragging
        const float startDragDiff = 5f;         //mouse distance with LMB when draggin should start
        const float maxMoveMesureTIme = 0.2f;   //time when moving start with LMB when can mesure distance to start dragging
        const float startDragTime = 2f;         //time with LMB when dragging can start

        Vector2 pointerDownposition; //mouse position when pointerDown event occurs
        Coroutine detectCouroutine;

        #region Event handlers

        public override void OnPointerClick(PointerEventData pointerEventData)
        {
            if (!isDragging)
            {
                float mouseDistance = (pointerEventData.position - pointerDownposition).magnitude;
                if (mouseDistance < startDragDiff)
                    base.OnPointerClick(pointerEventData);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!isDragging)
                base.OnPointerEnter(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            pointerDownposition = eventData.position;

            if (grouped)
            {
                transform.parent?.gameObject.GetComponentInParent<DragableBrick>()?.OnPointerDown(eventData);
                return;
            }

            if (!isDragging)
            {
                //pointerDownposition = eventData.position;
                if (eventData.button == PointerEventData.InputButton.Left)
                    detectCouroutine = StartCoroutine(DetectDragCoroutine(eventData));
            }

            base.OnPointerDown(eventData);
        }
            
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (detectCouroutine != null)
                StopCoroutine(detectCouroutine);
            
            base.OnPointerUp(eventData);
        }

        #endregion

        /// <summary>
        /// Coroutine to detect drag event.
        /// It is done by myself becouse eventsystem issue with detectiong draggind
        /// and problems with reporting click after wrong drag.
        /// </summary>
        /// <returns>The drag coroutine.</returns>
        /// <param name="eventData">Event data.</param>
        IEnumerator DetectDragCoroutine(PointerEventData eventData)
        {
            float detectFirstMoveEndTime = Time.time + startDragTime;
            var startMousePosition = eventData.position;

            //detect move
            while (detectFirstMoveEndTime > Time.time && startMousePosition == eventData.position)
                yield return null;

            //is moved, detect if distance is enought to start drag
            if (startMousePosition != eventData.position)
            {
                float detectEndTime = Time.time + maxMoveMesureTIme;

                while (detectEndTime > Time.time)
                {
                    float distance = (startMousePosition - eventData.position).magnitude;
                    if (distance >= startDragDiff)
                    {
                        ToolsManager.instance.tool.OnBeginDrag(this, eventData);
                        break;
                    }

                    yield return null;
                }
            }
        }
    }
}
