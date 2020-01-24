using UnityEngine;

namespace AgaQ.Bricks.History
{
	/// <summary>
	/// History node that holds state of position, scale and rotation of the brick.
	/// </summary>
    public class HistoryNodeTransform : HistoryNode
    {
        public Vector3 oldPosition;
        public Quaternion oldRotation;
        public Vector3 oldScale;
        public Vector3 newPosition;
        public Quaternion newRotation;
        public Vector3 newScale;

        public HistoryNodeTransform(GameObject gameObject) : base(gameObject)
        {
            oldPosition = gameObject.transform.position;
            oldRotation = gameObject.transform.rotation;
            oldScale = gameObject.transform.localScale;
        }

        public override void Undo()
        {
            gameObject.transform.position = oldPosition;
            gameObject.transform.rotation = oldRotation;
            gameObject.transform.localScale = oldScale;
        }

        public override void Redo()
        {
            gameObject.transform.position = newPosition;
            gameObject.transform.rotation = newRotation;
            gameObject.transform.localScale = newScale;
        }
    }
}
