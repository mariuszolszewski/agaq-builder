using UnityEngine;
using UnityEngine.UI.Extensions;
using AgaQ.Bricks.Groups;
using UnityEngine.UI;
using Lean.Localization;
using System.Reflection.Emit;

namespace AgaQ.UI.Groups
{
    /// <summary>
    /// Lazy initialization for whell menu with briks grups
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(HorizontalScrollSnap))]
    public class BricksGroupsWeelMenu : MonoBehaviour
    {
        [SerializeField] string groupsResourcesPath;
        [SerializeField] BricksList list;
        [SerializeField] Text label;

        HorizontalScrollSnap scrollSnap;
        ScrollRect scrollRect;
        BricksGroups groups;
        bool isInitialized = false;

        void Awake()
        {
            scrollSnap = GetComponent<HorizontalScrollSnap>();
            scrollRect = GetComponent<ScrollRect>();

            scrollSnap.OnSelectionPageChangedEvent.AddListener(OnSelectionChanging);
            scrollSnap.OnSelectionChangeEndEvent.AddListener(OnSelectionChange);
        }

        void OnEnable()
        {
            if (isInitialized)
                return;

            //load resources
            groups = Resources.Load(groupsResourcesPath) as BricksGroups;

            //rebuild dropdown controll
            if (groups != null)
            {
                GameObject allButton = scrollRect.content.transform.Find("All").gameObject;
                if (allButton != null)
                {
                    foreach (var group in groups.groups)
                    {
                        var newButton = Instantiate(allButton);
                        newButton.transform.SetParent(scrollRect.content);

                        var image = newButton.GetComponentInChildren<Image>();
                        image.sprite = group.icon;

                        var toolTip = newButton.GetComponentInChildren<ToolTipLocalized>();
                        toolTip.PhraseName = group.translationLabel;
                    }
                }

                if (label != null)
                {
                    if (groups.groups.Length > 0)
                        SetLabel("All");

                }
                else
                    label.text = "";
            }
            isInitialized = true;
        }

        void OnSelectionChanging(int currentPage)
        {
            if (currentPage == 0)
                SetLabel("All");
            else
                SetLabel(groups.groups[currentPage - 1].translationLabel);
        }

        void OnSelectionChange(int currentPage)
        {
            if (scrollSnap.CurrentPage == 0)
                list.SetGroupFilter(0);
            else
                list.SetGroupFilter(groups.groups[scrollSnap.CurrentPage - 1].groupId);
        }

        void SetLabel(string translationLabel)
        {
            var translation = LeanLocalization.GetTranslation(translationLabel);
            label.text = translation != null ? translation.Text : translationLabel;
        }
    }
}
