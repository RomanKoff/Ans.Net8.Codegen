using Ans.Net8.Codegen.Items;
using Ans.Net8.Codegen.Schema;
using Ans.Net8.Common;

namespace Ans.Net8.Codegen.Helper
{

	public class CodegenOptions
	{
		public string ProjectWebARMName { get; set; } = "WebApp";
		public string ProjectCommonName { get; set; } = "Common";
		public string CrudAreaLayout { get; set; } = "Current.DefaultLayout";
		public string CrudAreaName { get; set; } = "DbAdmin";
		public string CrudPath { get; set; } = "DbAdmin";
		public string CrudIndex { get; set; } = null;

		public bool DenyResources { get; set; }
		public bool DenyHub { get; set; }
		public bool DenyControllers_Base { get; set; }
		public bool DenyControllers_WebApp { get; set; }
		public bool DenyViews { get; set; }
	}



	public partial class CodegenHelper
	{

		/* ctor */


		public CodegenHelper(
			string dbContextName,
			CodegenOptions options)
		{
			SuppIO.Register_CodePagesEncodingProvider();
			Console.WriteLine();

			DbContextName = dbContextName;
			ProjectWebAppNamespace = $"{SolutionNamespace}.{options.ProjectWebARMName}";
			ProjectWebAppPath = $"{SolutionPath}/{ProjectWebAppNamespace}";
			ProjectCommonNamespace = (options.ProjectCommonName == null)
				? ProjectWebAppNamespace
				: $"{SolutionNamespace}.{options.ProjectCommonName}";
			ProjectCommonPath = (options.ProjectCommonName == null)
				? ProjectWebAppPath
				: $"{SolutionPath}/{ProjectCommonNamespace}";
			CrudAreaLayout = options.CrudAreaLayout ?? "Current.DefaultLayout";
			CrudAreaName = options.CrudAreaName;
			CrudPath = options.CrudPath ?? CrudAreaName;
			CrudIndex = options.CrudIndex;

			SuppConsole.WriteLineParam(nameof(SolutionNamespace), SolutionNamespace);
			SuppConsole.WriteLineParam(nameof(SolutionPath), SolutionPath);
			SuppConsole.WriteLineParam(nameof(DbContextName), DbContextName);
			SuppConsole.WriteLineParam(nameof(ProjectWebAppNamespace), ProjectWebAppNamespace);
			SuppConsole.WriteLineParam(nameof(ProjectWebAppPath), ProjectWebAppPath);
			SuppConsole.WriteLineParam(nameof(ProjectCommonNamespace), ProjectCommonNamespace);
			SuppConsole.WriteLineParam(nameof(ProjectCommonPath), ProjectCommonPath);
			SuppConsole.WriteLineParam(nameof(CrudAreaName), CrudAreaName);
			SuppConsole.WriteLineParam(nameof(CrudAreaLayout), CrudAreaLayout);
			Console.WriteLine();

			var schema1 = SuppXml.GetObjectFromXmlFile<SchemaXmlRoot>(
				$"{SuppApp.ProjectPath}/schema.xml",
				_Consts.ENCODING_UTF8,
				"http://tempuri.org/Ans.Net8.Codegen.Schema.xsd");

			// add faces
			foreach (var item1 in schema1.Faces)
			{
				Faces.Add(item1.Name, new CrudFaceHelper(
					item1.Name,
					item1.Title,
					item1.ShortTitle,
					item1.Description,
					item1.Sample,
					item1.HelpLink));
			}

			// add enums
			foreach (var item1 in schema1.Enums)
			{
				Enums.Add(item1.Name, new EnumItem(item1));
			}

			// add catalogs
			foreach (var catalog1 in schema1.Catalogs)
				Catalogs.Add(new CatalogItem(catalog1));

			// prepare tables
			foreach (var table1 in Tables)
			{
				foreach (var field1 in table1.Fields)
				{

					// readonly
					if (table1.IsReadonly)
					{
						field1.ReadonlyOnAdd = true;
						field1.ReadonlyOnEdit = true;
					}

					// enums
					if (field1.IsEnum && !field1.EnumData.Contains('='))
						field1.EnumData = Enums[field1.EnumData].Data;

					// refs
					if (!string.IsNullOrEmpty(field1.ReferenceTarget)
						&& field1.ReferenceTable == null)
						field1.ReferenceTable = _getTable(field1.ReferenceTarget);
					if (field1.ReferenceTarget != null)
					{
						field1.Title = field1.Name switch
						{
							//"MasterPtr" => $"Master {field1.ReferenceTable.HeaderSingular}",
							"ParentPtr" => string.Format(
								Common.Resources.Common.Template_ParentPtr,
								field1.ReferenceTable.HeaderSingular.ToLower()),
							_ => $"{field1.ReferenceTable.HeaderSingular}"
						};
						table1.ReferencesTo.Add(new ReferenceItem
						{
							Field = field1,
							Table = field1.ReferenceTable,
						});
						field1.ReferenceTable.ReferencesFrom.Add(new ReferenceItem
						{
							Field = field1,
							Table = table1
						});
					}

				}

				// manyrefs
				if (table1.IsManyref)
				{
					table1.HeaderSingular
						= $"{table1.ManyrefField.ReferenceTable.HeaderSingular} {table1.Master.HeaderWhoWhat}";
					table1.HeaderPluralize
						= $"{table1.ManyrefField.ReferenceTable.HeaderPluralize} {table1.Master.HeaderWhoWhat}";
					table1.HeaderWhoWhat
						= $"{table1.ManyrefField.ReferenceTable.HeaderWhoWhat} {table1.Master.HeaderWhoWhat}";
				}
				else if (table1.HeaderPluralize == null)
				{
					table1.HeaderSingular = table1.Name;
					table1.HeaderPluralize = table1.NamePluralize;
					table1.HeaderWhoWhat = table1.Name.ToLower();
				}

				// logs
				Console.WriteLine($"[{table1.Name}]");
				foreach (var ref1 in table1.ReferencesTo)
					Console.WriteLine($"    >>> .{ref1.Field.Name} [{ref1.Table.Name}]");
				foreach (var ref1 in table1.ReferencesFrom)
					Console.WriteLine($"    <<< {ref1.Table.Name}.{ref1.Field.Name}");
				foreach (var field1 in table1.Fields)
					Console.WriteLine($"  .{field1.Name} ({field1.CSharpTypeString})");
				Console.WriteLine();
			}

			// gen
			Gen_Entities();
			Gen_DbContext();
			Gen_DbInit();
			Gen_Reps();
			if (!options.DenyResources) Gen_Resources();
			if (!options.DenyHub) Gen_DbHub();
			if (!options.DenyControllers_Base) Gen_Controllers_Base();
			if (!options.DenyControllers_WebApp) Gen_Controllers_WebApp();
			if (!options.DenyViews) Gen_Views();
		}


		/* readonly properties */


		public static string SolutionNamespace
			=> SuppApp.SolutionNamespace;

		public static string SolutionPath
			=> SuppApp.SolutionPath;

		public string DbContextName { get; }
		public string ProjectWebAppNamespace { get; }
		public string ProjectWebAppPath { get; }
		public string ProjectCommonNamespace { get; }
		public string ProjectCommonPath { get; }
		public string CrudAreaLayout { get; }
		public string CrudAreaName { get; }
		public string CrudPath { get; }
		public string CrudIndex { get; }

		public Dictionary<string, CrudFaceHelper> Faces { get; } = [];
		public Dictionary<string, EnumItem> Enums { get; } = [];
		public List<CatalogItem> Catalogs { get; } = [];

		public bool ManyCatalogs
			=> Catalogs.Count > 1;


		public IEnumerable<TableItem> Tables
			=> Catalogs.SelectMany(x => x.Tables);

		public IEnumerable<TableItem> SlaveTables
			=> Tables.Where(x => x.HasMaster);

		public IEnumerable<TableItem> ReferenceTables
			=> Tables.Where(x => x.HasReferencesTo);

		public IEnumerable<TableItem> VisibleTables
			=> Tables.Where(x => !x.IsHidden);


		/* methods */


		public void Gen_Entities()
		{
			var path1 = $"{ProjectCommonPath}/Entities";
			SuppIO.CreateDirectoryIfNotExists(path1);
			if (Enums.Count > 0)
			{
				var filename1 = $"{path1}/~enums.cs";
				SuppIO.FileWrite(filename1, TML_Enums());
				_logFile(filename1);
			}
			foreach (var item1 in Tables)
			{
				var filename1 = $"{path1}/{item1.Name}.cs";
				SuppIO.FileWrite(filename1, TML_Entities(item1));
				_logFile(filename1);
			}
			Console.WriteLine();
		}


		public void Gen_DbContext()
		{
			var path1 = $"{ProjectCommonPath}";
			var filename1 = $"{path1}/{DbContextName}.cs";
			SuppIO.CreateDirectoryIfNotExists(path1);
			SuppIO.FileWrite(filename1, TML_DbContext());
			_logFile(filename1);
			Console.WriteLine();
		}


		public void Gen_DbInit()
		{
			var path1 = $"{ProjectCommonPath}";
			var filename1 = $"{path1}/{DbContextName}_Init.cs";
			SuppIO.CreateDirectoryIfNotExists(path1);
			SuppIO.FileWrite(filename1, TML_DbInit());
			_logFile(filename1);
			Console.WriteLine();
		}


		public void Gen_Reps()
		{
			var path1 = $"{ProjectCommonPath}/Repositories";
			SuppIO.CreateDirectoryIfNotExists(path1);
			foreach (var item1 in Tables)
			{
				var filename1 = $"{path1}/Rep_{item1.NamePluralize}.cs";
				SuppIO.FileWrite(filename1, TML_Reps(item1));
				_logFile(filename1);
			}
			Console.WriteLine();
		}


		public void Gen_Resources()
		{
			var path1 = $"{ProjectCommonPath}/Resources";
			var filename1 = $"{path1}/_Res_Catalogs.resx";
			var filename2 = $"{path1}/_Res_Faces.resx";
			var filename3 = $"{path1}/__project.txt";

			SuppIO.CreateDirectoryIfNotExists(path1);
			SuppIO.FileWrite(
				filename1,
				TML_Resources_Catalogs());
			_logFile(filename1);
			SuppIO.FileWrite(
				filename2,
				TML_Resources_Faces(null, _getFaceDict(Faces)));
			_logFile(filename2);
			SuppIO.FileWrite(
				filename3,
				TML_Resources_Project());
			_logFile(filename2);

			foreach (var item1 in VisibleTables)
			{
				var filename4 = $"{path1}/Res_{item1.NamePluralize}.resx";
				var d1 = item1.Fields
					.Where(x => x.HasFace)
					.Select(x => new { x.Name, Value = (CrudFaceHelper)x })
					.ToDictionary(x => x.Name, x => x.Value);
				SuppIO.FileWrite(
					filename4,
					TML_Resources_Faces(item1, _getFaceDict(d1)));
				_logFile(filename4);
			}

			Console.WriteLine();
		}


		public void Gen_DbHub()
		{
			var path1 = $"{ProjectCommonPath}";
			var filename1 = $"{path1}/DbHub.cs";
			SuppIO.CreateDirectoryIfNotExists(path1);
			SuppIO.FileWrite(filename1, TML_DbHub());
			_logFile(filename1);
			Console.WriteLine();
		}


		public void Gen_Controllers_Base()
		{
			var path1 = $"{ProjectCommonPath}/Controllers";
			SuppIO.CreateDirectoryIfNotExists(path1);
			foreach (var item1 in Tables)
			{
				var filename1 = $"{path1}/_{_getControllerName(item1)}_Base.cs";
				SuppIO.FileWrite(filename1, TML_Controllers_Base(item1));
				_logFile(filename1);
			}
			Console.WriteLine();
		}


		public void Gen_Controllers_WebApp()
		{
			var path1 = $"{ProjectWebAppPath}/Areas/{CrudAreaName}/Controllers";
			SuppIO.CreateDirectoryIfNotExists(path1);

			var filename1 = $"{path1}/_HomeController.cs";
			SuppIO.FileWrite(filename1, TML_Controllers_WebApp_Home());
			_logFile(filename1);

			foreach (var item1 in VisibleTables)
			{
				var filename2_ = $"{path1}/+{_getControllerName(item1)}.cs";
				if (!File.Exists(filename2_))
				{
					var filename3 = $"{path1}/{_getControllerName(item1)}.cs";
					SuppIO.FileWrite(filename3, TML_Controllers_WebApp_Entity(item1));
					_logFile(filename3);
				}
			}
			Console.WriteLine();
		}


		public void Gen_Views()
		{
			var path1 = $"{ProjectWebAppPath}/Areas/{CrudAreaName}/Views";
			var filename1 = $"{path1}/_ViewImports.cshtml";
			var filename2 = $"{path1}/_ViewStart.cshtml";
			var path3 = $"{path1}/_Home";
			var filename3 = $"{path3}/Index.cshtml";

			SuppIO.CreateDirectoryIfNotExists(path1);
			SuppIO.FileWrite(filename1, TML_Views_Root_ViewImports());
			_logFile(filename1);
			SuppIO.FileWrite(filename2, TML_Views_Root_ViewStart());
			_logFile(filename2);
			SuppIO.CreateDirectoryIfNotExists(path3);
			SuppIO.FileWrite(filename3, TML_Views_HomeIndex());
			_logFile(filename3);

			foreach (var item1 in VisibleTables)
			{
				var path4 = $"{path1}/{item1.NamePluralize}";
				SuppIO.CreateDirectoryIfNotExists(path4);
				_logFile(path4);

				SuppIO.FileWrite($"{path4}/_viewstart.cshtml", TML_Views_ViewStart(item1));
				SuppIO.FileWrite($"{path4}/List.cshtml", TML_Views_List(item1));
				SuppIO.FileWrite($"{path4}/Add.cshtml", TML_Views_Add(item1));
				SuppIO.FileWrite($"{path4}/Edit.cshtml", TML_Views_Edit(item1));
				SuppIO.FileWrite($"{path4}/Delete.cshtml", TML_Views_Delete(item1));
			}

			Console.WriteLine();
		}


		/* privates */


		private TableItem _getTable(
			string name)
		{
			var a1 = Tables.Where(x => x.Name == name);
			return a1.Count() switch
			{
				1 => a1.First(),
				0 => throw new Exception($"GenHelper: Table [{name}] not found!"),
				_ => throw new Exception($"GenHelper: More than one table named [{name}] found!")
			};
		}

	}

}
