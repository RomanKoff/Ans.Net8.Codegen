using Ans.Net8.Codegen.Items;
using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		/* ----------------------------------------------------------------- */
		private static string TML_Views_List(
			TableItem table)
		{
			return (table.IsTree)
				? TML_Views_List_Tree(table)
				: TML_Views_List_Table(table);
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_List_Table(
			TableItem table)
		{
			var allowAdd1 = !table.NotAdd;
			var allowEdit1 = !table.NotEdit;
			var allowDelete1 = !table.NotDelete;
			var linkAdd1 = allowAdd1.Make(_getLinkAdd(table));
			var sb1 = new StringBuilder(_getAttention_Razor());
			sb1.Append($@"
@model IEnumerable<{table.Name}>
@{{
{TML_Views_FromCommon(table)}{table.Extentions.Get("View_List", "Init", @"
	{0}
")}
	var pagination1 = ViewData.GetPaginationHelper();
	var count1 = pagination1.SkipItems;
	{table.HasMaster.Make(
		$@"
	var masterPtr1 = ViewContext.GetRouteValueAsInt(""masterPtr"", 0);
	Current.Page.PageItem = new MapPagesItem(null, $""{{form1.Res.TitlePluralize}} #{{masterPtr1}}"");",
		$@"
	Current.Page.PageItem = new MapPagesItem(null, form1.Res.TitlePluralize);")}
	{table.HasMaster.Make($@"
	var masterTitle1 = RegMasterPtr.GetValue(masterPtr1.ToString());
	Current.SetData(""PageSummary"", masterTitle1);
")}
}}
{linkAdd1}
@if (Model?.Count() > 0)
{{
	<style>
		.table-crud tbody th {{ white-space: nowrap; font-weight: normal; }}
		.table-crud tbody th a {{ text-decoration: none; font-size: 1.1rem; }}
		.table-crud tbody th span {{ opacity: .75; font-size: .65rem; margin: .4rem 0 0 .3rem; }}
		th.i1 {{ padding-top: .75rem; padding-left: 0; font-size: .75rem !important; opacity: .5; }}
	</style>

	<partial name=""/Areas/Ans/Helpers/Pagination.cshtml"" model='pagination1' />

	<div class=""table-responsive mb-3"">
	<table class=""table table-hover w-auto table-crud lh-sm"">
		<thead>
			<tr>
				<th>&nbsp;</th>
				<th class=""ps-0"">@Current.QueryString.GetSortingButton(""Id"", ""<i class=\""bi-key\""></i>"", false)</th>{TML_Views_List_Headers(table)}
				<th>&nbsp;</th>
				<th>&nbsp;</th>
			</tr>
		</thead>
		<tbody>
	@foreach (var item1 in Model)
	{{
		count1++;
			<tr>
				<th>{_getButtonEdit(allowEdit1)}</th>
				<th class=""i1"">@item1.Id</th>
{TML_Views_List_Table_Fields(table)}

				<th>{_getButtonDelete(allowDelete1)}</th>
				<th class=""i1"">@count1</th>
			</tr>
	}}
		</tbody>
	</table>
	</div>

	<partial name=""/Areas/Ans/Helpers/Pagination.cshtml"" model='pagination1' />
	{linkAdd1}
}}
else
{{
	<p>@form1.Res.Text_EmptyItems_Html</p>
}}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_List_Tree(
			TableItem table)
		{
			var linkAdd1 = _getLinkAdd(table);
			var allowEdit1 = !table.NotEdit;
			var allowDelete1 = !table.NotDelete;
			var sb1 = new StringBuilder(_getAttention_Razor());
			sb1.Append($@"
@model TreeHelper<{table.Name}>
@{{
{TML_Views_FromCommon(table)}{table.Extentions.Get("View_List", "Init", @"
	{0}
")}
	var count1 = 0;
	{table.HasMaster.Make(
		$@"
	var masterPtr1 = ViewContext.GetRouteValueAsInt(""masterPtr"", 0);
	Current.Page.PageItem = new MapPagesItem(null, $""{{form1.Res.TitlePluralize}} #{{masterPtr1}}"");",
		$@"
	Current.Page.PageItem = new MapPagesItem(null, form1.Res.TitlePluralize);")}
	{table.HasMaster.Make($@"
	var masterTitle1 = RegMasterPtr.GetValue(masterPtr1.ToString());
	Current.SetData(""PageSummary"", masterTitle1);
")}
}}
{linkAdd1}
@if (Model?.AllItems?.Count() > 0)
{{
	<style>
		.table-crud tbody th {{ white-space: nowrap; font-weight: normal; }}
		.table-crud tbody th a {{ text-decoration: none; font-size: 1.1rem; }}
		th.i1 {{ padding-top: .75rem; padding-left: 0; font-size: .75rem !important; opacity: .5; }}
		td.ofs-0 {{ font-weight: 600; font-size: 1.1rem; }}
		td.ofs-1 {{ padding-left: 1rem; }}
		td.ofs-2 {{ padding-left: 2rem; }}
		td.ofs-3 {{ padding-left: 3rem; font-size: .9rem; }}
		td.ofs-4 {{ padding-left: 4rem; font-size: .8rem; }}
		td.ofs-5 {{ padding-left: 5rem; font-size: .7rem; }}
	</style>

	<p>
		<input type=""text"" class=""form-control ans-table-filter"" style=""max-width:25rem;"" data-target=""#tree1"" placeholder=""Поиск по реестру"" />
	</p>

	<div class=""table-responsive mb-3"">
	<table id=""tree1"" class=""table table-hover w-auto table-crud lh-sm"">
		<thead>
			<tr>
				<th>&nbsp;</th>
				<th class=""ps-0""><i class=""bi-key""></i></th>{TML_Views_List_Headers(table)}
				<th>&nbsp;</th>
				<th>&nbsp;</th>
			</tr>
		</thead>
		<tbody>
	@foreach (var item0 in Model.AllItems)
	{{
		var item1 = item0.Value;
		var ofs1 = $""ofs-{{item0.Level}}"";
		count1++;
			<tr>
				<th>{_getButtonEdit(allowEdit1)}</th>
				<th class=""i1"">@item1.Id</th>
{TML_Views_List_Tree_Fields(table)}

				<th>{_getButtonDelete(allowDelete1)}</th>
				<th class=""i1"">@count1</th>
			</tr>
	}}
		</tbody>
	</table>
	</div>

	{linkAdd1}
}}
else
{{
	<p>@form1.Res.Text_EmptyItems_Html</p>
}}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_List_Headers(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.ViewListFields)
			{
				if (table.IsTree || !item1.IsSortable)
					sb1.Append($@"
				<th title=""{item1.Name}"">@form1.Face(""{item1.Name}"").ShortTitleCalc.ToHtml(true)</th>");
				else
					sb1.Append($@"
				<th title=""{item1.Name}"">@Current.QueryString.GetSortingButton(""{item1.Name}"", form1.Face(""{item1.Name}"").ShortTitleCalc, true)</th>");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_List_Table_Fields(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.ViewListFields)
			{
				if (item1.HasShowSlaves)
				{
					sb1.Append($@"
				<td>
					@({_getControlCell(item1)})
					<ul>
						@foreach (var item2 in item1.Slave_{table.Name}{item1.ShowSlavesTable})
						{{
							<li>@item2.{item1.ShowSlavesField}</li>
						}}
					</ul>
				</td>");
				}
				else
				{
					sb1.Append($@"
				@form1.AddCell({_getControlCell(item1)})");
				}
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_List_Tree_Fields(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.ViewListFields)
			{
				var s1 = item1.Name.Equals("Title").Make(", cssClasses: @ofs1");
				sb1.Append($@"
				@form1.AddCell({_getControlCell(item1)}{s1})");
			}
			return sb1.ToString();
		}



		/* privates */


		private static string _getLinkAdd(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			sb1.Append("<div class=\"my-3\">");
			sb1.Append("<a class=\"btn btn-success\" asp-action=\"Add\"");
			if (table.HasMaster)
				sb1.Append($" asp-route-masterPtr=\"@masterPtr1\"");
			sb1.Append(">@form1.Res.Text_Add_Html</a>");
			sb1.Append("</div>");
			return sb1.ToString();
		}


		private static string _getButtonEdit(
			bool allow)
		{
			return (allow)
				? $@"<a class=""text-success"" asp-action=""Edit"" asp-route-id=""@item1.Id"" title=""@form1.Res.Title_Edit_Html""><i class=""bi-pencil-square""></i></a>"
				: null;
		}


		private static string _getButtonDelete(
			bool allow)
		{
			return (allow)
				? $@"<a class=""text-danger"" asp-action=""Delete"" asp-route-id=""@item1.Id"" title=""@form1.Res.Title_Delete_Html""><i class=""bi-x-circle""></i></a>"
				: null;
		}

	}

}
