using Ans.Net8.Common;
using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	public class _FaceXmlElement_Base
		: ICrudFace
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("title")]
		public string Title { get; set; }

		[XmlAttribute("short")]
		public string ShortTitle { get; set; }

		[XmlAttribute("description")]
		public string Description { get; set; }

		[XmlAttribute("sample")]
		public string Sample { get; set; }

		[XmlAttribute("link")]
		public string HelpLink { get; set; }
	}

}