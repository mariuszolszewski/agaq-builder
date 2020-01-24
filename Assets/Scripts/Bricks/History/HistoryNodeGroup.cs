using System.Collections.Generic;

namespace AgaQ.Bricks.History
{
    /// <summary>
    /// History node that hold grouping operation
    /// </summary>
    public class HistoryNodeGroup : HistoryNodeGroupBase
    {
        public HistoryNodeGroup(List<SelectableBrick> selectables) : base(selectables)
        {}

        public override void Undo()
        {
            Ungroup();
        }

        public override void Redo()
        {
            Group();
        }
    }
}
