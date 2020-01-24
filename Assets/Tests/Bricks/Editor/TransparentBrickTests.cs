using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using AgaQ.Bricks;

namespace Tests.Bricks
{
    public class TransparentBrickMock : TransparentBrick
    {}

    public class TransparentBrickTests
    {
        GameObject gameObject;
        TransparentBrickMock brick;

        [OneTimeSetUp]
        public void BeforeTests()
        {
            gameObject = new GameObject();
            brick = gameObject.AddComponent<TransparentBrickMock>();
        }

        [UnityTest]
        public IEnumerator DragableBrickMoveToCursor()
        {
            brick.SetTransparent(true);
            Assert.AreEqual(brick.isTransparent, true);
            brick.SetTransparent(false);
            Assert.AreEqual(brick.isTransparent, false);
            return null;
        }

        [OneTimeTearDown]
        public void AfterTests()
        {
            Object.DestroyImmediate(gameObject);
        }
    }
}
