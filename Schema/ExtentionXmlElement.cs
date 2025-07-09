using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	public class ExtentionXmlElement
	{
		[XmlText]
		public string Value { get; set; }

		[XmlAttribute("target")]
		public string Target { get; set; }

		[XmlAttribute("key")]
		public string Key { get; set; }
	}

}