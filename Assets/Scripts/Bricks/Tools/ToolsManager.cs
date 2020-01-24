using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using AgaQ.UI;
using AgaQ.Camera;
using System;

namespace AgaQ.Bricks.Tools
{
    public class ToolsManager : Tools
    {
        /// <summary>
        /// Manages tools. Can set current tool.
        /// </summary>
        public static ToolsManager instance;

        [SerializeField] ToolButton handToolButton;
        [SerializeField] ToolButton selectToolButton;
        [SerializeField] ToolButton selectColorToolButton;
        [SerializeField] ToolButton selectShapeToolButton;
        [SerializeField] ToolButton cloneToolButton;
        [SerializeField] ToolButton colorToolButton;
        [SerializeField] ToolButton measureToolButton;
        [SerializeField] ToolButton multiToolButton;

        [Space]
        public ColorButton colorButton;
        public ScaleSelector scaleSelector;
        public ToggleButton snapToGridButton;

        public ITool tool;

        public Action<ToolType> OnToolChange;

        ToolType currentTool = ToolType.None; //currently selected tool
        ToolType previousTool = ToolType.None; //previous selected tool

        CameraController cam;

        #region MonoBehaviour event handlers

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            SetToolSelect();
        }

        protected new void Start()
        {
            cam = UnityEngine.Camera.main.GetComponent<CameraController>();

            base.Start();

            SetToolSelect();
        }

        void Update()
        {
            if (tool != null)
                tool.OnUpdate();

            HandleKeys();
        }

        #endregion

        #region Public functions

        /// <summary>
        /// Set active hand tool.
        /// </summary>
        public void SetHandTool()
        {
            SetTool(ToolType.HandTool);
        }

        /// <summary>
        /// Set active tool to select/
        /// </summary>
        public void SetToolSelect()
        {
            SetTool(ToolType.Select);
        }

        /// <summary>
        /// Set active tools to select by color.
        /// </summary>
        public void SetToolSelecColor()
        {
            SetTool(ToolType.SelectColor);
        }

        /// <summary>
        /// Set active tool to select by shape.
        /// </summary>
        public void SetToolSelectShape()
        {
            SetTool(ToolType.SelectShape);
        }

        /// <summary>
        /// Set active cloning tool.
        /// </summary>
        public void SetToolClone()
        {
            SetTool(ToolType.Clone);
        }

        /// <summary>
        /// Set active color tool.
        /// </summary>
        public void SetToolColor()
        {
            SetTool(ToolType.Colour);
        }

        /// <summary>
        /// Set active measure tool.
        /// </summary>
        public void SetToolMeasure()
        {
            SetTool((ToolType.Measure));
        }

        /// <summary>
        /// Set active multitool.
        /// </summary>
        public void SetMultiTool()
        {
            SetTool(ToolType.MultiTool);
        }

        /// <summary>
        /// Set active tool.
        /// </summary>
        /// <param name="toolType">Tool type.</param>
        public void SetTool(ToolType toolType)
        {
            //if (toolType == currentTool) //nothing to do?
                //return;

            if (currentTool != ToolType.Add && toolType != currentTool) //skip tools whithout buttons
                previousTool = currentTool;
            currentTool = toolType;

            //do cancel on curent tool
            if (tool != null)
                tool.OnCancel();

            switch (toolType)
            {
                case ToolType.HandTool:
                    tool = handTool;
                    handToolButton.SetSelected();
                    break;

                case ToolType.Select:
                    tool = selectSingleTool;
                    selectToolButton.SetSelected();
                    break;

                case ToolType.SelectColor:
                    tool = selectColorTool;
                    selectColorToolButton.SetSelected();
                    break;

                case ToolType.SelectShape:
                    tool = selectShapeTool;
                    selectShapeToolButton.SetSelected();
                    break;

                case ToolType.Rotate:
                    tool = rotateTool;
                    break;

                case ToolType.Clone:
                    tool = cloneTool;
                    cloneToolButton.SetSelected();
                    break;

                case ToolType.Colour:
                    tool = colorTool;
                    colorToolButton.SetSelected();
                    break;

                case ToolType.Add:
                    tool = addTool;
                    ToolButton.UnselectAll();
                    break;

                case ToolType.Measure:
                    tool = measureTool;
                    measureToolButton.SetSelected();
                    break;
                case ToolType.MultiTool:
                    tool = multiTool;
                    multiToolButton.SetSelected();
                    break;
            }

            OnToolChange?.Invoke(toolType);
        }

        /// <summary>
        /// Set previously selected tool.
        /// This one step back undo system.
        /// </summary>
        public void SetPreviousTool()
        {
            SetTool(previousTool);
        }

        /// <summary>
        /// Cancel current operation in current tool
        /// </summary>
        /// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
        public void Cancel()
        {
            tool.OnCancel();
        }
        #endregion

        #region Private functions

        /// <summary>
        /// handle all keyboard shortcuts for changing tools
        /// </summary>
        void HandleKeys()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown((int)MouseButton.MiddleMouse) && tool != null)
                tool.OnCancel();

            if (Input.GetKeyDown(KeyCode.H) && !InputFieldSelected())
                SetHandTool();
            if (Input.GetKeyDown(KeyCode.S) && !InputFieldSelected())
                SetToolSelect();
            if (Input.GetKeyDown(KeyCode.D) && !InputFieldSelected())
                SetToolSelecColor();
            if (Input.GetKeyDown(KeyCode.F) && !InputFieldSelected())
                SetToolSelectShape();
            if (Input.GetKeyDown(KeyCode.C) && !InputFieldSelected())
                SetToolClone();
            if (Input.GetKeyDown(KeyCode.P) && !InputFieldSelected())
                SetToolColor();
        }

        bool InputFieldSelected()
        {
            var currSelObject = EventSystem.current.currentSelectedGameObject;
            if (currSelObject != null)
            {
                var input = currSelObject.GetComponent<InputField>();
                if (input != null)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
