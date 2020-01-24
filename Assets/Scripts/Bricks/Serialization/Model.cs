using UnityEngine;
using SFB;
using Lean.Localization;
using AgaQ.Bricks.History;
using AgaQ.Bricks.Joints;
using AgaQ.Bricks.Tools;
using AgaQ.Bricks.Serialization.STL;
using AgaQ.UI.Dialogs;
using System.IO;
using System;
using System.Text;

namespace AgaQ.Bricks.Serialization
{
    /// <summary>
    /// Edited model.
    /// </summary>
    public class Model : ModelDeserializer
    {
        public static Model instance;

        string fileName;
        bool neverSaved = true;

        #region Event handlers

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Ask i shure to clear model and do it.
        /// </summary>
        public void New()
        {
            Dialog.ShowQuesttion(
                LeanLocalization.GetTranslation("UnsavedQuestion").Text,
                DialogButtonType.no,
                DialogButtonType.yes,
                DoNew);
        }

        /// <summary>
        /// Open file dialog than load choosen model.
        /// </summary>
        public void Load()
        {
            var label = LeanLocalization.GetTranslation("Open").Text;
            var fileNames = StandaloneFileBrowser.OpenFilePanel(label, "", "aga", false);

            if (fileNames.Length > 0 && fileNames[0] != "")
            {
                Load(fileNames[0]);
                JointsUtils.RebuildJoints();
            }
        }

        /// <summary>
        /// Popup choose file name dialog if needed and save model.
        /// </summary>
        public void Save()
        {
            if (neverSaved)
                SaveAs();
            else
                Save(this.fileName);
        }

        /// <summary>
        /// Popup choose filename dialog and save model.
        /// </summary>
        public void SaveAs()
        {
            if (name == "")
                name = LeanLocalization.GetTranslation("Untitled").Text;
            
            var label = LeanLocalization.GetTranslation("SaveAs").Text;
            var newFileName = StandaloneFileBrowser.SaveFilePanel(label, "", fileName, "aga");

            if (fileName != "")
                Save(newFileName);
        }

        /// <summary>
        /// Popu[ file choose dialog and next call sutable import method.
        /// </summary>
        public void Import()
        {
            var label = LeanLocalization.GetTranslation("Export").Text;
            var fileNames = StandaloneFileBrowser.OpenFilePanel(label, "", "stl", false);
            if (fileNames.Length == 0)
                return;

            if (Path.GetExtension(fileNames[0]).ToLower() == ".stl")
            {
                if (fileNames[0].Contains("file:"))
                    fileNames[0] = fileNames[0].Substring(7);
                fileNames[0] = Decode(fileNames[0]);

                try
                {
                    var meshes = STLImporter.Import(fileNames[0]);
                    var brick = BrickBuilder.Instansiate(meshes);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);

                    Dialog.ShowInfo(
                        LeanLocalization.GetTranslation("ImportFailed").Text,
                        DialogButtonType.ok);
                }
            }
        }

        /// <summary>
        /// Popup file choose dialog and call suitable export methothod acording to choosed file type.
        /// </summary>
        public void Export()
        {
            var label = LeanLocalization.GetTranslation("Export").Text;
            var newFileName = StandaloneFileBrowser.SaveFilePanel(label, "", fileName, "stl");

            if (newFileName != "")
                STLExporter.Export(newFileName, new GameObject[] { gameObject }, FileType.Binary);
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Clear model.
        /// </summary>
        /// <param name="answer">If set to <c>true</c> clear, otherwise not.</param>
        void DoNew(bool answer)
        {
            if (answer)
            {
                //reset model name
                fileName = LeanLocalization.GetTranslation("Untitled").Text;
                neverSaved = true;

                ClearModel();
            }
        }

        /// <summary>
        /// Clear model with its history and selections.
        /// </summary>
        void ClearModel()
        {
            //cancel current tool operation
            ToolsManager.instance.Cancel();

            //deselect all bricks
            SelectionManager.instance.Clear();

            //clear history
            HistoryManager.instance.ClearAll();

            //clear model
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);            
        }

        /// <summary>
        /// Load model from file. Before load clear old one.
        /// </summary>
        /// <param name="fileName">File name.</param>
        void Load(string fileName)
        {
            this.fileName = fileName;

            if (fileName.Contains("file:"))
                fileName = fileName.Substring(7);

            fileName = fileName.Replace("%20", " ");

            ClearModel();
            neverSaved = false;

            try
            {
                Deserialize(fileName);
            }
            catch
            {
                ClearModel();
                Dialog.ShowInfo(
                    LeanLocalization.GetTranslation("LoadFailed").Text, 
                    DialogButtonType.ok);
            }
        }

        /// <summary>
        /// Saves model to file.
        /// </summary>
        /// <returns>The save.</returns>
        /// <param name="fileName">File name.</param>
        void Save(string fileName)
        {
            this.fileName = fileName;
            if (fileName.Contains("file:"))
                fileName = fileName.Substring(7);

            neverSaved = false;

            try
            {
                Serialize(fileName);
            }
            catch
            {
                Dialog.ShowInfo(
                    LeanLocalization.GetTranslation("SaveFailed").Text,
                    DialogButtonType.ok);                
            }
        }

        string Decode(string text)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '%')
                {
                    builder.Append((char)Convert.ToInt32(text.Substring(i + 1, 2), 16));
                    i += 2;
                }
                else
                    builder.Append(text[i]);
            }

            return builder.ToString();
        }

        #endregion
    }
}
