using UnityEngine;
using NUnit.Framework;
using AgaQ.Bricks;

namespace Tests.Bricks
{
    public class AgaQBrickTests
    {
        AgaQBrick brick;

        [OneTimeSetUp]
        public void BeforeTests()
        {
            GameObject gameObject = new GameObject();
            brick = gameObject.AddComponent<AgaQBrick>();
        }

        //TODO: write tests for AgaQBrick
        //[UnityTest]
        //public IEnumerator DragableBrickMoveToCursor()
        //{
        //    return null;
        //}

        [OneTimeTearDown]
        public void AfterTests()
        {
            Object.DestroyImmediate(brick);
        }
    }
}
