using UnityEngine;
using UnityEngine.UI;
using AgaQ.Bricks;
using AgaQ.UI.Dialogs;
using AgaQ.Bricks.Tools;
using System.IO;

namespace AgaQ.UI.Groups
{
    /// <summary>
    /// Class to controll group saving dialog.
    /// </summary>
    public class SaveGroupDialog : MonoBehaviour
    {
        [SerializeField] GameObject dialog;
        [SerializeField] InputField nameField;
        [SerializeField] Dropdown dropDownGroup;
        [SerializeField] SavedBricksList groupsList;
        [SerializeField] SavedBricksList addsList;

        void OnEnable()
        {
            //clear fields
            nameField.text = "";
        }

        /// <summary>
        /// Chaeck if dialog can be opened and open it or not.
        /// </summary>
        public void OpenDialog()
        {
            var sm = SelectionManager.instance;
            var selected = sm.GetSelected();
            if (sm.SelectedAmount == 1 && (selected[0] is AgaQGroup || selected[0] is BricksGroup || selected[0] is OrdinaryBrick))
                dialog.SetActive(true);
        }

        public void Save()
        {
            if (nameField.text == "")
            {
                Dialog.ShowTranslatedInfo("FillInNameField");
                return;
            }

            var prefs = Preferences.instance;
            var selected = SelectionManager.instance.GetSelected()[0];
            string path = string.Concat(
                selected is AgaQGroup ? prefs.agaQGroupsSavePath : prefs.addsSavePath,
                Path.DirectorySeparatorChar);
            int category = (dropDownGroup.options[dropDownGroup.value] as GroupsDropDownOptionData).groupId;
            string filePath = ToolsManager.instance.groupTool.SaveGroup(nameField.text, category, path);
            dialog.SetActive(false);

            //add new button to list
            if (selected is AgaQGroup)
                groupsList.AddNewFile(filePath);
            else
                addsList.AddNewFile(filePath);
        }
    }
}
