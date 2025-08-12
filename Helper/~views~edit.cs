using Ans.Net8.Codegen.Items;
using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		/* ----------------------------------------------------------------- */
		private static string TML_Views_Edit(
			 TableItem table)
		{
			var sb1 = new StringBuilder(_getAttention_Razor());
			sb1.Append($@"
@model {table.Name}
@{{
{TML_Views_FromCommon(table)}{table.Extentions.Get("View_Edit", "Init", @"
	{0}
")}
	{_getViewsAddParentToList(table)}
	Current.Page.PageItem = new MapPagesItem(null, form1.Res.EditPageTitle);{_getPageEditOrDeleteSummary(table)}

}}
{TML_Views_SlaveLinks(table)}
<form class=""form"" asp-action=""Edit"">{_getTableDescription(table)}
");
			if (table.HasSlaveSimpleManyrefs)
			{
				sb1.Append($@"
	<div class=""row"">
		<div class=""col-12 col-md-8"">
");
			}
			sb1.Append($@"
	<div asp-validation-summary=""ModelOnly"" class=""text-danger""></div>
{TML_Views_Edit_System1(table)}{TML_Views_Edit_Fields(table)}{TML_Views_Edit_System2(table)}");
			if (table.HasSlaveSimpleManyrefs)
			{
				sb1.Append($@"

		</div>
		<div class=""col-12 col-md-4 bg-light rounded"">
{TML_Views_SlaveSimpleManyrefsEdit(table)}

		</div>
	</div>");
			}
			sb1.Append($@"

	<div class=""my-4"">
		<input class=""btn btn-primary"" type=""submit"" value=""@form1.Res.Text_SubmitSave_Html"" />
		{_getCancel2List(table)}
	</div>

</form>");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_Edit_System1(
			 TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.HasMaster)
			{
				sb1.Append($@"
	<div class=""my-4"">
		@form1.AddView({_getControlView("Reference", "MasterPtr", 0, "RegMasterPtr", null)})
	</div>");
			}
			if (table.IsTree)
			{
				sb1.Append($@"
	<div class=""my-4"">
		@form1.AddEdit({_getControlEdit("Reference", "ParentPtr", "RegParentPtr", null)})
		@form1.AddEdit({_getControlEdit("Int", "Order", null, null)})
	</div>");
			}
			else if (table.IsOrdered)
			{
				sb1.Append($@"
	<div class=""my-4"">
		@form1.AddEdit({_getControlEdit("Int", "Order", null, null)})
	</div>");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_Edit_System2(
			 TableItem table)
		{
			return TML_Views_Delete_System2(table);
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_Edit_Fields(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.HideOnEditFields.Any())
			{
				foreach (var item1 in table.HideOnEditFields)
				{
					sb1.Append($@"
	<input-hidden for=""@Model.{item1.Name}"" />");
				}
				sb1.AppendLine();
			}
			foreach (var item1 in table.ViewEditFields)
			{
				if (item1.ReadonlyOnEdit)
				{
					sb1.Append($@"
	<div class=""my-4"">
		@form1.AddView({_getControlView(item1)})
	</div>");
				}
				else
				{
					sb1.Append($@"
	<div class=""my-4"">
		@form1.AddEdit({_getControlEdit(item1)}{item1.IsRequired.Make(", isRequired: true")})
	</div>");
				}
			}
			return sb1.ToString();
		}

	}

}
