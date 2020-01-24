using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using AgaQ.Bricks;
using System.Collections.Generic;
using System.Collections;

namespace Tests.Bricks
{
    public class SelectionManagerTests
    {
        SelectionManager selectionManager;
        SelectableBrick selectableBrick;

        [SetUp]
        public void BeforeEveryTest()
        {
            GameObject gameObject = new GameObject();
            selectionManager = gameObject.AddComponent<SelectionManager>();
            selectionManager.Init();

            gameObject = new GameObject();
            selectableBrick = gameObject.AddComponent<SelectableBrickMock>();
        }

        [UnityTest]
        public IEnumerator AddTest()
        {
            SelectionManager.instance.Add(selectableBrick);
            Assert.AreEqual(SelectionManager.instance.SelectedAmount, 1);
            return null;
        }

        [UnityTest]
        public IEnumerator Removetest()
        {
            SelectionManager.instance.Add(selectableBrick);
            SelectionManager.instance.Remove(new List<SelectableBrick> { selectableBrick });
            Assert.AreEqual(SelectionManager.instance.SelectedAmount, 0);
            return null;
        }

        [UnityTest]
        public IEnumerator ReplaceTest()
        {
            SelectionManager.instance.Replace(new List<SelectableBrick> { selectableBrick });
            Assert.AreEqual(SelectionManager.instance.SelectedAmount, 1);

            SelectionManager.instance.Add(selectableBrick);
            SelectionManager.instance.Replace(new List<SelectableBrick> { selectableBrick });
            Assert.AreEqual(SelectionManager.instance.SelectedAmount, 1);

            return null;
        }

        [TearDown]
        public void AfterEveryTest()
        {
            Object.DestroyImmediate(selectionManager.gameObject);
            Object.DestroyImmediate(selectableBrick.gameObject);
        }
    }
}