using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace AgaQ.Camera
{
    /// <summary>
    /// Class represents grid bouds
    /// </summary>
    public class GridBounds : ICloneable
    {
        public float minX;
        public float maxX;
        public float minZ;
        public float maxZ;

        float halfInitialSize;

        #region Constructors

        public GridBounds(float size)
        {
            halfInitialSize = Mathf.Ceil(size / 2f);
            minX = -halfInitialSize;
            maxX = halfInitialSize;
            minZ = -halfInitialSize;
            maxZ = halfInitialSize;
        }

        #endregion

        /// <summary>
        /// Set new bounds size from other bound
        /// </summary>
        /// <param name="bounds">Bounds.</param>
        public void SetSize(Bounds bounds)
        {
            minX = bounds.min.x;
            maxX = bounds.max.x;
            minZ = bounds.min.z;
            maxZ = bounds.max.z;

            Normalize();
        }

        /// <summary>
        /// Expand bound size to first natural numners.
        /// </summary>
        void Normalize()
        {
            minX = Mathf.Floor(minX);
            maxX = Mathf.Ceil(maxX);
            minZ = Mathf.Floor(minZ);
            maxZ = Mathf.Ceil(maxZ);

            if (minX > -halfInitialSize)
                minX = -halfInitialSize;
            if (maxX < halfInitialSize)
                maxX = halfInitialSize;
            if (minZ > -halfInitialSize)
                minZ = -halfInitialSize;
            if (maxZ < halfInitialSize)
                maxZ = halfInitialSize;            
        }
            
        /// <summary>
        /// Clone grid structure
        /// </summary>
        /// <returns>The clone.</returns>
        public object Clone()
        {
            GridBounds newBounds = (GridBounds)this.MemberwiseClone();
            return newBounds;
        }
            
        /// <summary>
        /// Get bouds structure.
        /// </summary>
        /// <returns>The bounds.</returns>
        public Bounds GetBounds()
        {
            Vector3 center = new Vector3(minX + (maxX - minX) / 2, 0, minZ + (maxZ - minZ) / 2);
            Vector3 size = new Vector3(maxX - minX, 0, maxZ - minZ);

            Bounds bounds = new Bounds(center, size);

            return bounds;
        }

        #region Operators

        public static bool operator == (GridBounds bounds1, GridBounds bounds2)
        {
            return Comparison(bounds1, bounds2);
        }

        public static bool operator != (GridBounds bounds1, GridBounds bounds2)
        {
            return !Comparison(bounds1, bounds2);
        }

        static bool Comparison(GridBounds bounds1, GridBounds bounds2)
        {
            return
                bounds1.minX == bounds2.minX &&
                bounds1.maxX == bounds2.maxX &&
                bounds1.minZ == bounds2.minZ &&
                bounds1.maxZ == bounds2.maxZ;            
        }
            
        #endregion
    }
}
