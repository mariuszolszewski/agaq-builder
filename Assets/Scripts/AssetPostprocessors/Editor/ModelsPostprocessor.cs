using UnityEngine;
using UnityEditor;
using AgaQ.Bricks;
using AgaQ.Bricks.Joints;
using System.IO;
using System.Text.RegularExpressions;

namespace AssetPostprocessors
{
    /// <summary>
    /// This class changes import settings for all models in directory Models/AgaQ
    /// </summary>
    public class ModelsPostprocessor : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            return;
            if (assetPath.Contains("Models/AgaQ"))
            {
                //set model importer preferences
                ModelImporter modelImporter = (ModelImporter)assetImporter;
                modelImporter.importNormals = ModelImporterNormals.Calculate;
                modelImporter.isReadable = false;
                modelImporter.normalCalculationMode = ModelImporterNormalCalculationMode.AreaAndAngleWeighted;
                modelImporter.normalSmoothingAngle = 40;
                modelImporter.importLights = false;
                modelImporter.importCameras = false;
                modelImporter.importAnimation = false;
                modelImporter.importMaterials = false;

                //build prefab full path
                string fileName = Path.GetFileName(assetPath);
                string modelPath = Path.GetDirectoryName(assetPath);
                var rx = new Regex("Models");
                var results = rx.Split(modelPath);
                string prefabPath = string.Concat("Assets/Resources", results[results.Length - 1]);
                fileName = fileName.Remove(fileName.Length - Path.GetExtension(fileName).Length); //remove extension
                string prefabFullPath = string.Concat(prefabPath, "/", fileName, ".prefab");

                //check if there is prefab for that model
                if (!File.Exists(prefabFullPath))
                {
                    //instanstiate model
                    GameObject brickObject = Object.Instantiate((GameObject)EditorGUIUtility.Load(assetPath));

                    //reset position and rotation
                    brickObject.transform.position = Vector3.zero;
                    brickObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                    brickObject.transform.localScale = Vector3.one;

                    //remove animator if exists
                    Animator brickAnimator = brickObject.GetComponent<Animator>();
                    if (brickAnimator != null)
                        Object.DestroyImmediate(brickAnimator);

                    //set default material
                    MeshRenderer brickRenderer = brickObject.GetComponent<MeshRenderer>();
                    if (brickRenderer != null)
                    {
                        Material brickMaterial = (Material)EditorGUIUtility.Load("Assets/Gfx/Materials/Bricks materials/Standard brick material.mat");
                        brickRenderer.material = brickMaterial;
                    }

                    //add agaq script
                    brickObject.AddComponent<AgaQBrick>();

                    //add collider
                    MeshCollider collider = brickObject.AddComponent<MeshCollider>();
                    collider.convex = true;

                    //detect add male joints
                    BricksPinDetector pinsDetector = new BricksPinDetector();
                    var mesh = brickObject.GetComponent<MeshFilter>().sharedMesh;
                    var pins = pinsDetector.DetectPins(mesh);
                    foreach (var pin in pins)
                    {
                        GameObject jointObject = new GameObject();
                        jointObject.transform.SetParent(brickObject.transform);
                        jointObject.transform.position = pin.position;
                        jointObject.name = "joint male";
                        jointObject.AddComponent<MaleJoint>();
                    }

					//detect female joint
					BrickJointDetector jointsDetector = new BrickJointDetector ();
					var femaleJoints = jointsDetector.DetectPins (mesh);

                    //add one female joint
                    //jointObject = new GameObject();
                    //jointObject.transform.SetParent(brickObject.transform);
                    //jointObject.name = "joint female";
                    //joint = jointObject.AddComponent<AgaQ.Bricks.Joint>();
                    //joint.type = JointType.female;

                    //check if there is no directory for prefab, create one
                    if (!Directory.Exists(prefabPath))
                        Directory.CreateDirectory(prefabPath);

                    //save as prefab in resources diretory
                    PrefabUtility.CreatePrefab(prefabFullPath, brickObject);

                    //Cleanup - delete brick object
                    Object.DestroyImmediate(brickObject);
                }
            }
        }
    }
}
