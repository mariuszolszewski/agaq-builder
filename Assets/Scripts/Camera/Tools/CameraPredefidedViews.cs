using UnityEngine;

namespace AgaQ.Camera.Tools
{
    /// <summary>
    /// Class to setup camera in predefinded position after key press
    /// </summary>
    public class CameraPredefidedViews : ITool
    {
        float animationTime = 0.2f;

        public void Start(CameraController cameraController)
        {}

        public void Update(CameraController cameraController)
        {
            if (Input.GetKeyDown(KeyCode.F1)) SetViewFront(cameraController);
            if (Input.GetKeyDown(KeyCode.F2)) SetViewRear(cameraController);
            if (Input.GetKeyDown(KeyCode.F3)) SetViewLeft(cameraController);
            if (Input.GetKeyDown(KeyCode.F4)) SetViewRight(cameraController);
            if (Input.GetKeyDown(KeyCode.F5)) SetViewTop(cameraController);
            if (Input.GetKeyDown(KeyCode.F6)) SetViewBottom(cameraController);
            if (Input.GetKeyDown(KeyCode.F7)) SetViewPerspective(cameraController);
        }

        /// <summary>
        /// Set camera position and rotation to look from top
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        public void SetViewTop(CameraController cameraController)
        {
            cameraController.cameraRotationTool.SetCameraRotationWithAnimation(cameraController, new Vector3(90, 0, 0), animationTime);
        }

        /// <summary>
        /// Set camera position and rotation to look from bottom
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        public void SetViewBottom(CameraController cameraController)
        {
            cameraController.cameraRotationTool.SetCameraRotationWithAnimation(cameraController, new Vector3(-90, 0, 0), animationTime);
        }

        /// <summary>
        /// Set camera position and rotation to look from left
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        public void SetViewLeft(CameraController cameraController)
        {
            cameraController.cameraRotationTool.SetCameraRotationWithAnimation(cameraController, new Vector3(0, 90, 0), animationTime);
        }

        /// <summary>
        /// Set camera position and rotation to look from right
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        public void SetViewRight(CameraController cameraController)
        {
            cameraController.cameraRotationTool.SetCameraRotationWithAnimation(cameraController, new Vector3(0, -90, 0), animationTime);
        }

        /// <summary>
        /// Set camera position and rotation to look from fromt
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        public void SetViewFront(CameraController cameraController)
        {
            cameraController.cameraRotationTool.SetCameraRotationWithAnimation(cameraController, new Vector3(0, 0, 0), animationTime);
        }

        /// <summary>
        /// Set camera position and rotation to look from rear
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        public void SetViewRear(CameraController cameraController)
        {
            cameraController.cameraRotationTool.SetCameraRotationWithAnimation(cameraController, new Vector3(0, 180, 0), animationTime);
        }

        /// <summary>
        /// Set camera position and rotation to look from perspective
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        public void SetViewPerspective(CameraController cameraController)
        {
            cameraController.cameraRotationTool.SetCameraRotationWithAnimation(cameraController, new Vector3(35, -45, 0), animationTime);
        }
    }
}
