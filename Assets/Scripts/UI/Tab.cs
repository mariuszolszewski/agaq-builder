using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AgaQ.UI
{
    public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] Sprite normalSprite;
        [SerializeField] Sprite selectedSprite;
        [SerializeField] Color normalTint = Color.white;
        [SerializeField] Color hoverTint = new Color(0.9f, 0.9f, 0.9f, 1f);
        [SerializeField] GameObject tabPanel;

        [HideInInspector]
        public bool isSelected;

        Image backgroundImage;

        #region Event Handlers

        void Start()
        {
            backgroundImage = GetComponent<Image>();
            if (backgroundImage != null)
                backgroundImage.color = normalTint;

            //check if ther is firs selected
            var firstSlibingTab = transform.parent.GetComponentInChildren<Tab>();
            if (firstSlibingTab != null && !firstSlibingTab.isSelected)
                firstSlibingTab.Select(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (backgroundImage != null && !isSelected)
                backgroundImage.color = hoverTint;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (backgroundImage != null)
                backgroundImage.color = normalTint;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select(true);
        }

        #endregion

        public void Select(bool selected)
        {
            if (selected == isSelected)
                return;

            if (selected)
            {
                // deselect other tab
                var slibingsTabs = transform.parent.GetComponentsInChildren<Tab>();
                foreach (var otherTab in slibingsTabs)
                {
                    if (otherTab != this && otherTab.isSelected)
                        otherTab.Select(false);
                }

                //slect me
                if (backgroundImage != null)
                {
                    backgroundImage.sprite = selectedSprite;
                    backgroundImage.color = normalTint;
                }
            }
            else
            {
                if (backgroundImage != null)
                    backgroundImage.sprite = normalSprite;
            }

            tabPanel.SetActive(selected);
            isSelected = selected;
        }
    }
}
