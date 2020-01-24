using UnityEngine;

namespace AgaQ.Bricks.Positioners
{
    /// <summary>
    /// Helper class to establish brick postion during dragging.
    /// It's based on grid positioner and adds some position conditions du to other scene elements.
    /// </summary>
    public class SimplePositioner : GridPositioner
    {
        protected UnityEngine.Camera cam;

        public override void StartMoving(DragableBrick brick, Vector3 handlePoint)
        {
            cam = UnityEngine.Camera.main;
            base.StartMoving(brick, handlePoint);
        }

        public override ProposedPosition GetPosition(Vector3 mousePosition, bool snapToGrid, float gridSize)
        {
            var ray = cam.ScreenPointToRay(mousePosition);
            ray.origin -= dragPointOffset;
            var mouseRayDirection = Quaternion.LookRotation(ray.direction);

            //calculate bounds dimensions
            Bounds bounds = brick.GetBounds();
            float minBrickDimension = Mathf.Min(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            bounds.extents = new Vector3(minBrickDimension, minBrickDimension, 1000f);
            bounds.center = brick.transform.position + dragPointOffset;

            //move bouds to position in fornt of the camera
            var distanceBricCamera = bounds.extents.z - (cam.transform.position - bounds.center).magnitude;
            bounds.center += ray.direction * distanceBricCamera;

            //check box collision with other bricks
            var colliders = Physics.OverlapBox(bounds.center, bounds.extents, mouseRayDirection, 1);
//            DrawBounds(bounds, mouseRayDirection, Color.red);

            //select nearest point
            Vector3 closestPoint = Vector3.zero;
            float shortestDistance = 10000f;
            bool found = false;

            foreach (var collider in colliders)
            {
                if (collider.tag != "background" && !ContainCollider(brick, collider))
                {
                    var point = collider.bounds.ClosestPoint(ray.origin);
                    var distance = (ray.origin - point).magnitude;
                    if (distance < shortestDistance)
                    {
                        closestPoint = point;
                        shortestDistance = distance;
                        found = true;
                    }
                }
            }

            if (found)
            {
                //calculate new brick position
                float distance = shortestDistance;
                Vector3 point = ray.origin + ray.direction * distance;

				ProposedPosition position = new ProposedPosition (brick.transform);
                position.position = CorrectPosition(point, snapToGrid, gridSize);
                position.isValid = false;

                if (position.position.y + lowestYoffset >= 0)
                    return position;
            }

            //fall back positioning to grid positioner
            return base.GetPosition(mousePosition, snapToGrid, gridSize);
        }

        public override void EndMoving()
        {
            base.EndMoving();
        }

        /// <summary>
        /// This is only for debuging.
        /// </summary>
        protected void DrawBounds(Bounds bounds, Quaternion rotation, Color color)
        {
            Vector3 size = bounds.size * 0.5f;
            Vector3 center = bounds.center;

            Vector3[] p = new Vector3[8];
            p[0] = new Vector3(center.x - size.x, center.y - size.y, center.z - size.z);
            p[1] = new Vector3(center.x - size.x, center.y + size.y, center.z - size.z);
            p[2] = new Vector3(center.x + size.x, center.y - size.y, center.z - size.z);
            p[3] = new Vector3(center.x + size.x, center.y + size.y, center.z - size.z);
            p[4] = new Vector3(center.x - size.x, center.y - size.y, center.z + size.z);
            p[5] = new Vector3(center.x - size.x, center.y + size.y, center.z + size.z);
            p[6] = new Vector3(center.x + size.x, center.y - size.y, center.z + size.z);
            p[7] = new Vector3(center.x + size.x, center.y + size.y, center.z + size.z);

            for (int i = 0; i < p.Length; i++)
                p[i] = RotatePointAroundPivot(p[i], center, rotation);

            Debug.DrawLine(p[0], p[1], color);
            Debug.DrawLine(p[0], p[2], color);
            Debug.DrawLine(p[2], p[3], color);
            Debug.DrawLine(p[3], p[1], color);
            Debug.DrawLine(p[4], p[5], color);
            Debug.DrawLine(p[4], p[6], color);
            Debug.DrawLine(p[6], p[7], color);
            Debug.DrawLine(p[7], p[5], color);
            Debug.DrawLine(p[0], p[4], color);
            Debug.DrawLine(p[1], p[5], color);
            Debug.DrawLine(p[2], p[6], color);
            Debug.DrawLine(p[3], p[7], color);
        }

        /// <summary>
        /// Rotate single point around given pivot point.
        /// </summary>
        /// <returns>The point around pivot.</returns>
        /// <param name="point">Point.</param>
        /// <param name="pivot">Pivot.</param>
        /// <param name="angle">Angle.</param>
        Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }

        /// <summary>
        /// Check if brick contains collider
        /// </summary>
        /// <returns><c>true</c>, if collider was contained, <c>false</c> otherwise.</returns>
        /// <param name="brick">Brick.</param>
        /// <param name="collider">Collider.</param>
        bool ContainCollider(Brick brick, Collider collider)
        {
            if (brick is AgaQTemporaryGroup)
            {
                foreach (Transform child in brick.transform)
                {
                    if (child.gameObject == collider.gameObject)
                        return true;
                }

                return false;
            }

            return collider.gameObject == brick.gameObject;
        }
    }
}
