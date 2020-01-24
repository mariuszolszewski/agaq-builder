using UnityEngine;
using UnityEngine.PostProcessing;
using System.Linq;

namespace AgaQ.Bricks.Utils
{
    /// <summary>
    /// This class can render any mesh to texture.
    /// </summary>
    public class MeshProjector
    {
        Vector3 cameraPosition;
        int layer = 31;

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="position">Temporary position of rendering camera.</param>
        /// <param name="layer">Layer for all objects.</param>
        public MeshProjector(Vector3 position, int layer)
        {
            cameraPosition = position;
            this.layer = layer;
        }

        /// <summary>
        /// Render brick to texture
        /// </summary>
        /// <returns>The project.</returns>
        /// <param name="brick">Brick.</param>
        /// <param name="size">Texture size.</param>
        public RenderTexture Project(Brick brick, int size)
        {
            //create render texture
            RenderTexture texture = new RenderTexture(size, size, 16);

            //create camera
            GameObject cameraObject = new GameObject();
            UnityEngine.Camera camera = cameraObject.AddComponent<UnityEngine.Camera>();
            camera.targetTexture = texture;
            camera.transform.position = cameraPosition;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.allowHDR = true;
            camera.allowMSAA = false;
            camera.cullingMask = 1 << layer;
            //camera.orthographic = true;
            camera.orthographic = false;
            camera.aspect = 1f;
            camera.fieldOfView = 20;

            //add postprocessing stack to camera
            var posrprocessing = cameraObject.AddComponent<PostProcessingBehaviour>();
            posrprocessing.profile = (PostProcessingProfile)Resources.Load("MeshProjecotorPostProcessingProfile");

            //create lighting
            GameObject lightObject = new GameObject();
            lightObject.transform.rotation = Quaternion.Euler(new Vector3(60, 30, 0));
            lightObject.transform.SetParent(cameraObject.transform);
            lightObject.layer = layer; 
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = Color.white;
            light.shadows = LightShadows.None;

            //add object
            GameObject brickObject = UnityEngine.Object.Instantiate(brick.gameObject);
            brickObject.transform.rotation = Quaternion.Euler(new Vector3(-16, -43, 18));
            brickObject.transform.position = Vector3.zero;
            SetLayer(brickObject, layer);

            //position object to fit whole frame
            var instanciatedBrick = brickObject.GetComponent<Brick>();
            var bounds = instanciatedBrick.GetBounds();
            var corners = instanciatedBrick.GetBoundCorners();
            float boundSphereRadius = corners.Select(x => Vector3.Distance(x, bounds.center)).Max();
            float fov = Mathf.Deg2Rad * camera.fieldOfView;
            float camDistance = boundSphereRadius * 0.7f / Mathf.Tan(fov / 2f);

            camera.transform.position = new Vector3(
                bounds.center.x,
                bounds.center.y,
                bounds.center.z - camDistance
            );

            //render texture
            camera.Render();

            //cleanup
            camera.targetTexture = null;
            UnityEngine.Object.DestroyImmediate(brickObject);
            UnityEngine.Object.DestroyImmediate(cameraObject);

            return texture;
        }

        /// <summary>
        /// Set layer for object and all its childs
        /// </summary>
        /// <param name="o">O.</param>
        /// <param name="layer">Layer.</param>
        void SetLayer(GameObject o, int layer)
        {
            o.layer = layer;
            for (int i = 0; i < o.transform.childCount; i++)
            {
                var child = o.transform.GetChild(i);
                child.gameObject.layer = layer;
                SetLayer(child.gameObject, layer);
            }
        }
    }
}
