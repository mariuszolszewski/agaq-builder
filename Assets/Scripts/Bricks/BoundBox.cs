using UnityEngine;
using AgaQ.Camera;

namespace AgaQ.Bricks
{
	/// <summary>
	/// Draw rectangle mark around mesh.
	/// </summary>
    public class BoundBox : MonoBehaviour
    {
        Line[] lines;
        Vector3[] corners;
        DrawLines cameralines;
        Brick brick;
        int linesId;

        bool firstRun;
        Vector3 lastPosition;
        Quaternion lastRotation;
        Vector3 lastScale;
        Color lastColor;

        #region Monobehaviour events handlers

        void Awake()
        {
            cameralines = UnityEngine.Camera.main.GetComponent<DrawLines>();

            if (!cameralines)
            {
                Debug.LogError("No camera with DrawLines in the scene", gameObject);
                return;
            }

            brick = GetComponent<Brick>();

            RememberLastBox();
            firstRun = true;
		}

        void OnEnable()
        {
            if (firstRun || lastPosition != transform.position ||
                lastRotation != transform.rotation ||
                lastScale != transform.localScale ||
                lastColor != Preferences.instance.brickHighlightColor)
            {
                firstRun = false;
                SetupLines();
                RememberLastBox();
            }
            else
                linesId = cameralines.AddLines(lines);
        }

        void OnDisable()
        {
            if (cameralines != null)
                cameralines.RemoveLines(linesId);
        }

        #endregion

        /// <summary>
        /// Remember set of parameters that determines box size, position and color.
        /// </summary>
        void RememberLastBox()
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            lastScale = transform.localScale;
            lastColor = Preferences.instance.brickHighlightColor;
        }

		/// <summary>
		/// Prepare bound box lines.
		/// </summary>
        void SetupLines()
        {
            if (brick == null)
                return;

            lastColor = Preferences.instance.brickHighlightColor;

            var bounds = brick.GetBounds();
                
            //set corners
            if (corners == null)
            {
                corners = new Vector3[8];
                for (var i = 0; i < corners.Length; i++)
                    corners[i] = new Vector3();
            }

            float o = 0.01f; //oversize box by this value
            corners[0].x = bounds.max.x + o; corners[0].y = bounds.max.y + o; corners[0].z = bounds.min.z - o; // top front right
            corners[1].x = bounds.min.x - o; corners[1].y = bounds.max.y + o; corners[1].z = bounds.min.z - o; // top front left
            corners[2].x = bounds.min.x - o; corners[2].y = bounds.max.y + o; corners[2].z = bounds.max.z + o; // top back left
            corners[3].x = bounds.max.x + o; corners[3].y = bounds.max.y + o; corners[3].z = bounds.max.z + o; // top back right
            corners[4].x = bounds.max.x + o; corners[4].y = bounds.min.y - o; corners[4].z = bounds.min.z - o; // bottom front right
            corners[5].x = bounds.min.x - o; corners[5].y = bounds.min.y - o; corners[5].z = bounds.min.z - o; // bottom front left
            corners[6].x = bounds.min.x - o; corners[6].y = bounds.min.y - o; corners[6].z = bounds.max.z + o; // bottom back left
            corners[7].x = bounds.max.x + o; corners[7].y = bounds.min.y - o; corners[7].z = bounds.max.z + o; // bottom back right

            Quaternion rotation = transform.rotation;
            Vector3 position = transform.position;

            //setup lines
            Color c = Preferences.instance.brickHighlightColor;
            if (lines == null)
            {
                lines = new Line[12];
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = new Line();
                    lines[i].color = c;
                }
            }
            else
            {
                //change lines color
                for (int i = 0; i < lines.Length; i++)
                    lines[i].color = c;
            }

            lines[0].vertex1 = corners[0]; lines[0].vertex2 = corners[1];
            lines[1].vertex1 = corners[1]; lines[1].vertex2 = corners[2];
            lines[2].vertex1 = corners[2]; lines[2].vertex2 = corners[3];
            lines[3].vertex1 = corners[3]; lines[3].vertex2 = corners[0];
            lines[4].vertex1 = corners[0]; lines[4].vertex2 = corners[4];
            lines[5].vertex1 = corners[1]; lines[5].vertex2 = corners[5];
            lines[6].vertex1 = corners[2]; lines[6].vertex2 = corners[6];
            lines[7].vertex1 = corners[3]; lines[7].vertex2 = corners[7];
            lines[8].vertex1 = corners[4]; lines[8].vertex2 = corners[5];
            lines[9].vertex1 = corners[5]; lines[9].vertex2 = corners[6];
            lines[10].vertex1 = corners[6]; lines[10].vertex2 = corners[7];
            lines[11].vertex1 = corners[7]; lines[11].vertex2 = corners[4];

            //pass lines to drawing script
            cameralines.RemoveLines(linesId);
            linesId = cameralines.AddLines(lines);
        }
    }
}
