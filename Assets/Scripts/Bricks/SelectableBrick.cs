using UnityEngine;
using UnityEngine.EventSystems;
using AgaQ.Bricks.Tools;
using cakeslice;
using System;

namespace AgaQ.Bricks
{
    /// <summary>
    /// Brick model that can be selected by user.
    /// Class also handler visual effect of seleting
    /// </summary>
    public abstract class SelectableBrick : HighlightableBrick, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [NonSerialized] public bool selected;

        Outline _outline;
        Outline outline
        {
            get
            {
                if (_outline == null)
                {
                    if (this is AgaQGroup)
                    {
                        _outline = GetComponent<Outline>();
                        if (_outline == null)
                            _outline = this.gameObject.AddComponent<Outline>();
                    }
                    else
                    {
                        _outline = GetComponentInChildren<Outline>();
                        if (_outline == null)
                        {
                            var renderer = GetComponentInChildren<Renderer>();
                            if (renderer != null)
                                _outline = renderer.gameObject.AddComponent<Outline>();
                        }
                    }
                }

                return _outline;
            }
        }

        public virtual void OnPointerClick(PointerEventData pointerEventData)
        {
            if (grouped)
            {
                transform.parent?.gameObject.GetComponentInParent<SelectableBrick>()?.OnPointerClick(pointerEventData);
                return;
            }

            if (pointerEventData.button == PointerEventData.InputButton.Left)
                ToolsManager.instance.tool.OnClick(this, pointerEventData);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (grouped)
            {
                transform.parent?.gameObject.GetComponentInParent<SelectableBrick>()?.OnPointerDown(eventData);
                return;
            }

            ToolsManager.instance.tool.OnPointerDown(this, eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (grouped)
            {
                transform.parent?.gameObject.GetComponentInParent<SelectableBrick>()?.OnPointerUp(eventData);
                return;
            }

            ToolsManager.instance.tool.OnPointerUp(this, eventData);
        }

		/// <summary>
		/// Set material as selected or not.
		/// </summary>
		/// <param name="selected">If set to <c>true</c> selected.</param>
        public void SetSelected(bool selected)
        {
            this.selected = selected;

            if (outline == null)
                return;
            
            if (selected)
               outline.enabled = true;
            else
                outline.enabled = false;
        }
    }
}
