using UnityEngine;
using AgaQ.Bricks.Tools;

namespace AgaQ.Bricks.Positioners
{
    /// <summary>
    /// Helper class that can establish birck position during dragging.
    /// This one position brick only on grid level. Don't get into considerection other scene elements.
    /// </summary>
    public class GridPositioner : IPositioner
    {
        protected DragableBrick brick;
        protected Vector3 dragPointOffset; // drag point offset from brick position
        protected Vector3 positionOffset;  // brick position offest from ray hit position
        protected float lowestYoffset;     // lowest point offfset from brick position
        protected float rayYOffset;        // y offset for ray orygin point from oryginal mouse ray

        protected float minDistanceToCamera;
        protected float maxDistanceFromCamera;

        UnityEngine.Camera cam;

        public virtual void StartMoving(DragableBrick brick, Vector3 handlePoint)
        {
            dragPointOffset = handlePoint - brick.transform.position;

            //reset drag offset when it goes outside bouds
            if (Mathf.Abs(dragPointOffset.x) > brick.GetBounds().extents.x)
                dragPointOffset.x = 0;
            if (Mathf.Abs(dragPointOffset.y) > brick.GetBounds().extents.y)
                dragPointOffset.y = 0;
            if (Mathf.Abs(dragPointOffset.z) > brick.GetBounds().extents.z)
                dragPointOffset.z = 0;

            Vector3 brickLowestPoint = brick.GetLowestPoint();
            lowestYoffset = brickLowestPoint.y - brick.transform.position.y;
            rayYOffset = brickLowestPoint.y - handlePoint.y;
            positionOffset = handlePoint;
            positionOffset.y = brickLowestPoint.y;
            positionOffset = brick.transform.position - positionOffset;

            minDistanceToCamera = brick.GetBounds().extents.x * 20;
            maxDistanceFromCamera = brick.GetBounds().extents.x * 1000;

            cam = UnityEngine.Camera.main;
            this.brick = brick;
        }

        public virtual ProposedPosition GetPosition(Vector3 mousePosition, bool snapToGrid, float gridSize)
        {
            var cameraPosition = cam.transform.position;
            var mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane));
            var viewVector = mouseWorldPosition - cameraPosition;

            cameraPosition.y += rayYOffset;

            var gridPositon = Camera.Grid.instance.transform.position;
            float gridY = gridPositon.y;

            double t = (gridY - cameraPosition.y) / viewVector.y;
            Vector3 hitPoint = gridPositon;
            double x = (double)cameraPosition.x + (double)viewVector.x * t;
            double y = (double)cameraPosition.z + (double)viewVector.z * t;
            hitPoint.x = (float)x;
            hitPoint.z = (float)y;

            var position = new ProposedPosition(brick.transform);
            position.position = hitPoint + positionOffset;
            position.position.y = -lowestYoffset;
            position.position = CorrectPosition(position.position, snapToGrid, gridSize);
            position.isValid = true;

            return position;
        }

        public virtual void EndMoving()
        {
            brick = null;
        }

        /// <summary>
        /// Correct proposed brick position. It couldn't be to close and too far from camera.
        /// </summary>
        /// <returns>The position.</returns>
        /// <param name="position">Position.</param>
        protected Vector3 CorrectPosition(Vector3 position, bool snapToGrid, float gridSize)
        {
            Vector3 pointOnScreen = UnityEngine.Camera.main.WorldToScreenPoint(position);

            if (pointOnScreen.z < 0 ||
                pointOnScreen.x < 0 || pointOnScreen.x > Screen.width ||
                pointOnScreen.y < 0 || pointOnScreen.y > Screen.height)
            {
                var mouseRay = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                position = cam.transform.position + mouseRay.direction * minDistanceToCamera;
            }

            //snap to grid
            if (snapToGrid)
            {
                float brickGridSize = gridSize * brick.transform.localScale.x;
                float halfGridSize = brickGridSize * 0.5f;

                position.x = Mathf.Round((position.x + halfGridSize) / brickGridSize) * brickGridSize - halfGridSize;
                position.z = Mathf.Round((position.z + halfGridSize) / brickGridSize) * brickGridSize - halfGridSize;

                //check if is below grid
                if (position.y + lowestYoffset < 0)
                    position.y = brick.transform.position.y + lowestYoffset;
            }

            return position;
        }
    }
}
