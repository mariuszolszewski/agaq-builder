using UnityEngine;

namespace AgaQ.Bricks
{
	/// <summary>
	/// State of the mesh material.
	/// Can hold color and blendMode.
	/// </summary>
    public class MaterialState
    {
        public Material material;

        public MaterialState(Material material)
        {
            this.material = material;
        }
            
        public void SetTransparent()
        {
        }

        /// <summary>
        /// Restore material state
        /// </summary>
        public void RestoreState()
        {
        }
    }
}
