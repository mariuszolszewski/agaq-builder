using UnityEngine;
using System.Xml;
using System.Text;
using System.Globalization;

namespace AgaQ.Bricks
{
    public class OrdinaryBrick : SelectableBrick
    {
        #region Serialization

        public override void Serialize(XmlDocument doc, XmlElement parentElement)
        {
            XmlElement element = doc.CreateElement("OrdinaryBrick");

            if (element == null)
                return;

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

            var meshFilters = GetComponentsInChildren<MeshFilter>();
            if (meshFilters != null && meshFilters.Length > 0)
            {
                Mesh[] meshes = new Mesh[meshFilters.Length];
                for (int i = 0; i < meshes.Length; i++)
                    meshes[i] = meshFilters[i].mesh;
                
                SerializeGeometry(doc, element, meshes);
            }
        }

        /// <summary>
        /// Serialize all given meshes.
        /// </summary>
        /// <param name="doc">Document.</param>
        /// <param name="parentElement">Parent element.</param>
        /// <param name="meshes">Meshes.</param>
        void SerializeGeometry(XmlDocument doc, XmlElement parentElement, Mesh[] meshes)
        {
            foreach (var mesh in meshes)
            {
                XmlElement element = doc.CreateElement("Model");

                if (element == null)
                    return;

                SerializeVertices(doc, element, mesh);
                SerializeTris(doc, element, mesh);
                SerializeNormals(doc, element, mesh);

                parentElement.AppendChild(element);
            }
        }

        /// <summary>
        /// Serialize vetices from given mesh
        /// </summary>
        /// <param name="doc">Document.</param>
        /// <param name="parentElement">Parent element.</param>
        /// <param name="mesh">Mesh.</param>
        void SerializeVertices(XmlDocument doc, XmlElement parentElement, Mesh mesh)
        {
            XmlElement element = doc.CreateElement("Vertices");

            if (element == null)
                return;

            StringBuilder value = new StringBuilder();
            foreach (var vertis in mesh.vertices)
                value.Append(string.Format("{0};{1};{2}:", vertis.x, vertis.y, vertis.z));            

            element.SetAttribute("value", value.ToString().TrimEnd(new char[] { ':' }));
            parentElement.AppendChild(element);
        }

        /// <summary>
        /// Serialze trinagles from given mesh
        /// </summary>
        /// <param name="doc">Document.</param>
        /// <param name="parentElement">Parent element.</param>
        /// <param name="mesh">Mesh.</param>
        void SerializeTris(XmlDocument doc, XmlElement parentElement, Mesh mesh)
        {
            XmlElement element = doc.CreateElement("Tris");

            if (element == null)
                return;

            StringBuilder value = new StringBuilder();
            foreach (var triangle in mesh.triangles)
            {
                value.Append(triangle.ToString());
                value.Append(';');
            }

            element.SetAttribute("value", value.ToString().TrimEnd(new char[] { ';' }));
            parentElement.AppendChild(element);
        }

        /// <summary>
        /// Serialze normals form given mesh
        /// </summary>
        /// <param name="doc">Document.</param>
        /// <param name="parentElement">Parent element.</param>
        /// <param name="mesh">Mesh.</param>
        void SerializeNormals(XmlDocument doc, XmlElement parentElement, Mesh mesh)
        {
            XmlElement element = doc.CreateElement("Normals");

            if (element == null)
                return;

            StringBuilder value = new StringBuilder();
            foreach (var normal in mesh.normals)
                value.Append(string.Format("{0};{1};{2}:", normal.x, normal.y, normal.z));

            element.SetAttribute("value", value.ToString().TrimEnd(new char[] { ':' }));
            parentElement.AppendChild(element);
        }

        #endregion

        #region Deserialization

        public override void Deserialize(XmlReader reader)
        {
            Color c = new Color();
            ColorUtility.TryParseHtmlString(string.Concat("#", reader.GetAttribute("color")), out c);
            color = c;
            scale = float.Parse(reader.GetAttribute("scale"));
            transform.position = getVector3(reader.GetAttribute("position"));
            transform.rotation = getQuaternion(reader.GetAttribute("rotation"));

            var subReader = reader.ReadSubtree();
            subReader.Skip();
            DeserializeModel(subReader);
        }

        void DeserializeModel(XmlReader reader)
        {
            while (reader.Read())
            {
                //get mesh filter, when there is no one add it
                var meshFilter = GetComponent<MeshFilter>();
                if (meshFilter == null)
                    meshFilter = gameObject.AddComponent<MeshFilter>();
                var meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                    meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.material = Preferences.instance.defaultBrickMaterial;

                //set collider
                var meshCollider = GetComponent<MeshCollider>();
                if (meshCollider == null)
                    meshCollider = gameObject.AddComponent<MeshCollider>();

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Model")
                {
                    var subReader = reader.ReadSubtree();
                    subReader.Skip();
                    meshFilter.mesh = DeserializeGeometry(subReader);
                    meshCollider.sharedMesh = meshFilter.mesh;
                }
            }

            reader.Close();
        }

        /// <summary>
        /// Deserializes model mesh.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="mesh">Mesh.</param>
        Mesh DeserializeGeometry(XmlReader reader)
        {
            Vector3[] vertises = null;
            int[] triangles = null;
            Vector3[] normals = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "Vertices")
                        vertises = DeserializeVertices(reader);
                    else if (reader.Name == "Tris")
                        triangles = DeserializeTrises(reader);
                    else if (reader.Name == "Normals")
                        normals = DeserializeNormals(reader);
                }
            }
            reader.Close();

            Mesh mesh = new Mesh();
            mesh.vertices = vertises;
            mesh.triangles = triangles;
            mesh.normals = normals;

            return mesh;
        }

        /// <summary>
        /// Deserializes model vertises.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="mesh">Mesh.</param>
        Vector3[] DeserializeVertices(XmlReader reader)
        {
            var value = reader.GetAttribute("value");
            var verticesTexts = value.Split(':');

            Vector3[] vertices = new Vector3[verticesTexts.Length];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = getVector3(verticesTexts[i]);

            return vertices;
        }

        /// <summary>
        /// Deserializes model triangles.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="mesh">Mesh.</param>
        int[] DeserializeTrises(XmlReader reader)
        {
            var value = reader.GetAttribute("value");
            var trisesTexts = value.Split(';');

            int[] triangles = new int[trisesTexts.Length];
            for (int i = 0; i < trisesTexts.Length; i++)
                triangles[i] = int.Parse(trisesTexts[i]);

            return triangles;
        }

        /// <summary>
        /// Deserializes model normals.
        /// </summary>
        /// <param name="reader">Reader.</param>
        /// <param name="mesh">Mesh.</param>
        Vector3[] DeserializeNormals(XmlReader reader)
        {
            var value = reader.GetAttribute("value");
            var normalsTexts = value.Split(':');

            Vector3[] normals = new Vector3[normalsTexts.Length];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = getVector3(normalsTexts[i]);

            return normals;
        }

        #endregion
    }
}
