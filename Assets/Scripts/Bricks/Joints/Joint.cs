using UnityEngine;

namespace AgaQ.Bricks.Joints
{
    /// <summary>
    /// This class represents brick joint.
    /// </summary>
    public abstract class Joint : MonoBehaviour
    {
        //rotation, position and scale is from transform
        //add angle for sliced bricks

        public bool ignoreYRotation;
        public Joint other;

        AgaQBrick _parentBrick;
        public AgaQBrick parentBrick
        {
            get
            {
                if (_parentBrick == null)
                    _parentBrick = GetComponentInParent<AgaQBrick>();

                return _parentBrick;
            }
        }

        /// <summary>
        /// Check if two joints are compatibile (can be joined)
        /// </summary>
        /// <returns><c>true</c>, if are joinable, <c>false</c> otherwise.</returns>
        /// <param name="joint1">Joint1.</param>
        /// <param name="joint2">Joint2.</param>
        public static bool AreJoinable(Joint joint1, Joint joint2)
        {
            return
                (joint1.other == null && joint2.other == null &&
                ((joint1 is MaleJoint && (joint2 is FemaleJoint || joint2 is FemaleBorderJoint)) ||
                (joint1 is FemaleJoint && joint2 is MaleJoint) ||
                (joint1 is FemaleBorderJoint && (joint2 is MaleJoint || joint2 is FemaleBorderJoint))) &&
                joint1.transform.lossyScale == joint2.transform.lossyScale);
        }

        /// <summary>
        /// Clear all brick jopints (set to null) and in joined bricks.
        /// </summary>
        public void ClearJoint()
        {
            if (other != null)
            {
                other.other = null;
                other = null;
            }
        }

        /// <summary>
        /// Set joint to referece brick each other.1
        /// </summary>
        /// <param name="otherJoint"></param>
        public void SetJoint(Joint otherJoint)
        {
            otherJoint.other = this;
            other = otherJoint;
        }
    }
}
