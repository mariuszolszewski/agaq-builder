using UnityEngine;
using AgaQ.Bricks.Positioners;

namespace AgaQ.Bricks.Joints
{
    /// <summary>
    /// Usufull staff to manipulate joints.
    /// </summary>
    class JointsUtils
    {
        /// <summary>
        /// Rebuild informations about connected joints (all joints in scene).
        /// </summary>
        public static void RebuildJoints()
        {
            var model = GameObject.Find("Model");
            if (model == null)
                return;

            //rebuilt all joints
            var joints = model.GetComponentsInChildren<Joint>();
            for (int i = 0; i < joints.Length; i++)
            {
                //iterate over other joints to find other in the same position and space orientation
                joints[i].ClearJoint();
                for (int j = i + 1; j < joints.Length; j++)
                {
                    //compare joints position
                    if (Joint.AreJoinable(joints[i], joints[j]) &&
                       (joints[i].transform.position - joints[j].transform.position).magnitude <= JoinablePositioner.positionAccuracy &&
                       Quaternion.Angle(joints[i].transform.rotation, joints[j].transform.rotation) <= JoinablePositioner.rotationAccuracy)
                    {
                        joints[i].SetJoint(joints[j]);
                        break;
                    }
                }
            }
        }
    }
}
