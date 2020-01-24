using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AgaQ.UI
{
    /// <summary>
    /// Tool button. Kid of button that has selected color run after click and acts as radio buttons.
    /// Other buttoons of this kind are of. Only one can be selected.
    /// </summary>
    public class ToolButton : Button
    {
        [SerializeField] Color selectedColor;

        static ToolButton[] _otherButtons;
        static ToolButton[] otherButtons
        {
            get
            {
                if (_otherButtons == null)
                    _otherButtons = FindObjectsOfType<ToolButton>();

                return _otherButtons;
            }
        }

        /// <summary>
        /// Set selected color to button background image.
        /// </summary>
        public void SetSelected()
        {
            UnselectAll();

            var image = GetComponent<Image>();
            if (image != null)
                image.color = selectedColor;
        }

        /// <summary>
        /// Unselect all tool buttons
        /// </summary>
        public static void UnselectAll()
        {
            //set all other tool buttons to base color
            foreach (var button in otherButtons)
            {
                var buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                    buttonImage.color = button.colors.normalColor;                
            }
        }
    }
}
