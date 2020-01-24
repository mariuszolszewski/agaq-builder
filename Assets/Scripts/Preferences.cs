using UnityEngine;
using AgaQ.UI;
using System;

namespace AgaQ
{
    public class Preferences : MonoBehaviour
    {
        #region Public properties

        //move tool
        public float moveToolRotationStep = 10f; //angle in degrees
        public int movingLayer = 9; //change moving bricks to that layer to optimise physics

        [Space]
        [Header("Sounds")]
        public AudioClip errorSound;
        public AudioClip joinBrickSound;

        //camera
        [Space]
        [Header("Camera")]
        public float buttonRotationSpeed = 5f;
        public float zoomSpeed = 2f;
        public Color defaultEditorBackgroundColor;

        //brick
        [Space]
        [Header("Brick")]
        public Material transparentBrickMaterial;
        public Material normalBrickMaterial;
        public float transparentAlfa = 0.5f;
        public Color defaultBrickHighlightColor;
        public Color defaultSelectionColor;
        public Material defaultBrickMaterial;

        //cursors
        [Space]
        [Header("Cursors")]
        public CursorDefinition defaultCursor;
        public CursorDefinition activeEdgeCursor;
        public CursorDefinition handToolCursor;
        public CursorDefinition cloneToolCursor;
        public CursorDefinition paintToolCursor;

        //paths
        [Space]
        [Header("Paths")]
        public string agaQGroupsSavePath;
        public string addsSavePath;

        public static Preferences instance;

        #endregion

        #region Saveable properties

        [NonSerialized] public Color editorBackgroundColor;
        [NonSerialized] public Color brickHighlightColor;
        [NonSerialized] public Color brickSelectionColor;

        #endregion

        #region Event handlers

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            editorBackgroundColor = defaultEditorBackgroundColor;
            brickHighlightColor = defaultBrickHighlightColor;
            brickSelectionColor = defaultSelectionColor;

            Load();
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Load prefereces that can be adjusted by user.
        /// </summary>
        public void Load()
        {
            editorBackgroundColor = ParseColor(
                PlayerPrefs.GetString("editorBackgroundColor"),
                defaultEditorBackgroundColor);
            brickHighlightColor = ParseColor(
                PlayerPrefs.GetString("brickHighlightColor"),
                defaultBrickHighlightColor);
            brickSelectionColor = ParseColor(
                PlayerPrefs.GetString("birckSelectionColor"),
                defaultSelectionColor
            );

            UnityEngine.Camera.main.backgroundColor = editorBackgroundColor;
        }

        /// <summary>
        /// Save prefereces that can be adjusted by user.
        /// </summary>
        public void Save()
        {
            PlayerPrefs.SetString(
                "editorBackgroundColor",
                string.Concat("#", ColorUtility.ToHtmlStringRGBA(editorBackgroundColor)));
            PlayerPrefs.SetString(
                "brickHighlightColor",
                string.Concat("#", ColorUtility.ToHtmlStringRGBA(brickHighlightColor)));
            PlayerPrefs.SetString(
                "birckSelectionColor",
                string.Concat("#", ColorUtility.ToHtmlStringRGBA(brickSelectionColor)));

            PlayerPrefs.Save();
        }

        #endregion

        #region Private functions

        static Color ParseColor(string colorString, Color defaultColor)
        {
            Color c = new Color();
            if (ColorUtility.TryParseHtmlString(colorString, out c))
                return c;

            return defaultColor;
        }

        #endregion
    }
}
