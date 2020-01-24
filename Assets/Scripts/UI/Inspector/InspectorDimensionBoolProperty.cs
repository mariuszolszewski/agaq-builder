using UnityEngine;
using UnityEngine.UI;
using AgaQ.Bricks;
using AgaQ.Bricks.Tools;

namespace AgaQ.UI.Inspector
{
    public class InspectorDimensionBoolProperty : InspectorDimensionProperty
    {
        [SerializeField] Toggle valueToggle;

        /// <summary>
        /// Prepare property to display given dimension
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="index">Property index.</param>
        public override void ConfigureProperty(AgaQBrick brick, int index)
        {
            base.ConfigureProperty(brick, index);

            valueToggle.isOn = brick.dimensionParams[index] == "1";
            valueToggle.enabled = !brick.dimensionGroup.dimensions[index].readOnly;
        }

        /// <summary>
        /// On toggle event handler.
        /// </summary>
        public void OnToggle()
        {
            if (currentValueIndex == 1)
                currentValueIndex = 0;
            else
                currentValueIndex = 1;

            if (ToolsManager.instance.dimensionTool.ChangeDimension(brick, groupPropIndex, currentValueIndex + 1))
                currentValueIndex++;
        }
    }
}
