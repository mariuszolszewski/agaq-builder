using UnityEngine;

namespace AgaQ.Bricks.Positioners
{
    /// <summary>
    /// Struct represent proposed position and rotation returned by positioner class.
    /// </summary>
	public struct ProposedPosition
	{
        /// <summary>
        /// Proposed position.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// Proposed rotation.
        /// </summary>
        public Quaternion rotation;

        /// <summary>
        /// True if proposed position can be final position where you can stop moving brick.
        /// </summary>
        public bool isValid;

        /// <summary>
        /// True if brick in proposed position is joined with other.
        /// </summary>
        public bool isJoined;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgaQ.Bricks.Positioners.ProposedPosition"/> struct.
        /// </summary>
        /// <param name="transform">Transform.</param>
        public ProposedPosition(Transform transform) : this(transform, false, false)
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="AgaQ.Bricks.Positioners.ProposedPosition"/> struct.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="isValid">Is position valid</param>
        public ProposedPosition(Transform transform, bool isValid, bool isJoined)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
            this.isValid = isValid;
            this.isJoined = isJoined;
        }
	}
}
