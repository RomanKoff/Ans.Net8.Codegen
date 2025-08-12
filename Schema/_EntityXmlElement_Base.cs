using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	public interface ICrudEntity
	{
		List<PropertyXmlElement> Properties { get; set; }
		List<ExtentionXmlElement> Extentions { get; set; }
		bool AddUseinfo { get; set; }
		bool IsHidden { get; set; }
		string Headers { get; set; }
		string Description { get; set; }
		string DefaultSorting { get; set; }
		bool IsReadonly { get; set; }
		string RegistrySorting { get; set; }
		string CustomListViewName { get; set; }
		string CustomAddViewName { get; set; }
		string CustomEditViewName { get; set; }
		string CustomDeleteViewName { get; set; }
		string FuncTitle { get; set; }
		string FuncViewTitle { get; set; }
		string Interface { get; set; }
		string Remark { get; set; }
	}



	public class _EntityXmlElement_Base
		: ICrudEntity
	{
		[XmlElement("property")]
		public List<PropertyXmlElement> Properties { get; set; }

		[XmlElement("extention")]
		public List<ExtentionXmlElement> Extentions { get; set; }

		[XmlAttribute("useinfo")]
		public bool AddUseinfo { get; set; }

		[XmlAttribute("hidden")]
		public bool IsHidden { get; set; }

		[XmlAttribute("headers")]
		public string Headers { get; set; }

		[XmlAttribute("description")]
		public string Description { get; set; }

		[XmlAttribute("sorting")]
		public string DefaultSorting { get; set; }

		[XmlAttribute("readonly")]
		public bool IsReadonly { get; set; }

		[XmlAttribute("reg-sorting")]
		public string RegistrySorting { get; set; }

		[XmlAttribute("custom-view-list")]
		public string CustomListViewName { get; set; }

		[XmlAttribute("custom-view-add")]
		public string CustomAddViewName { get; set; }

		[XmlAttribute("custom-view-edit")]
		public string CustomEditViewName { get; set; }

		[XmlAttribute("custom-view-delete")]
		public string CustomDeleteViewName { get; set; }

		[XmlAttribute("func-title")]
		public string FuncTitle { get; set; }

		[XmlAttribute("func-view-title")]
		public string FuncViewTitle { get; set; }

		[XmlAttribute("interface")]
		public string Interface { get; set; }

		[XmlAttribute("rem")]
		public string Remark { get; set; }
	}

}