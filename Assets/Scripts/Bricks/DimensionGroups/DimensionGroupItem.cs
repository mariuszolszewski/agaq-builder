using System;
using UnityEngine;

namespace AgaQ.Bricks.DimensionsGroups
{
    [Serializable]
    public struct DimensionGroupItem
    {
        public string brickPath;
        public string[] paramsValues;

        public DimensionGroupItem(string path, string[] paramsValues)
        {
            this.brickPath = path;
            this.paramsValues = paramsValues;
        }

        /// <summary>
        /// Compare stored values with given array.
        /// </summary>
        /// <returns><c>true</c>, if values are identical, <c>false</c> otherwise.</returns>
        /// <param name="values">Values.</param>
        public bool CompareValues(string[] values)
        {
            if (values.Length != paramsValues.Length)
                return false;

            for (int i = 0; i < values.Length; i++)
            {
                if (paramsValues[i] != values[i])
                    return false;
            }

            return true;
        }
    }
}
