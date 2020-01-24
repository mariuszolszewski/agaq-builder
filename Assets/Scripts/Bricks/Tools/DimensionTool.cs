using AgaQ.Bricks.History;

namespace AgaQ.Bricks.Tools
{
    public class DimensionTool
    {
        /// <summary>
        /// Change brick dimension.
        /// Undefhood is replacing models.
        /// </summary>
        /// <returns><c>true</c>, if dimension was changed, <c>false</c> otherwise.</returns>
        /// <param name="brick">Brick.</param>
        public bool ChangeDimension(AgaQBrick brick, int propertyIndex, int valueIndex)
        {
            //check if new set of diemnsions is valid
            var values = brick.dimensionGroup.GetPropertyValues(propertyIndex);
            var newValues = brick.dimensionParams;
            newValues[propertyIndex] = values[valueIndex];
            var brickPath = brick.dimensionGroup.GetBrickPathForValues(newValues);
            if (brickPath == null)
                return false;

            //change brick
            var newBrick = ChangeBrick(brickPath, brick);
            if (newBrick == null)
                return false;

            //register history
            if (brick != null)
                HistoryManager.instance.Register(new HistoryNodeDimension(brick.gameObject, newBrick.gameObject));

            //change selection
            SelectionManager.instance.Clear();
            SelectionManager.instance.Add(newBrick as SelectableBrick);
                            
            return true;
        }

        /// <summary>
        /// Instansiate new brick in place of all one.
        /// Copy to new brick position, rotation and scale.
        /// Make old brick inactive.
        /// </summary>
        /// <returns>The new brick.</returns>
        /// <param name="newBrickPath">New brick resources path.</param>
        /// <param name="oldBrick">Old brick.</param>
        Brick ChangeBrick(string newBrickPath, AgaQBrick oldBrick)
        {
            var newBrick = BrickBuilder.InstansiateFromResources(newBrickPath);

            if (newBrick == null)
                return null;
            
            newBrick.transform.position = oldBrick.transform.position;
            newBrick.transform.rotation = oldBrick.transform.rotation;
            newBrick.transform.localScale = oldBrick.transform.localScale;

            if (oldBrick.isDragging && ToolsManager.instance.tool is MoveTool)
                //let exchange bricks in move tool
                (ToolsManager.instance.tool as MoveTool).ChangeBrick(newBrick as DragableBrick);
            else
                oldBrick.gameObject.SetActive(false);

            return newBrick;
        }
    }
}
