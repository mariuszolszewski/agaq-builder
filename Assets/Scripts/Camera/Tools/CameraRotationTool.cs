using UnityEngine;
using UnityEngine.Experimental.UIElements;
using System.Collections;

namespace AgaQ.Camera.Tools
{
    /// <summary>
    /// Class that handle scene rotation with mouse controll and keyboard.
    /// </summary>
    public class CameraRotationTool : ITool
    {
        bool rotationMode;
        Vector3 lastMousePos; // last mouse screen position
        float verticalAngel;  // last camera verticla euler angel

        #region Monobehaviour event handlers

        public void Start(CameraController cameraController)
        {
            cameraController.transform.LookAt(cameraController.rotationCenter);
            verticalAngel = GetVerticalAngle(cameraController.transform.position, cameraController.rotationCenter);
        }

        public void Update(CameraController cameraController)
        {
            if (rotationMode)
            {
                Vector3 mouseDif = lastMousePos - Input.mousePosition;
                lastMousePos = Input.mousePosition;

                RotateCamera(cameraController, new Vector2(mouseDif.x, mouseDif.y));

                if (Input.GetMouseButtonUp((int)MouseButton.RightMouse))
                    rotationMode = false;
            }
            else
            {
                if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
                {
                    rotationMode = true;
                    lastMousePos = Input.mousePosition;
                }
            }
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Roatate camera.
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        /// <param name="rotationDelta">Rotation delta.</param>
        public void RotateCamera(CameraController cameraController, Vector2 rotationDelta)
        {
            cameraController.transform.RotateAround(cameraController.rotationCenter, Vector3.up, -rotationDelta.x);

            if (verticalAngel + rotationDelta.y < 90 && verticalAngel + rotationDelta.y > -90)
            {
                cameraController.transform.RotateAround(
                    cameraController.rotationCenter,
                    Quaternion.Euler(0, cameraController.transform.rotation.eulerAngles.y, 0) * Vector3.right,
                    rotationDelta.y);
                verticalAngel = GetVerticalAngle(cameraController.transform.position, cameraController.rotationCenter);
            }

            cameraController.transform.LookAt(cameraController.rotationCenter);
        }

        /// <summary>
        /// Set camera rotation and change postion to keep distance to rotation center and look at it.
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        /// <param name="rotation">Rotation euler angles.</param>
        public void SetCameraRotation(CameraController cameraController, Vector3 rotation)
        {
            var distansToRotationCenter = (cameraController.rotationCenter - cameraController.transform.position).magnitude;
            var newPositionOffset = Quaternion.Euler(rotation) * Vector3.back * distansToRotationCenter;
            cameraController.transform.position = cameraController.rotationCenter + newPositionOffset;
            cameraController.transform.LookAt(cameraController.rotationCenter);
        }

        /// <summary>
        /// Sets the camera rotation and change position to keep distance to roatation center and look at it.
        /// All will be animated throught animationTime.
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        /// <param name="rotation">Rotation euler angles.</param>
        /// <param name="animationTime">Animation time.</param>
        public void SetCameraRotationWithAnimation(CameraController cameraController, Vector3 rotation, float animationTime)
        {
            cameraController.StartCoroutine(RotationAnimationCoroutine(cameraController, rotation, animationTime));
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Get camera vertical euler angle.
        /// </summary>
        /// <param name="position">Camera position.</param>
        /// <param name="rotationCenter">Camera rotation point.</param>
        /// <returns>The camera vertical euler angel.</returns>
        float GetVerticalAngle(Vector3 position, Vector3 rotationCenter)
        {
            Vector3 vect = position - rotationCenter;
            Vector3 vect2 = new Vector3(vect.x, 0, vect.z);

            float angle = Mathf.Acos(vect2.magnitude / vect.magnitude) * 180 / Mathf.PI;

            if (vect.y < 0)
                angle = -angle;

            return angle;
        }

        /// <summary>
        /// Coroutine function that animates camera rotation
        /// </summary>
        /// <returns>The animation coroutine.</returns>
        /// <param name="cameraController">Camera controller.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="animationTime">Animation time.</param>
        public IEnumerator RotationAnimationCoroutine(CameraController cameraController, Vector3 rotation, float animationTime)
        {
            Quaternion startAngle = cameraController.transform.rotation;
            Quaternion endAngle = Quaternion.Euler(rotation);
            var startTime = Time.time;

            while (Time.time < startTime + animationTime)
            {
                SetCameraRotation(cameraController, Quaternion.Slerp(startAngle, endAngle, (Time.time - startTime) / animationTime).eulerAngles);
                yield return null;
            }

            SetCameraRotation(cameraController, rotation);
        }

        #endregion
    }
}
