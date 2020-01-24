using UnityEngine;
using System.Collections.Generic;
using AgaQ.Bricks.History;
using AgaQ.Bricks.Serialization;
using AgaQ.UI.Dialogs;
using System.IO;
using System.Xml.Serialization;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Tool to join bricks into groups.
    /// </summary>
    public class GroupTool
    {
        /// <summary>
        /// Group selected bricks.
        /// </summary>
        public void Group()
        {
            var selectedBricks = SelectionManager.instance.GetSelected();

            //if is one or less selected bricks there is nothing to do, so return
            if (selectedBricks.Count <= 1)
                return;

            var groupScript = Group(selectedBricks);

            //move selection to newly created group
            SelectionManager.instance.Clear();
            SelectionManager.instance.Add(groupScript);

            HistoryManager.instance.Register(new HistoryNodeGroup(selectedBricks));
        }

        /// <summary>
        /// Group given bricks
        /// </summary>
        /// <returns>The group.</returns>
        /// <param name="bricsToGroup">Brics to group.</param>
        public SelectableBrick Group(List<SelectableBrick> bricsToGroup)
        {
            bool allOrdinary = true;
            foreach (var brick in bricsToGroup)
            {
                if (!(brick is OrdinaryBrick))
                {
                    allOrdinary = false;
                    break;
                }
            }

            //create group object and prepare it
            SelectableBrick groupScript = allOrdinary ?
                (SelectableBrick)BrickBuilder.InstansiateBricksGroup() :
                (SelectableBrick)BrickBuilder.InstansiateAgaQGroup();

            //set group center
            groupScript.gameObject.transform.position = bricsToGroup[0].gameObject.transform.position;

            //move selected brck to group
            //and disable brick scripts
            foreach (var brick in bricsToGroup)
            {
                brick.transform.SetParent(groupScript.gameObject.transform);
                brick.grouped = true;
            }

            return groupScript;
        }

        /// <summary>
        /// Ungroup selected bricks.
        /// </summary>
        public void Ungroup()
        {
            var selectedBricks = SelectionManager.instance.GetSelected();

            //if nothing is seleceted there is nothig to do, so return
            if (selectedBricks.Count == 0)
                return;

            //iterate over selection
            foreach (var selectedBrick in selectedBricks)
            {
                if (selectedBrick is AgaQGroup)
                {
                    //remove group from selection
                    SelectionManager.instance.Remove(new List<SelectableBrick> { selectedBrick });

                    Ungroup(selectedBrick.GetComponent<AgaQGroup>());
                }
            }

            HistoryManager.instance.Register(new HistoryNodeUngroup(selectedBricks));
        }

        /// <summary>
        /// Ungroup group.
        /// </summary>
        /// <returns>The ungroup.</returns>
        /// <param name="group">Group.</param>
        public void Ungroup(SelectableBrick group)
        {
            if (!(group is AgaQGroup) && !(group is BricksGroup))
                return;
            
            //move all brick from group outside,
            //add it to selection and enalble bricks scripts
            for (var i = group.transform.childCount - 1; i >= 0; i--)
            {
                var child = group.transform.GetChild(i);

                child.SetParent(group.transform.parent);

                //enable brick script
                var brickScript = child.GetComponent<Brick>();
                if (brickScript != null)
                    brickScript.grouped = false;

                //add birkc to selection if it is selectable
                var selectable = child.GetComponent<SelectableBrick>();
                if (selectable != null)
                    SelectionManager.instance.Add(selectable);
            }

            //remove group object
            Object.Destroy(group.gameObject);
        }

        /// <summary>
        /// Save currently selected group.
        /// </summary>
        /// <param name="name">group name</param>
        /// <param name="category">group category</param>
        /// <param name="path">path ehre to save group</param>
        /// <returns>File path where data where saved.</returns>
        public string SaveGroup(string name, int category, string path)
        {
            if (SelectionManager.instance.SelectedAmount < 1)
                return "";

            var toSave = SelectionManager.instance.GetSelected()[0];
            var brickToSave = toSave.GetComponent<Brick>();

            if (brickToSave == null)
                return "";

            var separator = Path.DirectorySeparatorChar;
            var fileName = name;
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c, '_');
            while (
                File.Exists(string.Concat(path, separator, fileName, ".meta")) ||
                File.Exists(string.Concat(path, separator, fileName, ".png")) ||
                File.Exists(string.Concat(path, separator, fileName, ".aga")))
            {
                fileName += "_";
            }

            try
            {
                //save meta file
                BricksGropuMetaData meta = new BricksGropuMetaData();
                meta.name = name;
                meta.category = category;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string filePath = string.Concat(path, separator, fileName, ".meta");
                using (var file = File.Create(filePath))
                {
                    var x = new XmlSerializer(meta.GetType());
                    x.Serialize(file, meta);
                }

                //make icon and save it
                BrickUtils.GenerateIcon(brickToSave, string.Concat(path, separator, fileName, ".png"), 256);

                //save group
                Model.instance.Serialize(string.Concat(path, separator, fileName, ".aga"), brickToSave);

                return filePath;
            }
            catch
            {
                try { File.Delete(string.Concat(path, separator, fileName, ".meta")); } catch {}
                try { File.Delete(string.Concat(path, separator, fileName, ".png")); } catch {}
                try { File.Delete(string.Concat(path, separator, fileName, ".aga"));} catch {}
                Dialog.ShowTranslatedInfo("CantSaveGroup");
                return "";
            }
        }
    }
}
