using System.Collections.Generic;

namespace AgaQ.Bricks.History
{
    /// <summary>
    /// History node that hold ungrouping operation.
    /// </summary>
    public class HistoryNodeUngroup : HistoryNodeGroupBase
    {
        public HistoryNodeUngroup(List<SelectableBrick> selectables) : base(selectables)
        {}

        public override void Undo()
        {
            Group();
        }

        public override void Redo()
        {
            Ungroup();
        }
    }
}
