using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace AgaQ.Bricks
{
    public class MeshPointsTool
    {
        static SphereCollider _collider;
        static SphereCollider collider
        {
            get
            {
                if (_collider == null)
                {
                    var go = new GameObject();
                    go.name = "MeshPointsTool";
                    go.layer = 0;
                    _collider = go.AddComponent<SphereCollider>();
                    _collider.radius = 0.1f;
                }

                return _collider;
            }
        }

        public static bool GetVisiblePointUnderCursor(
            Vector3[] vertices,
            Transform meshTransform,
            UnityEngine.Camera camera,
            Vector3 mousePos,
            float accuracy,
            out Vector2 screenVertex,
            out Vector3 worldVertex)
        {
            CustomSampler s = CustomSampler.Create("point-search");
            CustomSampler s2 = CustomSampler.Create("point-raycast");

            s.Begin();
            float minDist = float.MaxValue;
            Vector3 closetsPoint = Vector3.zero;
            var worldMatrix = meshTransform.localToWorldMatrix;
            var screenMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
            var mousePosViewSpace = camera.ScreenToViewportPoint(mousePos);
            var accuracyViewSpace = camera.ScreenToViewportPoint(new Vector3(accuracy, 0, 0)).x;
            for (int n = 0; n < vertices.Length; n++)
            {
                var worldPoint = worldMatrix.MultiplyPoint(vertices[n]);
                var screenPoint = camera.WorldToScreenPoint(worldPoint);
                //var screenPoint = screenMatrix.MultiplyPoint(worldPoint);
                //var distVect = screenPoint - mousePos;
                var distVect = screenPoint - mousePos;
                distVect.z = 0;
                if (distVect.magnitude <= accuracy)
                {
                    var dist = (worldPoint - camera.transform.position).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closetsPoint = worldPoint;
                    }
                }
            }
            s.End();
            s2.Begin();
            if (CheckVertexVisibility(closetsPoint, camera, mousePos))
            {
                var screenPoint = camera.WorldToScreenPoint(closetsPoint);
                screenVertex = new Vector2(screenPoint.x, screenPoint.y);
                worldVertex = closetsPoint;
                return true;
            }
            s2.End();

            screenVertex = Vector2.zero;
            worldVertex = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Check if given vertext is visible form current camera position.
        /// It is done by recast to small spehere collider placed at vertex position.
        /// </summary>
        /// <returns><c>true</c>, if vertex visibility was checked, <c>false</c> otherwise.</returns>
        /// <param name="vertex">Vertex.</param>
        public static bool CheckVertexVisibility(Vector3 vertex, UnityEngine.Camera camera, Vector3 scrennPoint)
        {
            var col = collider;
            col.enabled = true;
            col.transform.position = vertex;

            var camPos = camera.transform.position;
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(scrennPoint);
            if (Physics.Raycast(ray, out hit, 1000f, 1, QueryTriggerInteraction.Collide))
            {
                col.enabled = false;
                return hit.collider == collider;
            }

            col.enabled = false;
            return false;
        }
    }
}
