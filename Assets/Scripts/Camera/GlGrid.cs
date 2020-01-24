using UnityEngine;
using AgaQ.Bricks;
using System.Collections;
using System.Collections.Generic;
using RTEditor;

namespace AgaQ.Camera
{
    /// <summary>
    /// Class draws grid as base floor for bircks buildings
    /// </summary>
    public class GlGrid : MonoBehaviour
    {
        [SerializeField] Color lineColor = Color.gray;
        [SerializeField] int initialSize = 10;
        [SerializeField] float baseSize = 0.5f; //size of one grid cell
        [SerializeField] float updateTime = 0.5f;//time interal between grid updates
        [HideInInspector] public int scale = 1;
        [SerializeField] Material material;

        public static GlGrid instance;

        int linesId;
        GridBounds gridBounds;

        float lastUpdateTime; //time when grid as updated
        Coroutine updateCoroutine;
        GameObject modelRoot;
        UnityEngine.Camera cam;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        void Start()
        {
            gridBounds = new GridBounds(initialSize);
            lastUpdateTime = Time.time;
            modelRoot = GameObject.Find("Model");
            cam = UnityEngine.Camera.main;

            float zoom = Mathf.Abs(cam.transform.position.y - transform.position.y);
            float camFarPlaneScale = 0.2f * 1000.0f / cam.farClipPlane;

            material.SetFloat("_CamFarPlane", cam.farClipPlane * camFarPlaneScale);
            material.SetVector("_CamLook", cam.transform.forward);
            material.SetFloat("_FadeScale", zoom / 10.0f);
            material.SetPass(0);
        }

        void OnRenderObject()
        {
            float camFarPlaneScale = 0.2f * 1000.0f / cam.farClipPlane;
            CameraViewVolume viewVolume = cam.GetViewVolume(cam.farClipPlane * camFarPlaneScale);
            Bounds volumeAABB = viewVolume.AABB;

            float minX = volumeAABB.min.x;
            float minZ = volumeAABB.min.z;
            float maxX = volumeAABB.max.x;
            float maxZ = volumeAABB.max.z;

            float halfCellSizeX = 0.5f * baseSize;
            float halfCellSizeZ = 0.5f * baseSize;
            int minCellX = Mathf.FloorToInt((minX + halfCellSizeX) / baseSize) - 1;
            int maxCellX = Mathf.FloorToInt((maxX + halfCellSizeX) / baseSize) + 1;
            int minCellZ = Mathf.FloorToInt((minZ + halfCellSizeZ) / baseSize) - 1;
            int maxCellZ = Mathf.FloorToInt((maxZ + halfCellSizeZ) / baseSize) + 1;

            int minCellIndex = minCellX < minCellZ ? minCellX : minCellZ;
            int maxCellIndex = maxCellX > maxCellZ ? maxCellX : maxCellZ;

            GL.Begin(GL.LINES);
            float startZ = minCellIndex * baseSize;
            float endZ = (maxCellIndex + 1) * baseSize;
            float startX = minCellIndex * baseSize;
            float endX = (maxCellIndex + 1) * baseSize;
            Vector3 yOffset = Vector3.up * transform.position.y;
            for (int cell = minCellIndex; cell <= maxCellIndex; ++cell)
            {
                Vector3 xOffset = cell * Vector3.right * baseSize + yOffset;
                GL.Vertex(xOffset + Vector3.forward * startZ);
                GL.Vertex(xOffset + Vector3.forward * endZ);

                Vector3 zOffset = cell * Vector3.forward * baseSize + yOffset;
                GL.Vertex(zOffset + Vector3.right * startX);
                GL.Vertex(zOffset + Vector3.right * endX);
            }

            GL.End();
        }



        /// <summary>
        /// Rebuilt lines vertexes to draw current grid layout
        /// </summary>
        void RebuiltVertexes()
        {
            List<Line> lines = new List<Line>();

            //prepare lines array
            float currCellSize = baseSize * scale;
            float minX = Mathf.Floor(gridBounds.minX / currCellSize) * currCellSize;
            float maxX = Mathf.Ceil(gridBounds.maxX / currCellSize) * currCellSize;
            float minZ = Mathf.Floor(gridBounds.minZ / currCellSize) * currCellSize;
            float maxZ = Mathf.Ceil(gridBounds.maxZ / currCellSize) * currCellSize;
            for (float x = minX; x <= maxX; x += currCellSize)
                lines.Add(new Line
                {
                    vertex1 = new Vector3(x, transform.position.y, minZ),
                    vertex2 = new Vector3(x, transform.position.y, maxZ),
                    color = lineColor
                });
            for (float z = minZ; z <= maxZ; z += currCellSize)
                lines.Add(new Line
                {
                    vertex1 = new Vector3(minX, transform.position.y, z),
                    vertex2 = new Vector3(maxX, transform.position.y, z),
                    color = lineColor
                });

            //pass lines to draw
            DrawLines drawLines = UnityEngine.Camera.main.GetComponent<DrawLines>();
            drawLines.RemoveLines(linesId);
            linesId = drawLines.AddLines(lines.ToArray());
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

                    gridBounds.SetSize(bounds);

                    //if (oldBounds != gridBounds)
                        //RebuiltVertexes();

                    lastUpdateTime = Time.time;
                }
            }
        }
    }



    //public class GlGrid : MonoBehaviour
    //{
        //#region Private Variables

        //[SerializeField]
        //private float _cellSizeX = 1.0f;
        //[SerializeField]
        //private float _cellSizeZ = 1.0f;
        //[SerializeField]
        //private Color _lineColor = new Color(0.5f, 0.5f, 0.5f, 102.0f / 255.0f);
        //[SerializeField]
        //private float _yOffsetScrollSensitivity = 1.0f;
        //[SerializeField]
        //private float _yStep = 1.0f;
        //[SerializeField]
        //private float _yPos = 0.0f;

        //UnityEngine.Camera cam;

        //#endregion

        //#region Public Static Properties
        //public static float MinCellSize { get { return 0.1f; } }
        ////public static float MinLineFadeZoomFactor { get { return 1e-4f; } }
        ////public static int MinColorFadeCellCount { get { return 2; } }
        //#endregion

        //#region Public Properties
        //public float CellSizeX { get { return _cellSizeX; } set { _cellSizeX = Mathf.Max(value, MinCellSize); } }
        //public float CellSizeZ { get { return _cellSizeZ; } set { _cellSizeZ = Mathf.Max(value, MinCellSize); } }
        //public Color LineColor { get { return _lineColor; } set { _lineColor = value; } }
        //public float YStep { get { return _yStep; } set { _yStep = Mathf.Max(value, 1e-3f); } }
        //public float YOffsetScrollSensitivity { get { return _yOffsetScrollSensitivity; } set { _yOffsetScrollSensitivity = Mathf.Max(value, 1e-3f); } }
        //public float YPos { get { return _yPos; } set { _yPos = value; } }
        //public Plane Plane { get { return new Plane(Vector3.up, new Vector3(0.0f, _yPos, 0.0f)); } }
        //#endregion

        //#region Public Methods

        //void Awake()
        //{
        //    cam = UnityEngine.Camera.main;   
        //}

        //void Update()
        //{
        //    Render();
        //}

        //public void ScrollUpDown(float scrollValue)
        //{
        //    float offset = scrollValue * YOffsetScrollSensitivity;
        //    YPos += offset;
        //}

        //public void ScrollUpDownStep(float sign)
        //{
        //    YPos = (int)(YPos / YStep) * YStep + YStep * Mathf.Sign(sign);
        //}

        //public void Render()
        //{
        //    float zoom = Mathf.Abs(cam.transform.position.y - _yPos);
        //    int p0 = MathHelper.GetNumberOfDigits((int)zoom) - 1;
        //    int p1 = p0 + 1;

        //    float s0 = Mathf.Pow(10.0f, p0);
        //    float s1 = Mathf.Pow(10.0f, p1);

        //    float camFarPlaneScale = 0.2f * 1000.0f / cam.farClipPlane;

        //    Material material = MaterialPool.Instance.XZGrid;
        //    material.SetFloat("_CamFarPlane", cam.farClipPlane * camFarPlaneScale);
        //    material.SetVector("_CamLook", cam.transform.forward);
        //    material.SetFloat("_FadeScale", zoom / 10.0f);

        //    float alphaScale = Mathf.Clamp(1.0f - ((zoom - s0) / (s1 - s0)), 0.0f, 1.0f);
        //    GLPrimitives.DrawGridLines(_cellSizeX * s0, _cellSizeZ * s0, _yPos, cam, material, new Color(_lineColor.r, _lineColor.g, _lineColor.b, _lineColor.a * alphaScale), camFarPlaneScale);
        //    GLPrimitives.DrawGridLines(_cellSizeX * s1, _cellSizeZ * s1, _yPos, cam, material, new Color(_lineColor.r, _lineColor.g, _lineColor.b, _lineColor.a - _lineColor.a * alphaScale), camFarPlaneScale);
        //}

        //public XZGridCell GetCellFromWorldPoint(Vector3 worldPoint)
        //{
        //    Vector3 projectedPoint = Plane.ProjectPoint(worldPoint);
        //    return GetCellFromWorldXZ(projectedPoint.x, projectedPoint.z);
        //}

        //public XZGridCell GetCellFromWorldXZ(float worldX, float worldZ)
        //{
        //    int cellIndexX = Mathf.FloorToInt((worldX + 0.5f * _cellSizeX) / _cellSizeX);
        //    int cellIndexZ = Mathf.FloorToInt((worldZ + 0.5f * _cellSizeZ) / _cellSizeZ);

        //    return new XZGridCell(cellIndexX, cellIndexZ, this);
        //}

        //public List<Vector3> GetCellCornerPoints(XZGridCell gridCell)
        //{
        //    float startX = gridCell.CellIndexX * _cellSizeX;
        //    float startZ = gridCell.CellIndexZ * _cellSizeX;

        //    var cellCornerPoints = new List<Vector3>();
        //    cellCornerPoints.Add(new Vector3(startX, _yPos, startZ));
        //    cellCornerPoints.Add(new Vector3(startX + _cellSizeX, _yPos, startZ));
        //    cellCornerPoints.Add(new Vector3(startX + _cellSizeX, _yPos, startZ + _cellSizeZ));
        //    cellCornerPoints.Add(new Vector3(startX, _yPos, startZ + _cellSizeZ));

        //    return cellCornerPoints;
        //}

        //public Vector3 GetCellCornerPointClosestToInputDevPos()
        //{
        //    Ray ray;
        //    if (!InputDevice.Instance.GetPickRay(EditorCamera.Instance.Camera, out ray)) return Vector3.zero;

        //    float t;
        //    if (Plane.Raycast(ray, out t))
        //    {
        //        Vector3 pickPoint = ray.GetPoint(t);
        //        List<Vector3> cellCornerPoints = GetCellCornerPoints(GetCellFromWorldXZ(pickPoint.x, pickPoint.z));
        //        return Vector3Extensions.GetPointClosestToPoint(cellCornerPoints, pickPoint);
        //    }

        //    return Vector3.zero;
        //}

        //#endregion
    //}
}
