using UnityEngine;
using UnityEngine.UI;
using AgaQ.Bricks;
using AgaQ.Bricks.Tools;
using System;

namespace AgaQ.UI.Inspector
{
    public class InspectorDimensionNumberProperty : InspectorDimensionProperty
    {
        [SerializeField] Text valueText;
        [SerializeField] GameObject plusButton;
        [SerializeField] GameObject minusButton;

        /// <summary>
        /// Prepare property to display given dimension
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="index">Property index.</param>
        public override void ConfigureProperty(AgaQBrick brick, int index)
        {
            base.ConfigureProperty(brick, index);

            //value
            currentValueIndex = Array.IndexOf(values, brick.dimensionParams[index]);
            valueText.text = brick.dimensionParams[index];

            //controls
            bool show = !brick.dimensionGroup.dimensions[index].readOnly;
            plusButton.SetActive(show);
            minusButton.SetActive(show);
        }

        /// <summary>
        /// On button "+" pressed event handler.
        /// </summary>
        public void OnButtonPlus()
        {
            //check if we can icrease this diemnsion
            if (currentValueIndex + 1 >= values.Length)
                return;

            if (ToolsManager.instance.dimensionTool.ChangeDimension(brick, groupPropIndex, currentValueIndex + 1))
                currentValueIndex++;
        }

        /// <summary>
        /// On button "-" pressed event handler.
        /// </summary>
        public void OnButtonMinus()
        {
            //check if we can decrease this dimension
            if (currentValueIndex - 1 < 0)
                return;

            if (ToolsManager.instance.dimensionTool.ChangeDimension(brick, groupPropIndex, currentValueIndex - 1))
                currentValueIndex--;
        }
    }
}
