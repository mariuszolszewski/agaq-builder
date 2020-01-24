using UnityEngine;
using AgaQ.Bricks.Serialization;
using System;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Instanciate new bricks at editor scene.
    /// </summary>
    public class BrickBuilder
    {
        /// <summary>
        /// Instanciate new brick by at scene editor in the middle of camera view.
        /// </summary>
        /// <returns>The instansiate.</returns>
        /// <param name="resourcePath">Resource path of thr brick.</param>
        /// <returns>Brick</returns>
        public static Brick InstansiateFromResources(string resourcePath)
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer)
                resourcePath = resourcePath.Replace('/', '\\');

            GameObject gObject = Resources.Load(resourcePath) as GameObject;
            if (gObject == null)
                return null;

            Brick brick = gObject.GetComponent<Brick>();
            if (brick == null)
                return null;
            
            return Instansiate(brick);
        }

        public static Brick Instansiate(Mesh[] meshes)
        {
            if (meshes.Length == 0)
                return null;

            if (meshes.Length == 1)
                return PrepareBrickObject(meshes[0]);

            var parentObject = new GameObject();
            parentObject.transform.SetParent(GameObject.Find("Model").transform);
            var groupScript = parentObject.AddComponent<BricksGroup>();

            foreach (var mesh in meshes)
            {
                var brick = PrepareBrickObject(mesh);
                brick.grouped = true;
                brick.transform.SetParent(parentObject.transform);
            }

            return groupScript;
        }
 
        public static Brick InstansiateFromFile(string path)
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer)
                path = path.Replace('/', '\\');

            Brick brick = null;

            try
            {
                GameObject tmpObject = new GameObject();
                Model.instance.Deserialize(path, tmpObject.transform);
                var firstChild = tmpObject.transform.GetChild(0);
                if (firstChild != null)
                {
                    firstChild.SetParent(GameObject.Find("Model").transform);
                    brick = firstChild.GetComponent<Brick>();
                }
                GameObject.Destroy(tmpObject);

                brick.transform.rotation = Quaternion.Euler(Vector3.zero);
                brick.transform.position = Vector3.zero;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return brick;
        }

        /// <summary>
        /// Instanciate new group object.
        /// </summary>
        /// <returns>The group.</returns>
        public static AgaQGroup InstansiateAgaQGroup()
        {
            GameObject groupObject = new GameObject();
            groupObject.name = "AgaQ Group";
            groupObject.transform.SetParent(GameObject.Find("Model").transform);
            AgaQGroup groupScript = groupObject.AddComponent<AgaQGroup>();

            return groupScript;
        }

        /// <summary>
        /// Instanciate new group object.
        /// </summary>
        /// <returns>The group.</returns>
        public static BricksGroup InstansiateBricksGroup()
        {
            GameObject groupObject = new GameObject();
            groupObject.name = "Bricks Group";
            groupObject.transform.SetParent(GameObject.Find("Model").transform);
            BricksGroup groupScript = groupObject.AddComponent<BricksGroup>();

            return groupScript;
        }

        /// <summary>
        /// Clone brick.
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <returns>Brick</returns>
        public static Brick Clone(Brick brick)
        {
            //clone
            var newObject = GameObject.Instantiate(brick.gameObject);
            var newBrick = newObject.GetComponent<Brick>();

            //set parent
            newObject.transform.SetParent(GameObject.Find("Model")?.transform);

            //turn off higlight and select
            //and set grouped property for children
            var children = newObject.GetComponentsInChildren<SelectableBrick>();
            foreach (var child in children)
            {
                if (child != newBrick)
                    child.grouped = true;
                child.SetSelected(false);
                child.SetHighlighted(false);
            }

            return newBrick;
        }

        /// <summary>
        /// Instanciate new brick by at scene editor in the middle of camera view.
        /// </summary>
        /// <returns>The instansiate.</returns>
        /// <param name="brick">Brick.</param>
        /// <returns>Brick</returns>
        public static Brick Instansiate(Brick brick)
        {
            var toolsManager = ToolsManager.instance;
            return Instansiate(brick, toolsManager.colorButton.selectedColor, toolsManager.scaleSelector.scale);
        }

        /// <summary>
        /// Instanciate new brick by at scene editor in the middle of camera view.
        /// </summary>
        /// <returns>The instansiate.</returns>
        /// <param name="brick">Brick.</param>
        /// <param name="color">Brick color.</param>
        /// <param name="scale">Birck scale.</param>
        /// <returns>Brick</returns>
        public static Brick Instansiate(Brick brick, Color color, float scale)
        {
            //Find cross point of ray from camera thru sceen center and grid
            UnityEngine.Camera camera = UnityEngine.Camera.main;
            var cameraPosition = camera.transform.position;
            Vector3 midMousePosition = new Vector3(
                Mathf.RoundToInt(camera.pixelWidth / 2),
                Mathf.RoundToInt(camera.pixelHeight / 2),
                camera.nearClipPlane);
            var mouseWorldPosition = camera.ScreenToWorldPoint(midMousePosition);
            var viewVector = mouseWorldPosition - cameraPosition;

            var gridPositon = Camera.Grid.instance.transform.position;
            float gridY = gridPositon.y;

            float t = (gridY - cameraPosition.y) / viewVector.y;
            Vector3 hitPoint = gridPositon;
            hitPoint.x = cameraPosition.x + viewVector.x * t;
            hitPoint.z = cameraPosition.z + viewVector.z * t;

            //calcualte brick posiiton on the grid at the center of the screen
            var lowestPoint = brick.GetLowestPoint();
            Vector3 position = hitPoint;
            position.y = Camera.Grid.instance.transform.position.y - lowestPoint.y;
  
            return Instansiate(brick, color, scale, position, Quaternion.Euler(Vector3.zero));
        }

        /// <summary>
        /// Instanciate new brick by at scene editor.
        /// </summary>
        /// <param name="brick">Brick.</param>
        /// <param name="color">Color.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <returns>Brick</returns>
        public static Brick Instansiate(Brick brick, Color color, float scale, Vector3 position, Quaternion rotation)
        {
            //instatnciate brick
            var brickObject = GameObject.Instantiate(brick.gameObject) as GameObject;

            //set as child of Model node
            var model = GameObject.Find("Model");
            brickObject.transform.SetParent(model.transform);

            //set color
            var newBrick = brickObject.GetComponent<Brick>();
            if (newBrick != null)
                newBrick.color = color;

            //set scale
            brickObject.transform.localScale = new Vector3(scale, scale, scale);

            //posiiton brick
            brickObject.transform.position = position; 
            brickObject.transform.rotation = rotation;

            return brickObject.GetComponent<Brick>();
        }

        /// <summary>
        /// Build GameObject from mesh with all needed components.
        /// </summary>
        /// <returns>The brick object.</returns>
        /// <param name="mesh">Mesh.</param>
        static OrdinaryBrick PrepareBrickObject(Mesh mesh)
        {
            var newObject = new GameObject();
            newObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

            //set as child of Model node
            var model = GameObject.Find("Model");
            newObject.transform.SetParent(model.transform);

            var meshFilter = newObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            var meshRenderer = newObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = Preferences.instance.defaultBrickMaterial;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            var collider = newObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;

            var brickScript = newObject.AddComponent<OrdinaryBrick>();

            return brickScript;
        }
    }
}
