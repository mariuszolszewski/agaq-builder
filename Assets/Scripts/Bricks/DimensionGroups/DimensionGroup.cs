using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Lean.Localization;

namespace AgaQ.Bricks.DimensionsGroups
{
    [Serializable]
    [CreateAssetMenu(fileName = "DimensionGroup", menuName = "Dimension group")]
    public class DimensionGroup : ScriptableObject
    {
        public string groupName;
        public DimensionParam[] dimensions;

        public List<DimensionGroupItem> bricksInGroup = new List<DimensionGroupItem>();

        [Tooltip("Translation label that will be translated to resource path.")]
        public String translationImageResourcePath;

        /// <summary>
        /// Get set of valid property values
        /// </summary>
        /// <returns>The property values.</returns>
        /// <param name="index">Property index.</param>
        public string[] GetPropertyValues(int index)
        {
            List<string> values = new List<string>();

            if (index < bricksInGroup[index].paramsValues.Length)
            {
                //collect values
                foreach (var brick in bricksInGroup)
                {
                    if (!values.Contains(brick.paramsValues[index]))
                        values.Add(brick.paramsValues[index]);
                }

                //if param type is string, translate collection members
                if (dimensions[index].paramType == DimensionParamType.text)
                {
                    for (int i = 0; i < values.Count; i++)
                    {
                        var translation = LeanLocalization.GetTranslation(values[i]);
                        values[i] = translation == null ? values[i] : translation.Text;
                    }
                }

                //sort values
                if (dimensions[index].paramType == DimensionParamType.integerNumber)
                    values = values.OrderBy(x => int.Parse(x)).ToList();
                else if (dimensions[index].paramType == DimensionParamType.floatNumber)
                    values = values.OrderBy(x => float.Parse(x)).ToList();
                else
                    values = values.OrderBy(x => x).ToList();
            }

            return values.ToArray();
        }

        /// <summary>
        /// Get resources path to brick with given set of group values.
        /// </summary>
        /// <returns>The path or null when there is no defined brick for this set of values.</returns>
        /// <param name="values">Values.</param>
        public string GetBrickPathForValues(string[] values)
        {
            if (values.Length != dimensions.Length)
                return null;

            //find brick definition
            foreach (var dim in bricksInGroup)
            {
                if (dim.CompareValues(values))
                    return dim.brickPath;
            }

            return null;
        }
    }
}
