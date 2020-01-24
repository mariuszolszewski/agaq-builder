using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace AgaQ.Bricks.Serialization
{
    public class ModelSerializer : MonoBehaviour
    {
        public void Serialize(string fileName)
        {
            using (var file = File.Create(fileName))
            using (var writer = new GZipStream(file, CompressionMode.Compress))
            {
                var doc = new XmlDocument();

                //root element
                var root = doc.CreateElement("model");
                doc.AppendChild(root);

                //search for all elements (bricks, groups etc.) at first level
                //serialize each of the element
                for (int i = 0; i < transform.childCount; i++)
                {
                    var childObject = transform.GetChild(i);
                    if (!childObject.gameObject.activeSelf)
                        continue;
            
                    var brick = childObject.GetComponent<Brick>();
                    if (brick == null)
                        continue;

                    brick.Serialize(doc, doc.DocumentElement);
                }

                doc.Save(writer);

                writer.Close();
            }
        }

        public void Serialize(string fileName, Brick brick)
        {
            using (var file = File.Create(fileName))
            using (var writer = new GZipStream(file, CompressionMode.Compress))
            {
                var doc = new XmlDocument();

                //root element
                var root = doc.CreateElement("model");
                doc.AppendChild(root);

                if (brick != null)
                    brick.Serialize(doc, doc.DocumentElement);

                doc.Save(writer);

                writer.Close();
            }
        }
    }
}
