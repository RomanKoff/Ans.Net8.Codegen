using Ans.Net8.Common;
using Ans.Net8.Common.Crud;
using System.Xml.Serialization;

namespace Ans.Net8.Codegen.Schema
{

	public class PropertyXmlElement
		: _FaceXmlElement_Base,
		ICrudFace
	{
		[XmlAttribute("type")]
		public CrudFieldTypeEnum Type { get; set; }

		[XmlAttribute("mode")]
		public CrudFieldModeEnum Mode { get; set; } = CrudFieldModeEnum.Normal;

		[XmlAttribute("hide")]
		public string Hide { get; set; }

		[XmlAttribute("readonly")]
		public string Readonly { get; set; }

		[XmlAttribute("nullable")]
		public bool IsNullable { get; set; }

		[XmlAttribute("length-min")]
		public int LengthMin { get; set; }

		[XmlAttribute("length-max")]
		public int LengthMax { get; set; }

		[XmlAttribute("regex")]
		public string RegexTemplate { get; set; }
				
		[XmlAttribute("enum-data")]
		public string EnumData { get; set; }

		[XmlAttribute("func-sql")]
		public string FuncSql { get; set; }

		[XmlAttribute("func-init-add")]
		public string FuncInitAdd { get; set; }

		[XmlAttribute("func-add")]
		public string FuncAdd { get; set; }

		[XmlAttribute("func-edit")]
		public string FuncEdit { get; set; }

		[XmlAttribute("func-fix")]
		public string FuncFix { get; set; }

		[XmlAttribute("func-decode-edit")]
		public string FuncDecodeBeforeEdit { get; set; }

		[XmlAttribute("func-decode-view")]
		public string FuncDecodeBeforeView { get; set; }

		[XmlAttribute("func-encode-save")]
		public string FuncEncodeBeforeSave { get; set; }

		[XmlAttribute("ctrl")]
		public string ControlDefault { get; set; }

		[XmlAttribute("ctrl-cell")]
		public string ControlCell { get; set; }

		[XmlAttribute("ctrl-view")]
		public string ControlView { get; set; }

		[XmlAttribute("ctrl-edit")]
		public string ControlEdit { get; set; }

		[XmlAttribute("ctrl-reg")]
		public string ControlRegistry { get; set; }

		[XmlAttribute("ctrl-cell-css")]
		public string ControlCellCss { get; set; }

		[XmlAttribute("ctrl-view-css")]
		public string ControlViewCss { get; set; }

		[XmlAttribute("ctrl-edit-css")]
		public string ControlEditCss { get; set; }

		[XmlAttribute("ctrl-text-mw")]
		public int ControlTextMaxWidth { get; set; }

		[XmlAttribute("rem")]
		public string Remark { get; set; }
	}

}