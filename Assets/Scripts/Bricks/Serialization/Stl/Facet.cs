using UnityEngine;

namespace AgaQ.Bricks.Serialization.STL
{
    class Facet
    {
        public Vector3 normal;
        public Vector3 a, b, c;

        public override string ToString()
        {
            return string.Format("{0:F2}: {1:F2}, {2:F2}, {3:F2}", normal, a, b, c);
        }
    }
}
