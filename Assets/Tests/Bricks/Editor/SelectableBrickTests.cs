using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using AgaQ.Bricks;

namespace Tests.Bricks
{
    public class SelectableBrickMock : SelectableBrick
    {
    }

    public class SelectableBrickTests
    {
        SelectableBrickMock selectableBrick;

        [OneTimeSetUp]
        public void BeforeTests()
        {
            GameObject gameObject = new GameObject();
            selectableBrick = gameObject.AddComponent<SelectableBrickMock>();
        }

        [UnityTest]
        public IEnumerator SelectableBrickSelectTest()
        {
            selectableBrick.SetSelected(true);
            Assert.AreEqual(selectableBrick.selected, true);

            selectableBrick.SetSelected(false);
            Assert.AreEqual(selectableBrick.selected, false);

            return null;
        }

        [OneTimeTearDown]
        public void AfterTests()
        {
            Object.DestroyImmediate(selectableBrick.gameObject);
        }
    }
}
