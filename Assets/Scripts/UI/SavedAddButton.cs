using AgaQ.Bricks.Tools;
using UnityEngine.EventSystems;

namespace AgaQ.UI
{
    public class SavedAddButton : SavedBricsGroupButton
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            ToolsManager.instance.addTool.AddSaved(groupPath, false);
        }
    }
}
