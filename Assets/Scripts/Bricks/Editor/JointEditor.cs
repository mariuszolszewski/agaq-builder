using UnityEngine;
using UnityEditor;
using AgaQ.Bricks.Joints;

[CustomEditor(typeof(AgaQ.Bricks.Joints.Joint), true)]
public class JointEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();

		var joint = target as AgaQ.Bricks.Joints.Joint;
		var brick = joint.transform.parent;
		if (GUILayout.Button ("Set brick origin to this joint"))
		{
			var moveVector = brick.parent.position - joint.transform.position;
			moveVector.y = 0;
			brick.position =  brick.position + moveVector;
		}
	}
}
