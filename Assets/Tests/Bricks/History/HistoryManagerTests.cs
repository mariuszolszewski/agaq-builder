#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using AgaQ.Bricks.History;
using UnityEditor.VersionControl;

namespace Tests.Bricks.History
{
	public class HistoryManagerTests
	{
		GameObject gameObject;

		[UnityTest]
		public IEnumerator RegisterTest()
		{
			yield return null;

			gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			HistoryManager.instance.Register (HistoryTool.PrepareAddNodes(new GameObject[] { gameObject }));
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			HistoryManager.instance.Register (HistoryTool.PrepareColorNodes(new GameObject[] { gameObject }));
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			HistoryManager.instance.Register (HistoryTool.PrepareTransformNodes(new GameObject[] { gameObject }));
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			HistoryManager.instance.Register (HistoryTool.PrepareRemoveNodes(new GameObject[] { gameObject }));
		}

		[UnityTest]
		public IEnumerator UndoRedoTest()
		{
			yield return null;

			gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

			HistoryManager.instance.Register (HistoryTool.PrepareAddNodes(new GameObject[] { gameObject }));

			var mat = gameObject.GetComponent<Renderer> ().sharedMaterial;
			mat.color = Color.white;
			var nodes1 = HistoryTool.PrepareColorNodes (new GameObject[] { gameObject });
			mat.color = Color.red;
			HistoryManager.instance.Register (nodes1);

			var oldPos = gameObject.transform.position;
			var oldRot = gameObject.transform.rotation;
			var nodes2 = HistoryTool.PrepareTransformNodes (new GameObject[] { gameObject });
			gameObject.transform.position = new Vector3 (10, 10, 10);
			gameObject.transform.rotation = Quaternion.Euler (new Vector3 (10, 10, 10));
			HistoryManager.instance.Register (nodes2);

			var nodes3 = HistoryTool.PrepareRemoveNodes (new GameObject[] { gameObject });
			HistoryManager.instance.Register (nodes3);

			HistoryManager.instance.Undo ();
			Asset.Equals (gameObject.activeSelf, true);

			HistoryManager.instance.Undo ();
			Asset.Equals (gameObject.transform.position, oldPos);
			Asset.Equals (gameObject.transform.rotation, oldRot);

			HistoryManager.instance.Undo ();
			Asset.Equals (mat.color, Color.white);

			HistoryManager.instance.Undo ();
			Asset.Equals (gameObject.activeSelf, false);

			HistoryManager.instance.Redo ();
			Asset.Equals (gameObject.activeSelf, true);

			HistoryManager.instance.Redo ();
			Asset.Equals (mat.color, Color.red);
		
			HistoryManager.instance.Redo ();
			Asset.Equals (new Vector3 (10, 10, 10), oldPos);
			Asset.Equals (Quaternion.Euler (new Vector3 (10, 10, 10)), oldRot);

			HistoryManager.instance.Redo ();
			Asset.Equals (gameObject.activeSelf, false);
		}
						
		[OneTimeTearDown]
		public void AfterTests()
		{
			Object.DestroyImmediate(gameObject);
		}
	}
}

#endif
