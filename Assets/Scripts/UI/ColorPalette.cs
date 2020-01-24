using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace AgaQ.UI
{
    /// <summary>
    /// Class handles picking color from image
    /// </summary>
    public class ColorPalette : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField] int width = 12;
        [SerializeField] int height = 10;
        [SerializeField] Sprite colorTile;
        [SerializeField] Sprite selector;
        [SerializeField] Color[] colors;

        public Action<Color> OnColorSelected;
        public Action OnColorConfirmed;

        Transform pallete;
        GameObject frameObejct;
        bool initialized = false;

    	void Start()
    	{
            BuildColorGrid();
    	}

        public void OnPointerExit(PointerEventData eventData)
        {
            ClearPrevColor();
            gameObject.SetActive(false);
        }

        void OnColorHover(Color color)
        {
            OnColorSelected?.Invoke(color);
        }

        void OnColorClick(Color color)
        {
            OnColorConfirmed?.Invoke();
            ClearPrevColor();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Build color grid tiles
        /// </summary>
        void BuildColorGrid()
        {
            if (initialized)
                return;
            
            pallete = transform.Find("Palette");

            //build first with grayscale
            for (int i = 0; i < width; i++)
            {
                float gray = 1f - (float)i / (width - 1);
                AddColorTile(pallete, new Color(gray, gray, gray));
            }

            //build color array
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pos = y * width + x;
                    if (pos < colors.Length)
                        AddColorTile(pallete, colors[pos]);
                }
            }

            initialized = true;
        }

        /// <summary>
        /// Build and add color tile to the pallete.
        /// </summary>
        /// <param name="cointainer">Cointainer.</param>
        /// <param name="color">Color.</param>
        void AddColorTile(Transform cointainer, Color color)
        {
            var go = new GameObject();
            go.transform.SetParent(cointainer);
            var image = go.AddComponent<Image>();
            image.sprite = colorTile;
            color.a = 1;
            image.color = color;
            var tile = go.AddComponent<ColorPaletteTile>();
            tile.OnHover += OnColorHover;
            tile.OnCLick += OnColorClick;
        }

        /// <summary>
        /// Hide box that indicates previous color.
        /// </summary>
        void ClearPrevColor()
        {
            if (frameObejct != null)
            {
                Destroy(frameObejct);
                frameObejct = null;
            }
        }

        /// <summary>
        /// Setup box over color tile to indicate it as previously selected color.
        /// </summary>
        /// <param name="color">Color.</param>
        public void SetPrevColor(Color color)
        {
            if (!initialized)
                BuildColorGrid();
            else
                ClearPrevColor();

            //find tile with closets color
            ColorPaletteTile tileWithClosetsColor = null;
            var tiles = GetComponentsInChildren<ColorPaletteTile>();
            foreach (var tile in tiles)
            {
                if (tileWithClosetsColor == null ||
                    ColorDistance(color, tile.color) < ColorDistance(color, tileWithClosetsColor.color))
                {
                    tileWithClosetsColor = tile;
                }
            }

            frameObejct = new GameObject();
            frameObejct.transform.SetParent(tileWithClosetsColor.transform);
            var img = frameObejct.AddComponent<Image>();
            img.sprite = selector;
            var rt = frameObejct.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        float ColorDistance(Color color1, Color color2)
        {
            return Mathf.Sqrt(
                Mathf.Pow(color2.r - color1.r, 2) +
                Mathf.Pow(color2.g - color1.g, 2) +
                Mathf.Pow(color2.b - color1.b, 2) 
            );
        }
    }
}
