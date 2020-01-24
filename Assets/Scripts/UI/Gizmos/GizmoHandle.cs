using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using AgaQ.Bricks;
using AgaQ.Bricks.History;

namespace AgaQ.UI.Gizmos
{
    /// <summary>
    /// Abstarct base class for all gizmos handles.
    /// Provide functionality of haighlighting handle on mouse over event.
    /// </summary>
    public abstract class GizmoHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] protected Axis axis; //axis controlled by this gizmo
        [SerializeField] protected bool right; //true if this is gizmo in right direction (positivie values)
        [SerializeField] Material highlightMaterial; //material to set when gizmo is hover

        HistoryNodeTransform[] historyNodes;
        Material[] oldMaterials;

        protected Vector3 startDragPos; //drag handle position
        protected List<SelectableBrick> selected; //lsit of selected bricks;

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetHiglightMaterial();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            RestoreMaterials();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            startDragPos = Input.mousePosition;
            selected = SelectionManager.instance.GetSelected();
            historyNodes = HistoryTool.PrepareTransformNodes(selected);
        }

        public virtual void OnDrag(PointerEventData eventData) {}

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            HistoryManager.instance.Register(historyNodes);

            //turn off transparency in all bricks
            var bricks = GameObject.Find("Model").GetComponentsInChildren<TransparentBrick>();
            foreach (var brick in bricks)
                brick.SetTransparent(false);
        }

        /// <summary>
        /// Get bounds size acording to set axe.
        /// </summary>
        /// <returns>The size.</returns>
        /// <param name="bounds">Bounds.</param>
        protected float GetSize(Bounds bounds)
        {
            if (axis == Axis.x)
                return bounds.size.x;
            if (axis == Axis.y)
                return bounds.size.y;
            if (axis == Axis.z)
                return bounds.size.z;
            return 0;
        }

        /// <summary>
        /// Set highlight material for all child renderers.
        /// </summary>
        void SetHiglightMaterial()
        {
            var renderers = GetComponentsInChildren<MeshRenderer>();
            oldMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                oldMaterials[i] = renderers[i].sharedMaterial;              
                renderers[i].sharedMaterial = highlightMaterial;
            }
        }

        /// <summary>
        /// Restore old materials for all child renderers.
        /// </summary>
        void RestoreMaterials()
        {
            var renderers = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < oldMaterials.Length; i++)
            {
                renderers[i].sharedMaterial = oldMaterials[i];
            }
        }
    }
}
