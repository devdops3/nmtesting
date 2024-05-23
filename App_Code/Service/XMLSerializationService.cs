using System.IO;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// Summary description for XMLSerializationService
/// </summary>
public class XMLSerializationService<T>
{
	public XMLSerializationService()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string SerializeData(T data)
    {

        // Remove Declaration
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        // Remove Namespace
        var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });


        using (var stream = new StringWriter())
        using (var writer = XmlWriter.Create(stream, settings))
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, data, ns);
            return stream.ToString();
        }

    }
}