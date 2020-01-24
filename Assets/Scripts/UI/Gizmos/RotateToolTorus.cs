using UnityEngine;
using UnityEngine.EventSystems;

namespace AgaQ.UI.Gizmos
{
    /// <summary>
    /// Controller for rotation tool gizmo
    /// </summary>
    public class RotateToolTorus : GizmoHandle
    {
        Vector3 startPos;
        Vector3 startVector; //vector from pivot to clik point
        float lastAngle;
        RotationGizmo gizmo;

        void Start()
        {
            gizmo = GetComponentInParent<RotationGizmo>();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            startPos = transform.position;
            startVector = eventData.pointerPressRaycast.worldPosition - startPos;
            lastAngle = 0;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            Plane plane;
            Ray ray;
            float distance;

            switch (axis)
            {
                case Axis.x:
                    plane = new Plane(new Vector3(1, 0, 0), startPos);
                    if (eventData.enterEventCamera != null)
                    {
                        ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                        if (plane.Raycast(ray, out distance))
                        {
                            var hitPos = ray.GetPoint(distance);
                            var endVector = hitPos - startPos;
                            var angle = Vector3.SignedAngle(startVector, endVector, new Vector3(1, 0, 0));
                            Rotate(new Vector3(angle - lastAngle, 0, 0));
                            lastAngle = angle;
                        }
                    }
                    break;

                case Axis.y:
                    plane = new Plane(new Vector3(0, 1, 0), startPos);
                    if (eventData.enterEventCamera != null)
                    {
                        ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                        if (plane.Raycast(ray, out distance))
                        {
                            var hitPos = ray.GetPoint(distance);
                            var endVector = hitPos - startPos;
                            var angle = Vector3.SignedAngle(startVector, endVector, new Vector3(0, 1, 0));
                            Rotate(new Vector3(0, angle - lastAngle, 0));
                            lastAngle = angle;
                        }
                    }
                    break;

                case Axis.z:
                    plane = new Plane(new Vector3(0, 0, 1), startPos);
                    if (eventData.enterEventCamera != null)
                    {
                        ray = eventData.enterEventCamera.ScreenPointToRay(Input.mousePosition);
                        if (plane.Raycast(ray, out distance))
                        {
                            var hitPos = ray.GetPoint(distance);
                            var endVector = hitPos - startPos;
                            var angle = Vector3.SignedAngle(startVector, endVector, new Vector3(0, 0, 1));
                            Rotate(new Vector3(0, 0, angle - lastAngle));
                            lastAngle = angle;
                        }
                    }
                    break;
            }

            gizmo.AnglesChaged();
        }

        void Rotate(Vector3 offset)
        {
            var rotationCenter = Multitool.instance.GetBounds().center;
            //Debug.LogFormat("Rot offse = {0}", offset.ToEuler());

            for (var i = 0; i < selected.Count; i++)
            {
                selected[i].transform.RotateAround(rotationCenter, Vector3.right, offset.x);
                selected[i].transform.RotateAround(rotationCenter, Vector3.up, offset.y);
                selected[i].transform.RotateAround(rotationCenter, Vector3.forward, offset.z);

                //selected[i].transform.rotation *= offset;
                //selected[i].SetHighlighted(false);
            }
        }
    }
}
