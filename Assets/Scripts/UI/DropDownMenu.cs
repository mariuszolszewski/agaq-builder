using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace AgaQ.UI
{
    public delegate void PointerDelegate(PointerEventData eventDat);

    /// <summary>
    /// Scriot for dropdownmenu. Don't use manualy. It's used automaticly with DropDownMenuButton.
    /// </summary>
    public class DropDownMenu : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        public event PointerDelegate PointerEnter;
        public event PointerDelegate PointerExit;

        void Awake()
        {
            var buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
                button.onClick.AddListener(MenuButtonClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit(eventData);
        }

        public void MenuButtonClick()
        {
              StartCoroutine(CloseMenu());
        }

        IEnumerator CloseMenu()
        {
            yield return null;
            gameObject.SetActive(false);
        }
    }
}
