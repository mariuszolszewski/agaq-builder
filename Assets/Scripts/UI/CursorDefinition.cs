using System;
using UnityEngine;

namespace AgaQ.UI
{
    /// <summary>
    /// Cursor definition.
    /// </summary>
    [Serializable]
    public class CursorDefinition
    {
        /// <summary>
        /// Cursor image
        /// </summary>
        public Texture2D image;

        /// <summary>
        /// Host point offset
        /// </summary>
        public Vector2 hotspot;
    }
}
