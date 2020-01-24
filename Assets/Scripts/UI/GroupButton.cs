using UnityEngine;
using AgaQ.Bricks;
using System.Collections.Generic;
using AgaQ.Bricks.Tools;

namespace AgaQ.UI
{
    /// <summary>
    /// Button connected with other in pair but only one of them can be visible
    /// </summary>
    public class GroupButton : MonoBehaviour
    {
        [SerializeField] ToolButton groupButton;
        [SerializeField] ToolButton ungroupButton;

        #region Events handlers

        void Start()
        {
            SetGroupButton(true);

            SelectionManager.instance.OnSelectionChange += OnSelectionChange;
        }

        /// <summary>
        /// Event handler run on brick selection change.
        /// It adjust button visiblity.
        /// </summary>
        /// <param name="selectedBricks">Selected bricks.</param>
        void OnSelectionChange(List<SelectableBrick> selectedBricks)
        {
            bool group = selectedBricks.Count == 1 && selectedBricks[0] is AgaQGroup;
            //SetGroupButton(!AreAllGroups(selectedBricks));
            SetGroupButton(!group);
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Use group tool and change buttons visibility.
        /// </summary>
        public void Group()
        {
            //chekc if is selected are all groups
            if (SelectionManager.instance.SelectedAmount > 1)
            {
                //use group tool to group
                ToolsManager.instance.groupTool.Group();

                SetGroupButton(false);
            }
        }

        /// <summary>
        /// Use group tool and change buttons visibility.
        /// </summary>
        public void Ungroup()
        {
            //chekc if is selected are all groups
            if (AreAllGroups(SelectionManager.instance.GetSelected()))
            {
                //use group tool to ungroup
                ToolsManager.instance.groupTool.Ungroup();

                //changebuttons visibility
                SetGroupButton(true);
            }
        }

        #endregion

        /// <summary>
        /// Set viibility of the group/ungroup buttons.
        /// </summary>
        /// <param name="showGroupButton">If set to <c>true</c> group button is visible otherwise ungroup button is visible.</param>
        void SetGroupButton(bool showGroupButton)
        {
            groupButton.gameObject.SetActive(showGroupButton);
            ungroupButton.gameObject.SetActive(!showGroupButton);
        }

        /// <summary>
        /// Check if on the list are only groups
        /// </summary>
        /// <returns><c>true</c>, if all are group, <c>false</c> otherwise.</returns>
        /// <param name="selectedBricks">Selected bricks.</param>
        bool AreAllGroups(List<SelectableBrick> selectedBricks)
        {
            if (selectedBricks.Count > 0)
            {
                //check if all selected bricks are groups
                bool allGroups = true;
                foreach (var brick in selectedBricks)
                {
                    if (!(brick is AgaQGroup))
                    {
                        allGroups = false;
                        break;
                    }
                }

                return allGroups;
            }

            return false;
        }
    }
}
