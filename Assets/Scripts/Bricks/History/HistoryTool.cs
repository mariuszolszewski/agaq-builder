using UnityEngine;
using System.Collections.Generic;

namespace AgaQ.Bricks.History
{
    /// <summary>
    /// Provides some helper functions to prepare hostory nodes collections.
    /// </summary>
    public static class HistoryTool
    {
        #region Add node

        /// <summary>
        /// Prepare add nodes.
        /// </summary>
        /// <returns>The add nodes.</returns>
        /// <param name="gameObject">Game object.</param>
        public static HistoryNodeAdd[] PrepareAddNodes(GameObject gameObject)
        {
            return PrepareAddNodes(new GameObject[] { gameObject });
        }

        /// <summary>
        /// Prepare add nodes.
        /// </summary>
        /// <returns>The add nodes.</returns>
        /// <param name="bricks">Bricks.</param>
        public static HistoryNodeAdd[] PrepareAddNodes(List<SelectableBrick> bricks)
        {
            return PrepareAddNodes(GetGameObjects(bricks));
        }

        /// <summary>
        /// Prepare add nodes.
        /// </summary>
        /// <returns>The add nodes.</returns>
        /// <param name="gameObjects">Game objects.</param>
        public static HistoryNodeAdd[] PrepareAddNodes(GameObject[] gameObjects)
        {
            HistoryNodeAdd[] nodes = new HistoryNodeAdd[gameObjects.Length];

            for (var i = 0; i < nodes.Length; i++)
                nodes[i] = new HistoryNodeAdd(gameObjects[i]);

            return nodes;
        }

        #endregion

        #region Transform node

        /// <summary>
        /// Prepare position nodes. Just rember current position.
        /// </summary>
        /// <returns>The position nodes.</returns>
        /// <param name="gameObject">Game object.</param>
        public static HistoryNodeTransform[] PrepareTransformNodes(GameObject gameObject)
        {
            return PrepareTransformNodes(new GameObject[] { gameObject });
        }

        /// <summary>
        /// Prepare position nodes. Just rember current position.
        /// </summary>
        /// <returns>The position nodes.</returns>
        /// <param name="bricks">Bricks.</param>
        public static HistoryNodeTransform[] PrepareTransformNodes(List<SelectableBrick> bricks)
        {
            return PrepareTransformNodes(GetGameObjects(bricks));
        }

        /// <summary>
        /// Prepare position nodes. Just rember current position.
        /// </summary>
        /// <returns>The position nodes.</returns>
        /// <param name="gameObjects">Game objects.</param>
        public static HistoryNodeTransform[] PrepareTransformNodes(GameObject[] gameObjects)
        {
            HistoryNodeTransform[] nodes = new HistoryNodeTransform[gameObjects.Length];

            for (var i = 0; i < nodes.Length; i++)
                nodes[i] = new HistoryNodeTransform(gameObjects[i]);

            return nodes;
        }

        #endregion

        #region Change color node

        /// <summary>
        /// Prepare change color nodes. Just remember old color.
        /// </summary>
        /// <returns>The color nodes.</returns>
        /// <param name="gameObject">Game object.</param>
        public static HistoryNodeChangeColor[] PrepareColorNodes(GameObject gameObject)
        {
            return PrepareColorNodes(new GameObject[] { gameObject });
        }

        /// <summary>
        /// Prepare change color nodes. Just remember old color.
        /// </summary>
        /// <returns>The color nodes.</returns>
        /// <param name="bricks">Bricks.</param>
        public static HistoryNodeChangeColor[] PrepareColorNodes(List<SelectableBrick> bricks)
        {
            return PrepareColorNodes(GetGameObjects(bricks));
        }

        /// <summary>
        /// Prepare change color nodes. Just remember old color.
        /// </summary>
        /// <returns>The color nodes.</returns>
        /// <param name="gameObjects">Game objects.</param>
        public static HistoryNodeChangeColor[] PrepareColorNodes(GameObject[] gameObjects)
        {
            HistoryNodeChangeColor[] nodes = new HistoryNodeChangeColor[gameObjects.Length];

            for (var i = 0; i < nodes.Length; i++)
                nodes[i] = new HistoryNodeChangeColor(gameObjects[i]);

            return nodes;
        }

        #endregion

        #region Remove node

        /// <summary>
        /// Prepare remove nodes.
        /// </summary>
        /// <returns>The remove nodes.</returns>
        /// <param name="gameObject">Game object.</param>
        public static HistoryNodeRemove[] PrepareRemoveNodes(GameObject gameObject)
        {
            return PrepareRemoveNodes(new GameObject[] { gameObject });
        }

        /// <summary>
        /// Prepare remove nodes.
        /// </summary>
        /// <returns>The remove nodes.</returns>
        /// <param name="bricks">Bricks.</param>
        public static HistoryNodeRemove[] PrepareRemoveNodes(List<SelectableBrick> bricks)
        {
            return PrepareRemoveNodes(GetGameObjects(bricks));
        }

        /// <summary>
        /// Prepare remove nodes.
        /// </summary>
        /// <returns>The remove nodes.</returns>
        /// <param name="gameObjects">Game objects.</param>
        public static HistoryNodeRemove[] PrepareRemoveNodes(GameObject[] gameObjects)
        {
            HistoryNodeRemove[] nodes = new HistoryNodeRemove[gameObjects.Length];

            for (var i = 0; i < nodes.Length; i++)
                nodes[i] = new HistoryNodeRemove(gameObjects[i]);

            return nodes;
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Gets GameObjets array from selectable list
        /// </summary>
        /// <returns>The game objects.</returns>
        /// <param name="bricks">Bricks.</param>
        static GameObject[] GetGameObjects(List<SelectableBrick> bricks)
        {
            GameObject[] gameObjects = new GameObject[bricks.Count];

            for (var i = 0; i < gameObjects.Length; i++)
                gameObjects[i] = bricks[i].gameObject;

            return gameObjects;
        }

        #endregion
    }
}
