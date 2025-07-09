using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	public class ManyrefXmlElement
		: _EntityXmlElement_Base,
		ICrudEntity
	{
		[XmlAttribute("target")]
		public string Target { get; set; }

		[XmlAttribute("ext")]
		public bool IsExtended { get; set; }
	}

}