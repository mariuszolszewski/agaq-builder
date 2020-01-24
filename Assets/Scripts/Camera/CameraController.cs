using UnityEngine;
using AgaQ.Camera.Tools;
using System;

namespace AgaQ.Camera
{
    /// <summary>
    /// Class handles input (mouse, keyboard, touch screen) translate it to camera movement.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance;

        Vector3 _rotationCenter = Vector3.zero; // camera pivot point
        Vector3 _lastCenter = Vector3.zero;
        Vector3 _currentCenter = Vector3.zero;
        Vector3 _camPosOffset;

        RectTransform toolbar;
        RectTransform leftPanel;
        RectTransform rightPanel;

        [Tooltip("Animation time when changind point of view.")]
        public float animTime = 0.3f;

        [Space]
        [Tooltip("Minimal distance to rotation center.")]
        public float minZoom = 3f;
        [Tooltip("Maximum distanse from rotation center.")]
        public float maxZoom = 500f;

        float animStart;
		bool animCenter;

		public Vector3 rotationCenter
        {
            get
            {
                return _currentCenter;
            }
            set
            {
                _lastCenter = _rotationCenter;
                _rotationCenter = value;
                animCenter = true;
                animStart = Time.time;
                _camPosOffset = transform.position - _lastCenter;
            }
        }

        // camera tools
        [NonSerialized] public CameraRotationTool cameraRotationTool = new CameraRotationTool();
        [NonSerialized] public CameraZoomTool cameraZoomTool = new CameraZoomTool();
        [NonSerialized] public RotationCenterTool rotationCenterTool = new RotationCenterTool();
        [NonSerialized] public CameraPredefidedViews cameraPredefinedViews = new CameraPredefidedViews();

        Vector2 rotationDirection = Vector2.zero;
        float zoomAmount = 0;

        #region Monobehaviour events

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        void Start()
        {
            cameraRotationTool.Start(this);
            cameraZoomTool.Start(this);
            cameraRotationTool.Start(this);
            cameraPredefinedViews.Start(this);

            GameObject gObject = GameObject.Find("Toolbar");
            if (gObject != null)
                toolbar = gObject.GetComponent<RectTransform>();

            gObject = GameObject.Find("Library panel");
            if (gObject != null)
                leftPanel = gObject.GetComponent<RectTransform>();

            gObject = GameObject.Find("Preferences panel");
            if (gObject != null)
                rightPanel = gObject.GetComponent<RectTransform>();
        }

        void Update()
        {
            cameraRotationTool.Update(this);
            cameraZoomTool.Update(this);
            rotationCenterTool.Update(this);
            cameraPredefinedViews.Update(this);

            if (animCenter)
            {
                _currentCenter = Vector3.Lerp(_lastCenter, _rotationCenter, (Time.time - animStart) / animTime);
                transform.position = _currentCenter + _camPosOffset;
                transform.LookAt(_currentCenter);

                if (_currentCenter == _rotationCenter)
                    animCenter = false;
            }

            //handle rotation from buttons
            if (rotationDirection.x != 0 || rotationDirection.y != 0)
                cameraRotationTool.RotateCamera(this, rotationDirection * Preferences.instance.buttonRotationSpeed);

            //handle zoom from buttons
            if (zoomAmount != 0)
                cameraZoomTool.ChangeZoom(this, zoomAmount * 0.2f);
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Set new rotation senter without camera moving.
        /// </summary>
        /// <param name="newRotationCenter">New rotation center.</param>
        public void ChangeRotationCenter(Vector3 newRotationCenter)
        {
            _rotationCenter = newRotationCenter;
            _lastCenter = newRotationCenter;
            _currentCenter = newRotationCenter;
            _camPosOffset = transform.position - _lastCenter;
        }

        /// <summary>
        /// Zoom camera the specified amount.
        /// </summary>
        /// <param name="amount">Amount.</param>
        public void Zoom(float amount)
        {
            zoomAmount = amount;
        }

        /// <summary>
        /// Rorate camera verticaly
        /// </summary>
        /// <param name="amount">Amount -1,0,1.</param>
        public void RotateVertical(float amount)
        {
            rotationDirection.y = amount;
        }

        /// <summary>
        /// Rotate camera horizontaly
        /// </summary>
        /// <param name="amount">Amount -1,0,1.</param>
        public void RotateHorizontal(float amount)
        {
            rotationDirection.x = amount;
        }

        /// <summary>
        /// Sets the top view.
        /// </summary>
        public void SetTopView()
        {
            cameraPredefinedViews.SetViewTop(this);
        }

        /// <summary>
        /// Sets the bottom view.
        /// </summary>
        public void SetBottomView()
        {
            cameraPredefinedViews.SetViewBottom(this);
        }

        /// <summary>
        /// Sets the front view.
        /// </summary>
        public void SetFrontView()
        {
            cameraPredefinedViews.SetViewFront(this);
        }

        /// <summary>
        /// Sets the rear view.
        /// </summary>
        public void SetRearView()
        {
            cameraPredefinedViews.SetViewRear(this);
        }

        /// <summary>
        /// Sets the left view.
        /// </summary>
        public void SetLeftView()
        {
            cameraPredefinedViews.SetViewLeft(this);
        }

        /// <summary>
        /// Sets the right view.
        /// </summary>
        public void SetRightView()
        {
            cameraPredefinedViews.SetViewRight(this);
        }

        /// <summary>
        /// Set the perspective view.
        /// </summary>
        public void SetPerspectiveView()
        {
            cameraPredefinedViews.SetViewPerspective(this);
        }

        /// <summary>
        /// Gets working area bounds
        /// </summary>
        public Vector2[] GetWorkAreaBounds()
        {
            Vector3[] v = new Vector3[4];
            Vector2[] areaBounds = new Vector2[2];
            leftPanel.GetWorldCorners(v);
            areaBounds[0].x = v[2].x;
            areaBounds[0].y = v[2].y;

            rightPanel.GetWorldCorners(v);
            areaBounds[1].x = v[0].x;
            areaBounds[1].y = v[0].y;

            return areaBounds;
        }

        /// <summary>
        /// Check if given posiition is inside work area of the editor.
        /// </summary>
        /// <returns><c>true</c>, if inside work area was ised, <c>false</c> otherwise.</returns>
        /// <param name="mousePosition">Mouse position in screen space.</param>
        public bool IsInsideWorkArea(Vector3 mousePosition)
        {
            var areaBounds = GetWorkAreaBounds();

            if (mousePosition.x < areaBounds[0].x ||
                mousePosition.x > areaBounds[1].x ||
                mousePosition.y > areaBounds[0].y ||
                mousePosition.y < areaBounds[1].y)
                return false;

            return true;
        }

        #endregion
    }
}
