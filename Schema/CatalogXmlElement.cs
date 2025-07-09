using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	public class CatalogXmlElement
	{
		[XmlElement("entity")]
		public List<EntityXmlElement> Entities { get; set; }

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("title")]
		public string Title { get; set; }

		[XmlAttribute("rem")]
		public string Remark { get; set; }
	}

}