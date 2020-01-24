using UnityEngine;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Provides access to all brick tools.
    /// </summary>
    public abstract class Tools  : MonoBehaviour
    {
        HandTool _handTool;
        SelectSingleTool _selectSingleTool;
        SelectWithRectangleTool _selectWithRectangleTool;
        SelectShapeTool _selectShapeTool;
        SelectColorTool _selectColorTool;
        RotateTool _rotateTool;
        CloneTool _cloneTool;
        ColorTool _colorTool;
        AddTool _addTool;
        GroupTool _groupTool;
        DimensionTool _dimensionTool;
        MeasureTool _measureTool;
        MultiTool _multiTool;

        Camera.Grid grid;

        protected void Start()
        {
            var gridGO = GameObject.Find("Grid");
            if (gridGO != null)
                grid = gridGO.GetComponent<Camera.Grid>();
        }

        public HandTool handTool
        {
            get
            {
                if (_handTool == null)
                {
                    _handTool = new HandTool();
                    _handTool.OnModelChange += OnModelChange;
                    _handTool.Start();
                }

                return _handTool;
            }
        }

        public SelectSingleTool selectSingleTool
        {
            get
            {
                if (_selectSingleTool == null)
                {
                    _selectSingleTool = new SelectSingleTool();
                    _selectSingleTool.OnModelChange += OnModelChange;
                    _selectSingleTool.Start();
                }

                return _selectSingleTool;
            }
        }

        public SelectWithRectangleTool selectWithRectangleTool
        {
            get
            {
                if (_selectWithRectangleTool == null)
                {
                    _selectWithRectangleTool = new SelectWithRectangleTool();
                    _selectWithRectangleTool.OnModelChange += OnModelChange;
                    _selectWithRectangleTool.Start();
                }

                return _selectWithRectangleTool;
            }
        }

        public SelectShapeTool selectShapeTool
        {
            get
            {
                if (_selectShapeTool == null)
                {
                    _selectShapeTool = new SelectShapeTool();
                    _selectShapeTool.OnModelChange += OnModelChange;
                    _selectShapeTool.Start();
                }

                return _selectShapeTool;
            }
        }

        public SelectColorTool selectColorTool
        {
            get
            {
                if (_selectColorTool == null)
                {
                    _selectColorTool = new SelectColorTool();
                    _selectColorTool.OnModelChange += OnModelChange;
                    _selectColorTool.Start();
                }

                return _selectColorTool;
            }
        }

        public RotateTool rotateTool
        {
            get
            {
                if (_rotateTool == null)
                {
                    _rotateTool = new RotateTool();
                    _rotateTool.OnModelChange += OnModelChange;
                    _rotateTool.Start();
                }

                return _rotateTool;
            }
        }

        public CloneTool cloneTool
        {
            get
            {
                if (_cloneTool == null)
                {
                    _cloneTool = new CloneTool();
                    _cloneTool.OnModelChange += OnModelChange;
                    _cloneTool.Start();
                }

                return _cloneTool;
            }
        }

        public ColorTool colorTool
        {
            get
            {
                if (_colorTool == null)
                {
                    _colorTool = new ColorTool();
                    _colorTool.OnModelChange += OnModelChange;
                    _colorTool.Start();
                }

                return _colorTool;
            }
        }

        public AddTool addTool
        {
            get
            {
                if (_addTool == null)
                {
                    _addTool = new AddTool();
                    _addTool.OnModelChange += OnModelChange;
                    _addTool.Start();
                }

                return _addTool;
            }
        }

        public GroupTool groupTool
        {
            get
            {
                if (_groupTool == null)
                    _groupTool = new GroupTool();

                return _groupTool;
            }    
        }

        public DimensionTool dimensionTool
        {
            get
            {
                if (_dimensionTool == null)
                    _dimensionTool = new DimensionTool();

                return _dimensionTool;
            }
        }

        public MeasureTool measureTool
        {
            get
            {
                if (_measureTool == null)
                {
                    _measureTool = new MeasureTool();
                    _measureTool.Start();
                }

                return _measureTool;
            }    
        }

        public MultiTool multiTool
        {
            get
            {
                if (_multiTool == null)
                {
                    _multiTool = new MultiTool();
                    _multiTool.Start();
                }

                return _multiTool;
            }
        }

        //Do some actions when tool is modyfing model
        public virtual void OnModelChange()
        {
            //update grid bounds
            if (grid != null)
                grid.UpdateBounds();
        }
    }
}
