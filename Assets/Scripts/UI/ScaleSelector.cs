using UnityEngine;
using UnityEngine.UI;
using System;
using AgaQ.Bricks;
using System.Collections.Generic;

namespace AgaQ.UI
{
	/// <summary>
	/// UI control to set brick scale.
	/// </summary>
	public class ScaleSelector : MonoBehaviour
	{
		[SerializeField] Text scaleText;
		[SerializeField] float scaleStep = 2;
        [SerializeField] float maxScale = 4;
        [SerializeField] float minScale = 0.25f;

		public float scale {
			get;
			private set;
		}

        public Action OnScaleChange;

		void Awake ()
		{
			scale = 1;
            SelectionManager.instance.OnSelectionChange += OnSelectionChange;
		}

		/// <summary>
		/// Update control text field.
		/// </summary>
		void UpdateScaleText()
		{
			scaleText.text = string.Format ("{0}", scale);
		}

		/// <summary>
		/// Change scale up by scaleStep.
		/// </summary>
		public void UpScale()
		{
            if (scale >= maxScale)
                return;
            
			scale *= scaleStep;
			UpdateScaleText ();
            ScaleChange();

            if (OnScaleChange != null)
                OnScaleChange();
		}

		/// <summary>
		/// Change scale down by scaleStep.
		/// </summary>
		public void DownScale()
		{
            if (scale <= minScale)
                return;
            
			scale /= scaleStep;
			UpdateScaleText ();
            ScaleChange();

            if (OnScaleChange != null)
                OnScaleChange();
		}

        public void SetScale(float scale)
        {
            if (scale < minScale)
                scale = minScale;
            else if (scale > maxScale)
                scale = maxScale;

            this.scale = scale;

            UpdateScaleText();
        }

        void OnSelectionChange(List<SelectableBrick> selected)
        {
            if (selected.Count == 0)
                SetScale(1);
            else
            {
                if (selected[0] is AgaQBrick)
                    SetScale(selected[0].transform.localScale.x);
                else
                    SetScale(selected[0].transform.localScale.x * 20f);
            }
            
            UpdateScaleText();
        }

        void ScaleChange()
        {
            var bricks = SelectionManager.instance.GetSelected();
            if (bricks.Count > 0)
            {
                Vector3 center = bricks[0].transform.position;
                for (int i = 1; i < bricks.Count; i++)
                    center += bricks[i].transform.position;
                center /= bricks.Count;

                float scaleFactor = scale / bricks[0].scale;

                //iterate over all bricks
                foreach (var brick in bricks)
                {
                    //change position acording to selection center and scale
                    var posVector = brick.transform.position - center;
                    float distance = posVector.magnitude * scaleFactor;
                    brick.transform.position = center + posVector.normalized * distance;

                    //change scale
                    if (brick is AgaQBrick)
                        brick.scale = scale;
                    else
                        brick.scale = scale / 20f;
                }
            }
        }
    }
}
