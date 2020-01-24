using UnityEngine;

namespace AgaQ.Bricks.Joints
{
    /// <summary>
    /// Brick joint male type.
    /// </summary>
    public class MaleJoint : Joint
    {
        #if UNITY_EDITOR

        void OnDrawGizmos()
        {
            DebugExtension.DebugCylinder(transform.position + transform.up * 0.15f, transform.position, Color.red, 0.1f);
            DebugExtension.DebugLocalCube(transform, new Vector3(0.01f, 0.15f, 0.4f), Color.red, new Vector3(-0.09f, 0.075f, 0));
        }

        #endif

//        override public bool isJoined()
//        {
//            return joinedWith != null;
//        }
    }
}
