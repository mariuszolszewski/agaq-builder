using UnityEngine;
using UnityEngine.UI;
using AgaQ.Bricks;
using AgaQ.Bricks.Tools;
using System;

namespace AgaQ.UI.Inspector
{
    public class InspectorDimensionStringProperty : InspectorDimensionProperty
    {
        [SerializeField] Text valueText;
        [SerializeField] Dropdown valueDropDown;

        /// <summary>
        /// Prepare property to display given dimension
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="index">Property index.</param>
        public override void ConfigureProperty(AgaQBrick brick, int index)
        {
            base.ConfigureProperty(brick, index);

            if (brick.dimensionGroup.dimensions[index].readOnly)
            {
                valueText.gameObject.SetActive(true);
                valueText.text = brick.dimensionParams[index];
                valueDropDown.gameObject.SetActive(false);
            }
            else
            {
                valueText.gameObject.SetActive(false);
                valueDropDown.gameObject.SetActive(true);
                valueDropDown.ClearOptions();
                for (int i = 0; i < values.Length; i++)
                {
                    var option = new Dropdown.OptionData(values[i]);
                    valueDropDown.options.Add(option);
                    if (brick.dimensionParams[index] == values[i])
                        valueDropDown.value = i;
                }
            }
        }

        /// <summary>
        /// On value chaneg event handler.
        /// </summary>
        public void OnChange()
        {
            int newValueIndex = valueDropDown.value;
            if (ToolsManager.instance.dimensionTool.ChangeDimension(brick, groupPropIndex, newValueIndex))
                currentValueIndex = newValueIndex;
        }
    }
}
