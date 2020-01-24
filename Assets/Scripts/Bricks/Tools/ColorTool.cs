using UnityEngine.EventSystems;
using System.Collections.Generic;
using AgaQ.Bricks.History;

namespace AgaQ.Bricks.Tools
{
    public class ColorTool : BaseTool
    {
        bool coloringOn;
        List<HistoryNodeChangeColor> historyNodes = new List<HistoryNodeChangeColor>();

        public override void OnPointerDown(SelectableBrick brick, PointerEventData pointerEventData)
        {
            if (pointerEventData.button == PointerEventData.InputButton.Left)
            {
                coloringOn = true;
                historyNodes.Add(new HistoryNodeChangeColor(brick.gameObject));
                brick.color = ToolsManager.instance.colorButton.selectedColor;
            }
        }

        public override void OnPointerUp(SelectableBrick brick, PointerEventData pointerEventData)
        {
            if (pointerEventData.button == PointerEventData.InputButton.Left)
            {
                coloringOn = false;

                if (historyNodes.Count > 0)
                {
                    HistoryManager.instance.Register(historyNodes.ToArray());
                    historyNodes.Clear();
                }
            }
        }

        public override void OnEneter(HighlightableBrick brick)
        {
            if (coloringOn)
            {
                historyNodes.Add(new HistoryNodeChangeColor(brick.gameObject));
                brick.color = ToolsManager.instance.colorButton.selectedColor;
            }
        }

        public override bool OnBeginDrag(DragableBrick brick, PointerEventData eventData)
        {
            return false; //return false to break drag
        }
    }
}
