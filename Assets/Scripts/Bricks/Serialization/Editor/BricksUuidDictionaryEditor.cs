using UnityEngine;
using UnityEditor;
using AgaQ.Bricks.Serialization;

[CustomEditor(typeof(BricksUuidDictionary))]
public class BricksUuidDictionaryEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.Space (10);
		if (GUILayout.Button ("Rebuild disctionary"))
		{
			var dictionary = target as BricksUuidDictionary;
			dictionary.Rebuild ();
		}
	}
}
