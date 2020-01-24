using UnityEngine;
using AgaQ.Bricks.Joints;
using System.Collections.Generic;

namespace AgaQ.Bricks.History
{
    /// <summary>
    /// History manager. Provides functionality to remeber model states histroy.
    /// </summary>
    public class HistoryManager : MonoBehaviour
    {
        public static HistoryManager instance;

        List<HistoryNode[]> history = new List<HistoryNode[]>();
        int historyCursor = -1; //positon in history list

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        #region Public functions

        /// <summary>
        /// Revert last operation.
        /// </summary>
        public void Undo()
        {
            if (historyCursor >= 0)
            {
                var nodes = history[historyCursor--];
                for (var i = 0; i < nodes.Length; i++)
                    nodes[i].Undo();

                JointsUtils.RebuildJoints();
            }
        }

        /// <summary>
        /// Redo one operation if aviable
        /// </summary>
        public void Redo()
        {
            if (historyCursor + 1 < history.Count)
            {
                var nodes = history[++historyCursor];
                for (var i = 0; i < nodes.Length; i++)
                    nodes[i].Redo();

                JointsUtils.RebuildJoints();
            }
        }

        /// <summary>
        /// Register gameobjects position and rotation before changing it.
        /// </summary>
        /// <param name="nodes">history node</param>
        public void Register(HistoryNodeTransform[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].newPosition = nodes[i].gameObject.transform.position;
                nodes[i].newRotation = nodes[i].gameObject.transform.rotation;
            }

            ClearFromPosition(historyCursor + 1);
            history.Add(nodes);
            historyCursor++;
        }

        /// <summary>
        /// Register new gameobjects.
        /// </summary>
        /// <param name="nodes">history nodes</param>
        public void Register(HistoryNodeAdd[] nodes)
        {
            ClearFromPosition(historyCursor + 1);
            history.Add(nodes);
            historyCursor++;
        }

        /// <summary>
        /// Register game objets that should be removed (can't be removed, have to be setActive(false).
        /// Will be removed by HistoryManager when will be useless.
        /// </summary>
        /// <param name="nodes">history nodes</param>
        public void Register(HistoryNodeRemove[] nodes)
        {
            ClearFromPosition(historyCursor + 1);
            history.Add(nodes);
            historyCursor++;
        }

        /// <summary>
        /// Register gamesobjects before color change.
        /// </summary>
        /// <param name="nodes">history nodes</param>
        public void Register(HistoryNodeChangeColor[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
                nodes[i].RememberNewColor();

            ClearFromPosition(historyCursor + 1);
            history.Add(nodes);
            historyCursor++;
        }

        /// <summary>
        /// Register gamesobjects at group action.
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="node">Node.</param>
        public void Register(HistoryNodeGroup node)
        {
            history.Add(new HistoryNodeGroup[] { node });
            historyCursor++;
        }

        /// <summary>
        /// Register gamesobjects at ungroup action.
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="node">Node.</param>
        public void Register(HistoryNodeUngroup node)
        {
            history.Add(new HistoryNodeUngroup[] { node });
            historyCursor++;
        }

        /// <summary>
        /// Register gamesobjects at change dimension action.
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="node">Node.</param>
        public void Register(HistoryNodeDimension node)
        {
            history.Add((new HistoryNodeDimension[] { node }));
            historyCursor++;
        }

        public void ClearAll()
        {
            ClearFromPosition(0);
            historyCursor = -1;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Clear history list from given position.
        /// </summary>
        /// <param name="position">Position.</param>
        void ClearFromPosition(int position)
        {
            for (int i = history.Count - 1; i >= position; i--)
            {
				foreach (var node in history[i])
					node.Clear();

				history.RemoveAt(i);
            }
        }

        #endregion
    }
}
