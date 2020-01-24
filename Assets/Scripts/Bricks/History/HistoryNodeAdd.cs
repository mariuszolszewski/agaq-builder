using UnityEngine;

namespace AgaQ.Bricks.History
{
	/// <summary>
	/// History node that holds state of newly added brick.
	/// </summary>
    public class HistoryNodeAdd : HistoryNode
    {
        public HistoryNodeAdd(GameObject gameObject) : base(gameObject)
        {}

        public override void Undo()
        {
            gameObject.SetActive(false);
        }

        public override void Redo()
        {
            gameObject.SetActive(true);
        }

        public override void Clear()
        {
            Object.Destroy(gameObject);
            base.Clear();
        }
    }
}
