using UnityEngine;
using NUnit.Framework;
using System.IO;
using AgaQ.Bricks.Serialization.STL;

namespace Tests.Bricks.Serialization.STL
{
	public class STLTests
	{
		const string TEMP_FILE_DIR = "Assets/Tests/Bricks/Serialization/STL/Editor/Temp";
		const string TEST_MODELS = "Assets/Tests/Bricks/Serialization/STL/Editor/Models/";

		[Test]
		public void VerifyWriteASCIIText()
		{
			DoVerifyWriteString(TEST_MODELS + "Cylinder_ASCII_RH.stl", GameObject.CreatePrimitive(PrimitiveType.Cylinder));
			DoVerifyWriteString(TEST_MODELS + "Sphere_ASCII_RH.stl", GameObject.CreatePrimitive(PrimitiveType.Sphere));
		}

		[Test]
		public void VerifyWriteBinaryText()
		{
			if(!Directory.Exists(TEMP_FILE_DIR))
				Directory.CreateDirectory(TEMP_FILE_DIR);

			DoVerifyWriteBinary(TEST_MODELS + "Cylinder_BINARY_RH.stl", GameObject.CreatePrimitive(PrimitiveType.Cylinder));
			DoVerifyWriteBinary(TEST_MODELS + "Sphere_BINARY_RH.stl", GameObject.CreatePrimitive(PrimitiveType.Sphere));

			Directory.Delete(TEMP_FILE_DIR, true);
		}

		[Test]
		public void ExportMultipleTest()
		{
			GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
			GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);

			a.transform.position = Vector3.right;
			b.transform.position = new Vector3(3f, 5f, 2.4f);
			b.transform.localRotation = Quaternion.Euler( new Vector3(45f, 45f, 10f) );

			if(!Directory.Exists(TEMP_FILE_DIR))
				Directory.CreateDirectory(TEMP_FILE_DIR);

			string temp_model_path = string.Format("{0}/multiple.stl", TEMP_FILE_DIR);
			STLExporter.Export(temp_model_path, new GameObject[] { a, b }, FileType.Binary );

			// Comparing binary files isn't great
			// Assert.IsTrue(CompareFiles(string.Format("{0}/CompositeCubes_BINARY.stl", TEST_MODELS), temp_model_path));
			Mesh[] expected = STLImporter.Import(string.Format("{0}/CompositeCubes_BINARY.stl", TEST_MODELS));
			Mesh[] results = STLImporter.Import(temp_model_path);

			Assert.IsTrue(expected != null);
			Assert.IsTrue(results != null);

			Assert.IsTrue(expected.Length == 1);
			Assert.IsTrue(results.Length == 1);

			Assert.AreEqual(expected[0].vertexCount, results[0].vertexCount);
			Assert.AreEqual(expected[0].triangles, results[0].triangles);

			// Can't use Assert.AreEqual(positions, normals, uvs) because Vec3 comparison is subject to floating point inaccuracy
			for(int i = 0; i < expected[0].vertexCount; i++)
			{
				Assert.Less( Vector3.Distance(expected[0].vertices[i], results[0].vertices[i]), .00001f );
				Assert.Less( Vector3.Distance(expected[0].normals[i], results[0].normals[i]), .00001f );
			}

			GameObject.DestroyImmediate(a);
			GameObject.DestroyImmediate(b);

			Directory.Delete(TEMP_FILE_DIR, true);
		}

		[Test]
		public void ImportAsciiTest()
		{
			Mesh[] meshes = STLImporter.Import(string.Format("{0}/Cylinder_ASCII_RH.stl", TEST_MODELS));
			Assert.IsTrue(meshes != null);
			Assert.AreEqual(1, meshes.Length);
			Assert.AreEqual(240, meshes[0].triangles.Length);
			Assert.AreEqual(240, meshes[0].vertexCount);
		}

		[Test]
		public void ImportBinaryTest()
		{
			Mesh[] meshes = STLImporter.Import(string.Format("{0}/Cylinder_BINARY_RH.stl", TEST_MODELS));
			Assert.IsTrue(meshes != null);
			Assert.AreEqual(1, meshes.Length);
			Assert.AreEqual(240, meshes[0].triangles.Length);
			Assert.AreEqual(240, meshes[0].vertexCount);
		}

	    [Test]
		public void ImportBinaryWithHeadersTest()
	    {
	        Mesh[] meshes = STLImporter.Import(string.Format("{0}/CubedShape_BINARY_H.stl", TEST_MODELS));
	        Assert.IsTrue(meshes != null);
	        Assert.AreEqual(1, meshes.Length);
	        Assert.AreEqual(204, meshes[0].triangles.Length);
	        Assert.AreEqual(204, meshes[0].vertexCount);
	    }

		#region Private functions

	    void DoVerifyWriteBinary(string expected_path, GameObject go)
		{
			string temp_model_path = string.Format("{0}/binary_file.stl", TEMP_FILE_DIR);

			Assert.IsTrue(STLWriter.WriteFile(temp_model_path, go.GetComponent<MeshFilter>().sharedMesh, FileType.Binary));
			Assert.IsTrue(CompareFiles(temp_model_path, expected_path));

			GameObject.DestroyImmediate(go);
		}

		void DoVerifyWriteString(string path, GameObject go)
		{
			string ascii = STLWriter.WriteString(go.GetComponent<MeshFilter>().sharedMesh, true);
			ascii = ascii.Replace("\r\n", "\n");
			Assert.AreNotEqual(ascii, null);
			Assert.AreNotEqual(ascii, "");
			string expected = File.ReadAllText(path);
			Assert.AreNotEqual(expected, null);
			Assert.AreNotEqual(expected, "");
			Assert.AreEqual(ascii, expected);
			GameObject.DestroyImmediate(go);
		}

		bool CompareFiles(string left, string right)
		{
			if(left == null || right == null)
				return false;

			FileInfo a = new FileInfo(left);
			FileInfo b = new FileInfo(right);

			if (a.Length != b.Length)
				return false;

			using(FileStream f0 = a.OpenRead())
			using(FileStream f1 = b.OpenRead())
			using(BufferedStream bs0 = new BufferedStream(f0))
			using(BufferedStream bs1 = new BufferedStream(f1))
			{
				for (long i = 0; i < a.Length; i++)
				{
					if(bs0.ReadByte() != bs1.ReadByte())
						return false;
				}
			}

			return true;
		}

		#endregion
	}
}
