using UnityEngine;

namespace AgaQ.Bricks.History
{
    public class HistoryNodeDimension : HistoryNode
    {
        public GameObject newGameObject
        {
            get;
            protected set;
        }

        public HistoryNodeDimension(GameObject oldGameObject, GameObject newGameObject) : base(oldGameObject)
        {
            this.newGameObject = newGameObject;
        }

        public override void Undo()
        {
            gameObject.SetActive(true);
            newGameObject.SetActive(false);
        }

        public override void Redo()
        {
            gameObject.SetActive(false);
            newGameObject.SetActive(true);
        }

        public override void Clear()
        {
            Object.Destroy(newGameObject);
            base.Clear();
        }
    }
}
