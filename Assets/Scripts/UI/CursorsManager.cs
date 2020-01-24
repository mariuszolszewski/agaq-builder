using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AgaQ.Camera;
using AgaQ.Bricks.Tools;
using System.Collections;
using System.Collections.Generic;

namespace AgaQ.UI
{
    /// <summary>
    /// Class to manage apliction cursor apperance.
    /// </summary>
    public class CursorsManager : MonoBehaviour
    {
        [SerializeField] float cursorUpdateTime = 0.3f;

        public static CursorsManager instance;

        CursorDefinition previouseCursor;
        CursorDefinition currentCursor;

        CameraController cam;
        ToolType currentTool = ToolType.Select;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        void Start()
        {
            cam = UnityEngine.Camera.main.GetComponent<CameraController>();

            ToolsManager.instance.OnToolChange += OnToolChange;
            SetCursor(Preferences.instance.defaultCursor);    
            StartCoroutine(ToolCursorCoroutine());
        }

        /// <summary>
        /// Set new cursor
        /// </summary>
        /// <param name="newCursor">Cursor.</param>
        void SetCursor(CursorDefinition newCursor)
        {
            if (newCursor == null || currentCursor == newCursor)
                return;
            
            previouseCursor = currentCursor;
            currentCursor = newCursor;

            Cursor.SetCursor(newCursor.image, newCursor.hotspot, CursorMode.Auto);
        }

        /// <summary>
        /// Come back to previous cursor
        /// </summary>
        void SetPreviousCursor()
        {
            if (previouseCursor == currentCursor)
                return;

            if (previouseCursor == null)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                previouseCursor = currentCursor;
                currentCursor = null;
            }
            else
                SetCursor(previouseCursor);
        }

        void OnToolChange(ToolType type)
        {
            currentTool = type;  
        }

        /// <summary>
        /// Update cursor based on cursor location and current tool.
        /// </summary>
        /// <returns>The cursor coroutine.</returns>
        IEnumerator ToolCursorCoroutine()
        {
            yield return new WaitForSeconds(cursorUpdateTime);

            while (true)
            {
                PointerEventData cursor = new PointerEventData(EventSystem.current);
                cursor.position = Input.mousePosition;
                List<RaycastResult> objectsHit = new List<RaycastResult>();
                EventSystem.current.RaycastAll(cursor, objectsHit);

                bool hitInUIElement = false;
                bool activeEdge = false;
                foreach (var o in objectsHit)
                {
                    if (o.gameObject.layer == 5)
                    {
                        hitInUIElement = true;
                        if (o.gameObject.name == "ActiveEdge")
                        {
                            activeEdge = true;
                            break;
                        }
                    }
                }

                if (activeEdge)
                    SetCursor(Preferences.instance.activeEdgeCursor);
                else if (hitInUIElement)
                    SetCursor(Preferences.instance.defaultCursor);
                else
                {
                    if (currentTool== ToolType.Clone)
                        SetCursor(Preferences.instance.cloneToolCursor);
                    else if (currentTool == ToolType.HandTool)
                        SetCursor(Preferences.instance.handToolCursor);
                    else if (currentTool == ToolType.Colour)
                        SetCursor(Preferences.instance.paintToolCursor);
                    else
                        SetCursor(Preferences.instance.defaultCursor);
                }

                yield return new WaitForSeconds(cursorUpdateTime);
            }
        }
    }
}
