using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace AgaQ.UI
{
    /// <summary>
    /// Script for dropdown menu button.
    /// </summary>
    public class DropDownMenuButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler
    {
        public GameObject menuPanel;

        [Tooltip("Close menu after this time from PointerExit event.")]
        [SerializeField] float closeTime = 0.2f;
        [SerializeField] Vector2 positionOffset;

        IEnumerator closeCoorutine;

        void Awake()
        {
            if (menuPanel != null)
            {
                var script = menuPanel.AddComponent<DropDownMenu>();
                script.PointerEnter += OnPointerEnter;
                script.PointerExit += OnPointerExit;

                menuPanel.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (menuPanel != null)
            {
                //show panel
                menuPanel.SetActive(!menuPanel.activeSelf);

                if (menuPanel.activeSelf)
                    SetMenuPanelPosition();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (menuPanel.activeSelf)
                StopCoroutine(closeCoorutine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            closeCoorutine = CloseCoroutine();
            StartCoroutine(closeCoorutine);
        }

        IEnumerator CloseCoroutine()
        {
            yield return new WaitForSeconds(closeTime);
            menuPanel.SetActive(false);
        }

        void SetMenuPanelPosition()
        {
            //set panel position coreponding to button position
            var rectTransform = GetComponent<RectTransform>();
            var pos = transform.position;
            pos.x -= rectTransform.rect.width / 2 - positionOffset.x;
            pos.y -= rectTransform.rect.height / 2 - positionOffset.y;
            menuPanel.transform.position = pos;
        }
    }
}
