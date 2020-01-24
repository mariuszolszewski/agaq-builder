using UnityEngine;
using NUnit.Framework;
using AgaQ.Bricks.History;

namespace Tests.Bricks.History
{
	public class HistoryToolTests
	{
		GameObject gameObject;

		[SetUp]
		public void BeforeTests()
		{
			gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		}

		[Test]
		public void PrepareAddNodesTest()
		{
			var nodes = HistoryTool.PrepareAddNodes (new GameObject[] { gameObject });
			Assert.IsNotNull (nodes);
			Assert.Greater (nodes.Length, 0);
			Assert.IsInstanceOf<HistoryNodeAdd>(nodes[0]);
		}

		[Test]
		public void PreparePositionNodesTest()
		{
			var nodes = HistoryTool.PrepareTransformNodes (new GameObject[] { gameObject });
			Assert.IsNotNull (nodes);
			Assert.Greater (nodes.Length, 0);
			Assert.IsInstanceOf<HistoryNodeTransform>(nodes[0]);
		}

		[Test]
		public void PrepareColorNodesTest()
		{
			var nodes = HistoryTool.PrepareColorNodes (new GameObject[] { gameObject });
			Assert.IsNotNull (nodes);
			Assert.Greater (nodes.Length, 0);
			Assert.IsInstanceOf<HistoryNodeChangeColor>(nodes[0]);
		}

		[Test]
		public void PrepareRemoveNodesTest()
		{
			var nodes = HistoryTool.PrepareRemoveNodes (new GameObject[] { gameObject });
			Assert.IsNotNull (nodes);
			Assert.Greater (nodes.Length, 0);
			Assert.IsInstanceOf<HistoryNodeRemove>(nodes[0]);
		}

		[TearDown]
		public void AfterTests()
		{
			Object.DestroyImmediate(gameObject);
		}
	}
}
