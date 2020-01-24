using UnityEngine;
using AgaQ.Bricks;
using System.Collections;
using System.Collections.Generic;

namespace AgaQ.Camera
{
    /// <summary>
    /// Class draws grid as base floor for bircks buildings
    /// </summary>
    public class Grid : MonoBehaviour
    {
        [SerializeField] Color lineColor = Color.gray;
        [SerializeField] int initialSize = 10;
        [SerializeField] float baseSize = 0.5f; //size of one grid cell
        [SerializeField] float updateTime = 0.5f ;//time interal between grid updates
        [HideInInspector] public int scale = 1;

        public static Grid instance;

        GridBounds gridBounds;

        float lastUpdateTime; //time when grid was updated
        Coroutine updateCoroutine;
        GameObject modelRoot;
        MeshRenderer meshRenderer;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            gridBounds = new GridBounds(initialSize);
            SetUpPlane();
            lastUpdateTime = Time.time;
            modelRoot = GameObject.Find("Model");
        }

        /// <summary>
        /// Update grid bounds acording to brick location and redraw.
        /// </summary>
        public void UpdateBounds()
        {
            if (updateCoroutine != null)
                StopCoroutine(updateCoroutine);

            updateCoroutine = StartCoroutine(UpdateBoundsCoroutine());
        }

        IEnumerator UpdateBoundsCoroutine()
        {
            float time = Time.time - lastUpdateTime;
            if (time < updateTime)
                yield return new WaitForSeconds(time);
            
            //get model root
            if (modelRoot != null)
            {
                GridBounds oldBounds = (GridBounds)gridBounds.Clone();

                //get all brick
                var bricks = modelRoot.GetComponentsInChildren<Brick>();
                if (bricks.Length > 0)
                {
                    //calculate all bricks bounds
                    var bounds = bricks[0].GetBounds();
                    for (int i = 1; i < bricks.Length; i++)
                    {
                        if (!(bricks[i] is AgaQTemporaryGroup) && bricks[i].gameObject.activeSelf)
                            bounds.Encapsulate(bricks[i].GetBounds());
                    }
                        
                    bounds.Expand(5f);
                    gridBounds.SetSize(bounds);

                    if (oldBounds != gridBounds)
                        SetUpPlane();

                    lastUpdateTime = Time.time;
                }
            }
        }

        /// <summary>
        /// Setup plane pozition, size and texture tiling
        /// </summary>
        void SetUpPlane()
        {
            float currCellSize = baseSize * scale;
            float minX = Mathf.Floor(gridBounds.minX / currCellSize) * currCellSize;
            float maxX = Mathf.Ceil(gridBounds.maxX / currCellSize) * currCellSize;
            float minZ = Mathf.Floor(gridBounds.minZ / currCellSize) * currCellSize;
            float maxZ = Mathf.Ceil(gridBounds.maxZ / currCellSize) * currCellSize;
            float width = maxX - minX;
            float lenght = maxZ - minZ;

            transform.position = new Vector3(minX + width / 2f, 0, minZ + lenght / 2f);
            transform.localScale = new Vector3(width / currCellSize / initialSize * 10f, 0.001f, lenght / currCellSize / initialSize * 10f);
            meshRenderer.sharedMaterial.mainTextureScale = new Vector2(width / currCellSize, lenght / currCellSize);
        }
    }
}
