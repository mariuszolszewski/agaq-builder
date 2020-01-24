using System.Collections.Generic;
using UnityEngine;


namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Color select tool (select all brick with the same color
    /// </summary>
    public class SelectColorTool : SelectTool
    {
        protected override List<SelectableBrick> GetSelected(SelectableBrick brick)
        {
            List<SelectableBrick> toSelect = new List<SelectableBrick>();

            //get model root
            var modelRoot = GameObject.Find("Model");

            //get list of all selectable bricks
            var bricks = modelRoot.GetComponentsInChildren<SelectableBrick>();

            //iterate over all bricks
            foreach (var b in bricks)
            {
                //skip groups
                if (b is AgaQTemporaryGroup)
                    continue;

                if (b.color == brick.color)
                    toSelect.Add(b);
            }

            return toSelect;
        }
    }
}
