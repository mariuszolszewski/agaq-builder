using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using AgaQ.Bricks;

namespace Tests.Bricks
{
    public class HighlightableBrickMock : HighlightableBrick
    {
    }

    public class HiglightableBrickTests
    {
        //BoundBox boundBox;
        HighlightableBrickMock highlitableBrick;

        [OneTimeSetUp]
        public void BeforeTests()
        {
            GameObject gameObject = new GameObject();
            highlitableBrick = gameObject.AddComponent<HighlightableBrickMock>();
        }

        [UnityTest]
        public IEnumerator HihligtableBrickHiglightTest()
        {
            highlitableBrick.SetHighlighted(true);
            Assert.AreEqual(highlitableBrick.highlighted, true);

            highlitableBrick.SetHighlighted(false);
            Assert.AreEqual(highlitableBrick.highlighted, false);

            return null;
        }

        [OneTimeTearDown]
        public void AfterTests()
        {
            Object.DestroyImmediate(highlitableBrick.gameObject);
        }
    }
}
