using Ans.Net8.Codegen.Items;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		/* ----------------------------------------------------------------- */
		private static string TML_Views_Delete(
			 TableItem table)
		{
			var sb1 = new StringBuilder(COM_Attention_Razor());
			sb1.Append($@"@model {table.Name}
@{{{TML_Views_FromCommon(table)}{table.Extentions.Get("View_Delete", "Init", @"
	{0}
")}
	{_getView_Parents(table)}
	{_getViewDelete_PageTitle(table)}
}}
{TML_Views_SlaveLinks(table)}
<form class=""form"" asp-action=""Delete"">
");
			if (table.HasSlaveSimpleManyrefs)
			{
				sb1.Append($@"
	<div class=""row"">
		<div class=""col-md-8"">
");
			}
			sb1.Append($@"
	<div asp-validation-summary=""ModelOnly"" class=""text-danger""></div>
{TML_Views_Delete_System1(table)}{TML_Views_Delete_Fields(table)}{TML_Views_Delete_System2(table)}");
			if (table.HasSlaveSimpleManyrefs)
			{
				sb1.Append($@"

		</div>
		<div class=""col-md-4"">
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
	</div>
");
			}
//			if (table.IsTree)
//			{
//				sb1.Append($@"
//	<div class=""my-4"">
//		@form1.AddView({_getControlView("Reference", "ParentPtr", 0, "RegParentPtr", null)})
//		@form1.AddView({_getControlView("Int", "Order", 0, null, null)})
//	</div>
//");
//			}
//			else if (table.IsOrdered)
//			{
//				sb1.Append($@"
//	<div class=""my-4"">
//		@form1.AddView({_getControlView("Int", "Order", 0, null, null)})
//	</div>
//");
//			}
			return sb1.ToString();
		}





		/* ----------------------------------------------------------------- */
		private static string TML_Views_Delete_System2(
			 TableItem table)
		{
			return TML_Views_Edit_System2(table);
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
	</div>
");
			}
			return sb1.ToString();
		}





		/* privates */


		private static string _getViewDelete_PageTitle(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"Current.Page.PageItem = new MapPagesItem(null, form1.Res.DeletePageTitle);
	{_getViewEditOrDelete_PageSummary(table)}");
			return sb1.ToString();
		}

	}

}
