using Ans.Net8.Common.Crud;
using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	public class EntityXmlElement
		: _EntityXmlElement_Base,
		ICrudEntity
	{
		[XmlElement("entity")]
		public List<EntityXmlElement> Entities { get; set; }

		[XmlElement("manyref")]
		public List<ManyrefXmlElement> Manyrefs { get; set; }

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("type")]
		public CrudEntityTypeEnum Type { get; set; } = CrudEntityTypeEnum.Normal;

		[XmlAttribute("prohibits")]
		public string Prohibits { get; set; }

		[XmlAttribute("after-add")]
		public CrudEntityAfterAddEnum AfterAdd { get; set; } = CrudEntityAfterAddEnum.List;
	}

}