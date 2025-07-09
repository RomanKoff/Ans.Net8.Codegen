using Ans.Net8.Codegen.Items;
using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		public void Gen_Views()
		{
			var path1 = $"{ProjectWebArmPath}/Areas/{CrudAreaName}/Views";
			SuppIO.CreateDirectoryIfNotExists(path1);

			var filename1 = $"{path1}/_ViewImports.cshtml";
			SuppIO.FileWrite(filename1, TML_Views_ViewImports());
			_logFile(filename1);

			var filename2 = $"{path1}/_ViewStart.cshtml";
			SuppIO.FileWrite(filename2, TML_Views_AreaViewstart());
			_logFile(filename2);

			var path3 = $"{path1}/_Home";
			SuppIO.CreateDirectoryIfNotExists(path3);

			var filename3 = $"{path3}/Index.cshtml";
			SuppIO.FileWrite(filename3, TML_Views_HomeIndex());
			_logFile(filename3);

			foreach (var item1 in VisibleTables)
			{
				var path4 = $"{path1}/{item1.NamePluralize}";
				SuppIO.CreateDirectoryIfNotExists(path4);
				_logFile(path4);

				SuppIO.FileWrite($"{path4}/_viewstart.cshtml", TML_Views_Viewstart(item1));
				SuppIO.FileWrite($"{path4}/List.cshtml", TML_Views_List(item1));
				SuppIO.FileWrite($"{path4}/Add.cshtml", TML_Views_Add(item1));
				//SuppIO.FileWrite($"{path4}/Details.cshtml", _TML_Details(item1));
				SuppIO.FileWrite($"{path4}/Edit.cshtml", TML_Views_Edit(item1));
				SuppIO.FileWrite($"{path4}/Delete.cshtml", TML_Views_Delete(item1));
			}

			Console.WriteLine();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Views_ViewImports()
		{
			var sb1 = new StringBuilder(_getAttention_Razor());
			sb1.Append($@"

@using System
@using System.Net
@using System.Net.Http
@using System.Text
@using System.Text.Json
@using Microsoft.AspNetCore.Html

@using Ans.Net8.Common
@using Ans.Net8.Web
@using Ans.Net8.Web.Forms
@using Guap.Net8.Web
@using {ProjectCommonNamespace}.Resources
@using {ProjectCommonNamespace}.Entities
@using {ProjectWebArmNamespace}

@inject CurrentContext Current

@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, Ans.Net8.Web
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Views_AreaViewstart()
		{
			var sb1 = new StringBuilder(_getAttention_Razor());
			sb1.Append(@$"
@{{
	Layout = {CrudAreaLayout};");
			if (!string.IsNullOrEmpty(CrudPath))
			{
				sb1.Append($@"
	Current.Node.AddParent($""{{Current.Host.ApplicationUrl}}{CrudPath}"", ""{CrudAreaName}"");");
			}
			sb1.Append(@$"
}}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Views_HomeIndex()
		{
			var sb1 = new StringBuilder(_getAttention_Razor());
			sb1.Append($@"
{TML_Views_HomeIndex_Navigation()}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Views_HomeIndex_Navigation()
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"@{{
	bool f1 = Current.HttpContext.IsClaimsAdmin();");
			foreach (var catalog1 in Catalogs)
			{
				sb1.Append($@"
	var test{catalog1.Name}1 = User.GetTestClaimsActionHelper(
		""{catalog1.Name}"",
		""{catalog1.GetTopTablesList()}"");");
			}
			sb1.Append($@"
}}
<ul>");
			foreach (var catalog1 in Catalogs)
			{
				if (ManyCatalogs)
				{
					sb1.Append($@"
	@if (f1 || test{catalog1.Name}1.AllowCatalog)
	{{
		<li>
			<span>@_Res_Catalogs.{catalog1.Name}.ToHtml(true)</span>
			<ul>");
				}
				foreach (var table1 in catalog1.TopTables)
				{
					sb1.Append($@"
			@if (f1 || test{catalog1.Name}1.TestAllowController(""{table1.NamePluralize}""))
			{{
				<li><a asp-area=""{CrudAreaName}"" asp-controller=""{table1.NamePluralize}"" asp-action=""List"">@Res_{table1.NamePluralize}._TitlePluralize.ToHtml(true)</a></li>
			}}");
				}
				if (ManyCatalogs)
				{
					sb1.Append($@"
			</ul>
		</li>
	}}");
				}
			}
			sb1.Append($@"
</ul>");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_Viewstart(
			TableItem table)
		{
			var sb1 = new StringBuilder(_getAttention_Razor());
			sb1.Append($@"
@{{
	// по умолчанию отключено отображение родительских страниц в заголовке

    var form1 = Html.AppendFormHelper(
		""{table.NamePluralize}"",
		Res_{table.NamePluralize}.ResourceManager, _Res_Faces.ResourceManager);
");
			if (table.HasMaster)
			{
				sb1.Append($@"
    var masterPtr1
		= ViewContext.GetRouteValueAsInt(""masterPtr"")
		?? ViewData.GetInt(""MasterPtr"", 0);

	var title1 = $""#{{masterPtr1}}""; // reg1.GetValue(masterPtr1.ToString());

    Current.Page.AddParentFromAction(
		""List"", ""{table.Master.NamePluralize}"", null, $""{{Res_{table.Master.NamePluralize}._TitlePluralize}}"");
	Current.Page.AddParentFromAction(
		""Edit"", ""{table.Master.NamePluralize}"", new {{ id = masterPtr1 }}, title1);
    Current.Page.AddParentFromAction(
		""List"", ""{table.NamePluralize}"", new {{ masterPtr = masterPtr1 }}, form1.Res.TitlePluralize);
");
			}
			else
			{
				sb1.Append($@"
    Current.Page.AddParentFromAction(
		""List"", ""{table.NamePluralize}"", null, form1.Res.TitlePluralize);
");
			}
			sb1.Append($@"
}}");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_FromCommon(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"
	var form1 = Html.GetFormHelper(""{table.NamePluralize}"");
");
			if (table.RegistryFields?.Count() > 0)
			{
				foreach (var item1 in table.RegistryFields)
				{
					sb1.Append($@"
	var Reg{item1.Name} = ViewData[""Reg_{item1.Name}""] as RegistryList;");
				}
				sb1.Append($@"
");
			}
			if (table.SlaveSimpleManyrefs?.Count() > 0)
			{
				foreach (var item1 in table.SlaveSimpleManyrefs)
				{
					var s1 = item1.ManyrefField.ReferenceTable.NamePluralize;
					sb1.Append($@"
	var Reg{s1} = ViewData[""Reg_{s1}""] as RegistryList;");
				}
				sb1.Append($@"
");
			}
			sb1.Append(table.Extentions.Get("View", "Init", @"
	{0}
"));
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_SlaveLinks(
			 TableItem table)
		{
			if (!table.HasSlaveAdvanceds)
				return null;
			var sb1 = new StringBuilder();
			sb1.Append($@"
<div class=""my-3 d-flex flex-wrap gap-1"">
");
			foreach (var item1 in table.SlaveAdvanceds)
			{
				sb1.Append($@"
	<a class=""btn btn-info btn-sm"" asp-controller=""{item1.NamePluralize}"" asp-action=""List"" asp-route-masterPtr=""@Model.Id"">@Res_{item1.NamePluralize}._TitlePluralize.ToHtml(true) @Model.Slave_{item1.NamePluralize}?.Count.Make(""({{0}})"")</a>
");
			}
			sb1.Append($@"
</div>
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_SlaveSimpleManyrefsEdit(
			 TableItem table)
		{
			var items1 = table.SlaveSimpleManyrefs;
			if (!items1.Any())
				return null;
			var sb1 = new StringBuilder();
			foreach (var item1 in items1)
			{
				var s1 = item1.ManyrefField.ReferenceTable.NamePluralize;
				sb1.Append($@"
	<div class=""my-4"">
		@form1.AddEdit(new Edit_Set(""DataSM_{s1}"", Model.DataSM_{s1}, Reg{s1}))
	</div>");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_SlaveSimpleManyrefsViews(
			 TableItem table)
		{
			var items1 = table.SlaveSimpleManyrefs;
			if (!items1.Any())
				return null;
			var sb1 = new StringBuilder();
			foreach (var item1 in items1)
			{
				var s1 = item1.ManyrefField.ReferenceTable.NamePluralize;
				sb1.Append($@"
	<div class=""my-4"">
		@form1.AddView(new View_Set(""DataSM_{s1}"", Model.DataSM_{s1}, Reg{s1}))
	</div>");
			}
			return sb1.ToString();
		}



		/* privates */


		private static string _getCancel2List(
			 TableItem table)
		{
			var s1 = table.HasMaster.Make(" asp-route-masterPtr=\"@Model.MasterPtr\"");
			return $"<a class=\"btn btn-outline-dark\" asp-action=\"List\" {s1} role=\"button\">@form1.Res.Text_Cancel_Html</a>";
		}


		private string _getCatalogTitle(
			TableItem table)
		{
			return ManyCatalogs.Make($@"
	Current.Node.AddParent(null, ""{table.Catalog.Title}"");");
		}


		private static string _getPageSummary(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"

	Func<{table.Name}, string> exp1 = x => {table.FuncTitle};
	var itemTitle1 = exp1(Model);");
			if (table.HasMaster)
			{
				sb1.Append($@"
	var masterTitle1 = {$"RegMasterPtr.GetValue(Model.MasterPtr.ToString())"};
	ViewData[""PageSummary""] = $""{{masterTitle1}} / {{itemTitle1}}"";");
			}
			else
			{
				sb1.Append($@"
	ViewData[""PageSummary""] = $""{{itemTitle1}}"";");
			}
			return sb1.ToString();
		}


		private static string _getTableDescription(
			TableItem table)
		{
			return table.Description.Make($@"

	<div class=""alert alert-info d-flex lh-sm small"">
		<i class=""bi-exclamation-circle fs-4 flex-shrink-0 me-2""></i>
		<p>{SuppTypograph.GetTypografMin(table.Description)}</p>
	</div>");
		}


		private static string _getControlCell(
			string controlName,
			string name,
			string registry,
			string cssClasses)
		{
			var sb1 = new StringBuilder();
			sb1.Append($"new Cell_{controlName}(");
			sb1.Append($"item1.{name}");
			if (registry != null)
				sb1.Append($", registry: {registry}");
			if (cssClasses != null)
				sb1.Append($", cssClasses: \"{cssClasses}\"");
			sb1.Append(')');
			return sb1.ToString();
		}


		private static string _getControlCell(FieldItem item)
		{
			return _getControlCell(
				item.ControlCell,
				item.Name,
				item.ControlRegistry,
				item.ControlCellCss);
		}


		private static string _getControlView(
			string controlName,
			string name,
			int textMaxWidth,
			string registry,
			string cssClasses)
		{
			var sb1 = new StringBuilder();
			sb1.Append($"new View_{controlName}(");
			sb1.Append($"\"{name}\"");
			sb1.Append($", Model.{name}");
			if (textMaxWidth > 0)
				sb1.Append($", maxWidth: {textMaxWidth}");
			if (registry != null)
				sb1.Append($", registry: {registry}");
			if (cssClasses != null)
				sb1.Append($", cssClasses: \"{cssClasses}\"");
			sb1.Append(')');
			return sb1.ToString();
		}


		private static string _getControlView(FieldItem item)
		{
			return _getControlView(
				item.ControlView,
				item.Name,
				item.ControlTextMaxWidth,
				item.ControlRegistry,
				item.ControlViewCss);
		}


		private static string _getControlEdit(
			string controlName,
			string name,
			string registry,
			string cssClasses)
		{
			var sb1 = new StringBuilder();
			sb1.Append($"new Edit_{controlName}(");
			sb1.Append($"\"{name}\"");
			sb1.Append($", Model.{name}");
			if (registry != null)
				sb1.Append($", registry: {registry}");
			if (cssClasses != null)
				sb1.Append($", cssClasses: \"{cssClasses}\"");
			sb1.Append(')');
			return sb1.ToString();
		}


		private static string _getControlEdit(
			FieldItem item)
		{
			return _getControlEdit(
				item.ControlEdit,
				item.Name,
				item.ControlRegistry,
				item.ControlEditCss);
		}
	}

}
