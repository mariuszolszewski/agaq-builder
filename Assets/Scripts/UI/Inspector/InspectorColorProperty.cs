using UnityEngine;
using AgaQ.Bricks;

namespace AgaQ.UI.Inspector
{
    public class InspectorColorProperty : InspectorProperty
    {
        /// <summary>
        /// Color button control build in property.
        /// </summary>
        public ColorButton colorButton
        {
            get;
            protected set;
        }

        protected override void Start()
        {
            base.Start();
            colorButton = GetComponentInChildren<ColorButton>();
            colorButton.OnColorSelected += OnColorSelected;
        }

        /// <summary>
        /// Set property active / inactive
        /// </summary>
        public override void SetActive(bool active)
        {
            base.SetActive(active);

            if (colorButton != null)
                colorButton.gameObject.SetActive(active);
        }

        /// <summary>
        /// Set selected color at color controll.
        /// </summary>
        /// <param name="color">Color.</param>
        public void SetColor(Color color)
        {
            if (colorButton != null)
            {
                colorButton.SetColor(color);
                colorButton.ConfirmColor();
            }
        }

        /// <summary>
        /// Event handler called at new color selection.
        /// Method change color at all selected bricks.
        /// </summary>
        void OnColorSelected()
        {
            if (colorButton == null)
                return;

            var bricks = SelectionManager.instance.GetSelected();
            foreach (var brick in bricks)
                brick.color = colorButton.selectedColor;
        }
    }
}
    