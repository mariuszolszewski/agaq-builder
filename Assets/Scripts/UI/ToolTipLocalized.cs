using UnityEngine.EventSystems;
using Lean.Localization;
using UnityEngine;

namespace AgaQ.UI
{
    /// <summary>
    /// Tooltip component. Shows tooltip over element.
    /// Time and apperance are defined at TooltipManager.
    /// </summary>
    public class ToolTipLocalized : LeanLocalizedBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string paramText;

        [HideInInspector]
        public string tooltipText;

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltipText != "")
                TooltipsManager.instance.OnPointerEnterTooltip(this);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            TooltipsManager.instance.OnPointerExitTooltip();
        }

        /// <summary>
        /// This gets called every time the translation needs updating.
        /// </summary>
        /// <param name="translation">Translation.</param>
        public override void UpdateTranslation(LeanTranslation translation)
        {
            if (translation != null)
                tooltipText = string.Format(translation.Text, paramText);
            else
                tooltipText = PhraseName;
        }
    }
}
