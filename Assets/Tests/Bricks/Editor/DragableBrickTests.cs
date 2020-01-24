using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using AgaQ.Bricks;

namespace Tests.Bricks
{
    public class DragableBrickTests
    {
        DragableBrick brick;

        [OneTimeSetUp]
        public void BeforeTests()
        {
            GameObject gameObject = new GameObject();
            brick = gameObject.AddComponent<DragableBrick>();
        }

        [OneTimeTearDown]
        public void AfterTests()
        {
            Object.DestroyImmediate(brick);
        }
    }
}
