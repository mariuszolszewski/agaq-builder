using UnityEngine;

namespace AgaQ.Camera.Tools
{
    /// <summary>
    /// Class to handle camera zoom with mouse wheel
    /// </summary>
    public class CameraZoomTool : ITool
    {
        public void Start(CameraController cameraController)
        {}

        public void Update(CameraController cameraController)
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel != 0 && cameraController.IsInsideWorkArea(Input.mousePosition))
                ChangeZoom(cameraController, wheel);
        }

        public void ChangeZoom(CameraController cameraController, float amount)
        {
            //distance between camera and rotation center
            float distance = (cameraController.transform.position - cameraController.rotationCenter).magnitude;

            //exit if distance to roation center is to small too large
            if ((amount > 0 && distance <= cameraController.minZoom) ||
                (amount < 0 && distance >= cameraController.maxZoom))
                return;
            
            cameraController.transform.position =
                cameraController.transform.forward * (amount * Preferences.instance.zoomSpeed) +
                cameraController.transform.position;
        }
    }
}
