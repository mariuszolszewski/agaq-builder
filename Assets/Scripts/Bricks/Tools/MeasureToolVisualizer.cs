using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MeasureToolVisualizer : MonoBehaviour
{
    [SerializeField] RectTransform highliteImage;
    [SerializeField] RectTransform selectImage;
    [SerializeField] RectTransform select2Image;
    [SerializeField] GameObject measure;
    [SerializeField] RectTransform leftArrow;
    [SerializeField] RectTransform rightArrow;
    [SerializeField] UILineRenderer line;
    [SerializeField] RectTransform textTransform;
    [SerializeField] Text text;

    public static MeasureToolVisualizer instance;

    Camera cam;

    Vector3 highlightedVertex;
    Vector3 firstPoint;
    Vector3 secondPoint;
    int amountOfPoints = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        UpdateHighlight();
        UpdateSelect();
        UpdateMeasure();
    }

    #region Public functions

    /// <summary>
    /// Set vertex to highlight. There can be only one highlighted, previouse becomes off.
    /// </summary>
    /// <param name="vertex">Vertex.</param>
    public void HighlighVertex(Vector3 vertex)
    {
        highlightedVertex = vertex;
        highliteImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Reset highliting of vertex if there is any.
    /// </summary>
    public void ResetHighlight()
    {
        highliteImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Set vertex to be selected. There could be only two selected.
    /// </summary>
    /// <param name="vertex">Vertex.</param>
    public void SelectVertex(Vector3 vertex)
    {
        if (amountOfPoints == 0)
        {
            amountOfPoints = 1;
            firstPoint = vertex;
            selectImage.gameObject.SetActive(true);
        }
        else if (amountOfPoints == 1)
        {
            amountOfPoints = 2;
            secondPoint = vertex;
            select2Image.gameObject.SetActive(true);
        }
        else if (amountOfPoints == 2)
        {
            firstPoint = secondPoint;
            secondPoint = vertex;
        }
    }

    /// <summary>
    /// Clear selected points.
    /// </summary>
    public void ClearVertexes()
    {
        amountOfPoints = 0;
        selectImage.gameObject.SetActive(false);
        select2Image.gameObject.SetActive(false);
    }

    #endregion

    #region Private functions

    void UpdateHighlight()
    {
        if (highliteImage.gameObject.activeSelf)
            highliteImage.anchoredPosition = new Vector2(highlightedVertex.x, highlightedVertex.y);
    }

    void UpdateSelect()
    {
        if (amountOfPoints > 0)
        {
            var pos = cam.WorldToScreenPoint(firstPoint);
            selectImage.anchoredPosition = new Vector2(pos.x, pos.y);
        }
        if (amountOfPoints == 2)
        {
            var pos = cam.WorldToScreenPoint(secondPoint);
            select2Image.anchoredPosition = new Vector2(pos.x, pos.y);

            measure.SetActive(true);
            text.text = string.Format("{0:0.#}mm", (firstPoint - secondPoint).magnitude * 20);
        }
    }

    void UpdateMeasure()
    {
        if (amountOfPoints == 2)
        {
            //position left arrow
            var pos = cam.WorldToScreenPoint(firstPoint);
            leftArrow.anchoredPosition = new Vector2(pos.x, pos.y);
            //position right arrow
            pos = cam.WorldToScreenPoint(secondPoint);
            rightArrow.anchoredPosition = new Vector2(pos.x, pos.y);
            //angle left arrow
            var arrowVector = rightArrow.anchoredPosition - leftArrow.anchoredPosition;
            var angle = Vector3.SignedAngle(Vector3.right, arrowVector, Vector3.forward);
            leftArrow.rotation = Quaternion.Euler(0, 0, angle);
            //angle right arrow
            rightArrow.rotation = Quaternion.Euler(0, 0, angle + 180f);
            //position line
            line.Points[0] = leftArrow.anchoredPosition;
            line.Points[1] = rightArrow.anchoredPosition;
            line.SetAllDirty();
            //position text
            textTransform.anchoredPosition = new Vector2(
                (leftArrow.anchoredPosition.x + rightArrow.anchoredPosition.x) / 2f,
                (leftArrow.anchoredPosition.y + rightArrow.anchoredPosition.y) / 2f);
            //angle text
            if (angle > 90 || angle < -90)
                angle += 180;
            textTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
            measure.SetActive(false);
    }

    #endregion
}
