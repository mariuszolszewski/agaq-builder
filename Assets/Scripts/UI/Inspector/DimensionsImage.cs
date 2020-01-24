using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AgaQ.UI.Inspector
{
    /// <summary>
    /// Class to controll dimensions preview image in isnpector
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class DimensionsImage : MonoBehaviour
    {
        [SerializeField] GameObject zoomImage;
        [SerializeField] Image largeImage;
        [SerializeField] RectTransform widthReferece;
        [SerializeField] float widthUpdateTime = 0.5f;

        Image image;
        float aspect; //image aspect ratio

        void Awake()
        {
            image = GetComponent<Image>();
            largeImage.gameObject.SetActive(false);
            ClearImage();
        }

        public void SetImage(Sprite sprite)
        {
            image.enabled = true;
            image.sprite = sprite;
            aspect =  (float)image.sprite.texture.height / (float)image.sprite.texture.width;
            var rect = GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 180f * aspect);
            zoomImage.SetActive(true);

            largeImage.sprite = sprite;
            rect = largeImage.gameObject.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 400f * aspect);

            StartCoroutine(SetWidthCoroutine());
        }

        public void ClearImage()
        {
            StopCoroutine(SetWidthCoroutine());
            image.enabled = false;
            zoomImage.SetActive(false);
        }

        public void OnZoomImagePointerEnter()
        {
            largeImage.gameObject.SetActive(true);
        }

        public void OnZoomImagePointerExit()
        {
            largeImage.gameObject.SetActive(false);
        }

        IEnumerator SetWidthCoroutine()
        {
            while (true)
            {
                //image.rectTransform.SetSizeWithCurrentAnchors(
                //    RectTransform.Axis.Vertical,
                //    LayoutUtility.GetPreferredWidth(GetComponent<RectTransform>()) * aspect
                //);

                yield return new WaitForSeconds(widthUpdateTime);
            }
        }
    }
}
