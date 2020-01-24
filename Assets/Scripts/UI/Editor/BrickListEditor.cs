using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using AgaQ.UI;
using AgaQ.Bricks;
using System.IO;

/// <summary>
/// Custom editor for BrickList
/// </summary>
[CustomEditor(typeof(BricksList), true)]
public class BrickListEditor : Editor
{
    int counter;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Rebuild bricks buttons") &&
            EditorUtility.DisplayDialog("Rebuild all buttons?", "Are you sure?", "rebuild", "cancel"))
        {
            RebuildBricksButtons((BricksList)target);
        }

        if (GUILayout.Button("Rebuild all icons") && 
            EditorUtility.DisplayDialog("Rebuild all icons?", "Are you sure?", "rebuild", "cancel"))
        {
            RebuildButtonsIcons((BricksList)target);
        }

        if (GUILayout.Button("Refresh bricks descriptions"))
        {
            RebuildButtonsIcons((BricksList)target);
        }
    }

    void RefreshBricksNames(BricksList list)
    {
        var buttons = list.gameObject.GetComponentsInChildren<BrickButton>();
        foreach (var button in buttons)
        {
            var go = Resources.Load(button.brickResourcePath) as GameObject;
            var brick = go.GetComponent<Brick>();
            button.paramText = brick.name;
            button.tooltipText = "{0}";
        }
    }

    /// <summary>
    /// Rebuild all buttons icons.
    /// </summary>
    /// <param name="">.</param>
    void RebuildButtonsIcons(BricksList list)
    {
        var buttons = list.gameObject.GetComponentsInChildren<BrickButton>();
        foreach (var button in buttons)
        {
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                //generate icon
                var brick = Resources.Load(button.brickResourcePath) as GameObject;
                string path = string.Format("Assets/Resources/Icons/{0}.png", brick.name);
                BrickUtils.GenerateIcon(brick.GetComponent<Brick>(), path, 256);
            }
        }
    }

    /// <summary>
    /// Rebuild from scratch whole buttons lists
    /// </summary>
    void RebuildBricksButtons(BricksList list)
    {
        //clear existing list
        var buttons = list.GetComponentsInChildren<BrickButton>();

        counter = buttons.Length;
        var separator = Path.DirectorySeparatorChar;
        RebuildBricksFromDirectory(list, string.Concat("Assets", separator, "Resources", separator, "AgaQ", separator));
    }


    /// <summary>
    /// Rebuild buttons list from directory content.
    /// </summary>
    /// <param name="list">List.</param>
    /// <param name="path">Path.</param>
    void RebuildBricksFromDirectory(BricksList list, string path)
    {
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            if (file.EndsWith(".prefab"))
            {
                //check if button exists
                var button = FindButton(file, list);
                if (button == null)
                    GenerateButton(file, counter++, list.transform);
            }
        }

        var subPaths = Directory.GetDirectories(path);
        foreach (var subPath in subPaths)
            RebuildBricksFromDirectory(list, subPath);
    }

    /// <summary>
    /// Find button at given list with prefab path.
    /// </summary>
    /// <returns>The button.</returns>
    /// <param name="prefabPath">Prefab path.</param>
    /// <param name="list">List.</param>
    BrickButton FindButton(string prefabPath, BricksList list)
    {
        var buttons = list.GetComponentsInChildren<BrickButton>();
        string resourcesPath = ExtractResourcesPath(prefabPath);

        foreach (var button in buttons)
        {
            if (button.brickResourcePath == resourcesPath)
                return button;
        }

        return null;
    }

    /// <summary>
    /// Generate new brick button.
    /// </summary>
    /// <param name="file">Prefa file name.</param>
    /// <param name="number">Number added to name</param>
    /// <param name="parent">:arent node to new button.</param>
    void GenerateButton(string file, int number, Transform parent)
    {
        //create GameObject
        GameObject buttonObject = new GameObject();
        buttonObject.transform.SetParent(parent);
        buttonObject.name = string.Format("brick button {0}", number);

        //atache Image component
        Image image = buttonObject.AddComponent<Image>();

        //attach BrickButtonScript and configure it
        BrickButton button = buttonObject.AddComponent<BrickButton>();
        button.brickResourcePath = ExtractResourcesPath(file);

        //generate button icon 
        GameObject brickPrefab = (GameObject)EditorGUIUtility.Load(file);
        DragableBrick brick = brickPrefab.GetComponent<DragableBrick>();
        var s = Path.DirectorySeparatorChar;
        string iconPath = string.Format(string.Concat("Assets", s, "Resources", s, "Icons", s, "{0}.png"), brick.name);
        BrickUtils.GenerateIcon(brick, iconPath, 256);

        //configure icon
        button.imageResourcePath = ExtractResourcesPath(iconPath);
        string standardIconPath = string.Concat("Assets", s, "Gfx", s, "UI", s, "Icons", s, "BrickButtonIcon.png");
        //AssetDatabase.ImportAsset(standardIconPath);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(standardIconPath);
        image.sprite = sprite;

        //configure group
        if (file.Contains("AgaQ Lukowe Poziome i Pionowe STL"))
            button.group = 1;
        else if (file.Contains("AgaQ Prostopadłoscianowe STL"))
            button.group = 2;
        else if (file.Contains("AgaQ Rownolegloboczne poziome STL"))
            button.group = 3;
        else if (file.Contains("AgaQ Schodkowe STL"))
            button.group = 4;
        else if (file.Contains("AgaQ Skalowe przechodzace STL"))
            button.group = 5;
        else if (file.Contains("AgaQ Trapezowe STL"))
            button.group = 6;
    }

    /// <summary>
    /// Return part of the given path that is relative to resources directory.
    /// </summary>
    /// <returns>The resources path.</returns>
    /// <param name="path">Path.</param>
    string ExtractResourcesPath(string path)
    {
        string[] separators = { string.Concat("Resources", Path.DirectorySeparatorChar) };
        var pathElements = path.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
        string resourcesPath = pathElements[pathElements.Length - 1];
        if (resourcesPath.EndsWith(".prefab"))
            resourcesPath = resourcesPath.Remove(resourcesPath.Length - 7);
        else if (resourcesPath.EndsWith(".png"))
            resourcesPath = resourcesPath.Remove(resourcesPath.Length - 4);

        return resourcesPath;
    }
}
