using NUnit.Framework;
using AgaQ.Bricks.Tools;

namespace Tests.Bricks.Tools
{
	class ToolsMock : AgaQ.Bricks.Tools.Tools
	{}

	public class ToolsTests
	{
		ToolsMock tools;

		[OneTimeSetUp]
		public void BeforeTests()
		{
			tools = new ToolsMock ();
		}

		[Test]
		public void SelectSingleToolTest()
		{
			var tool = tools.selectSingleTool;
			Assert.IsTrue (tool is SelectSingleTool);
		}

		[Test]
		public void SelectShapeToolTest()
		{
			var tool = tools.selectShapeTool;
			Assert.IsTrue (tool is SelectShapeTool);
		}

		[Test]
		public void SelectColorToolTest()
		{
			var tool = tools.selectColorTool;
			Assert.IsTrue (tool is SelectColorTool);
		}

		[Test]
		public void RotateToolTest()
		{
			var tool = tools.rotateTool;
			Assert.IsTrue (tool is RotateTool);
		}

		[Test]
		public void CloneToolTest()
		{
			var tool = tools.cloneTool;
			Assert.IsTrue (tool is CloneTool);
		}
		[Test]
		public void ColorToolTest()
		{
			var tool = tools.colorTool;
			Assert.IsTrue (tool is ColorTool);
		}

		[Test]
		public void AddToolTest()
		{
			var tool = tools.addTool;
			Assert.IsTrue (tool is AddTool);
		}
	}
}
