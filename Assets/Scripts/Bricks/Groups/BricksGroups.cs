using UnityEngine;

namespace AgaQ.Bricks.Groups
{
    /// <summary>
    /// Collection of brics groupefinitions.
    /// </summary>
    [CreateAssetMenu(fileName = "BricsGroups", menuName = "Bricks groups")]
    public class BricksGroups : ScriptableObject
    {
        public BricksGroup[] groups;
    }
}
