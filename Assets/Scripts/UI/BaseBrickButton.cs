using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace AgaQ.UI
{
    public abstract class BaseBrickButton : ToolTipLocalized, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Space]
        [SerializeField] protected Color normalTint = Color.white;
        [SerializeField] protected Color hoverTint = new Color(0.96f, 0.96f, 0.96f);
        [SerializeField] protected Color clickTInt = new Color(0.85f, 0.85f, 0.85f);

        [NonSerialized] public bool isLoaded;

        RectTransform _rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public int group;
        protected Image image;

        protected void Start()
        {
            image = GetComponent<Image>();
            image.color = normalTint;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {}

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            image.color = hoverTint;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            image.color = normalTint;
        }

        /// <summary>
        /// Load button icon.
        /// </summary>
        public abstract void LoadIcon();
    }
}
