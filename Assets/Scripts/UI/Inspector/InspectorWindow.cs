using UnityEngine;
using UnityEngine.EventSystems;
using AgaQ.Bricks;
using System.Collections.Generic;
using AgaQ.Bricks.Tools;

namespace AgaQ.UI.Inspector
{
    /// <summary>
    /// Inspector window that shows InspectorPrpertys for curent set of selected bricks.
    /// </summary>
    public class InspectorWindow : UIBehaviour
    {
        [SerializeField] InspectorColorProperty colorProperty;
        [SerializeField] InspectorScaleProperty scaleProperty;
        [SerializeField] InspectorDimensions dimensionsProperties;

        protected override void Start()
        {
            base.Start();

            SelectionManager.instance.OnSelectionChange += OnSelectionChange;
            ToolsManager.instance.addTool.OnStartMove += ShowProperties;
            ToolsManager.instance.addTool.OnCancelMove += TurnOffAllProperties;
            ToolsManager.instance.addTool.OnEndMove += TurnOffAllProperties;

            TurnOffAllProperties();
        }

        public void OnSelectionChange(List<SelectableBrick> currentSelection)
        {
            if (currentSelection.Count == 0)
            {
                TurnOffAllProperties();
                return;
            }

            ShowProperties(currentSelection[0], currentSelection.Count == 1);                
        }

       public void ShowProperties(DragableBrick brick)
        {
            ShowProperties(brick, brick is AgaQBrick);                
        }

        /// <summary>
        /// Show properties in inspector for given brick.
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="showDimensions">If set to <c>true</c> show dimensions.</param>
        void ShowProperties(SelectableBrick brick, bool showDimensions)
        {
            colorProperty.SetActive(true);
            colorProperty.SetColor(brick.color);

            if (brick is AgaQBrick)
            {
                scaleProperty.SetActive(true);
                scaleProperty.scaleSelector.SetScale(brick.scale);
            }
            else
                scaleProperty.SetActive(false);

            if (showDimensions && brick is AgaQBrick)
                dimensionsProperties.PrepareProperties(brick as AgaQBrick);
            else
                dimensionsProperties.Clear();
        }

        /// <summary>
        /// Turn off all properties controlls.
        /// </summary>
        void TurnOffAllProperties()
        {
            colorProperty.SetActive(false);
            scaleProperty.SetActive(false);
            dimensionsProperties.Clear();
        }
    }
}
