using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AgaQ.UI
{
    public abstract class BricksList : MonoBehaviour
    {
        [SerializeField] protected Slider scaleSlider;
        [SerializeField] protected int minCellSize = 40;
        [SerializeField] protected int defaultCellSize = 70;
        [SerializeField] protected int maxCellSize = 120;
        [SerializeField] protected RectTransform listContainer;

        protected BaseBrickButton[] brickButtons;
        protected Queue<BaseBrickButton> buttonsToLoadIcon = new Queue<BaseBrickButton>();

        GridLayoutGroup gridLayoutGroup;

        //current filters
        int groupFilter = 0;
        string nameFilter = "";

        protected void Start()
        {
            scaleSlider.value = (float)(defaultCellSize - minCellSize) / (float)(maxCellSize - minCellSize);
            scaleSlider.onValueChanged.AddListener(delegate { ScaleSliderValueChanged(); });
            ScaleSliderValueChanged();

            //run refreshing incons coroutine
            StartCoroutine(LoadIcons());

            GetComponentInParent<ScrollRect>().onValueChanged.AddListener(UpdateIcons);
        }

        #region Public functions

        /// <summary>
        /// Coroutine that loads asynchronusly icons for all brick buttons at the list
        /// </summary>
        IEnumerator LoadIcons()
        {
            while (true)
            {
                if (buttonsToLoadIcon.Count > 0)
                {
                    int every = 3;
                    while (buttonsToLoadIcon.Count > 0)
                    {
                        var button = buttonsToLoadIcon.Dequeue();
                        if (!button.isLoaded)
                        {
                            button.LoadIcon();

                            every--;
                            if (every == 0)
                            {
                                every = 3;
                                yield return null;
                            }
                        }
                    }
                }
                else
                    yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// Filter buttons by group.
        /// </summary>
        /// <param name="group">Group.</param>
        public void SetGroupFilter(int group)
        {
            groupFilter = group;
            UpdateListByFilter();
            UpdateIcons(Vector2.zero);
        }

        public void SetNameFilter(string name)
        {
            nameFilter = name;
            UpdateListByFilter();
            UpdateIcons(Vector2.zero);
        }

        /// <summary>
        /// Call this function after layout change or scroll,
        /// to check if actually visible buttons has loaded icons.
        /// </summary>
        public void UpdateIcons(Vector2 value)
        {
            //calculate list container y coordinate range
            Vector3[] corners = new Vector3[4];
            listContainer.GetWorldCorners(corners);
            Vector2 yRange = new Vector2(corners[0].y, corners[1].y);

            buttonsToLoadIcon.Clear();

            foreach (var button in brickButtons)
            {
                if (!button.isLoaded && IsButtonVisible(button, yRange))
                    buttonsToLoadIcon.Enqueue(button);
            }
        }

        #endregion

        #region Protected functions

        /// <summary>
        /// Check if button is visible in list container window.
        /// </summary>
        /// <returns><c>true</c>, if button visible was ised, <c>false</c> otherwise.</returns>
        /// <param name="button">Button.</param>
        /// <param name="yRange">Y range of the conatainer x is y min, y is y max.</param>
        protected bool IsButtonVisible(BaseBrickButton button, Vector2 yRange)
        {
            Vector3[] corners = new Vector3[4];
            button.rectTransform.GetWorldCorners(corners);
            if ((corners[0].y > yRange.x && corners[0].y < yRange.y) ||
                (corners[1].y > yRange.x && corners[1].y < yRange.y))
                return true;

            return false;
        }

        protected void UpdateListByFilter()
        {
            foreach (var button in brickButtons)
                button.gameObject.SetActive(CheckFilter(button));
        }

        /// <summary>
        /// Check if given button shuld bw visible with current filters setting.
        /// </summary>
        /// <returns><c>true</c>, if should be visible, <c>false</c> otherwise.</returns>
        /// <param name="button">Button.</param>
        protected bool CheckFilter(BaseBrickButton button)
        {
            return
                (groupFilter == 0 ||
                 button.group == groupFilter) &&
                (nameFilter == "" ||
                 button.name.IndexOf(nameFilter, StringComparison.OrdinalIgnoreCase) > 0);
        }

        #endregion

        #region Private functions

        void ScaleSliderValueChanged()
        {
            if (gridLayoutGroup == null)
                gridLayoutGroup = GetComponent<GridLayoutGroup>();

            float cellSize = (maxCellSize - minCellSize) * scaleSlider.value + minCellSize;
            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        }

        #endregion
    }
}
