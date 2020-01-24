using UnityEngine;
using System.Collections.Generic;
using System;
using AgaQ.Bricks.Tools;

namespace AgaQ.Bricks
{
	/// <summary>
	/// Holds information about what bricks are selected.
	/// </summary>
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager instance;

        List<SelectableBrick> selectedBricks = new List<SelectableBrick>();

        /// <summary>
        /// Get seelcted brics amount.
        /// </summary>
        /// <value>The selected amount.</value>
        public int SelectedAmount
        {
            get {
                return selectedBricks.Count;
            }
        }

        public Action<List<SelectableBrick>> OnSelectionChange;

        #region Monobehaviour events handlers

        void Awake()
        {
            Init();
        }

        void Update()
        {
            //selection shortkeys handling
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetKey(KeyCode.A))
                SelectAll();

            //select all
            if (Input.GetKeyDown(KeyCode.A) && 
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                 Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) &&
                (ToolsManager.instance.tool is MoveTool && !(ToolsManager.instance.tool as MoveTool).isDragging))
            {
                SelectAll();
            }

            //deselect all
            if (Input.GetKeyDown(KeyCode.D) && 
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                    Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) &&
                (ToolsManager.instance.tool is MoveTool && !(ToolsManager.instance.tool as MoveTool).isDragging))
            {
                Clear();
            }

        }

        #endregion

        #region Public functions

        public void Init()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);            
        }

        /// <summary>
        /// Add brick to selection.
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="brick">Brick.</param>
        public void Add(SelectableBrick brick)
        {
            if (!selectedBricks.Contains(brick))
            {
                selectedBricks.Add(brick);
                brick.SetSelected(true);
            }

            if (OnSelectionChange != null)
                OnSelectionChange(selectedBricks);
        }

        /// <summary>
        /// Adds bricks to selection.
        /// </summary>
        /// <param name="bricks">Bricks.</param>
        public void Add(List<SelectableBrick> bricks)
        {
            foreach (var brick in bricks)
            {
                if (!selectedBricks.Contains(brick))
                {
                    selectedBricks.Add(brick);
                    brick.SetSelected(true);
                }
            }

            if (OnSelectionChange != null)
                OnSelectionChange(selectedBricks);
        }

        /// <summary>
        /// Remove bricks from selection.
        /// </summary>
        /// <param name="bricks">Bricks.</param>
        public void Remove(List<SelectableBrick> bricks)
        {
            foreach (var brick in bricks)
            {
                selectedBricks.Remove(brick);
                brick.SetSelected(false);
            }

            if (OnSelectionChange != null)
                OnSelectionChange(selectedBricks);
        }

        /// <summary>
        /// Replace selected bricks with new one
        /// </summary>
        /// <param name="bricks">Bricks.</param>
        public void Replace(List<SelectableBrick> bricks)
        {
            foreach (var brick in selectedBricks)
                brick.SetSelected(false);

            selectedBricks = bricks;

            foreach (var brick in bricks)
                brick.SetSelected(true);

            if (OnSelectionChange != null)
                OnSelectionChange(selectedBricks);
        }

        /// <summary>
        /// Clear selction.
        /// </summary>
        public void Clear()
        {
            foreach (var brick in selectedBricks)
                brick.SetSelected(false);
            selectedBricks.Clear();

            if (OnSelectionChange != null)
                OnSelectionChange(selectedBricks);
        }

        /// <summary>
        /// Select all objects in scene
        /// </summary>
        public void SelectAll()
        {
            var root = GameObject.Find("Model");
            if (root != null)
            {
                var bricks = root.GetComponentsInChildren<SelectableBrick>();
                Replace(new List<SelectableBrick>(bricks));
            }
        }

        /// <summary>
        /// Get list of selected bricks.
        /// </summary>
        /// <returns>The selected.</returns>
        public List<SelectableBrick> GetSelected()
        {
            return new List<SelectableBrick>(selectedBricks);   
        }

        /// <summary>
        /// Turn off visual effect of selection on all selected bricks.
        /// </summary>
        public void TurnOffSelectionEffect()
        {
            foreach (var brick in selectedBricks)
                brick.SetSelected(false);
        }

        /// <summary>
        /// Restore visual effect of selection on all selected bricks.
        /// </summary>
        public void RestoreSelectionEffect()
        {
            foreach (var brick in selectedBricks)
                brick.SetSelected(true);
        }

        /// <summary>
        /// Check if brick is selected
        /// </summary>
        /// <returns><c>true</c>, if brick is selected , <c>false</c> otherwise.</returns>
        /// <param name="brick">Brick.</param>
        public bool IsSelected(SelectableBrick brick)
        {
            return selectedBricks.Contains(brick);
        }

        #endregion
    }
}
