using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using AgaQ.Bricks;


namespace Tests.Bricks
{
    public class BrickMock : Brick
    {}

    public class BrickTests
    {
        BrickMock cube;
        BrickMock sphere;

        [SetUp]
        public void BeforeTests()
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube = gameObject.AddComponent<BrickMock>();

            gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere = gameObject.AddComponent<BrickMock>();
        }

        [UnityTest]
        public IEnumerator BrickGetLowestPointTest()
        {
            var cubeLowestPoint = cube.GetLowestPoint();
            Assert.AreEqual(cubeLowestPoint.y, -0.5f);
            var sphereLowestPont = sphere.GetLowestPoint();
            Assert.AreEqual(sphereLowestPont, new Vector3(-0.5f, -0.5f, -0.5f));
            return null;
        }

        [UnityTest]
        public IEnumerator BrickGetUnrotatetdBounds()
        {
            cube.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            var bounds1 = cube.GetUnrotatedBounds();
            cube.gameObject.transform.rotation = Quaternion.Euler(new Vector3(1, 2, 3));
            var bounds2 = cube.GetUnrotatedBounds();
            Assert.AreEqual(bounds1.center, bounds2.center);
            Assert.AreEqual(bounds1.size, bounds2.size);
            return null;
        }

        [UnityTest]
        public IEnumerator BrickGetBounds()
        {
            cube.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            var bounds1 = cube.GetUnrotatedBounds();
            cube.gameObject.transform.rotation = Quaternion.Euler(new Vector3(1, 2, 3));
            var bounds2 = cube.GetUnrotatedBounds();
            Assert.AreEqual(bounds1.center, bounds2.center);
            Assert.AreEqual(bounds1.size, bounds2.size);
            return null;
        }

        [TearDown]
        public void AfterTests()
        {
            Object.DestroyImmediate(cube.gameObject);
            Object.DestroyImmediate(sphere.gameObject);
        }
    }
}
