using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace AgaQ.UI
{
    public class ColorPaletteTile : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        public Action<Color> OnHover;
        public Action<Color> OnCLick;

        Image _image;
        Image image
        {
            get {
                if (_image == null)
                    _image = GetComponent<Image>();

                return _image;
            }
        }

        public Color color
        {
            get {
                return image.color;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnCLick?.Invoke(image.color);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(image.color);
        }
    }
}
