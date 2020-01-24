using System.IO.Compression;
using System.IO;
using System.Xml;
using System;
using AgaQ.Bricks.Tools;
using UnityEngine;


namespace AgaQ.Bricks.Serialization
{
    public class ModelDeserializer : ModelSerializer
    {
        /// <summary>
        /// Deserialize file to set of bricks.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Deserialize(string fileName)
        {            
            using (var file = File.OpenRead(fileName))
            using (var reader = new GZipStream(file, CompressionMode.Decompress))
            {
                var xmlReader = new XmlTextReader(reader);
                Deserialize(xmlReader, null, false);
            }
        }

        /// <summary>
        /// Deserialize file to set of bricks.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="parent">Parent.</param>
        public void Deserialize(string fileName, Transform parent)
        {
            using (var file = File.OpenRead(fileName))
            using (var reader = new GZipStream(file, CompressionMode.Decompress))
            {
                var xmlReader = new XmlTextReader(reader);
                Deserialize(xmlReader, parent, false);
            }
        }

        /// <summary>
        /// Deserialize one level of xml tree.
        /// </summary>
        /// <returns>The deserialize.</returns>
        /// <param name="xmlReader">Xml reader.</param>
        void Deserialize(XmlReader xmlReader, Transform parent, bool group)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)                    
                {
                    if (xmlReader.Name == "AgaQBrick")
                        DeserializeAgaQ(xmlReader, parent, group);
                    else if (xmlReader.Name == "OrdinaryBrick")
                        DeserializeOrdinary(xmlReader, parent, group);
                    else if (xmlReader.Name == "AgaQGroup")
                        DeserializeAgaQGroup(xmlReader, parent, group);
                    else if (xmlReader.Name == "BricksGroup")
                        DeserializeBricksGroup(xmlReader, parent, group);
                }
            }

            xmlReader.Close();
        }

        /// <summary>
        /// Deserializes the AgaQ brick.
        /// </summary>
        /// <param name="xmlReader">Xml reader.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="group">If set to <c>true</c> group.</param>
        void DeserializeAgaQ(XmlReader xmlReader, Transform parent, bool group)
        {
            Int64 uuid = Int64.Parse(xmlReader.GetAttribute("uuid"));
            if (uuid > 0)
            {
                var def = Array.Find(
                    BricksUuidDictionary.instance.definitions,
                    x => x.brickUuid == uuid);
                var brick = BrickBuilder.InstansiateFromResources(def.resourcePath);

                if (brick != null)
                {
                    if (parent != null)
                        brick.transform.SetParent(parent);

                    brick.Deserialize(xmlReader);

                    //disable brick scripts inside group
                    if (group)
                        brick.grouped = true;
                }
            }
        }

        /// <summary>
        /// Deserializes ordinary bick.
        /// </summary>
        /// <param name="xmlReader">Xml reader.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="group">If set to <c>true</c> group.</param>
        void DeserializeOrdinary(XmlReader xmlReader, Transform parent, bool group)
        {
            var obj = new GameObject();
            var brick = obj.AddComponent<OrdinaryBrick>();
            var meshFilter = obj.AddComponent<MeshFilter>();
            var meshRenderer = obj.AddComponent<MeshRenderer>();
            meshRenderer.material = Preferences.instance.defaultBrickMaterial;
            var meshCollider = obj.AddComponent<MeshCollider>();

            if (parent != null)
                brick.transform.SetParent(parent);

            brick.Deserialize(xmlReader);

            //disable brick scripts inside group
            if (group)
                brick.grouped = true;
        }

        /// <summary>
        /// Deserializes AgaQ group.
        /// </summary>
        /// <param name="xmlReader">Xml reader.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="group">If set to <c>true</c> group.</param>
        void DeserializeAgaQGroup(XmlReader xmlReader, Transform parent, bool group)
        {
            AgaQGroup groupScript = BrickBuilder.InstansiateAgaQGroup();
            groupScript.Deserialize(xmlReader);

            if (parent != null)
                groupScript.transform.SetParent(parent);

            var subReader = xmlReader.ReadSubtree();
            subReader.Skip();
            Deserialize(subReader, groupScript.gameObject.transform, true);

            xmlReader.Skip();
        }

        void DeserializeBricksGroup(XmlReader xmlReader, Transform parent, bool group)
        {
            BricksGroup groupScript = BrickBuilder.InstansiateBricksGroup();
            groupScript.Deserialize(xmlReader);

            if (parent != null)
                groupScript.transform.SetParent(parent);

            var subReader = xmlReader.ReadSubtree();
            subReader.Skip();
            Deserialize(subReader, groupScript.gameObject.transform, true);

            xmlReader.Skip();
        }
    }
}
