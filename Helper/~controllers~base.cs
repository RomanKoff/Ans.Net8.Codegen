using Ans.Net8.Codegen.Items;
using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		public void Gen_Controllers_Base()
		{
			var path1 = $"{ProjectCommonPath}/Controllers";
			SuppIO.CreateDirectoryIfNotExists(path1);

			foreach (var item1 in Tables)
			{
				var filename2 = $"{path1}/_{_getControllerName(item1)}_Base.cs";
				SuppIO.FileWrite(filename2, TML_Controllers_Base(item1));
				_logFile(filename2);
			}

			Console.WriteLine();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Controllers_Base(
			TableItem table)
		{
			var sb1 = new StringBuilder(_getAttention_CSharp());
			sb1.Append($@"
using Ans.Net8.Common;
using Ans.Net8.Common.Crud;
using Ans.Net8.Web;
using Ans.Net8.Web.Crud;
using {ProjectCommonNamespace};
using {ProjectCommonNamespace}.Entities;
using {ProjectCommonNamespace}.Repositories;

namespace {ProjectCommonNamespace}.Controllers
{{

	public partial class _{_getControllerName(table)}_Base(
		{DbContextName} db)
		: _Crud{table.EntityPrefixString}Controller_Proto<{table.Name}>(
			new Rep_{table.NamePluralize}(db))
	{{

		/* readonly properties */

{TML_Controllers_Base_Repositories(table)}{TML_Controllers_Base_Enums(table)}
{TML_Controllers_Base_Overrides(table)}	}}

}}");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_Repositories(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"
		public new Rep_{table.NamePluralize} Repository
			=> (Rep_{table.NamePluralize})base.Repository;

		public DbHub DbHub {{ get; }} = new(db);
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Controllers_Base_Enums(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.EnumFields)
			{
				if (Enums.ContainsKey(item1.EnumData))
				{
					sb1.Append($@"
		public RegistryList Enum_{item1.EnumData}
			=> DbHub.Enum_{item1.EnumData};
");
				}
				else
				{
					sb1.Append($@"
		public RegistryList Enum_{item1.Name} {{ get; }}
			= new(""{item1.EnumData}"");
");
				}
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_Overrides(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"
		/* overrides */
{TML_Controllers_Base_InitView(table)}{TML_Controllers_Base_PrepareForAdd(table)}{TML_Controllers_Base_PrepareForDetails(table)}{TML_Controllers_Base_BeforeAdd(table)}{TML_Controllers_Base_BeforeUpdate(table)}{TML_Controllers_Base_AfterChange(table)}{TML_Controllers_Base_FixModelAfterInput(table)}{TML_Controllers_Base_DecodeModelBeforeEdit(table)}{TML_Controllers_Base_DecodeModelBeforeView(table)}{TML_Controllers_Base_EncodeModelBeforeSave(table)}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_InitView(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Enums registries
			if (table.EnumFields?.Count() > 0)
			{
				sb1.Append($@"
			// init enums registries");
				foreach (var item1 in table.EnumFields)
				{
					sb1.Append($@"
			ViewData.SetRegistryList(
				""{item1.Name}"",
				Enum_{item1.Name});");
				}
			}

			// References to
			if (table.ReferencesTo?.Count > 0)
			{
				sb1.Append($@"
			// init references to");
				foreach (var item1 in table.ReferencesTo)
				{
					sb1.Append($@"
			ViewData.SetRegistryList(
				""{item1.Field.Name}"",
				DbHub.Rep_{item1.Table.NamePluralize}.GetRegistry());");
				}
			}

			// Simple manyrefs
			if (table.SlaveSimpleManyrefs?.Count() > 0)
			{
				sb1.Append($@"
			// init simple manyrefs");
				foreach (var item1 in table.SlaveSimpleManyrefs)
				{
					var s1 = item1.ManyrefField.ReferenceTable.NamePluralize;
					sb1.Append($@"
			ViewData.SetRegistryList(
				""{s1}"",
				DbHub.Rep_{s1}.GetRegistry(true));");
				}
			}

			// Extentions
			sb1.Append($@"{table.Extentions.Get("Controllers_Base", "InitView", @"
			// init extentions
			{0}
")}");

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void InitView({table.HasMaster.Make($@"
			int masterPtr")})
		{{");
				sb1.Append($@"
			base.InitView({table.HasMaster.Make($@"masterPtr")});
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_PrepareForAdd(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Tree
			if (table.IsTree)
			{
				sb1.Append($@"
			// prepare tree
			ViewData.SetRegistryList(
				""ParentPtr"",
				DbHub.Rep_{table.NamePluralize}.GetRegistryTree({table.HasMaster.Make("model.MasterPtr, ")}null, x => {table.FuncTitle}, null));");
			}

			// Model
			if (table.FuncInitAddFields?.Count() > 0)
			{
				sb1.Append($@"
			// prepare model");
				foreach (var item1 in table.FuncInitAddFields)
				{
					sb1.Append($@"
			model.{item1.Name} = {item1.FuncInitAdd};");
				}
			}

			// Extentions
			sb1.Append($@"{table.Extentions.Get("Controllers_Base", "PrepareForAdd", @"
			// prepare extentions
			{0}")}");

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void PrepareForAdd(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.PrepareForAdd(model);
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_PrepareForDetails(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Tree
			if (table.IsTree)
			{
				sb1.Append($@"
			// prepare tree
			ViewData.SetRegistryList(
				""ParentPtr"",
				DbHub.Rep_{table.NamePluralize}.GetRegistryTree({table.HasMaster.Make("model.MasterPtr, ")}model.Id, x => {table.FuncTitle}, null));");
			}

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void PrepareForDetails(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.PrepareForDetails(model);
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_BeforeAdd(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Model
			if (table.FuncAddFields?.Count() > 0)
			{
				sb1.Append($@"
			// prepare model");
				foreach (var item1 in table.FuncAddFields)
				{
					sb1.Append($@"
			model.{item1.Name} = {item1.FuncAdd};");
				}
			}

			// Extentions
			sb1.Append($@"{table.Extentions.Get("Controllers_Base", "BeforeAdd", @"
			// prepare extentions
			{0}")}");

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void BeforeAdd(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.BeforeAdd(model);
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_BeforeUpdate(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Model
			if (table.FuncEditFields?.Count() > 0)
			{
				sb1.Append($@"
			// prepare model");
				foreach (var item1 in table.FuncEditFields)
				{
					sb1.Append($@"
			model.{item1.Name} = {item1.FuncEdit};");
				}
			}

			// Extentions
			sb1.Append($@"{table.Extentions.Get("Controllers_Base", "BeforeUpdate", @"
			// prepare extentions
			{0}")}");

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void BeforeUpdate(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.BeforeUpdate(model);
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_AfterChange(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Slave simple manyrefs
			if (table.HasSlaveSimpleManyrefs)
			{
				sb1.Append($@"
			// after slave simple manyrefs
			var old1 = Repository.GetItem(model.Id);");
				foreach (var item1 in table.SlaveSimpleManyrefs)
				{
					sb1.Append($@"
			DbHub.Rep_{item1.NamePluralize}.ManyrefUpdate(
				model.Id,
				old1.DataSM_{item1.ManyrefField.ReferenceTable.NamePluralize},
				model.DataSM_{item1.ManyrefField.ReferenceTable.NamePluralize});");
				}
			}

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void AfterChange(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.AfterChange(model);
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_FixModelAfterInput(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Fix
			foreach (var item1 in table.ViewEditFields)
			{
				sb1.Append($@"
			model.{item1.Name} = {_getFixFunc(item1)}(model.{item1.Name});");
			}

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void FixModelAfterInput(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.FixModelAfterInput(model);
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_DecodeModelBeforeEdit(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Decode model
			if (table.FuncDecodeBeforeEditFields?.Count() > 0)
			{
				sb1.Append($@"
			// decode model");
				foreach (var item1 in table.FuncDecodeBeforeEditFields)
				{
					sb1.Append($@"
			model.{item1.Name} = {item1.FuncDecodeBeforeEdit}(model.{item1.Name});");
				}
			}

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void DecodeModelBeforeEdit(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.DecodeModelBeforeEdit(model);
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_DecodeModelBeforeView(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Decode model
			if (table.FuncDecodeBeforeViewFields?.Count() > 0)
			{
				sb1.Append($@"
			// decode model");
				foreach (var item1 in table.FuncDecodeBeforeViewFields)
				{
					sb1.Append($@"
			model.{item1.Name} = {item1.FuncDecodeBeforeView}(model.{item1.Name});");
				}
			}

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void DecodeModelBeforeView(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.DecodeModelBeforeView(model);
		}}
");
			}

			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_Base_EncodeModelBeforeSave(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			// Encode model
			if (table.FuncEncodeBeforeSaveFields?.Count() > 0)
			{
				sb1.Append($@"
			// encode model");
				foreach (var item1 in table.FuncEncodeBeforeSaveFields)
				{
					sb1.Append($@"
			model.{item1.Name} = {item1.FuncEncodeBeforeSave}(model.{item1.Name});");
				}
			}

			// Make
			if (sb1.Length > 0)
			{
				sb1.Insert(0, $@"

		public override void EncodeModelBeforeSave(
			{table.Name} model)
		{{");
				sb1.Append($@"
			base.EncodeModelBeforeSave(model);
		}}
");
			}

			return sb1.ToString();
		}


		/* privates */


		private static string _getControllerName(
			TableItem table)
		{
			return $"{table.NamePluralize}Controller";
		}


		private static string _getFixFunc(
			FieldItem field)
		{
			return field.FuncFix != null
				? $"{field.FuncFix}"
				: $"SuppCrud.{field.Type}_Fix";
		}

	}

}
