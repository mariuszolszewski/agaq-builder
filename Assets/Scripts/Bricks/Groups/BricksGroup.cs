using UnityEngine;
using System;

namespace AgaQ.Bricks.Groups
{
    /// <summary>
    /// Provides definition of the bricks group
    /// </summary>
    [Serializable]
    public class BricksGroup
    {
        public int groupId;
        public string translationLabel;
        public Sprite icon;
    }
}
