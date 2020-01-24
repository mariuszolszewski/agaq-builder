using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AgaQ.UI
{
    /// <summary>
    /// Script for UI brick list at library panel.
    /// It contains lazy loading dunctionality for brick button icons.
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    public class AgaQBricksList : BricksList
    {
        protected new void Start()
        {
            brickButtons = GetComponentsInChildren<BrickButton>();

            base.Start();

            //update iconds to refresh on start
            StartCoroutine(UpdateIconsAtTheStart());
        }

        /// <summary>
        /// Run icons refresh and load just afetr first frame,
        /// to be shure that all canvas layout is finished.
        /// </summary>
        IEnumerator UpdateIconsAtTheStart()
        {
            yield return null;
            UpdateIcons(Vector2.zero);
        }
    }
}
