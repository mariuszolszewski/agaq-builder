using UnityEngine;
using UnityEngine.UI;
using AgaQ.Bricks;
using Lean.Localization;
using System.Collections.Generic;

namespace AgaQ.UI.Inspector
{
    public class InspectorDimensions : MonoBehaviour
    {
        [SerializeField] GameObject DimensionBoolPropertyPrefab;
        [SerializeField] GameObject DimensionNumberPropertyPrefab;
        [SerializeField] GameObject DimensionStringPropertyPrefab;

        [SerializeField] DimensionsImage descriptionImage;

        List<InspectorDimensionProperty> properties = new List<InspectorDimensionProperty>();

        /// <summary>
        /// Build dimencsion properties
        /// </summary>
        /// <param name="brick">Brick.</param>
        public void PrepareProperties(AgaQBrick brick)
        {
            Clear();

            if (brick.dimensionGroup == null)
                return;

            int index = 0;
            foreach (var dimension in brick.dimensionGroup.dimensions)
            {
                GameObject propertyObject;

                if (dimension.paramType == Bricks.DimensionsGroups.DimensionParamType.boolean)
                    propertyObject = Instantiate(DimensionBoolPropertyPrefab);
                else if (dimension.paramType == Bricks.DimensionsGroups.DimensionParamType.floatNumber ||
                    dimension.paramType == Bricks.DimensionsGroups.DimensionParamType.integerNumber)
                    propertyObject = Instantiate(DimensionNumberPropertyPrefab);
                else
                    propertyObject = Instantiate(DimensionStringPropertyPrefab);

                propertyObject.transform.SetParent(transform);
                var propertyScript = propertyObject.GetComponent<InspectorDimensionProperty>();
                propertyScript.ConfigureProperty(brick, index);
                properties.Add(propertyScript);
                index++;
            }

            //load description image
            string imagePath = brick.dimensionGroup.translationImageResourcePath;
            var translation = LeanLocalization.GetTranslation(imagePath);
            if (translation != null)
            {
                imagePath = translation.Text;
                if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                    imagePath = imagePath.Replace('/', '\\');

                var sprite = Resources.Load<Sprite>(imagePath);
                if (sprite != null)
                    descriptionImage.SetImage(sprite);
            }
        }

        /// <summary>
        /// Remove all diension properties
        /// </summary>
        public void Clear()
        {
            foreach (var property in properties)
                Destroy(property.gameObject);

            properties.Clear();

            descriptionImage.ClearImage();
        }
    }
}
