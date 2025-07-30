using Ans.Net8.Codegen.Items;
using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		public void Gen_Controllers_WebArm()
		{
			var path1 = $"{ProjectWebArmPath}/Areas/{CrudAreaName}/Controllers";
			SuppIO.CreateDirectoryIfNotExists(path1);

			var filename1 = $"{path1}/_HomeController.cs";
			SuppIO.FileWrite(filename1, TML_Controllers_WebArm_Home());
			_logFile(filename1);

			foreach (var item1 in VisibleTables)
			{
				var filename2_ = $"{path1}/+{_getControllerName(item1)}.cs";
				if (!File.Exists(filename2_))
				{
					var filename3 = $"{path1}/{_getControllerName(item1)}.cs";
					SuppIO.FileWrite(filename3, TML_Controllers_WebArm_Entity(item1));
					_logFile(filename3);
				}
			}
			Console.WriteLine();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Controllers_WebArm_Home()
		{
			var sb1 = new StringBuilder(_getAttention_CSharp());
			sb1.Append($@"
using Ans.Net8.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace {ProjectWebArmNamespace}.Areas.{CrudAreaName}.Controllers
{{

	[Authorize(policy: _Consts.AUTH_POLICY_USERS)]
	[Area(""{CrudAreaName}"")]
	[Route(""{CrudPath}"")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class _HomeController
		: Controller
	{{

		/* actions */


		[HttpGet("""")]
		public ActionResult Index()
		{{
			return View();
		}}

	}}

}}");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Controllers_WebArm_Entity(
			TableItem table)
		{
			var params1 = $"\"{table.Catalog.Name}\", \"{table.NamePluralize}\"";
			var sb1 = new StringBuilder(_getAttention_CSharp());
			sb1.Append($@"
using Ans.Net8.Common;
using Ans.Net8.Web;
using Ans.Net8.Web.Attributes;
using Ans.Net8.Web.Crud;
using {ProjectCommonNamespace};
using {ProjectCommonNamespace}.Controllers;
using {ProjectCommonNamespace}.Entities;
using {ProjectCommonNamespace}.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;{table.HasSlaveAdvanceds.Make(@"
using Microsoft.EntityFrameworkCore;")}

namespace {ProjectWebArmNamespace}.Areas.{CrudAreaName}.Controllers
{{

	[Authorize()]
	[ActionAccess({params1}, null)]
	[Area(""{CrudAreaName}"")]
	[Route(""{CrudPath}/{table.NamePluralize}"")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public partial class {_getControllerName(table)}(
		{DbContextName} db)
		: _{_getControllerName(table)}_Base(db)
	{{

		/* overrides */


		public override void InitController()
		{{{TML_Controllers_WebArm_InitController(table)}
			base.InitController();
		}}{TML_Controllers_WebArm_GetListQuery(table)}


		/* actions */


		[ActionAccess({params1}, ""List"")]
		[HttpGet(""{table.HasMaster.Make("{masterPtr:int}")}"")]
		{TML_Controllers_WebArm_List(table)}


		[ActionAccess({params1}, ""Add"")]
		[HttpGet(""{table.HasMaster.Make("{masterPtr:int}/add", "add")}"")]
		public override ActionResult Add({table.HasMaster.Make($@"
			int masterPtr")})
		{{
			{TML_Controllers_WebArm_Add(table)}
		}}


		[ActionAccess({params1}, ""Add"")]
		[HttpPost(""{table.HasMaster.Make("{masterPtr:int}/add", "add")}"")]
		[ActionName(""Add"")]
		[ValidateAntiForgeryToken]
		public override ActionResult AddPost({table.HasMaster.Make($@"
			int masterPtr,")}
			{table.Name} model)
		{{
			{TML_Controllers_WebArm_AddPost(table)}
		}}


		[ActionAccess({params1}, ""Details"")]
		[HttpGet(""details/{{id:int}}"")]
		public override ActionResult Details(
			int id)
		{{
			return base.Details(id);
		}}


		[ActionAccess({params1}, ""Edit"")]
		[HttpGet(""edit/{{id:int}}"")]
		public override ActionResult Edit(
			int id)
		{{
			{TML_Controllers_WebArm_Edit(table)}
		}}


		[ActionAccess({params1}, ""Edit"")]
		[HttpPost(""edit/{{id:int}}"")]
		[ActionName(""Edit"")]
		[ValidateAntiForgeryToken]
		public override ActionResult EditPost(
			int id,
			{table.Name} model)
		{{
			{TML_Controllers_WebArm_EditPost(table)}
		}}


		[ActionAccess({params1}, ""Delete"")]
		[HttpGet(""delete/{{id:int}}"")]
		public override ActionResult Delete(
			int id)
		{{
			{TML_Controllers_WebArm_Delete(table)}
		}}


		[ActionAccess({params1}, ""Delete"")]
		[HttpPost(""delete/{{id:int}}"")]
		[ActionName(""Delete"")]
		[ValidateAntiForgeryToken]
		public override ActionResult DeletePost(
			int id)
		{{
			{TML_Controllers_WebArm_DeletePost(table)}
		}}

	}}

}}");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_InitController(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.CustomListViewName != null)
				sb1.Append($@"
			CustomListViewName = ""{table.CustomListViewName}"";");
			if (table.CustomAddViewName != null)
				sb1.Append($@"
			CustomAddViewName = ""{table.CustomAddViewName}"";");
			if (table.CustomEditViewName != null)
				sb1.Append($@"
			CustomEditViewName = ""{table.CustomEditViewName}"";");
			if (table.CustomDeleteViewName != null)
				sb1.Append($@"
			CustomDeleteViewName = ""{table.CustomDeleteViewName}"";");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_GetListQuery(
			TableItem table)
		{
			if (!table.HasShowSlavesFields)
				return null;
			var sb1 = new StringBuilder();
			sb1.Append($@"


		public override IQueryable<IntCountry> GetListQuery()
		{{
			return base.GetListQuery(){_get_ShowSlaves(table)};
		}}");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_List(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"public override ActionResult List({table.HasMaster.Make($@"
			int masterPtr,")}
			string order = null,
			int page = 0,
			int itemsOnPage = 0)
		{{");
			if (table.IsTree)
			{
				sb1.Append($@"
			// for tree
			// page and itemsOnPage are ignored
			var model1 = Repository.GetItemsAsQueryable({table.HasMaster.Make($"masterPtr, ")}null)
				.AsEnumerable();
			PrepareForList(model1);
			// order: {table.DefaultSorting}
			var tree1 = new TreeHelper<{table.Name}>(
				model1,
				null,
				o => o{new OrderBuilder(table.DefaultSorting).GetLinqCode("\t\t\t\t\t")});
			InitView({table.HasMaster.Make($"masterPtr")});
			return CustomListViewName == null
				? View(tree1)
				: View(CustomListViewName, tree1);
		}}");
			}
			else
			{
				sb1.Append($@"
			return base.List({table.HasMaster.Make($"masterPtr, ")}order{table.DefaultSorting.Make(" ?? \"{0}\"")}, page, itemsOnPage);
		}}");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_Add(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.NotAdd)
			{
				sb1.Append($@"return Forbid();
			// return base.Add({table.HasMaster.Make("masterPtr")});");
			}
			else
			{
				sb1.Append($@"return base.Add({table.HasMaster.Make("masterPtr")});");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_AddPost(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.NotAdd)
			{
				sb1.Append($@"return Forbid();
			// return base.AddPost({table.HasMaster.Make("masterPtr, ")}model);");
			}
			else
			{
				sb1.Append($@"return base.AddPost({table.HasMaster.Make("masterPtr, ")}model);");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_Edit(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.NotEdit)
			{
				sb1.Append($@"return Forbid();
			// return base.Edit(id);");
			}
			else
			{
				sb1.Append($@"return base.Edit(id);");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_EditPost(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.NotEdit)
			{
				sb1.Append($@"return Forbid();
			// return base.EditPost(id, model);");
			}
			else
			{
				sb1.Append($@"return base.EditPost(id, model);");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_Delete(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.NotDelete)
			{
				sb1.Append($@"return Forbid();
			// return base.Delete(id);");
			}
			else
			{
				sb1.Append($@"return base.Delete(id);");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Controllers_WebArm_DeletePost(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.NotDelete)
			{
				sb1.Append($@"return Forbid();
			// return base.DeletePost(id);");
			}
			else
			{
				sb1.Append($@"return base.DeletePost(id);");
			}
			return sb1.ToString();
		}


		/* ----------------------------------------------------------------- */
		private static string _get_ShowSlaves(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.ShowSlavesFields)
			{
				sb1.Append($@"
				.Include(x => x.Slave_{table.Name}{item1.ShowSlavesTable}
					.OrderBy(x => x.{item1.ShowSlavesField}))");
			}
			return sb1.ToString();
		}

	}

}
