using Ans.Net8.Codegen.Schema;
using Ans.Net8.Common;
using Ans.Net8.Common.Crud;

namespace Ans.Net8.Codegen.Items
{

	public class TableItem
	{

		private readonly string _funcTitle;


		/* ctor */


		protected TableItem(
			CatalogItem catalog,
			TableItem master,
			ICrudEntity source,
			string name,
			int level)
		{
			Catalog = catalog;
			Name = name;
			AddUseinfo = source.AddUseinfo;
			if (!string.IsNullOrEmpty(source.Headers))
			{
				var a1 = new StringParser(source.Headers);
				var legacy1 = (master != null).Make($" {master?.HeaderWhoWhat}");
				HeaderSingular = $"{a1.Get(0, Name)}{legacy1}";
				HeaderPluralize = $"{a1.Get(1, NamePluralize)}{legacy1}";
				HeaderWhoWhat = $"{a1.Get(2, Name.ToLower())}{legacy1}";
			}
			IsHidden = source.IsHidden;
			Description = source.Description;
			CustomListViewName = source.CustomListViewName;
			CustomAddViewName = source.CustomAddViewName;
			CustomEditViewName = source.CustomEditViewName;
			CustomDeleteViewName = source.CustomDeleteViewName;
			DefaultSorting = source.DefaultSorting;
			IsReadonly = source.IsReadonly;
			var s1 = source.RegistrySorting ?? DefaultSorting;
			RegistrySorting = string.IsNullOrEmpty(s1)
				? "null"
				: $"\"{s1}\"";

			_funcTitle = source.FuncTitle;

			AdditionalInterface = source.Interface;
			Remark = source.Remark;
			Level = level;

			if (master != null)
			{
				Master = master;
				master.Slaves.Add(this);
				Fields.Add(new FieldItem(
					new PropertyXmlElement
					{
						Name = "Master",
						Type = CrudFieldTypeEnum.Reference,
						Mode = CrudFieldModeEnum.Required,
						//Access = CrudFieldAccessEnum.Constant,
						Hide = "laed",
						Readonly = "ae",
						IsNullable = false,
						Remark = $"^{master.Name}",
					})
				{
					ReferenceTarget = master.Name,
					IsSystem = true,
				});
			}

			foreach (var group1 in source.Extentions.GroupBy(x => x.Target))
			{
				var ext1 = new ExtentionItemDict();
				foreach (var item2 in group1)
					ext1.Add(item2.Key, item2.Value.Trim());
				Extentions.Add(group1.Key, ext1);
			}
		}


		/// <summary>
		/// Добавление стандартной таблицы
		/// </summary>
		public TableItem(
			CatalogItem catalog,
			TableItem master,
			EntityXmlElement source,
			int level)
			: this(
				  catalog, master, source,
				  $"{master?.Name ?? catalog.Name}{source.Name}", level)
		{
			IsTree = source.Type == CrudEntityTypeEnum.Tree;
			IsOrdered = source.Type != CrudEntityTypeEnum.Normal;
			NotAdd = source.Prohibits?.Contains('a') ?? false;
			NotEdit = source.Prohibits?.Contains('e') ?? false;
			NotDelete = source.Prohibits?.Contains('d') ?? false;
			AfterAdd = source.AfterAdd;

			if (IsTree)
			{
				Fields.Add(new FieldItem(
					new PropertyXmlElement
					{
						Name = "Parent",
						Type = CrudFieldTypeEnum.Reference,
						IsNullable = true,
						Remark = $"^{Name}",
						ControlEdit = "_SelectXXX",
					})
				{
					ReferenceTarget = Name,
					IsSystem = true,
				});
			}

			if (IsOrdered)
			{
				Fields.Add(new FieldItem(
					new PropertyXmlElement
					{
						Name = "Order",
						Type = CrudFieldTypeEnum.Int,
					})
				{
					IsSystem = true,
				});
				DefaultSorting = source.DefaultSorting ?? "Order";
			}

			_fieldsAdd(source.Properties);
		}


		/// <summary>
		/// Добавление служебной таблицы связи многие-ко-многим
		/// </summary>
		public TableItem(
			CatalogItem catalog,
			TableItem master,
			ManyrefXmlElement source,
			int level)
			: this(
				  catalog, master, source,
				  $"{master.Name}To{source.Target}Ref", level)
		{
			if (!(source.Properties?.Count > 0))
			{
				NotEdit = true;
				IsSimpleManyref = true;
			}

			IsExtendedManyref = source.IsExtended;

			ManyrefField = new FieldItem(
				new PropertyXmlElement
				{
					Name = source.Target,
					Type = CrudFieldTypeEnum.Reference,
					Mode = CrudFieldModeEnum.Required,
					//Access = CrudFieldAccessEnum.Constant,
					Hide = "laed",
					Readonly = "ae",
					IsNullable = false,
					Remark = $"^{source.Target}"
				})
			{
				ReferenceTarget = source.Target,
			};
			Fields.Add(ManyrefField);

			_fieldsAdd(source.Properties);
		}


		/* readonly properties */


		public CatalogItem Catalog { get; }
		public string Name { get; }
		public bool IsTree { get; }
		public bool IsOrdered { get; }
		public bool AddUseinfo { get; }
		public bool IsHidden { get; set; }
		public string DefaultSorting { get; }
		public bool IsReadonly { get; }
		public string RegistrySorting { get; }
		public bool NotAdd { get; set; }
		public bool NotEdit { get; set; }
		public bool NotDelete { get; set; }
		public bool IsSimpleManyref { get; set; }
		public bool IsExtendedManyref { get; set; }
		public string Description { get; }
		public string CustomListViewName { get; set; }
		public string CustomAddViewName { get; set; }
		public string CustomEditViewName { get; set; }
		public string CustomDeleteViewName { get; set; }
		public CrudEntityAfterAddEnum AfterAdd { get; }
		public string AdditionalInterface { get; }
		public string Remark { get; }

		public int Level { get; }


		private string _namePluralize;
		public string NamePluralize
			=> _namePluralize ??= SuppLangEn.GetPluralizeEn(Name);

		public TableItem Master { get; }
		public List<TableItem> Slaves { get; } = [];

		public IEnumerable<TableItem> SlaveManyrefs
			=> Slaves.Where(x => x.IsManyref);

		public IEnumerable<TableItem> SlaveSimpleManyrefs
			=> SlaveManyrefs.Where(x => x.IsSimpleManyref && !x.IsExtendedManyref);

		public IEnumerable<TableItem> SlaveAdvanceds
			=> Slaves.Where(x => !x.IsSimpleManyref);

		public ExtentionsDict Extentions { get; } = [];

		public List<FieldItem> Fields { get; } = [];
		public List<ReferenceItem> ReferencesTo { get; } = [];
		public List<ReferenceItem> ReferencesFrom { get; } = [];
		public FieldItem ManyrefField { get; }


		public bool HasMaster
			=> Master != null;

		public bool HasSlaves
			=> Slaves?.Count > 0;

		public bool HasSlaveManyrefs
			=> SlaveManyrefs?.Count() > 0;

		public bool HasSlaveSimpleManyrefs
			=> SlaveSimpleManyrefs?.Count() > 0;

		public bool HasSlaveAdvanceds
			=> SlaveAdvanceds?.Count() > 0;

		public bool IsManyref
			=> ManyrefField != null;

		public bool HasReferencesTo
			=> ReferencesTo?.Count > 0;

		public bool HasReferencesFrom
			=> ReferencesFrom?.Count > 0;

		public bool HasNavigation
			=> HasMaster || HasSlaves || HasReferencesTo || HasReferencesFrom;


		public string EntityPrefixString
			=> $"{(HasMaster ? "Slave" : "Master")}";

		public string InterfaceName
			=> $"I{Name}";

		public string BaseInterfaceName
			=> $"I{EntityPrefixString}Entity";

		public string Interfaces
			=> $"{InterfaceName}, {BaseInterfaceName}{IsTree.Make(", ITreeItem")}{AdditionalInterface.Make(", {0}")}";

		public string BaseClassName
			=> $"_{Name}_Base";

		public string FuncTitle
			=> _funcTitle ??
				(Fields.Any(x => x.Name == "Title")
					? "x.Title"
					: Fields.Any(x => x.Name == "Name")
						? "x.Name"
						: "x.Id.ToString()");


		/* properties */


		public string HeaderSingular { get; set; }
		public string HeaderPluralize { get; set; }
		public string HeaderWhoWhat { get; set; }


		/* fields */


		public IEnumerable<FieldItem> ConstraintFields
			=> Fields.Where(x => x.IsUnique);

		public IEnumerable<FieldItem> RequiredFields
			=> Fields.Where(x => x.IsRequired);

		public IEnumerable<FieldItem> HideOnAddFields
			=> Fields.Where(x => x.HideOnAdd && x.IsRequired);

		public IEnumerable<FieldItem> HideOnEditFields
			=> Fields.Where(x => x.HideOnEdit);

		public IEnumerable<FieldItem> FuncSqlFields
			=> Fields.Where(x => !string.IsNullOrEmpty(x.FuncSql));

		public IEnumerable<FieldItem> FuncInitAddFields
			=> Fields.Where(x => !string.IsNullOrEmpty(x.FuncInitAdd)
				|| !string.IsNullOrEmpty(x.FuncAdd));

		public IEnumerable<FieldItem> FuncAddFields
			=> Fields.Where(x => !string.IsNullOrEmpty(x.FuncAdd));

		public IEnumerable<FieldItem> FuncEditFields
			=> Fields.Where(x => !string.IsNullOrEmpty(x.FuncEdit));

		public IEnumerable<FieldItem> FuncDecodeBeforeEditFields
			=> Fields.Where(x => x.FuncDecodeBeforeEdit != null);

		public IEnumerable<FieldItem> FuncDecodeBeforeViewFields
			=> Fields.Where(x => x.FuncDecodeBeforeView != null);

		public IEnumerable<FieldItem> FuncEncodeBeforeSaveFields
			=> Fields.Where(x => x.FuncEncodeBeforeSave != null);

		public IEnumerable<FieldItem> EnumFields
			=> Fields.Where(x => x.Type == CrudFieldTypeEnum.Enum);

		public IEnumerable<FieldItem> RegistryFields
			=> Fields.Where(x => x.IsRegistry);

		public IEnumerable<FieldItem> ViewListFields
			=> Fields.Where(x => !(x.IsSystem || x.HideOnList));

		public IEnumerable<FieldItem> ViewAddFields
			=> Fields.Where(
				x => !(x.IsSystem || x.HideOnAdd));

		public IEnumerable<FieldItem> ViewEditFields
			=> Fields.Where(
				x => !(x.IsSystem || x.HideOnEdit));

		public IEnumerable<FieldItem> ViewDetailsFields
			=> Fields.Where(
				x => !(x.IsSystem || x.HideOnDetails));

		public string ListFieldsString
			=> ViewListFields
				.Select(x => x.Name)
				.MakeFromCollection(null, null, ";");


		/* privates */


		private void _fieldsAdd(
			IEnumerable<PropertyXmlElement> properties)
		{
			foreach (var item1 in properties)
				Fields.Add(new FieldItem(item1));
			if (AddUseinfo)
			{
				Fields.Add(new FieldItem(
					new PropertyXmlElement
					{
						Name = "DateCreate",
						Type = CrudFieldTypeEnum.DateTime,
						Hide = "laed",
						Readonly = "ae",
						FuncAdd = "DateTime.Now",
					})
				{
					IsSystem = true,
				});
				Fields.Add(new FieldItem(
					new PropertyXmlElement
					{
						Name = "CreateUser",
						Type = CrudFieldTypeEnum.Text50,
						Hide = "laed",
						Readonly = "ae",
						FuncAdd = "this.User.Identity.Name",
						ControlDefault = "EnumString",
						ControlRegistry = "RegUsers",
					})
				{
					IsSystem = true,
				});
				Fields.Add(new FieldItem(
					new PropertyXmlElement
					{
						Name = "DateUpdate",
						Type = CrudFieldTypeEnum.DateTime,
						Hide = "laed",
						Readonly = "ae",
						FuncAdd = "DateTime.Now",
						FuncEdit = "DateTime.Now",
					})
				{
					IsSystem = true,
				});
				Fields.Add(new FieldItem(
					new PropertyXmlElement
					{
						Name = "UpdateUser",
						Type = CrudFieldTypeEnum.Text50,
						Hide = "laed",
						Readonly = "ae",
						FuncEdit = "this.User.Identity.Name",
						ControlDefault = "EnumString",
						ControlRegistry = "RegUsers",
					})
				{
					IsSystem = true,
				});
			}
		}

	}

}