using UnityEngine;
using AgaQ.Bricks.Tools;
using System.Collections.Generic;

namespace AgaQ.Bricks.History
{
    /// <summary>
    /// Base class for hisory nodes group and ungroup that holds common parts of the code.
    /// </summary>
    public abstract class HistoryNodeGroupBase : HistoryNode
    {
        private HistoryNodeGroupBase(GameObject gameObject) : base(gameObject) { }

        List<SelectableBrick> groupBricks;

        public HistoryNodeGroupBase(List<SelectableBrick> selectables)
        {
            groupBricks = selectables;
        }

        public override void Clear()
        {
            groupBricks = null;
            base.Clear();
        }

        /// <summary>
        /// Group all brick on the gameObjects array
        /// </summary>
        protected void Group()
        {
            ToolsManager.instance.groupTool.Group(groupBricks);
        }

        /// <summary>
        /// Ung
        /// roup all brick on the gameObjects array
        /// </summary>
        protected void Ungroup()
        {
            if (groupBricks.Count == 0)
                return;

            //find group script
            var parent = groupBricks[0].transform.parent;
            if (parent == null)
                return;
            var groupScript = parent.GetComponent<AgaQGroup>();
            if (groupScript == null)
                return;

            ToolsManager.instance.groupTool.Ungroup(groupScript);
        }
    }
}
