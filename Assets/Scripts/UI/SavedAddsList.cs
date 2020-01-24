using UnityEngine;
using AgaQ.Bricks.Serialization;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

namespace AgaQ.UI
{
    /// <summary>
    /// Controller for list of saved adds.
    /// </summary>
    public class SavedAddsList : BricksList
    {
        [SerializeField] GameObject buttonPrefab;

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
            var dir = Preferences.instance.addsSavePath;
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
            }

            brickButtons = GetComponentsInChildren<SavedBricsGroupButton>();
            UpdateIcons(Vector2.zero);
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
