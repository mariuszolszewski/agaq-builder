using System.Xml;

namespace AgaQ.Bricks
{
    /// <summary>
    /// AgaQ bricks temporary grouped.
    /// </summary>
    public class AgaQTemporaryGroup : AgaQBrick
    {
        public override void Serialize(XmlDocument doc, XmlElement parentElement)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var childObject = transform.GetChild(i);
                if (!childObject.gameObject.activeSelf)
                    continue;

                var brick = childObject.GetComponent<Brick>();
                if (brick == null)
                    continue;

                brick.Serialize(doc, parentElement);
            }
        }
    }
}
    