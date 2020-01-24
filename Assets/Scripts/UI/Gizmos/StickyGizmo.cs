using UnityEngine;

namespace AgaQ.UI.Gizmos
{
    public class StickyGizmo : MonoBehaviour
    {
        public void SetUp(Bounds bounds)
        {
            transform.position = bounds.center;
        }

        void SetUpAxis(Bounds bounds, Axis axis, float scale, Transform cone, Transform cylinder)
        {
            float distance = GetSize(bounds, axis) / scale;

            //adjust cone position
            cone.localPosition = new Vector3(0, 0, -distance);

            //adjust cylinder scale
            float newScale = distance * 1.4f / 0.35f * 12f;
            cylinder.localScale = new Vector3(cylinder.localScale.x, cylinder.localScale.y, newScale);
        }

        float GetSize(Bounds bounds, Axis axis)
        {
            if (axis == Axis.x)
                return bounds.size.x;
            if (axis == Axis.y)
                return bounds.size.y;
            if (axis == Axis.z)
                return bounds.size.z;
            return 0;
        }
    }
}