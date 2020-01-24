using UnityEngine;

namespace AgaQ.Bricks.Positioners
{
    public class PointJointPositioner : IPositioner
    {
        DragableBrick brick;

        public void StartMoving(DragableBrick brick, Vector3 handlePoint)
        {
            this.brick = brick;
        }

        public void EndMoving()
        {
            brick = null;
        }

        public ProposedPosition GetPosition(Vector3 mousePosition, bool snapToGrid, float gridSize)
        {
            return new ProposedPosition(
            );
        }
    }
}
