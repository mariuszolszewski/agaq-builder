using UnityEngine;
using System.Collections.Generic;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Shape select tool (selct all brick the same type)
    /// </summary>
    public class SelectShapeTool : SelectTool
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

                if (b.uuid == brick.uuid)
                    toSelect.Add(b);
            }

            return toSelect;
        }
    }
}
