using UnityEngine;

namespace AgaQ.Bricks.History
{
	/// <summary>
	/// History node that holds state of the brick color.
	/// </summary>
    public class HistoryNodeChangeColor : HistoryNode
    {
        public Color oldColor;
        public Color newColor;

        Material material;

        public HistoryNodeChangeColor(GameObject gameObject) : base(gameObject)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
			if (renderer != null && renderer.material != null)
            {
                material = renderer.material;
                oldColor = material.color;
            }
        }

        public override void Undo()
        {
            if (material != null)
                material.color = oldColor;
        }

        public override void Redo()
        {
            if (material != null)
                material.color = newColor;
        }

        public void RememberNewColor()
        {
            if (material != null)
                newColor = material.color;                
        }
    }
}
