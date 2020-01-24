using UnityEngine;
using AgaQ.Bricks;
using AgaQ.Bricks.Tools;

namespace AgaQ.UI.Inspector
{
    public class InspectorScaleProperty : InspectorProperty
    {
        /// <summary>
        /// Scale selector controll built in property.
        /// </summary>
        public ScaleSelector scaleSelector
        {
            get;
            protected set;
        }

        protected override void Start()
        {
            base.Start();

            scaleSelector = GetComponentInChildren<ScaleSelector>();   
            if (scaleSelector != null)
                scaleSelector.OnScaleChange += OnScaleChanged;
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            if (scaleSelector != null)
                scaleSelector.gameObject.SetActive(active);
        }

        void OnScaleChanged()
        {
            var bricks = SelectionManager.instance.GetSelected();

            if (bricks.Count == 0)
            {
                var tm = ToolsManager.instance;
                if (tm.tool is MoveTool)
                {
                    var moveTool = tm.tool as MoveTool;
                    if (moveTool.isDragging && moveTool.brick != null)
                        moveTool.brick.scale = scaleSelector.scale;
                }
            }
            else
            {
                //calculate center of selection
                Vector3 center = bricks[0].transform.position;
                for (int i = 1; i < bricks.Count; i++)
                    center += bricks[i].transform.position;
                center /= bricks.Count;

                float scaleFactor = scaleSelector.scale / bricks[0].scale;

                //iterate over all bricks
                foreach (var brick in bricks)
                {
                    //change position acording to selection center and scale
                    var posVector = brick.transform.position - center;
                    float distance = posVector.magnitude * scaleFactor;
                    brick.transform.position = center + posVector.normalized * distance;

                    //change scale
                    brick.scale = scaleSelector.scale;
                }
            }
        }
    }
}
