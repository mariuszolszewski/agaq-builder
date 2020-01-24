using System.Xml;

namespace AgaQ.Bricks.Serialization
{
    public interface IBrickSerializer
    {
        void Serialize(XmlDocument doc, XmlElement parentElement);
        void Deserialize(XmlReader reader);
    }
}

