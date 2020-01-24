using UnityEngine;
using UnityEditor;
using AgaQ.Bricks;
using AgaQ.Bricks.DimensionsGroups;
using System.IO;

[CustomEditor(typeof(DimensionGroup))]
public class DimensionGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        DimensionGroup group = target as DimensionGroup;

        GUILayout.Space(10);
        if (GUILayout.Button("Rebuilt group data"))
            RebuiltGroupData(group);

        serializedObject.ApplyModifiedProperties();
    }

    void RebuiltGroupData(DimensionGroup group)
    {
        //find all bricks in resources
        var separator = Path.DirectorySeparatorChar;
        string path = string.Concat("Assets", separator, "Resources", separator, "AgaQ");
        var bricks = AssetDatabase.FindAssets("t:Prefab", new string[] { path });

        //remove all data about bricks in group
        group.bricksInGroup.Clear();

        //iterate over found items an collect data
        foreach (var brickGUID in bricks)
        {
            var brickPath = AssetDatabase.GUIDToAssetPath(brickGUID);
            GameObject brickObject = AssetDatabase.LoadAssetAtPath<GameObject>(brickPath);
            AgaQBrick brickScript = brickObject.GetComponent<AgaQBrick>();
            brickPath = brickPath.Substring(17, brickPath.Length - 17 - 7); //remove trailing path /Assets/Resources/ and extension .prefab

            if (brickScript.dimensionGroup == group)
            {
                brickScript.name = string.Format(group.groupName, brickScript.dimensionParams);

                //add brick to group
                group.bricksInGroup.Add(new DimensionGroupItem(brickPath, brickScript.dimensionParams));
            }
        }

        EditorUtility.SetDirty(target);
    }
}
