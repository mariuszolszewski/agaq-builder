using UnityEngine;
using UnityEngine.UI;
using AgaQ.Bricks.Groups;
using Lean.Localization;
using System.Collections.Generic;
using System.Linq;

namespace AgaQ.UI.Groups
{
    [RequireComponent(typeof(Dropdown))]
    public class BricsGroupsDropdown : MonoBehaviour
    {
        [SerializeField] string groupsResourcesPath;

        BricksGroups groups;
        Dropdown dropdown;
        bool isInitialized = false;

        void OnEnable()
        {
            if (!isInitialized)
            {
                //find dropdown controll
                dropdown = GetComponent<Dropdown>();

                //load resources
                groups = Resources.Load(groupsResourcesPath) as BricksGroups;

                //rebuild dropdown controll
                dropdown.ClearOptions();
                if (groups != null)
                {
                    List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

                    foreach (var group in groups.groups)
                    {
                        var option = new GroupsDropDownOptionData();

                        var translation = LeanLocalization.GetTranslation(group.translationLabel);
                        option.text = translation != null ? translation.Text : group.translationLabel;
                        option.groupId = group.groupId;

                        options.Add(option);
                    }
                       
                    var sortedOptions = options.OrderBy(x => x.text).ToList();
                    dropdown.AddOptions(sortedOptions);
                }

                isInitialized = true;
            }
        }
    }
}
