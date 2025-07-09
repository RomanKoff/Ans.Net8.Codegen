using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	public class EnumXmlElement
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("data")]
		public string Data { get; set; }
	}

}