using UnityEngine;
using AgaQ.Bricks.Joints;

namespace AgaQ.Bricks.Positioners
{
    /// <summary>
    /// Struct represents brick joint projected to camera surface.
    /// </summary>
	public struct ProjectedJoint
	{
		public const float rotationTolerance = 17f;   //rotation tolerance in degrees when joining brics
		//public const float positionTolerance = 30f; //position tolerance when joining brics

		public Vector3 projectedPosition; //position after projection to plane
		public AgaQ.Bricks.Joints.Joint joint;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgaQ.Bricks.Positioners.ProjectedJoint"/> struct.
        /// </summary>
        /// <param name="projectedPosition">Position.</param>
        /// <param name="joint">Joint.</param>
		public ProjectedJoint(Vector3 projectedPosition, AgaQ.Bricks.Joints.Joint joint)
		{
			this.projectedPosition = projectedPosition;
			this.joint = joint;
		}

		/// <summary>
		/// Comare joint by type, rotation and position.
		/// </summary>
		/// <returns><c>true</c>, if joints can be joined, <c>false</c> otherwise.</returns>
        /// <param name="otherProjectedJoint">Other projected joint.</param>
        /// <param name="positionTolerance">Position tolerance.</param>
		public bool Compare(ProjectedJoint otherProjectedJoint, float positionTolerance)
		{
            return Compare (this, otherProjectedJoint, positionTolerance);
		}

		/// <summary>
		/// Comare two projected joints by type, rotation and position.
		/// </summary>
		/// <returns><c>true</c>, if joints can be joined, <c>false</c> otherwise.</returns>
		/// <param name="projectedJoint1">Joint 1.</param>
		/// <param name="projectedJoint2">Joint 2.</param>
        /// <param name="positionTolerance">Position tolerance.</param>
		public static bool Compare(ProjectedJoint projectedJoint1, ProjectedJoint projectedJoint2, float positionTolerance)
		{
			//compare joints type, has to be oposite type
            if (!Joints.Joint.AreJoinable(projectedJoint1.joint, projectedJoint2.joint))
				return false;

            //compare joints rotation
            float angle = 0;
            if (projectedJoint1.joint.ignoreYRotation || projectedJoint2.joint.ignoreYRotation)
                angle = Quaternion.Angle(
                    new Quaternion(
                        projectedJoint1.joint.transform.rotation.x, 0,
                        projectedJoint1.joint.transform.rotation.z,
                        projectedJoint1.joint.transform.rotation.w),
                    new Quaternion(
                        projectedJoint2.joint.transform.rotation.x, 0,
                        projectedJoint2.joint.transform.rotation.z,
                        projectedJoint2.joint.transform.rotation.w)
                );
            else
                angle = Quaternion.Angle(projectedJoint1.joint.transform.rotation, projectedJoint2.joint.transform.rotation);            
             if (angle > rotationTolerance)
                    return false;

			//compare position
			if ((projectedJoint1.projectedPosition - projectedJoint2.projectedPosition).magnitude > positionTolerance)
				return false;

			return true;
		}
	}
}
	