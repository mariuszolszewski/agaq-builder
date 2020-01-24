using UnityEngine;
using UnityEngine.Experimental.UIElements;
using AgaQ.Camera;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Hand tool to move camera view horizontally and vertically.
    /// </summary>
    public class HandTool : BaseTool
    {
        //move speed factor
        float moveSpeedFactor = 0.02f;

        UnityEngine.Camera camera;
        CameraController cameraController;

        Vector3 lastMousePosition;

        public HandTool()
        {
            camera = UnityEngine.Camera.main;
            cameraController = camera.gameObject.GetComponent<CameraController>();
        }

        public override void OnUpdate()
        {
            if (cameraController.IsInsideWorkArea(Input.mousePosition))
            {
                //Start dragging
                if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
                    lastMousePosition = Input.mousePosition;

                //during dragging
                if (Input.GetMouseButton((int)MouseButton.LeftMouse))
                {
                    var mousePositionDelta = lastMousePosition - Input.mousePosition;
                    var cameraPositionDelta =
                        camera.transform.right * mousePositionDelta.x * moveSpeedFactor +
                        camera.transform.up * mousePositionDelta.y * moveSpeedFactor;

                    camera.transform.position += cameraPositionDelta;
                    cameraController.ChangeRotationCenter(cameraController.rotationCenter + cameraPositionDelta);

                    lastMousePosition = Input.mousePosition;
                }
            }
        }
    }
}
