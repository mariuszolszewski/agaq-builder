using UnityEngine;

namespace AgaQ.Bricks.History
{
	/// <summary>
	/// Base class for all hostory nodes that can hold information about brick state.
	/// </summary>
    public abstract class HistoryNode
    {
        public GameObject gameObject
        {
            get;
            protected set;
        }

        protected HistoryNode() {}

        public HistoryNode(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Undo this node.
        /// </summary>
        public virtual void Undo()
        {}

		/// <summary>
		/// Redo this node
		/// </summary>
        public virtual void Redo()
        {}

        /// <summary>
        /// Clear this node data before deleting.
        /// </summary>
        public virtual void Clear()
        {
            gameObject = null;
        }
    }
}
