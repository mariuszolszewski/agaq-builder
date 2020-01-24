#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using AgaQ.Camera;

namespace Tests.Camera
{
	public class CameraControllerTests
	{
	    [UnityTest]
	    public IEnumerator RotateVerticalTest()
	    {
	        yield return null;

			var cam = UnityEngine.Camera.main.GetComponent<CameraController>();
			var oldPos = cam.transform.position;
			var oldRot = cam.transform.rotation; 

			cam.RotateVertical (3);
			yield return new WaitForSeconds (cam.animTime);

			Assert.AreNotEqual(oldPos, cam.transform.position);
			Assert.AreNotEqual(oldRot, cam.transform.rotation);
		}

		[UnityTest]
		public IEnumerator RotateHorizontalTest()
		{
			yield return null;

			var cam = UnityEngine.Camera.main.GetComponent<CameraController>();
			var oldPos = cam.transform.position;
			var oldRot = cam.transform.rotation; 

			cam.RotateHorizontal (3);
			yield return new WaitForSeconds (cam.animTime);

			Assert.AreNotEqual(oldPos, cam.transform.position);
			Assert.AreNotEqual(oldRot, cam.transform.rotation);
		}

		[UnityTest]
		public IEnumerator SetTopViewTest()
		{
			yield return null;

			var cam = UnityEngine.Camera.main.GetComponent<CameraController>();
			var oldPos = cam.transform.position;
			var oldRot = cam.transform.rotation; 

			cam.SetTopView ();
			yield return new WaitForSeconds (cam.animTime);

			Assert.AreNotEqual(oldPos, cam.transform.position);
			Assert.AreNotEqual(oldRot, cam.transform.rotation);
		}

		[UnityTest]
		public IEnumerator SetBottomViewTest()
		{
			yield return null;

			var cam = UnityEngine.Camera.main.GetComponent<CameraController>();
			var oldPos = cam.transform.position;
			var oldRot = cam.transform.rotation; 

			cam.SetBottomView ();
			yield return new WaitForSeconds (cam.animTime);

			Assert.AreNotEqual(oldPos, cam.transform.position);
			Assert.AreNotEqual(oldRot, cam.transform.rotation);
		}

		[UnityTest]
		public IEnumerator SetFrontViewTest()
		{
			yield return null;

			var cam = UnityEngine.Camera.main.GetComponent<CameraController>();
			var oldPos = cam.transform.position;
			var oldRot = cam.transform.rotation; 

			cam.SetFrontView ();
			yield return new WaitForSeconds (cam.animTime);

			Assert.AreNotEqual(oldPos, cam.transform.position);
			Assert.AreNotEqual(oldRot, cam.transform.rotation);
		}

		[UnityTest]
		public IEnumerator SetRearViewTest()
		{
			yield return null;

			var cam = UnityEngine.Camera.main.GetComponent<CameraController>();
			var oldPos = cam.transform.position;
			var oldRot = cam.transform.rotation; 

			cam.SetRearView ();
			yield return new WaitForSeconds (cam.animTime);

			Assert.AreNotEqual(oldPos, cam.transform.position);
			Assert.AreNotEqual(oldRot, cam.transform.rotation);
		}

		[UnityTest]
		public IEnumerator SetLeftViewTest()
		{
			yield return null;

			var cam = UnityEngine.Camera.main.GetComponent<CameraController>();
			var oldPos = cam.transform.position;
			var oldRot = cam.transform.rotation; 

			cam.SetLeftView ();
			yield return new WaitForSeconds (cam.animTime);

			Assert.AreNotEqual(oldPos, cam.transform.position);
			Assert.AreNotEqual(oldRot, cam.transform.rotation);
		}

		[UnityTest]
		public IEnumerator SetRightViewTest()
		{
			yield return null;

			var cam = UnityEngine.Camera.main.GetComponent<CameraController>();
			var oldPos = cam.transform.position;
			var oldRot = cam.transform.rotation; 

			cam.SetRightView ();
			yield return new WaitForSeconds (cam.animTime);

			Assert.AreNotEqual(oldPos, cam.transform.position);
			Assert.AreNotEqual(oldRot, cam.transform.rotation);
		}
	}
}

#endif
