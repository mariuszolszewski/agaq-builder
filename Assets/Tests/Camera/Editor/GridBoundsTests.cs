using UnityEngine.TestTools;
using NUnit.Framework;
using AgaQ.Camera;
using UnityEngine;
using System.Collections;

namespace Tests.Camera
{
	public class GridBoundsTests {

		[UnityTest]
		[TestCase(1)]
		[TestCase(10)]
		public void ConstructorTest(float size)
		{
			var gridBounds = new GridBounds (size);

			Assert.AreEqual (gridBounds.minX, gridBounds.minZ);
			Assert.AreEqual (gridBounds.maxX, gridBounds.maxZ);
			Assert.IsTrue (Mathf.Approximately(gridBounds.minX + gridBounds.maxX, 0));
			Assert.IsTrue (Mathf.Approximately(gridBounds.minZ + gridBounds.maxZ, 0));
		}

		[UnityTest]
		public IEnumerator SetSizeTest()
		{
			var gridBound = new GridBounds (1);
			var bounds = new Bounds ( new Vector3 (1, 1, 1), new Vector3 (5, 5, 5));
			gridBound.SetSize (bounds);

			Assert.IsTrue (gridBound.minX < -1);
			Assert.IsTrue (gridBound.maxX > 1);
			Assert.IsTrue (gridBound.minZ < -1);
			Assert.IsTrue (gridBound.maxZ > 1);

			return null;
		}
	}
}
