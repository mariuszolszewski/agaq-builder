using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AgaQ.UI;

namespace AgaQ.UI
{
    [RequireComponent(typeof(Image))]
    public class BrickGroupButton : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] int group;
        [SerializeField] BricksList bricksList;

        [SerializeField] Color normalTint = Color.white;
        [SerializeField] Color hoverTint = new Color(0.96f, 0.96f, 0.96f);
        [SerializeField] Color selectedTint = Color.red;

        Image image;
        bool selected;

        void Start()
        {
            image = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bricksList.SetGroupFilter(selected ? 0 : group);
            if (!selected)
            {
                var slibings = transform.parent.GetComponentsInChildren<BrickGroupButton>();
                foreach (var button in slibings)
                    button.Reset();
            }
            selected = !selected;
            image.color = selected ? selectedTint : normalTint;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!selected)
                image.color = hoverTint;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.color = selected ? selectedTint : normalTint;
        }

        /// <summary>
        /// Restore initial state of the button
        /// </summary>
        public void Reset()
        {
            selected = false;
            image.color = normalTint;
        }
    }
}
