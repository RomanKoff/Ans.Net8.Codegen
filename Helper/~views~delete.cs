using Ans.Net8.Codegen.Items;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		/* ----------------------------------------------------------------- */
		private string TML_Views_Delete(
			 TableItem table)
		{
			var sb1 = new StringBuilder(_getAttention_Razor());
			sb1.Append($@"
@model {table.Name}
@{{
{TML_Views_FromCommon(table)}{table.Extentions.Get("View_Delete", "Init", @"
	{0}
")}{_getCatalogTitle(table)}
	Current.Page.Title = form1.Res.DeletePageTitle;{_getPageSummary(table)}

}}
{TML_Views_SlaveLinks(table)}
<form class=""ans-form"" asp-action=""Delete"">
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
{TML_Views_Delete_System1(table)}{TML_Views_Delete_Fields(table)}{TML_Views_Delete_System2(table)}");
			if (table.HasSlaveSimpleManyrefs)
			{
				sb1.Append($@"

		</div>
		<div class=""col-12 col-md-4 bg-light rounded"">
{TML_Views_SlaveSimpleManyrefsViews(table)}

		</div>
	</div>");
			}
			sb1.Append($@"

	<div class=""my-5"">
		<input class=""btn btn-primary"" type=""submit"" value=""@form1.Res.Text_SubmitDelete_Html"" />
		{_getCancel2List(table)}
	</div>

</form>");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_Delete_System1(
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
		@form1.AddView({_getControlView("Reference", "ParentPtr", 0, "RegParentPtr", null)})
		@form1.AddView({_getControlView("Int", "Order", 0, null, null)})
	</div>");
			}
			else if (table.IsOrdered)
			{
				sb1.Append($@"
	<div class=""my-4"">
		@form1.AddView({_getControlView("Int", "Order", 0, null, null)})
	</div>");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_Delete_System2(
			 TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.AddUseinfo)
			{
				sb1.Append($@"
	<div class=""my-4 small opacity-50"">
		<partial name=""/Areas/Guap/Helpers/CrudUseinfo.cshtml"" model='(Model.DateCreate, Model.CreateUser, Model.DateUpdate, Model.UpdateUser)' />
	</div>");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Views_Delete_Fields(
			 TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.ViewDetailsFields)
			{
				sb1.Append($@"
	<div class=""my-4"">
		@form1.AddView({_getControlView(item1)})
	</div>");
			}
			return sb1.ToString();
		}

	}

}
