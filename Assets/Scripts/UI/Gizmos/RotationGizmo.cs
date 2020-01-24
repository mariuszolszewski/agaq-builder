using UnityEngine;
using UnityEngine.UI;
using AgaQ.Bricks;
using System.Collections.Generic;

namespace AgaQ.UI.Gizmos
{
    /// <summary>
    /// Controller for rotation gizmo from multitool
    /// </summary>
    public class RotationGizmo : MonoBehaviour
    {
        [SerializeField] GameObject anglesPanel;
        [SerializeField] InputField xInput;
        [SerializeField] InputField yInput;
        [SerializeField] InputField zInput;

        List<SelectableBrick> selected;

        void Awake()
        {
            xInput.onValueChanged.AddListener(delegate { InputValueChanged(GizmoAxis.X); });
            yInput.onValueChanged.AddListener(delegate { InputValueChanged(GizmoAxis.Y); });
            zInput.onValueChanged.AddListener(delegate { InputValueChanged(GizmoAxis.Z); });
            SelectionManager.instance.OnSelectionChange += OnSelectionChange;
        }

        void OnEnable()
        {
            selected = SelectionManager.instance.GetSelected();
            if (selected.Count == 1)
            {
                AnglesToInput();
                anglesPanel.SetActive(true);
            }
        }

        void OnDisable()
        {
            anglesPanel.SetActive(false);
        }

        void OnSelectionChange(List<SelectableBrick> bricks)
        {
            selected = bricks;
            if (selected.Count > 0)
                AnglesToInput();
        }

        /// <summary>
        /// Transform angles to text at inout fields.
        /// </summary>
        void AnglesToInput()
        {
            var angles = selected[0].transform.rotation.eulerAngles;
            xInput.text = angles.x.ToString();
            yInput.text = angles.y.ToString();
            zInput.text = angles.z.ToString();
        }

        /// <summary>
        /// Event handler fired when gizmo torus change angle.
        /// </summary>
        public void AnglesChaged()
        {
            AnglesToInput();
        }

        /// <summary>
        /// Event handler for angle input fields.
        /// </summary>
        /// <param name="axis">Axis.</param>
        void InputValueChanged(GizmoAxis axis)
        {
            Vector3 angles;
            float.TryParse(xInput.text, out angles.x);
            float.TryParse(yInput.text, out angles.y);
            float.TryParse(zInput.text, out angles.z);

            var rotationCenter = Multitool.instance.GetBounds().center;
            var euler = selected[0].transform.rotation.eulerAngles;

            selected[0].transform.RotateAround(rotationCenter, Vector3.right, angles.x - euler.x);
            selected[0].transform.RotateAround(rotationCenter, Vector3.up, angles.y - euler.y);
            selected[0].transform.RotateAround(rotationCenter, Vector3.forward, angles.z - euler.z);
        }
    }
}
