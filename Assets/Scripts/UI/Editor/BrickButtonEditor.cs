using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using AgaQ.UI;
using AgaQ.Bricks;

/// <summary>
/// Custom inpector for BrickButton
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(BrickButton))]
public class BrickButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Generate icon"))
        {
            serializedObject.Update();

            BrickButton button = (BrickButton)target;

            var image = button.GetComponent<Image>();
            if (image != null)
            {
                //generate icon
                var brick = Resources.Load(button.brickResourcePath) as GameObject;
                string path = string.Format("Assets/Resources/Icons/{0}.png", brick.name);
                BrickUtils.GenerateIcon(brick.GetComponent<Brick>(), path, 256);

                //attache generated icon to button
//                AssetDatabase.ImportAsset(path);
//                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
//                image.sprite = sprite;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
