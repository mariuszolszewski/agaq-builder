using UnityEngine;
using AgaQ.Bricks.DimensionsGroups;
using AgaQ.Bricks.Positioners;
using System.Xml;
using System.Globalization;

namespace AgaQ.Bricks
{
    /// <summary>
    /// AgaQ brick, can be joint with other AgaQ bricks.
    /// </summary>
    public class AgaQBrick : DragableBrick
    {
        public DimensionGroup dimensionGroup;
        public string[] dimensionParams;

        /// <summary>
        /// Cached brick joints.
        /// </summary>
        Joints.Joint[] _joints;

        /// <summary>
        /// Get all brick joints.
        /// </summary>
        /// <value>The joints.</value>
        public Joints.Joint[] joints
        {
            get
            {
                if (_joints == null)
                    _joints = GetComponentsInChildren<Joints.Joint>();

                return _joints;
            }
        }

        /// <summary>
        /// Clear brick joints.
        /// </summary>
        public void ClearJoints()
        {
            foreach (var joint in joints)
                joint.ClearJoint();
        }

        /// <summary>
        /// Rebuild information about connections for all joints in brick.
        /// </summary>
        public void RebuildJoints()
        {
            var model = GameObject.Find("Model");
            if (model == null)
                return;

            //clear brick joints
            var brickJoints = joints;
            foreach (var brickJoint in brickJoints)
                brickJoint.ClearJoint();

            //rebuilt all joints
            var otherJoints = model.GetComponentsInChildren<Joints.Joint>();
            int lastJoined = 0; //last processed joiunt of our brick
            foreach (var othertJoint in otherJoints)
            {
                for (int i = lastJoined; i < brickJoints.Length; i++)
                {
                    //compare joints position
                    if (othertJoint != brickJoints[i] &&
                        Joints.Joint.AreJoinable(othertJoint, brickJoints[i]) &&
                       (brickJoints[i].transform.position - othertJoint.transform.position).magnitude <= JoinablePositioner.positionAccuracy &&
                       Quaternion.Angle(joints[i].transform.rotation, joints[i].transform.rotation) <= JoinablePositioner.rotationAccuracy)
                    {
                        joints[i].SetJoint(joints[i]);
                        lastJoined++;
                        break;
                    }
                }

                if (lastJoined >= brickJoints.Length)
                    break;
            }
        }

#region Serialization

        public override void Serialize(XmlDocument doc, XmlElement parentElement)
        {
            XmlElement element = doc.CreateElement("AgaQBrick");

            if (element == null)
                return;

            element.SetAttribute("uuid", uuid.ToString());
            element.SetAttribute("color", ColorUtility.ToHtmlStringRGBA(color));
            element.SetAttribute("scale", scale.ToString());
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
        }

        public override void Deserialize(XmlReader reader)
        {
            Color c = new Color();
            ColorUtility.TryParseHtmlString(string.Concat("#", reader.GetAttribute("color")), out c);
            color = c;
            scale = float.Parse(reader.GetAttribute("scale"));
            transform.position = getVector3(reader.GetAttribute("position"));
            transform.rotation = getQuaternion(reader.GetAttribute("rotation"));
        }

#endregion
    }
}
