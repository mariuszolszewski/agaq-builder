using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AgaQ.Bricks.History;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Tool to mesure distance between point of the meshes
    /// </summary>
    public class MeasureTool : BaseTool
    {
        const float pointSelectAccuracy = 5f; //maximum distance form point to cursor when can recognize point hover

        Vertices currentMeshVerises;
        Transform meshTransform;
        Vector2 currentScreenPoint; //point that is under cursor
        Vector3 currentWorldPoint;
        bool cursorOverPoint; //true if cursor is over point (point coordiantes are stored in currentPoint)

        Button joinButton;
        Vector3[] points = new Vector3[2];
        GameObject[] objects = new GameObject[2];
        int amountOfPoints = 0;

        UnityEngine.Camera cam;

        HistoryNodeTransform[] historyNodes;

        public override void Start()
        {
            cam = UnityEngine.Camera.main;


            var buttons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (var button in buttons)
            {
                if (button.gameObject.name == "JoinButton")
                {
                    joinButton = button;
                    joinButton.onClick.AddListener(Join);
                    break;
                }
            }                   
        }

        public override void OnEneter(HighlightableBrick brick)
        {
            var meshFilter = brick.GetComponentInChildren<MeshFilter>();

            currentMeshVerises = meshFilter.gameObject.GetComponent<Vertices>();
            if (currentMeshVerises == null)
                currentMeshVerises = meshFilter.gameObject.AddComponent<Vertices>();

            meshTransform = meshFilter.gameObject.transform;
        }

        public override void OnExit(HighlightableBrick brick)
        {
            currentMeshVerises = null;
            meshTransform = null;
        }

        public override void OnClick(SelectableBrick brick, PointerEventData pointerEventData)
        {
            if (cursorOverPoint)
                MeasureToolVisualizer.instance.SelectVertex(currentWorldPoint);

            amountOfPoints++;
            if (amountOfPoints == 3)
            {
                amountOfPoints = 2;
                points[0] = points[1];
                objects[0] = objects[1];
                points[1] = currentWorldPoint;
                objects[1] = brick.gameObject;
            }
            else
            {
                points[amountOfPoints - 1] = currentWorldPoint;
                objects[amountOfPoints - 1] = brick.gameObject;
            }

            if (joinButton != null)
                joinButton.gameObject.SetActive(amountOfPoints == 2 && objects[0] != objects[1]);
        }

        public override void OnUpdate()
        {
            HighlightVertexUnderCursor();
        }

        public override void OnCancel()
        {
            currentMeshVerises = null;
            meshTransform = null;
            MeasureToolVisualizer.instance.ResetHighlight();
            MeasureToolVisualizer.instance.ClearVertexes();
            joinButton.gameObject.SetActive(false);
            amountOfPoints = 0;
        }

        #region Private functions

        /// <summary>
        /// Find point under cursor and set variables currentPoint and cursorOverPoint.
        /// </summary>
        void HighlightVertexUnderCursor()
        {
            if (meshTransform != null && currentMeshVerises != null &&
                MeshPointsTool.GetVisiblePointUnderCursor(
                    currentMeshVerises.uniqueVertices,
                    meshTransform,
                    UnityEngine.Camera.main,
                    Input.mousePosition,
                    pointSelectAccuracy,
                    out currentScreenPoint,
                    out currentWorldPoint))
            {
                cursorOverPoint = true;
            }
            else
                cursorOverPoint = false;

            if (cursorOverPoint)
                MeasureToolVisualizer.instance.HighlighVertex(currentScreenPoint);
            else
                MeasureToolVisualizer.instance.ResetHighlight();
        }


        void Join()
        {
            if (amountOfPoints != 2)
                return;

            //prepare history
            historyNodes = HistoryTool.PrepareTransformNodes(objects[0]);

            var offset = points[0] - objects[0].transform.position;
            objects[0].transform.position = points[1] - offset;

            amountOfPoints = 0;
            MeasureToolVisualizer.instance.ClearVertexes();

            //register hostory
            HistoryManager.instance.Register(historyNodes);
        }

        #endregion
    }
}
