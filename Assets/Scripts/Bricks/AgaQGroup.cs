using System.Globalization;
using System.Xml;

namespace AgaQ.Bricks
{
    public class AgaQGroup : AgaQBrick
    {
        #region Serialization

        public override void Serialize(XmlDocument doc, XmlElement parentElement)
        {
            XmlElement element = doc.CreateElement("AgaQGroup");

            if (element == null)
                return;

            element.SetAttribute("position",
                                 string.Format(CultureInfo.InvariantCulture, "{0};{1};{2}",
                                               transform.position.x,
                                               transform.position.y,
                                               transform.position.z));
            element.SetAttribute("rotation",
                                 string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3}",
                                               transform.rotation.x,
                                               transform.rotation.y,
                                               transform.rotation.z,
                                               transform.rotation.w));
            parentElement.AppendChild(element);

            for (int i = 0; i < transform.childCount; i++)
            {
                var childObject = transform.GetChild(i);
                if (!childObject.gameObject.activeSelf)
                    continue;

                var brick = childObject.GetComponent<Brick>();
                if (brick == null)
                    continue;

                brick.Serialize(doc, element);
            }
        }

        public override void Deserialize(XmlReader reader)
        {
            transform.position = getVector3(reader.GetAttribute("position"));
            transform.rotation = getQuaternion(reader.GetAttribute("rotation"));
        }

        #endregion
    }
}
