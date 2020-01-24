using UnityEngine;

namespace AgaQ.Bricks.Joints
{
    /// <summary>
    /// Female brick joint that is on border of the mesh,
    /// so it can has exacly the same position as other FemaleBorderJoint
    /// at brick next to it.
    /// </summary>
    public class FemaleBorderJoint : FemaleJoint
    {
        #if UNITY_EDITOR

        void OnDrawGizmos()
        {
            DebugExtension.DebugCylinder(transform.position + transform.up * 0.15f, transform.position, Color.gray, 0.1f);
            DebugExtension.DebugLocalCube(transform, new Vector3(0.01f, 0.15f, 0.4f), Color.gray, new Vector3(0.09f, 0.075f, 0));
        }

        #endif
    }
}
