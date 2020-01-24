using UnityEngine;
using System;

namespace AgaQ.Bricks.Positioners
{
	/// <summary>
	/// Some useful function to project joint position to camera plane.
	/// </summary>
	public static class ProjectionTool
	{
		/// <summary>
		/// Project joint to screen coordinates.
		/// </summary>
		/// <returns>The joint.</returns>
		/// <param name="joint">Joint.</param>
		public static ProjectedJoint ProjectJoint(AgaQ.Bricks.Joints.Joint joint)
		{
			return new ProjectedJoint (ProjectPosition (joint.transform.position), joint);
		}

		/// <summary>
		/// Project joints to screen coordinates.
		/// </summary>
		/// <returns>The joints.</returns>
		/// <param name="brick">AgaQ brick</param>
		public static ProjectedJoint[] ProjectJoints(AgaQBrick brick)
		{
			ProjectedJoint[] brickJoints = new ProjectedJoint[brick.joints.Length];

            //itareate over all joints and move its coordinates to screen space
            int j = 0;
            for (int i = 0; i < brickJoints.Length; i++)
            {
                if (brick.joints[i].other == null)
                    brickJoints[j++] = new ProjectedJoint(ProjectPosition(brick.joints[i].transform.position), brick.joints[i]);
            }

            Array.Resize<ProjectedJoint>(ref brickJoints, j);
			return brickJoints;
		}

        /// <summary>
        /// Projects position to camera screen space.
        /// </summary>
        /// <returns>The postion.</returns>
        /// <param name="position">Position.</param>
        public static Vector3 ProjectPosition(Vector3 position)
        {
            UnityEngine.Camera cam = UnityEngine.Camera.main;
            var projectedPosition = cam.WorldToScreenPoint(position);
            projectedPosition.z = 0;

            return projectedPosition;
        }
	}
}
