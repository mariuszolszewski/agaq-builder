using UnityEngine;
using UnityEngine.UI;

namespace AgaQ.UI.Gizmos
{
    public class RotateEditorAxis : MonoBehaviour
    {
        [SerializeField] InputField inputField;

        public void RotateRight90()
        {
            ChangeAngle90(90);
        }

        public void RotateLeft90()
        {
            ChangeAngle90(-90);
        }

        /// <summary>
        /// Change angle in input field by given step and round it to 90.
        /// </summary>
        /// <param name="angle">Angle.</param>
        void ChangeAngle90(int angle)
        {
            var value = float.Parse(inputField.text);
            value += angle;
            value = Mathf.Round(Mathf.Round(value / 90) * 90 * 100) / 100f;
            if (value <= -360 || value >= 360)
                value = value % 360f;
            inputField.text = value.ToString();
        }
    }
}
