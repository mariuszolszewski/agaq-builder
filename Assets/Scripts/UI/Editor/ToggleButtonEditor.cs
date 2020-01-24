using UnityEditor;
using AgaQ.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(ToggleButton))]
public class ToggleButtonEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
	}
}
