using UnityEngine;

namespace AgaQ.Bricks.Positioners
{
    public interface IPositioner
    {
        /// <summary>
        /// Called once before brick move.
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="handlePoint">Handle point.</param>
        void StartMoving(DragableBrick brick, Vector3 handlePoint);

        /// <summary>
        /// Called every frame to calculate new brick position.
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="mousePosition">Mouse position.</param>
        /// <param name="snapToGrid">Should snap position to grid</param>
        /// <param name="gridSize">Grid cell size.</param>
        ProposedPosition GetPosition(Vector3 mousePosition, bool snapToGrid, float gridSize);

        /// <summary>
        /// Called once after move.
        /// </summary>
        void EndMoving();
    }
}
