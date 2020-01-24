using UnityEngine;
using UnityEngine.EventSystems;
using AgaQ.Bricks;

namespace AgaQ.Camera
{
    public class Background : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {         
            // clear selection on grid click
            if (eventData.button == PointerEventData.InputButton.Left)
                SelectionManager.instance.Clear();
        }
    }
}
