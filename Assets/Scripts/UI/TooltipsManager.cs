using UnityEngine;
using UnityEngine.UI;

namespace AgaQ.UI
{
    /// <summary>
    /// This class manage tooltips and show tem when needed.
    /// This functionality is centralized due to performace reasons to prevent
    /// a lot of Update calls across all tooltips elements.
    /// </summary>
    public class TooltipsManager : MonoBehaviour
    {
        [Tooltip("How long pointer have to stay still befor tootip will appear.")]
        [SerializeField] float tooltipTime = 0.5f;
        [SerializeField] Vector3 positionOffset;
        [SerializeField] GameObject tooltipObject;
        [SerializeField] Text tooltipText;

        public static TooltipsManager instance;

        ToolTipLocalized currentTooltip;
        float pointerStillTime; //time when pointer become still over tooltip
        bool pointerOverTooltip = false;
        bool pointerStill = false;
        Vector3 mousePosition;

        bool tooltipVisible = false; //is tooltip visible

        #region Event handlers

        void Start()
        {
            if (instance == null)
            {
                instance = this;
                tooltipObject.SetActive(false);
            }
            else if (instance != this)
                Destroy(this.gameObject);
        }

        void Update()
        {
            if (pointerOverTooltip)
            {
                //show tooltip when pointer is still for some time
                if (pointerStill && Time.time - pointerStillTime >= tooltipTime)
                {
                    tooltipText.text = currentTooltip.tooltipText;
                    tooltipObject.transform.position = mousePosition + positionOffset;
                    tooltipObject.SetActive(true);
                    tooltipVisible = true;
                }
                //hide tooltip when needed
                else if (tooltipVisible && !pointerStill)
                {
                    tooltipObject.SetActive(false);
                    tooltipVisible = false;
                }

                //check if pointer is still
                if (pointerStill)
                {
                    if (mousePosition != Input.mousePosition)
                        pointerStill = false;
                }
                else if (mousePosition == Input.mousePosition)
                {
                    pointerStill = true;
                    pointerStillTime = Time.time;
                }

                else
                    mousePosition = Input.mousePosition;
            }
        }

        #endregion

        #region Public functions

        public void OnPointerEnterTooltip(ToolTipLocalized tooltip)
        {
            currentTooltip = tooltip;
            mousePosition = Input.mousePosition;
            pointerOverTooltip = true;
        }

        public void OnPointerExitTooltip()
        {
            pointerOverTooltip = false;
            pointerStill = false;
            tooltipObject.SetActive(false);

        }

        #endregion
    }
}
