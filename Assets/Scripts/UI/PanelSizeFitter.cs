using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AgaQ.UI
{
    internal static class SetPropertyUtility
    {
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals(newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class PanelSizeFitter : UIBehaviour, ILayoutSelfController
    {
        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize
        }

        [SerializeField] protected FitMode m_VerticalFit = FitMode.Unconstrained;
        public FitMode verticalFit {
            get {
                return m_VerticalFit;
            }
            set {
                if (SetPropertyUtility.SetStruct(ref m_VerticalFit, value))
                    SetDirty(); 
            }
        }

        [System.NonSerialized] private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private DrivenRectTransformTracker m_Tracker;

        protected PanelSizeFitter()
        { }

        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        #endregion

        protected override void OnRectTransformDimensionsChange()
        {
            //SetDirty();
        }


        public virtual void SetLayoutHorizontal()
        {
        }

        public virtual void SetLayoutVertical()
        {
            if (verticalFit == FitMode.Unconstrained)
                return;

            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);

            // Set size to min or preferred size
            if (verticalFit == FitMode.MinSize)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, LayoutUtility.GetMinSize(m_Rect, 1));
            else
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, LayoutUtility.GetPreferredSize(m_Rect, 1));
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            SetDirty();
        }

#endif  
    }
}