using UnityEngine;
using AgaQ.Bricks.Serialization;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

namespace AgaQ.UI
{
    public enum ListType
    {
        groups,
        adds
    }

    /// <summary>
    /// Controller for lsit of saved blocks.
    /// </summary>
    public class SavedBricksList : BricksList
    {
        [SerializeField] GameObject buttonPrefab;
        [SerializeField] ListType listType;

        protected bool isInitialized = false;

        protected new void Start()
        {
            base.Start();

            StartCoroutine(LoadSavedGroups());
        }

        /// <summary>
        /// Add new button with saved group from file.
        /// </summary>
        /// <param name="path">Path.</param>
        public void AddNewFile(string path)
        {
            if (!isInitialized)
                return;
            
            BuildButton(path);
            brickButtons = GetComponentsInChildren<SavedBricsGroupButton>();
            UpdateListByFilter();
            UpdateIcons(Vector2.zero);
        }

        /// <summary>
        /// Coroutine to load all groups definitions from disk in bacground.
        /// </summary>
        /// <returns>The saved groups.</returns>
        IEnumerator LoadSavedGroups()
        {
            int every = 3;

            //read bricks
            var prefs = Preferences.instance;
            string dir = "";
            if (listType == ListType.groups)
                dir = prefs.agaQGroupsSavePath;
            else if (listType == ListType.adds)
                dir = prefs.addsSavePath;
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);
                foreach (var filePath in files)
                {
                    if (Path.GetExtension(filePath) != ".meta")
                        continue;

                    BuildButton(filePath);

                    every--;
                    if (every == 0)
                    {
                        brickButtons = GetComponentsInChildren<SavedBricsGroupButton>();
                        UpdateIcons(Vector2.zero);

                        every = 3;
                        yield return null;
                    }
                }

                yield return null;
                brickButtons = GetComponentsInChildren<SavedBricsGroupButton>();
                UpdateIcons(Vector2.zero);
            }

            isInitialized = true;
        }

        /// <summary>
        /// Build new button from definition at disk
        /// </summary>
        /// <param name="filePath">File path.</param>
        void BuildButton(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BricksGropuMetaData));

            try
            {
                //deserialize meta file
                StreamReader reader = new StreamReader(filePath);
                //reader.ReadToEnd();
                BricksGropuMetaData metaData = (BricksGropuMetaData)serializer.Deserialize(reader);
                reader.Close();

                //create button
                var buttonObject = Instantiate(buttonPrefab);
                buttonObject.transform.SetParent(listContainer.transform);
                var button = buttonObject.GetComponent<SavedBricsGroupButton>();
                button.Init(metaData, filePath.Substring(0, filePath.Length - Path.GetExtension(filePath).Length));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
