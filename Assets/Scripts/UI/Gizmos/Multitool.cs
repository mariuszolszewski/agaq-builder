using UnityEngine;
using UnityEngine.UI;
using AgaQ.Bricks;
using AgaQ.Bricks.Tools;
using System.Collections.Generic;

namespace AgaQ.UI.Gizmos
{
    /// <summary>
    /// Class control mutlitool gizmo. Adjust it's scale and locations.
    /// </summary>
    public class Multitool : MonoBehaviour
    {
        public static Multitool instance;

        [SerializeField] float scaleFactor = 15f;
        public GameObject stickyAxis;
        public GameObject scaleTool;

        GameObject tools;
        UnityEngine.Camera camera;
        ToolsManager toolManager;
        SelectionManager selectionManager;

        bool _global = true;
        public bool global { get { return _global; } }
        [SerializeField] Button globalButton;
        [SerializeField] GameObject globalTexl;
        [SerializeField] GameObject localText;

		void Awake()
		{
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
		}

		void Start()
        {
            camera = UnityEngine.Camera.main;
            tools = transform.Find("Tools").gameObject;
            tools.SetActive(false);

            toolManager = ToolsManager.instance;
            selectionManager = SelectionManager.instance;

            selectionManager.OnSelectionChange += OnSelectionChange;
            toolManager.OnToolChange += OnToolChange;

            tools.SetActive(ToolPreferedState());

            stickyAxis.transform.SetParent(null);
            stickyAxis.SetActive(false);

            globalButton.onClick.AddListener(delegate { OnGLobalButtonClick(); });
        }

        void LateUpdate()
        {
            if (tools.activeSelf)
            {
                AdjustScale();
                AdjustPostion();
                AdjustRotation();
                AdjustAxesSize();
            }
        }

        /// <summary>
        /// Adjusts scale o f the tool to keep size of the tool on sceen unchanged.
        /// </summary>
        void AdjustScale()
        {
            float factor = (camera.transform.position - transform.position).magnitude / scaleFactor;
            transform.localScale = new Vector3(factor, factor, factor);
        }

        /// <summary>
        /// Adjust tool position acording to selected bricks.
        /// </summary>
        void AdjustPostion()
        {
            var bounds = GetBounds();
            transform.position = bounds.center;
        }

        public Bounds GetBounds()
        {
            var selected = selectionManager.GetSelected();

            if (selected.Count == 1)
                return selected[0].GetBounds();

            var bounds = new Bounds(selected[0].transform.position, Vector3.zero);
            for (var i = 0; i < selected.Count; i++)
                bounds.Encapsulate(Brick.GetBounds(selected[0].gameObject));

            return bounds;
        }

        public Bounds GetUnrotatedBounds()
        {
            var selected = selectionManager.GetSelected();

            if (selected.Count == 1)
                return selected[0].GetUnrotatedBounds();

            var bounds = new Bounds(selected[0].transform.position, Vector3.zero);
            for (var i = 0; i < selected.Count; i++)
                bounds.Encapsulate(Brick.GetUnrotatedBounds(selected[0].gameObject));

            return bounds;
        }

        void AdjustRotation()
        {
            if (_global)
                tools.transform.rotation = Quaternion.Euler(Vector3.zero);
            else
            {
                var selected = selectionManager.GetSelected();
                if (selected.Count > 0)
                    tools.transform.rotation = selected[0].transform.rotation;
                else
                    tools.transform.rotation = Quaternion.Euler(Vector3.zero);
            }
        }

        void AdjustAxesSize()
        {
            Bounds bounds = global ? GetBounds() : GetUnrotatedBounds();

            var moveAxes = GetComponentsInChildren<MoveToolAxis>();
            foreach (var axe in moveAxes)
                axe.AdjustAxis(bounds, transform.localScale.x * 50f);

            var scaleAxes = GetComponentsInChildren<ScaleToolAxis>();
            foreach (var axe in scaleAxes)
                axe.AdjustAxis(bounds, transform.localScale.x * 50f);

            var minSize = Mathf.Max(new float[] { bounds.size.x, bounds.size.y, bounds.size.z }) * 0.4f / transform.localScale.x;
            var rotationCircles = GetComponentsInChildren<RotationGizmo>();
            foreach (var circle in rotationCircles)
                circle.transform.localScale = new Vector3(minSize, minSize, minSize);
        }

        bool AreAllSelectedNotRotated()
        {
            foreach (var sel in selectionManager.GetSelected())
            {
                if (sel.transform.localRotation.eulerAngles != Vector3.zero)
                    return false;
            }

            return true;
        }

        void OnSelectionChange(List<SelectableBrick> bricks)
        {
            var preferedState = ToolPreferedState();
            tools.SetActive(preferedState);
            if (preferedState)
            {
                scaleTool.SetActive(!AllAgaQ() && !_global);
                globalTexl.SetActive(_global);
                localText.SetActive(!_global);
            }
        }

        void OnToolChange(ToolType type)
        {
            var preferedState = ToolPreferedState();
            tools.SetActive(preferedState);
            if (preferedState)
                scaleTool.SetActive(!AllAgaQ() && !_global);
        }

        void OnGLobalButtonClick()
        {
            _global = !_global;
            globalTexl.SetActive(_global);
            localText.SetActive(!_global);
            scaleTool.SetActive(!AllAgaQ() && !_global);
        }

        /// <summary>
        /// Check if tool should be on or off.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        bool ToolPreferedState()
        {
            if (selectionManager.SelectedAmount > 0)
            {
                if (toolManager.tool is MultiTool)
                    return true;
                else if (toolManager.tool is MoveTool)
                {
                    bool allOrdinary = true;
                    var selected = selectionManager.GetSelected();
                    foreach (var brick in selected)
                    {
                        if (!(brick is OrdinaryBrick))
                        {
                            allOrdinary = false;
                            break;
                        }
                    }

                    return allOrdinary;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if all aselected bricks are agaq.
        /// </summary>
        bool AllAgaQ()
        {
            var bricks = SelectionManager.instance.GetSelected();
            foreach (var brick in bricks)
            {
                if (!(brick is AgaQBrick))
                    return false;
            }

            return true;
        }
    }
}
