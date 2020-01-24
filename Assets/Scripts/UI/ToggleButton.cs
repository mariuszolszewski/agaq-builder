using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AgaQ.UI
{
    /// <summary>
    /// Button that can be on or off.
    /// </summary>
    public class ToggleButton : Button
    {
        [SerializeField] Color selectedColor;
        [SerializeField] bool _isSelected;
        public bool isSelected {
            get { return _isSelected; }
            protected set { _isSelected = value; }
        }

        protected override void Start()
        {
            base.Start();

            if (image != null)
                image.color = _isSelected ? selectedColor : colors.normalColor;
        }
        public override void OnPointerClick (PointerEventData eventData)
        {
            Toggle();
            base.OnPointerClick(eventData);
        }

        /// <summary>
        /// Toggle button.
        /// </summary>
        public void Toggle()
        {
            _isSelected = !_isSelected;

            if (image != null)
                image.color = _isSelected ? selectedColor : colors.normalColor;
        }
    }
}
