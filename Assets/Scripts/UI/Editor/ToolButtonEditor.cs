using UnityEditor;
using AgaQ.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(ToolButton), true)]
public class ToolButtonEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
	}
}
