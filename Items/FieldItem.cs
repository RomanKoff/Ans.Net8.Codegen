using Ans.Net8.Codegen.Schema;
using Ans.Net8.Common;
using Ans.Net8.Common.Crud;

namespace Ans.Net8.Codegen.Items
{

	public class FieldItem
		: CrudFaceHelper,
		ICrudFace
	{

		/* ctor */


		public FieldItem(
			PropertyXmlElement source)
			: base(
				  source.Name,
				  source.Title,
				  source.ShortTitle,
				  source.Description,
				  source.Sample,
				  source.HelpLink)
		{
			Type = source.Type;
			Mode = source.Mode;
			HasShowSlaves = !string.IsNullOrEmpty(source.ShowSlaves);
			if (HasShowSlaves)
			{
				var a1 = source.ShowSlaves.Split(':');
				ShowSlavesTable = SuppLangEn.GetPluralizeEn(a1[0]);
				ShowSlavesField = a1[1];
			}
			if (source.Hide != null)
			{
				HideOnList = source.Hide.Contains('l') || HideOnList;
				HideOnAdd = source.Hide.Contains('a') || HideOnAdd;
				HideOnEdit = source.Hide.Contains('e') || HideOnEdit;
				HideOnDetails = source.Hide.Contains('d') || HideOnDetails;
			}
			if (source.Readonly != null)
			{
				ReadonlyOnAdd = source.Readonly.Contains('a') || ReadonlyOnAdd;
				ReadonlyOnEdit = source.Readonly.Contains('e') || ReadonlyOnEdit;
			}
			IsNullable = source.IsNullable || IsNullable;
			LengthMin = SuppValues.Detault(LengthMin, source.LengthMin);
			LengthMax = SuppValues.Detault(LengthMax, source.LengthMax);
			RegexTemplate = SuppValues.Detault(RegexTemplate, source.RegexTemplate);
			EnumData = SuppValues.Detault(EnumData, source.EnumData);

			FuncSql = SuppValues.Detault(
				FuncSql, source.FuncSql);

			FuncInitAdd = SuppValues.Detault(
				FuncInitAdd, source.FuncInitAdd ?? source.FuncAdd);
			FuncAdd = SuppValues.Detault(
				FuncAdd, source.FuncAdd);

			FuncEdit = SuppValues.Detault(
				FuncEdit, source.FuncEdit);

			FuncFix = SuppValues.Detault(
				FuncFix, source.FuncFix);
			FuncDecodeBeforeEdit = SuppValues.Detault(
				FuncDecodeBeforeEdit, source.FuncDecodeBeforeEdit);
			FuncDecodeBeforeView = SuppValues.Detault(
				FuncDecodeBeforeView, source.FuncDecodeBeforeView);
			FuncEncodeBeforeSave = SuppValues.Detault(
				FuncEncodeBeforeSave, source.FuncEncodeBeforeSave);

			ControlDefault = source.ControlDefault ?? Type.ToString();
			ControlCell = source.ControlCell ?? ControlDefault;
			ControlView = source.ControlView ?? ControlDefault;
			ControlEdit = source.ControlEdit ?? ControlDefault;

			ControlRegistry = source.ControlRegistry ?? IsRegistry.Make($"Reg{Name}");

			ControlCellCss = source.ControlCellCss;
			ControlViewCss = source.ControlViewCss;
			ControlEditCss = source.ControlEditCss;

			ControlTextMaxWidth = source.ControlTextMaxWidth;

			Remark = SuppValues.Detault(Remark, source.Remark);
		}


		/* properties */


		private CrudFieldTypeEnum _type;
		public CrudFieldTypeEnum Type
		{
			get => _type;
			set
			{
				_type = value;
				switch (value)
				{
					case CrudFieldTypeEnum.Text50:
						CSharpType = typeof(string);
						LengthMax = 50;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Text100:
						CSharpType = typeof(string);
						LengthMax = 100;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Text250:
						CSharpType = typeof(string);
						LengthMax = 250;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Text400:
						CSharpType = typeof(string);
						LengthMax = 400;
						break;
					case CrudFieldTypeEnum.Memo:
						CSharpType = typeof(string);
						break;
					case CrudFieldTypeEnum.Name:
						CSharpType = typeof(string);
						LengthMax = 50;
						RegexTemplate = _Consts.REGEX_NAME;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Varname:
						CSharpType = typeof(string);
						LengthMax = 50;
						RegexTemplate = _Consts.REGEX_VARNAME;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Email:
						CSharpType = typeof(string);
						LengthMax = 50;
						RegexTemplate = _Consts.REGEX_EMAIL;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Int:
						CSharpType = typeof(int);
						FuncSql = "0";
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Long:
						CSharpType = typeof(long);
						FuncSql = "0";
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Float:
						CSharpType = typeof(float);
						FuncSql = "0";
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Double:
						CSharpType = typeof(double);
						FuncSql = "0";
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Decimal:
						CSharpType = typeof(decimal);
						FuncSql = "0";
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.DateTime:
						CSharpType = typeof(DateTime);
						IsNullable = true;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.DateOnly:
						CSharpType = typeof(DateOnly);
						IsNullable = true;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.TimeOnly:
						CSharpType = typeof(TimeOnly);
						IsNullable = true;
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Bool:
						CSharpType = typeof(bool);
						FuncSql = "false";
						IsSortable = true;
						break;
					case CrudFieldTypeEnum.Enum:
						CSharpType = typeof(int);
						FuncSql = "0";
						IsSortable = true;
						IsEnum = true;
						IsRegistry = true;
						break;
					case CrudFieldTypeEnum.Set:
						CSharpType = typeof(string);
						IsRegistry = true;
						break;
					case CrudFieldTypeEnum.Reference:
						CSharpType = typeof(int);
						ReferenceTarget = Name;
						Name = $"{ReferenceTarget}Ptr";
						IsNullable = true;
						IsSortable = true;
						IsRegistry = true;
						break;
				}
			}
		}


		private CrudFieldModeEnum _mode;
		public CrudFieldModeEnum Mode
		{
			get => _mode;
			set
			{
				_mode = value;
				if (value != CrudFieldModeEnum.Normal)
				{
					IsRequired = true;
					IsNullable = false;
					if (value != CrudFieldModeEnum.Required)
					{
						IsUnique = true;
						IsAbsoluteUnique = value != CrudFieldModeEnum.Unique;
					}
				}
			}
		}


		public bool HideOnList { get; set; }
		public bool HideOnAdd { get; set; }
		public bool HideOnEdit { get; set; }
		public bool HideOnDetails { get; set; }
		public bool ReadonlyOnAdd { get; set; }
		public bool ReadonlyOnEdit { get; set; }
		public bool IsNullable { get; set; }
		public int LengthMin { get; set; }
		public int LengthMax { get; set; }
		public string RegexTemplate { get; set; }
		public string EnumData { get; set; }

		public string FuncSql { get; set; }

		public string FuncInitAdd { get; set; }
		public string FuncAdd { get; set; }
		public string FuncEdit { get; set; }

		public string FuncFix { get; set; }
		public string FuncDecodeBeforeEdit { get; set; }
		public string FuncDecodeBeforeView { get; set; }
		public string FuncEncodeBeforeSave { get; set; }

		public string Remark { get; set; }

		public string ReferenceTarget { get; set; }
		public TableItem ReferenceTable { get; set; }
		public bool IsSystem { get; set; }


		/* readonly properties */


		public Type CSharpType { get; private set; }
		public bool IsRequired { get; private set; }
		public bool IsUnique { get; private set; }
		public bool IsAbsoluteUnique { get; private set; }
		public bool IsSortable { get; private set; }
		public bool IsEnum { get; private set; }
		public bool IsRegistry { get; private set; }

		public bool HasShowSlaves { get; }
		public string ShowSlavesTable { get; }
		public string ShowSlavesField { get; }

		public string ControlDefault { get; }
		public string ControlCell { get; }
		public string ControlView { get; }
		public string ControlEdit { get; }

		public string ControlRegistry { get; }

		public string ControlCellCss { get; }
		public string ControlViewCss { get; }
		public string ControlEditCss { get; }

		public int ControlTextMaxWidth { get; }


		private string _cSharpTypeString;
		public string CSharpTypeString
		{
			get
			{
				if (_cSharpTypeString == null)
				{
					var s1 = CSharpType.GetCSharpTypeName();
					_cSharpTypeString = (IsNullable || s1.StartsWith("Date"))
						? $"{s1}?" : s1;
				}
				return _cSharpTypeString;
			}
		}


		private string _cSharpDeclareString;
		public string CSharpDeclareString
			=> _cSharpDeclareString ??= $"{CSharpTypeString} {Name} {{ get; set; }}{Remark.Make(" // {0}")}";


		/* functions */


		public IEnumerable<string> GetCSharpAttributes()
		{
			var items1 = new List<string>();
			if (IsRequired)
				items1.Add($"[AnsRequired]");
			if (LengthMin > 0)
				items1.Add($"[AnsLengthMin({LengthMin})]");
			if (LengthMax > 0)
				items1.Add($"[AnsLengthMax({LengthMax})]");
			if (!string.IsNullOrEmpty(RegexTemplate))
				items1.Add($"[AnsRegex(@\"{RegexTemplate}\")]");
			return items1;
		}

	}

}