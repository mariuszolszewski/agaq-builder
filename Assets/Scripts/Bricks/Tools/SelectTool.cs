using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Base select tool class. Use it to build all select tools (by shape, color, etc.)
    /// </summary>
    public abstract class SelectTool : DeleteTool
    {
        public override void OnClick(SelectableBrick brick, PointerEventData pointerEventData)
        {
            if (!isDragging)
            {
                var bricks = GetSelected(brick);

#if UNITY_STANDALONE
#if UNITY_STANDALONE_WIN
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
#endif
#if UNITY_STANDALONE_LINUX
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
#endif
#if UNITY_STANDALONE_OSX
                if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
#endif
                SelectionManager.instance.Add(bricks);
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
#endif
                SelectionManager.instance.Remove(bricks);
                else
                    SelectionManager.instance.Replace(bricks);
            }

            base.OnClick(brick, pointerEventData);
        }

        /// <summary>
        /// Returns list of selected brics based on first one.
        /// </summary>
        /// <returns>The selected bricks</returns>
        /// <param name="brick">Brick.</param>
        protected virtual List<SelectableBrick> GetSelected(SelectableBrick brick)
        {
            return new List<SelectableBrick> { brick };
        }
    }
}
