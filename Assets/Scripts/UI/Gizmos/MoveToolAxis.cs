using UnityEngine;
using UnityEngine.EventSystems;
using AgaQ.Bricks;
using AgaQ.Bricks.Tools;

namespace AgaQ.UI.Gizmos
{
    /// <summary>
    /// Controller for move tool gizmo.
    /// </summary>
    public class MoveToolAxis : GizmoHandle
    {
        [SerializeField] Transform cone;
        [SerializeField] Transform cylinder;

        Vector3 dragOffset; //offset bettwen drag point and axis position
        Vector3 startPos;
        Vector3 lastPos;

        bool stickyMove = false;
        GameObject saxis;
        Bounds bounds;

        const float pointSelectAccuracy = 5f;

        void Start()
        {
            saxis = GetComponentInParent<Multitool>().stickyAxis;    
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            stickyMove = eventData.pointerPressRaycast.gameObject.name == "point";
            startPos = transform.position;
            lastPos = startPos;
            dragOffset = transform.position - eventData.pointerPressRaycast.worldPosition;

            //add rigidbodies
            foreach (var s in selected)
            {
                var rb = s.GetComponent<Rigidbody>();
                if (rb == null)
                    rb = s.gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;

                rb.constraints = RigidbodyConstraints.FreezeRotation;
                if (axis == Axis.x)
                    rb.constraints = rb.constraints | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                else if (axis == Axis.y)
                    rb.constraints = rb.constraints | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                else if (axis == Axis.z)
                    rb.constraints = rb.constraints | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
            }

            //set all colliders to trigger
            foreach (var s in selected)
            {
                var clds = s.GetComponentsInChildren<Collider>();
                foreach (var cld in clds)
                {
                    if (!(cld is MeshCollider))
                        cld.isTrigger = true;
                }
            }

            //calculate bounds
            bounds = selected[0].GetBounds();
            for (int i = 1; i < selected.Count; i++)
                bounds.Encapsulate(selected[i].GetBounds());
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (stickyMove)
                HandleStickyMove(eventData);
            else
            {
                Plane plane;
                Ray ray;
                float distance;

                switch (axis)
                {
                    case Axis.x:
                        if (Multitool.instance.global)
                            plane = new Plane(new Vector3(0, 0, 1), startPos);
                        else
                            plane = new Plane(Multitool.instance.transform.rotation * new Vector3(0, 0, 1), startPos);
                        if (eventData.enterEventCamera != null)
                        {
                            ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                            if (plane.Raycast(ray, out distance))
                            {
                                var hitPos = ray.GetPoint(distance);
                                var o = hitPos - lastPos + dragOffset;
                                if (Multitool.instance.global)
                                {
                                    o.y = 0;
                                    o.z = 0;
                                }
                                else
                                    o = Vector3.Project(o, (cone.transform.position - cylinder.transform.position).normalized);
                                ChangePosition(o);
                            }
                        }
                        break;

                    case Axis.y:
                        if (Multitool.instance.global)
                            plane = new Plane(new Vector3(1, 0, 0), startPos);
                        else
                            plane = new Plane(Multitool.instance.transform.rotation * new Vector3(1, 0, 0), startPos);
                        if (eventData.enterEventCamera != null)
                        {
                            ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                            if (plane.Raycast(ray, out distance))
                            {
                                var hitPos = ray.GetPoint(distance);
                                var o = hitPos - lastPos + dragOffset;
                                if (Multitool.instance.global)
                                {
                                    o.x = 0;
                                    o.z = 0;
                                }
                                else
                                    o = Vector3.Project(o, (cone.transform.position - cylinder.transform.position).normalized);
                                ChangePosition(o);
                            }
                        }
                        break;

                    case Axis.z:
                        if (Multitool.instance.global)
                            plane = new Plane(new Vector3(0, 1, 0), startPos);
                        else
                            plane = new Plane(Multitool.instance.transform.rotation * new Vector3(0, 1, 0), startPos);
                        if (eventData.enterEventCamera != null)
                        {
                            ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                            if (plane.Raycast(ray, out distance))
                            {
                                var hitPos = ray.GetPoint(distance);
                                var o = hitPos - lastPos + dragOffset;
                                    if (Multitool.instance.global)
                                {
                                    o.x = 0;
                                    o.y = 0;
                                }
                                else
                                    o = Vector3.Project(o, (cone.transform.position - cylinder.transform.position).normalized);
                                ChangePosition(o);
                            }
                        }
                        break;
                }
            }
        }

        void HandleStickyMove(PointerEventData eventData)
        {
            var selectioManager = SelectionManager.instance;
            var seleted = selectioManager.GetSelected();

            //change temporary layer of selcted bricks (that are under movement)
            foreach (var sel in seleted)
                sel.gameObject.layer = 9;

            var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, 1, QueryTriggerInteraction.Collide))
            {
                var go = hit.collider.gameObject;
                var meshFilter = go.GetComponentInChildren<MeshFilter>();
                if (meshFilter != null)
                {
                    var currentMeshVerises = meshFilter.gameObject.GetComponent<Vertices>();
                    if (currentMeshVerises == null)
                        currentMeshVerises = meshFilter.gameObject.AddComponent<Vertices>();

                    Vector2 currentScreenPoint;
                    Vector3 currentWorldPoint;

                    if (MeshPointsTool.GetVisiblePointUnderCursor(
                        currentMeshVerises.uniqueVertices,
                        meshFilter.gameObject.transform,
                        UnityEngine.Camera.main,
                        Input.mousePosition,
                        pointSelectAccuracy,
                        out currentScreenPoint,
                        out currentWorldPoint))
                    {
                        MeasureToolVisualizer.instance.HighlighVertex(currentScreenPoint);

                        //move there
                        Vector3 relPos = currentWorldPoint - cone.position;
                        ChangePosition(relPos);
                    }
                    else
                        MeasureToolVisualizer.instance.ResetHighlight();  
                }
                else
                    MeasureToolVisualizer.instance.ResetHighlight();  
            }
            else
                MeasureToolVisualizer.instance.ResetHighlight();  

            //set back default layer mask for seleced
            foreach (var sel in seleted)
                sel.gameObject.layer = 0;
        }

        float BoundsDistance(Bounds b1, Bounds b2, Axis axis)
        {
            if (axis == Axis.x)
                return Mathf.Abs(b1.center.x - b2.center.x) - b1.size.x - b2.size.x;
            if (axis == Axis.y)
                return Mathf.Abs(b1.center.y - b2.center.y) - b1.size.y - b2.size.y;
            return Mathf.Abs(b1.center.z - b2.center.z) - b1.size.z - b2.size.z;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            //remove rigidbodies
            foreach (var s in selected)
            {
                var rbs = s.GetComponentsInChildren<Rigidbody>();
                foreach (var rb in rbs)
                    Destroy(rb);
            }

            saxis.SetActive(false);

            if (stickyMove)
                MeasureToolVisualizer.instance.ResetHighlight();
        }

        public void AdjustAxis(Bounds bounds, float scale)
        {
            float distance = GetSize(bounds) / scale;

            //adjust cone position
            cone.localPosition = new Vector3(0, 0, -distance);

            //adjust cylinder scale
            float newScale = distance * 1.4f / 0.35f * 12f;
            cylinder.localScale = new Vector3(cylinder.localScale.x, cylinder.localScale.y, newScale);
        }

        void ChangePosition(Vector3 offset)
        {
            if (ToolsManager.instance.snapToGridButton.isSelected)
            {
                if (lastPos.y + offset.y - bounds.size.y / 2 < 0)
                    offset.y = bounds.size.y - lastPos.y - bounds.size.y / 2;
            }

            lastPos += offset;

            for (var i = 0; i < selected.Count; i++)
            {
                selected[i].transform.position += offset;
                selected[i].SetHighlighted(false);
            }
        }

        void OnTriggerEnter(Collider other)
        {
        }
    }
}
