using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AgaQ.UI
{
    public class SidePanel : MonoBehaviour
    {
        [SerializeField] Image closeButtonImage;
        [SerializeField] Sprite closeImage;
        [SerializeField] Sprite openImage;

        [Space]
        [Tooltip("Open/close animation time.")]
        [SerializeField] float animationTime = 0.2f;
        [Tooltip("Side of the panel. -1 - close to left, 1 - close to right")]
        public int side = -1;

        RectTransform rectTransform;
        bool open = true; //is panel open

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Toggle panel open/close
        /// </summary>
        public void Toggle()
        {
            open = !open;
            closeButtonImage.sprite = open ? closeImage : openImage;
            StartCoroutine(OpenCloseAnimationCoorutine());
        }

        /// <summary>
        /// Opens or close panel using some pece of animation.
        /// </summary>
        /// <returns>The close animation coorutine.</returns>
        IEnumerator OpenCloseAnimationCoorutine()
        {
            float startTime = Time.time;
            float endTime = startTime + animationTime;
            float startPos = transform.position.x;
            float endPos = open ? startPos - side * rectTransform.sizeDelta.x : startPos + side * rectTransform.sizeDelta.x;
            Vector3 pos;

            while (Time.time <= endTime)
            {
                pos = transform.position;
                pos.x = Mathf.SmoothStep(startPos, endPos, (Time.time - startTime) / animationTime);
                transform.position = pos;

                yield return null;
            }

            // set position to end to be sure that panel will at the vey end position after animation
            pos = transform.position;
            pos.x = endPos;
            transform.position = pos;
        }
    }
}
