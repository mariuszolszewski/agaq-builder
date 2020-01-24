using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AgaQ.Bricks.Serialization
{
    public class BricksUuidDictionary : MonoBehaviour
    {
        public BrickUuidDefinition[] definitions;
        public static BricksUuidDictionary instance;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        #if UNITY_EDITOR

        /// <summary>
        /// Rebuild dictionary
        /// </summary>
        public void Rebuild()
        {
            List<BrickUuidDefinition> newDefinisions = new List<BrickUuidDefinition>();

            var assets = AssetDatabase.FindAssets("", new String[] {"Assets/Resources/AgaQ"});
            foreach (var assetGuuid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetGuuid);
                GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    var brick = go.GetComponent<Brick>();
                    if (brick != null)
                    {
                        //chekc if there is current uuid id dictionary
                        //this starnhe that assetdatabase returns so many duplicats
                        if (!newDefinisions.Exists(x => x.brickUuid == brick.uuid))
                        {
                            if (path.Contains("Assets/Resources"))
                                path = path.Substring(17);
                            if (path.Contains(".prefab"))
                                path = path.Substring(0, path.Length - 7);
                            
                            newDefinisions.Add(new BrickUuidDefinition(brick.uuid, path));
                        }
                    }
                }
            }

            definitions = newDefinisions.ToArray();
        }

        #endif
    }
}
