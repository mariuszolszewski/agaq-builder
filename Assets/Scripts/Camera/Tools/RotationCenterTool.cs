using UnityEngine;
using UnityEngine.Experimental.UIElements;
using AgaQ.Bricks;

namespace AgaQ.Camera.Tools
{
    /// <summary>
    /// Class to handle selectiong new rotation center for camera.
    /// </summary>
    public class RotationCenterTool : ITool
    {
        Vector3 mouseClickPosition;

        public void Start(CameraController cameraController)
        {}

        public void Update(CameraController cameraController)
        {
            if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
                mouseClickPosition = Input.mousePosition;

            if (Input.GetMouseButtonUp((int)MouseButton.RightMouse) && mouseClickPosition == Input.mousePosition)
            {
                var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f, 255, QueryTriggerInteraction.Collide))
                {
                    var dragable = hit.collider.gameObject.GetComponent<Brick>();
                    if (dragable != null)
                        dragable = hit.collider.gameObject.GetComponentInParent<Brick>();
                    if (dragable != null)
                        cameraController.rotationCenter = dragable.transform.position;
                }
            }
        }
    }
}
