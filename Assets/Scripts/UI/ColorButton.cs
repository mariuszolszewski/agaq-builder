using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace AgaQ.UI
{
    [RequireComponent(typeof(Image))]
    public class ColorButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] ColorPalette colorPalette;
        [SerializeField] Vector2 positionOffset;

        public Color selectedColor
        {
            get;
            protected set;
        }

        public Action OnColorSelected;

        Image _image;
        Image image {
            get {
                if (_image == null)
                    _image = GetComponent<Image>();

                return _image;
            }
        }

        void Awake()
        {
            selectedColor = image.color;
            colorPalette?.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //set panel position
            var rectTransform = GetComponent<RectTransform>();
            Vector3 pos = transform.position;
            pos.x -= rectTransform.rect.width / 2 - positionOffset.x;
            pos.y -= rectTransform.rect.height / 2 - positionOffset.y;
            colorPalette.transform.position = pos;

            colorPalette.gameObject.SetActive(!colorPalette.gameObject.activeSelf);
            image.color = selectedColor;

            colorPalette.OnColorSelected = SetColor;
            colorPalette.OnColorConfirmed = ConfirmColor;
            colorPalette.SetPrevColor(image.color);
        }

        /// <summary>
        /// Set temporary color selection
        /// </summary>
        /// <param name="color">Color.</param>
        public void SetColor(Color color)
        {
            image.color = color;           
        }

        /// <summary>
        /// Confirm selected color and close color pallete
        /// </summary>
        public void ConfirmColor()
        {
            selectedColor = image.color;

            if (OnColorSelected != null)
                OnColorSelected();
        }
    }
}
