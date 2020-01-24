using UnityEngine;
using UnityEngine.EventSystems;
using AgaQ.Bricks.History;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AgaQ.Bricks.Tools
{
    /// <summary>
    /// Tool to remove bricks from scene.
    /// </summary>
    public abstract class DeleteTool : MoveTool
    {
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
                DeleteSelected();

            base.OnUpdate();
        }

        /// <summary>
        /// Tunrn off (set inactive) selected bricks
        /// and register it HistoryManager.
        /// </summary>
        public void DeleteSelected()
        {
            var currSelObject = EventSystem.current.currentSelectedGameObject;
            if (currSelObject != null)
            {
                var input = currSelObject.GetComponent<InputField>();
                if (input != null)
                    return;
            }
                       
            var selectedBricks = SelectionManager.instance.GetSelected();
            SelectionManager.instance.Clear();

            if (selectedBricks.Count > 0 || isDragging)
            {
                DragableBrick dragObject = null;

                //if dragging is pending break it
                if (isDragging)
                {
                    dragObject = dragableBrick;
                    OnCancel();
                }

                //prepare state for undo
                HistoryNodeRemove[] historyNodes = selectedBricks.Count > 0 ?
                    HistoryTool.PrepareRemoveNodes(selectedBricks) :
                    HistoryTool.PrepareRemoveNodes(dragObject.gameObject);

                //remove selected bricks
                //(realy don't delete them, just set inactive to be able to do undo action)
                if (selectedBricks.Count > 0)
                {
                    foreach (var brick in selectedBricks)
                    {
                        if (brick is AgaQBrick)
                            (brick as AgaQBrick).ClearJoints();

                        brick.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (dragObject is AgaQBrick)
                        (dragObject as AgaQBrick).ClearJoints();

                    dragObject.gameObject.SetActive(false);
                }

                //register state in history
                HistoryManager.instance.Register(historyNodes);

                if (OnModelChange != null)
                    OnModelChange();
            }
        }
    }
}
