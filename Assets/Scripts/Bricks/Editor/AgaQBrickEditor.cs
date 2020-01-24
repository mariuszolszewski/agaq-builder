using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using AgaQ.Bricks;
using AgaQ.Bricks.DimensionsGroups;
using System;

[CustomEditor(typeof(AgaQBrick))]
[CanEditMultipleObjects]
public class AgaQBrickEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));
        serializedObject.ApplyModifiedProperties();
        
        //Encapsualte button
		GUILayout.Space (10);
		AgaQBrick brick = target as AgaQBrick;
        if (!brick.gameObject.transform.Find("brick") && GUILayout.Button("Encapsulate"))
            Encapsualte(brick);

        //Group
        GUILayout.Space(10);
        brick.dimensionGroup = EditorGUILayout.ObjectField(brick.dimensionGroup, typeof(DimensionGroup), false) as DimensionGroup;

        DimensionGroup group = brick.dimensionGroup;
        if (group != null)
        {
            //group params
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            int index = 0;

            if (brick.dimensionParams == null || group.dimensions.Length != brick.dimensionParams.Length)
                brick.dimensionParams = new string[group.dimensions.Length];

            foreach (var dimension in group.dimensions)
            {
                GUILayout.BeginHorizontal();

                //label
                GUILayout.Label(dimension.translationLabel);

                //property field
                if (dimension.paramType == DimensionParamType.integerNumber)
                {
                    int val = 0;
                    int.TryParse(brick.dimensionParams[index], out val);
                    val = EditorGUILayout.IntField(val);
                    brick.dimensionParams[index] = val.ToString();
                }

                else if (dimension.paramType == DimensionParamType.floatNumber)
                {
                    float val = 0;
                    float.TryParse(brick.dimensionParams[index], out val);
                    val = EditorGUILayout.FloatField(val);
                    brick.dimensionParams[index] = val.ToString();
                }

                else if (dimension.paramType == DimensionParamType.text)
                {
                    brick.dimensionParams[index] = EditorGUILayout.TextField(brick.dimensionParams[index]);
                }

                else if (dimension.paramType == DimensionParamType.boolean)
                {
                    bool toggleValue = brick.dimensionParams[index] == "1";
                    bool toggleNewValue = EditorGUILayout.Toggle(toggleValue);
                    brick.dimensionParams[index] = toggleValue ? "1" : "0";
                }

                GUILayout.EndHorizontal();

                index++;
            }
            GUILayout.EndVertical();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Move colliders"))
        {
            var colliders = brick.gameObject.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                Collider newCollider = brick.gameObject.AddComponent(collider.GetType()) as Collider;
                EditorUtility.CopySerialized(collider, newCollider);

                if (newCollider is BoxCollider)
                    ((BoxCollider)newCollider).center += collider.gameObject.transform.localPosition;

                DestroyImmediate(collider);
            }
        }

        EditorUtility.SetDirty(brick);
    }

    /// <summary>
    /// Encapsulate all object components into child brick.
    /// </summary>
    void Encapsualte(AgaQBrick brick)
    {
        //create new GameObject
        var childBrick = new GameObject("brick");
        childBrick.transform.SetParent(brick.transform);
        childBrick.transform.localPosition = Vector3.zero;

        //move all children from brick to childBrick
        for (int i = brick.transform.childCount - 1; i >= 0; i--)
        {
            var child = brick.transform.GetChild(i);
            if (child != childBrick.transform)
                child.SetParent(childBrick.transform);
        }

        //move all colliders
        var colliders = brick.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            ComponentUtility.CopyComponent(collider);
            ComponentUtility.PasteComponentAsNew(childBrick);
            DestroyImmediate(collider);
        }

        //move mesh
        var meshFilter = brick.GetComponent<MeshFilter>();
        var renderer = brick.GetComponent<MeshRenderer>();
        if (meshFilter != null && renderer != null)
        {
            ComponentUtility.CopyComponent(meshFilter);
            ComponentUtility.PasteComponentAsNew(childBrick);
            ComponentUtility.CopyComponent(renderer);
            ComponentUtility.PasteComponentAsNew(childBrick);
            DestroyImmediate(renderer);
            DestroyImmediate(meshFilter);
        }        
    }
}
