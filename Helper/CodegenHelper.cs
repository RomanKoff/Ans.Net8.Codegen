using Ans.Net8.Codegen.Items;
using Ans.Net8.Codegen.Schema;
using Ans.Net8.Common;

namespace Ans.Net8.Codegen.Helper
{

	public class CodegenOptions
	{
		public string ProjectWebArmName { get; set; }
		public string ProjectCommonName { get; set; }
		public string CrudAreaLayout { get; set; }
		public string CrudAreaName { get; set; }
		public string CrudPath { get; set; }

		public bool DenyResources { get; set; }
		public bool DenyHub { get; set; }
		public bool DenyControllers_Base { get; set; }
		public bool DenyControllers_WebArm { get; set; }
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
			ProjectWebArmNamespace = $"{SolutionNamespace}.{options.ProjectWebArmName}";
			ProjectWebArmPath = $"{SolutionPath}/{ProjectWebArmNamespace}";
			ProjectCommonNamespace = (options.ProjectCommonName == null)
				? ProjectWebArmNamespace
				: $"{SolutionNamespace}.{options.ProjectCommonName}";
			ProjectCommonPath = (options.ProjectCommonName == null)
				? ProjectWebArmPath
				: $"{SolutionPath}/{ProjectCommonNamespace}";
			CrudAreaLayout = options.CrudAreaLayout ?? "Current.DefaultLayout";
			CrudAreaName = options.CrudAreaName;
			CrudPath = options.CrudPath ?? CrudAreaName;

			SuppConsole.WriteLineParam(nameof(SolutionNamespace), SolutionNamespace);
			SuppConsole.WriteLineParam(nameof(SolutionPath), SolutionPath);
			SuppConsole.WriteLineParam(nameof(DbContextName), DbContextName);
			SuppConsole.WriteLineParam(nameof(ProjectWebArmNamespace), ProjectWebArmNamespace);
			SuppConsole.WriteLineParam(nameof(ProjectWebArmPath), ProjectWebArmPath);
			SuppConsole.WriteLineParam(nameof(ProjectCommonNamespace), ProjectCommonNamespace);
			SuppConsole.WriteLineParam(nameof(ProjectCommonPath), ProjectCommonPath);
			SuppConsole.WriteLineParam(nameof(CrudAreaName), CrudAreaName);
			SuppConsole.WriteLineParam(nameof(CrudAreaLayout), CrudAreaLayout);
			Console.WriteLine();

			var schema1 = SuppXml.GetObjectFromXmlFile<SchemaXmlRoot>(
				$"{SuppApp.ProjectPath}/schema.xml",
				Common._Consts.ENCODING_UTF8,
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
				Enums.Add(item1.Name, item1.Data);
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
						field1.EnumData = Enums[field1.EnumData];

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
			if (!options.DenyControllers_WebArm) Gen_Controllers_WebArm();
			if (!options.DenyViews) Gen_Views();
		}


		/* readonly properties */


		public static string SolutionNamespace
			=> SuppApp.SolutionNamespace;

		public static string SolutionPath
			=> SuppApp.SolutionPath;

		public string DbContextName { get; }
		public string ProjectWebArmNamespace { get; }
		public string ProjectWebArmPath { get; }
		public string ProjectCommonNamespace { get; }
		public string ProjectCommonPath { get; }
		public string CrudAreaLayout { get; }
		public string CrudAreaName { get; }
		public string CrudPath { get; }

		public Dictionary<string, CrudFaceHelper> Faces { get; } = [];
		public Dictionary<string, string> Enums { get; } = [];
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


		/* privates */


		private static void _logFile(
			string filename)
		{
			SuppConsole.WriteLineParam("Add file", filename[(SolutionPath.Length)..]);
		}


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


		private static string _getAttention_CSharp()
		{
			return $@"/*
 * Внимание!
 * Этот код сгенерирован автоматически {DateTime.Now}.
 * Внесенные изменения будут утеряны при следующей генерации.
 */";
		}


		private static string _getAttention_Razor()
		{
			return $@"@*
	Внимание!
	Этот код сгенерирован автоматически {DateTime.Now}.
	Внесенные изменения будут утеряны при следующей генерации.
*@";
		}


		private static string _getAttention_Xml()
		{
			return $@"	<!--
	Внимание!
	Этот код сгенерирован автоматически {DateTime.Now}.
	Внесенные изменения будут утеряны при следующей генерации.
	-->";
		}

	}

}
