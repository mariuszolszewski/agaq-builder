using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

namespace AgaQ.UI
{
    public class ActiveEdge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] float panelMinWidth = 122f;
        float panelMaxWidth;

        SidePanel panel;
        RectTransform panelRectTransform;

        float startMousePositionX; //mouse position when start dragging
        float startPanelWidth;
        bool dragging;

        void Start()
        {
            panel = GetComponentInParent<SidePanel>();
            panelRectTransform = panel.gameObject.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (dragging)
            {
                //calculate new width
                float xDelta = startMousePositionX - Input.mousePosition.x;
                float newWidth = startPanelWidth + panel.side * xDelta;
                newWidth = Mathf.Clamp(newWidth, panelMinWidth, panelMaxWidth);

                //set new width
                panelRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

                //if lmb up than end dragging
                if (Input.GetMouseButtonUp((int)MouseButton.LeftMouse))
                    dragging = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (panelRectTransform != null && Input.GetMouseButton((int)MouseButton.LeftMouse))
            {
                dragging = true;
                startMousePositionX = Input.mousePosition.x;
                startPanelWidth = panelRectTransform.rect.width;
                panelMaxWidth = GameObject.Find("Rotate scene up button").transform.position.x - 27;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            dragging = false;
        }
    }
}
