using UnityEngine;
namespace AgaQ.Bricks.History
{
	/// <summary>
	/// History node that holds remove brick state.
	/// </summary>
    public class HistoryNodeRemove : HistoryNode
    {
        public HistoryNodeRemove(GameObject gameObject) : base(gameObject)
        {}

        public override void Undo()
        {
            gameObject.SetActive(true);
        }

        public override void Redo()
        {
            gameObject.SetActive(false);
        }
    }
}
