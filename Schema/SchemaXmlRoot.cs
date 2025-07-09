using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	[XmlRoot("schema")]
	public class SchemaXmlRoot
	{
		[XmlElement("face")]
		public List<FaceXmlElement> Faces { get; set; }

		[XmlElement("enum")]
		public List<EnumXmlElement> Enums { get; set; }

		[XmlElement("catalog")]
		public List<CatalogXmlElement> Catalogs { get; set; }
	}

}
