using UnityEngine.EventSystems;

namespace AgaQ.UI.Inspector
{
    /// <summary>
    /// Base class for all inspector properties.
    /// </summary>
    public abstract class InspectorProperty : UIBehaviour
    {
        /// <summary>
        /// Is property active
        /// </summary>
        public bool isActive
        {
            get;
            protected set;
        }

        /// <summary>
        /// Set property activtive / inactive
        /// </summary>
        /// <param name="active">If set to <c>true</c> active.</param>
        public virtual void SetActive(bool active)
        {
            isActive = active;
        }
    }
}
