using UnityEngine;
using AgaQ.UI.Inspector;
using AgaQ.Bricks.Serialization;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace AgaQ.Bricks
{
    /// <summary>
    /// Base abstact class for all bricks.
    /// </summary>
    public abstract class Brick : Uuid, IBrickSerializer
    {
        Bounds ubounds;     //unrotated brick bounds
        bool cachedUBounds; //unrotatetd bouds are cached

        Bounds bounds; //brick bouds (use current transform)
        bool cachedBounds;
        Vector3 cachedPosition;
        Vector3 cachedScale;
        Quaternion cachedRotation;
        bool flashIsRunning = false;

        void Awake()
        {
            var vertices = GetComponent<Vertices>();
            if (vertices == null)
                gameObject.AddComponent<Vertices>();   
        }

        #region Public properties

        /// <summary>
        /// Brick name.
        /// </summary>
        public string name;

        /// <summary>
        /// Color of the brick
        /// </summary>
        /// <value>The color.</value>
        [Inspector]
        [LabelKey("Color")]
        public Color color
        {
            get
            {
                var renderer = GetComponentInChildren<Renderer>();
                if (renderer == null)
                    return Color.white;

                if (renderer.material == null)
                    return renderer.sharedMaterial.color;

                return renderer.material.color;
            }

            set
            {
                var renderers = GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                {
                    if (r.material != null)
                        r.material.color = value;
                }
            }
        }

        /// <summary>
        /// Scale of the brick. It's local transform scale.
        /// </summary>
        /// <value>The scale.</value>
        [Inspector]
        [LabelKey("Scale")]
        public float scale
        {
            get
            {
                return transform.localScale.x;
            }

            set
            {
                transform.localScale = new Vector3(value, value, value);
            }
        }

        [NonSerialized] public bool grouped = false;

        #endregion

        #region Public functions

        /// <summary>
        /// Get lowest poit of mesh after transform.
        /// </summary>
        /// <returns>The lowest point.</returns>
        public Vector3 GetLowestPoint()
        {
            return GetBounds().min;
        }

        /// <summary>
        /// Get mesh bounds without applaying transpform.
        /// It's differ from mesh bounds, it's returns bounds using also all child meshes.
        /// </summary>
        /// <returns>The unrotated bounds.</returns>
        public Bounds GetUnrotatedBounds()
        {
            if (!cachedUBounds)
            {
                MeshFilter[] meshes = GetComponentsInChildren<MeshFilter>();
                ubounds = meshes[0].sharedMesh.bounds;
                for (int i = 1; i < meshes.Length; i++)
                    ubounds.Encapsulate(meshes[i].sharedMesh.bounds);
                cachedUBounds = true;
            }

            return ubounds;
        }

        /// <summary>
        /// Gets mesh bounds (using all child meshes).
        /// </summary>
        /// <returns>The bounds.</returns>
        public Bounds GetBounds()
        {
            if (!cachedBounds || cachedPosition != transform.position || cachedScale != transform.localScale || cachedRotation != transform.rotation)
            {
                bounds = GetBounds(gameObject);
                cachedBounds = true;
                cachedPosition = transform.position;
                cachedScale = transform.localScale;
                cachedRotation = transform.rotation;
            }

            return bounds;
        }

        /// <summary>
        /// Gets mesh bounds (using all child meshes).
        /// </summary>
        /// <returns>The bounds.</returns>
        public static Bounds GetBounds(GameObject gameObject)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
                return new Bounds(gameObject.transform.position, Vector3.zero);

            var bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                var boundsToAdd = renderers[i].bounds;
                bounds.Encapsulate(boundsToAdd);
            }

            return bounds;
        }

        public static Bounds GetUnrotatedBounds(GameObject gameObject)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
                return new Bounds(gameObject.transform.position, Vector3.zero);

            var bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                var oldRot = renderers[i].gameObject.transform.rotation;
                renderers[i].gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                var boundsToAdd = renderers[i].bounds;
                bounds.Encapsulate(boundsToAdd);
                renderers[i].gameObject.transform.rotation = oldRot;
            }

            return bounds;
        }

        public IList<Vector3> GetBoundCorners()
        {
            var bounds = GetBounds();
            var Vec0 = bounds.min;
            var Vec1 = bounds.max;

            Vector3[] corners = new[]
            {
                new Vector3 ( Vec0.x, Vec0.y, Vec0.z ),
                new Vector3 ( Vec1.x, Vec0.y, Vec0.z ),
                new Vector3 ( Vec0.x, Vec1.y, Vec0.z ),
                new Vector3 ( Vec0.x, Vec0.y, Vec1.z ),
                new Vector3 ( Vec1.x, Vec1.y, Vec0.z ),
                new Vector3 ( Vec1.x, Vec0.y, Vec1.z ),
                new Vector3 ( Vec0.x, Vec1.y, Vec1.z ),
                new Vector3 ( Vec1.x, Vec1.y, Vec1.z ),
            };

            return corners;
        }

        /// <summary>
        /// Set layer for brick and for some child components with colliders.
        /// </summary>
        /// <param name="layer">Layer.</param>
        public void SetLayer(int layer)
        {
            gameObject.layer = layer;
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
                collider.gameObject.layer = layer;
        }

        public void FlashBrick()
        {
            StartCoroutine(FlashCoroutine());
        }

        #endregion

        #region Serialization

        public virtual void Serialize(XmlDocument doc, XmlElement parentElement) { }
        public virtual void Deserialize(XmlReader reader) { }

        /// <summary>
        /// Try to get vector form string.
        /// </summary>
        /// <returns>The vector3 or null when can't read coordinates from string.</returns>
        /// <param name="rString">R string.</param>
        protected Vector3 getVector3(string rString)
        {
            if (rString == null)
                return Vector3.zero;

            string[] temp = rString.Split(';');
            if (temp.Length < 3)
                return Vector3.zero;

            temp[0] = temp[0].Replace(',', '.');
            temp[1] = temp[1].Replace(',', '.');
            temp[2] = temp[2].Replace(',', '.');

            float x = float.Parse(temp[0], NumberStyles.Float, CultureInfo.InvariantCulture);
            float y = float.Parse(temp[1], NumberStyles.Float, CultureInfo.InvariantCulture);
            float z = float.Parse(temp[2], NumberStyles.Float, CultureInfo.InvariantCulture);
            var rValue = new Vector3(x, y, z);

            return rValue;
        }

        /// <summary>
        /// Try to get quaterinon from string
        /// </summary>
        /// <returns>The quaternion or null.</returns>
        /// <param name="rString">R string.</param>
        protected Quaternion getQuaternion(string rString)
        {
            if (rString == null)
                return Quaternion.Euler(Vector3.zero);

            string[] temp = rString.Split(';');
            if (temp.Length < 4)
                return Quaternion.Euler(Vector3.zero);

            temp[0] = temp[0].Replace(',', '.');
            temp[1] = temp[1].Replace(',', '.');
            temp[2] = temp[2].Replace(',', '.');
            temp[3] = temp[3].Replace(',', '.');

            float x = float.Parse(temp[0], NumberStyles.Float, CultureInfo.InvariantCulture);
            float y = float.Parse(temp[1], NumberStyles.Float, CultureInfo.InvariantCulture);
            float z = float.Parse(temp[2], NumberStyles.Float, CultureInfo.InvariantCulture);
            float w = float.Parse(temp[3], NumberStyles.Float, CultureInfo.InvariantCulture);
            var rValue = new Quaternion(x, y, z, w);

            return rValue;
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// Coroutine to makecolor flash effect.
        /// </summary>
        /// <returns>The coroutine.</returns>
        IEnumerator FlashCoroutine()
        {
            if (!flashIsRunning)
            {
                flashIsRunning = true;

                //collect materials to flash
                var renderers = GetComponentsInChildren<MeshRenderer>();

                //remember base colors and choose flash colors
                Color[] baseColors = new Color[renderers.Length];
                Color[] flashColors = new Color[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    baseColors[i] = renderers[i].material.color;
                    if (baseColors[i].r > 220 && baseColors[i].g > 220 && baseColors[i].b > 220)
                        flashColors[i] = Color.black;
                    else
                        flashColors[i] = Color.white;
                }

                //animate to flash colors
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].material.color = flashColors[i];
                yield return new WaitForSeconds(0.1f);

                //animate to base colors
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].material.color = baseColors[i];

                flashIsRunning = false;
            }
        }

        #endregion
    }
}
