using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;
using AgaQ.Bricks;

namespace AgaQ.UI.Inspector
{
    /// <summary>
    /// base inspector control to change brick dimension.
    /// </summary>
    public abstract class InspectorDimensionProperty : MonoBehaviour
    {
        [SerializeField] Text labelText;

        protected AgaQBrick brick;
        protected int groupPropIndex;   //property index in group
        protected string[] values;       //valid property values, sorted increasing
        protected int currentValueIndex;

        /// <summary>
        /// Prepare property to display given dimension
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="index">Property index.</param>
        public virtual void ConfigureProperty(AgaQBrick brick, int index)
        {
            this.brick = brick;
            groupPropIndex = index;
            values = brick.dimensionGroup.GetPropertyValues(index);

            //set label
            var translationKey = brick.dimensionGroup.dimensions[index].translationLabel;
            var translation = LeanLocalization.GetTranslation(translationKey);
            string translatedLabel = translation == null ? translationKey : translation.Text;
            labelText.text = translatedLabel;
        }
    }
}
