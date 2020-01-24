using UnityEngine;
using AgaQ.Bricks.Joints;

namespace AgaQ.Bricks.Positioners
{
    /// <summary>
    /// Struct represents posssible brick joint between two projected joints.
    /// </summary>
	public struct PossibleJoint
	{
		/// <summary>
		/// Distance between projected positions.
		/// </summary>
		public readonly float projectedDistance;

        /// <summary>
        /// Distance in direction of camera vector between positions.
        /// </summary>
        public readonly float cameraVectorDistance;

        public ProjectedJoint projectedBrickJoint;
        public ProjectedJoint projectedOtherJoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgaQ.Bricks.Positioners.PossibleJoint"/> struct.
        /// </summary>
        /// <param name="projectedBrickJoint">Proejected brick joint.</param>
        /// <param name="projectedOtherJoint">Proejected other joint.</param>
		public PossibleJoint(ProjectedJoint projectedBrickJoint, ProjectedJoint projectedOtherJoint)
		{
			this.projectedBrickJoint = projectedBrickJoint;
			this.projectedOtherJoint = projectedOtherJoint;

            projectedDistance = (projectedBrickJoint.projectedPosition - projectedOtherJoint.projectedPosition).magnitude;

            var vector = projectedBrickJoint.joint.transform.position - projectedOtherJoint.joint.transform.position;
            var projectedVector = Vector3.Project(vector, UnityEngine.Camera.main.transform.forward);
            cameraVectorDistance = projectedVector.magnitude;
		}
	}
}
