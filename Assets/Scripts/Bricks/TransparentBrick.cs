using UnityEngine;
using System.Collections.Generic;
using AgaQ.Bricks.Tools;

namespace AgaQ.Bricks
{
    /// <summary>
    /// Brick that can become transparent.
    /// </summary>
    public abstract class TransparentBrick : SelectableBrick
    {
        public bool isTransparent {
            get;
            private set;
        }

        Renderer[] _renderers;
        Renderer[] rendrers {
            get {
                if (_renderers== null)
                    _renderers = GetComponentsInChildren<Renderer>();   

                return _renderers;
            }
        }

        List<Collider> colliders = new List<Collider>(); //list of collided colliders

        #region Event handlers

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 0 && !colliders.Contains(other))
            {
                colliders.Add(other);

                if (ToolsManager.instance.tool is MultiTool && selected)
                    return;

                if (!isTransparent && colliders.Count == 1)
                    SetTransparent(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 0)
            {
                colliders.Remove(other);

                if (isTransparent && colliders.Count == 0)
                    SetTransparent(false);
            }
        }

        #endregion

        public void SetTransparent(bool transparent)
        {
            if (transparent == this.isTransparent)
                return;
            
            this.isTransparent = transparent;

            if (transparent)
                TransparentOn();
            else
                TransparentOff();
        }

        /// <summary>
        /// Turn on transparency mode.
        /// </summary>
        void TransparentOn()
        {
            if (rendrers.Length == 0)
                return;
            
            foreach (var renderer in rendrers)
            {
                var color = renderer.material.color;
                color.a = Preferences.instance.transparentAlfa;
                renderer.material = Preferences.instance.transparentBrickMaterial;
                renderer.material.color = color;
            }
        }

        /// <summary>
        /// Turn off trnasparency mode
        /// </summary>
        void TransparentOff()
        {
            if (rendrers.Length == 0)
                return;
            
            foreach (var renderer in rendrers)
            {
                var color = renderer.material.color;
                color.a = 1f;
                renderer.material = Preferences.instance.normalBrickMaterial;
                renderer.material.color = color;

            }
        }
    }
}
