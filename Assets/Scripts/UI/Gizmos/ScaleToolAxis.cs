using UnityEngine;
using UnityEngine.EventSystems;

namespace AgaQ.UI.Gizmos
{
    /// <summary>
    /// Script to controll gizmo axis for scale tool.
    /// </summary>
    public class ScaleToolAxis : GizmoHandle
    {
        Vector3 startPos; //base position
        Vector3[] startScale; //base scale of selectd bricks

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            startPos = transform.position;

            //remember start scale
            startScale = new Vector3[selected.Count];
            for (var i = 0; i < selected.Count; i++)
                startScale[i] = selected[i].transform.localScale;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            Plane plane;
            Ray ray;
            float distance, scaleFactor;

            switch (axis)
            {
                case Axis.x:
                    plane = new Plane(new Vector3(0, 0, 1), startPos);
                    if (eventData.enterEventCamera != null)
                    {
                        ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                        if (plane.Raycast(ray, out distance))
                        {
                            var hitPos = ray.GetPoint(distance);
                            transform.position = new Vector3(hitPos.x, transform.position.y, transform.position.z);
                            scaleFactor = Mathf.Clamp(1 + (startPos - transform.position).magnitude * Mathf.Sign(hitPos.x - startPos.x) * (right ? 1f : -1f), 0.001f, 100f);
                            ModifyScale(new Vector3(scaleFactor, 1, 1));
                        }
                    }
                    break;

                case Axis.y:
                    plane = new Plane(new Vector3(1, 0, 0), startPos);
                    if (eventData.enterEventCamera != null)
                    {
                        ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                        if (plane.Raycast(ray, out distance))
                        {
                            var hitPos = ray.GetPoint(distance);
                            transform.position = new Vector3(transform.position.x, hitPos.y, transform.position.z);
                            scaleFactor = Mathf.Clamp(1 + (startPos - transform.position).magnitude * Mathf.Sign(hitPos.y - startPos.y) * (right ? 1f : -1f), 0.001f, 100f);
                            ModifyScale(new Vector3(1, scaleFactor, 1));
                        }
                    }
                    break;

                case Axis.z:
                    plane = new Plane(new Vector3(0, 1, 0), startPos);
                    if (eventData.enterEventCamera != null)
                    {
                        ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                        if (plane.Raycast(ray, out distance))
                        {
                            var hitPos = ray.GetPoint(distance);
                            transform.position = new Vector3(transform.position.x, transform.position.y, hitPos.z);
                            scaleFactor = Mathf.Clamp(1 + (startPos - transform.position).magnitude * Mathf.Sign(hitPos.z - startPos.z) * (right ? 1f : -1f), 0.001f, 100f);
                            ModifyScale(new Vector3(1, 1, scaleFactor));
                        }
                    }
                    break;

                case Axis.any:
                    plane = new Plane(eventData.pointerCurrentRaycast.worldNormal, startPos);
                    if (eventData.enterEventCamera != null)
                    {
                        ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                        if (plane.Raycast(ray, out distance))
                        {
                            var hitPos = ray.GetPoint(distance);
                            transform.position = hitPos;
                            float sign = Mathf.Sign(Vector3.Dot(eventData.enterEventCamera.transform.right, hitPos - startPos));
                            scaleFactor = Mathf.Clamp(1 + (startPos - hitPos).magnitude * sign, 0.001f, 100f);
                            ModifyScale(new Vector3(scaleFactor, scaleFactor, scaleFactor));
                        }
                    }
                    break;
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            transform.position = startPos;
        }

        public void AdjustAxis(Bounds bounds, float scale)
        {
            if (axis == Axis.any)
                return;
            
            float distance = GetSize(bounds) / scale * 100f;

            if (axis == Axis.x)
                transform.localPosition = new Vector3(distance * (right ? -1f : 1f), 0, 0);
            else if (axis == Axis.y)
                transform.localPosition = new Vector3(0, distance * (right ? 1f : -1f), 0);
            else if (axis == Axis.z)
                transform.localPosition = new Vector3(0, 0, distance * (right ? -1f : 1f));
        }

        /// <summary>
        /// Modify scale of selected bricks.
        /// </summary>
        /// <param name="scaleFactor">Scale factor.</param>
        void ModifyScale(Vector3 scaleFactor)
        {
            for (var i = 0; i < selected.Count && i < startScale.Length; i++)
                selected[i].transform.localScale = new Vector3(
                    startScale[i].x * scaleFactor.x,
                    startScale[i].y * scaleFactor.y,
                    startScale[i].z * scaleFactor.z);
        }
    }
}
