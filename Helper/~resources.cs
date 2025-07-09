using Ans.Net8.Codegen.Items;
using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		public void Gen_Resources()
		{
			var path1 = $"{ProjectCommonPath}/Resources";
			SuppIO.CreateDirectoryIfNotExists(path1);

			var filename1 = $"{path1}/_Res_Catalogs.resx";
			SuppIO.FileWrite(
				filename1,
				TML_Resources_Catalogs());
			_logFile(filename1);

			var filename2 = $"{path1}/_Res_Faces.resx";
			SuppIO.FileWrite(
				filename2,
				TML_Resources_Faces(null, _getFaceDict(Faces)));
			_logFile(filename2);

			foreach (var item1 in VisibleTables)
			{
				var filename3 = $"{path1}/Res_{item1.NamePluralize}.resx";
				var d1 = item1.Fields
					.Where(x => x.HasTitle)
					.Select(x => new { x.Name, Value = (CrudFaceHelper)x })
					.ToDictionary(x => x.Name, x => x.Value);
				SuppIO.FileWrite(
					filename3,
					TML_Resources_Faces(item1, _getFaceDict(d1)));
				_logFile(filename3);
			}

			var filename4 = $"{path1}/__project.txt";
			SuppIO.FileWrite(
				filename4,
				TML_Resources_Project());
			_logFile(filename2);

			Console.WriteLine();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Resources_Catalogs()
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
{_getAttention_Xml()}
	<resheader name=""resmimetype""><value>text/microsoft-resx</value></resheader>
	<resheader name=""version""><value>2.0</value></resheader>
	<resheader name=""reader""><value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
	<resheader name=""writer""><value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
");
			foreach (var item1 in Catalogs)
			{
				sb1.Append($@"
<data name=""{item1.Name}"" xml:space=""preserve""><value>{item1.Title}</value><comment>{item1.Remark}</comment></data>");
			}
			sb1.Append($@"
</root>");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Resources_Faces(
			TableItem table,
			Dictionary<string, string> items)
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
{_getAttention_Xml()}
	<resheader name=""resmimetype""><value>text/microsoft-resx</value></resheader>
	<resheader name=""version""><value>2.0</value></resheader>
	<resheader name=""reader""><value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
	<resheader name=""writer""><value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value></resheader>
");
			if (table != null)
			{
				sb1.Append($@"
	<data name=""_TitlePluralize"" xml:space=""preserve""><value>{table.HeaderPluralize}</value></data>
	<data name=""_TitleWhoWhat"" xml:space=""preserve""><value>{table.HeaderWhoWhat}</value></data>
");
				foreach (var item1 in table.SlaveSimpleManyrefs)
				{
					var table1 = item1.ManyrefField.ReferenceTable;
					sb1.Append($@"
	<data name=""DataSM_{table1.NamePluralize}"" xml:space=""preserve""><value>{table1.HeaderPluralize}</value></data>");
				}
			}
			foreach (var item1 in items)
			{
				sb1.Append($@"
	<data name=""{item1.Key}"" xml:space=""preserve""><value>{item1.Value}</value></data>");
			}
			sb1.Append($@"
</root>");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Resources_Project()
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"
	<!-- RESOURCES BEGIN -->
{_getResourceItemGroup("_Res_Catalogs")}{_getResourceItemGroup("_Res_Faces")}");
			foreach (var item1 in VisibleTables.OrderBy(x => x.NamePluralize))
			{
				sb1.Append(_getResourceItemGroup($"Res_{item1.NamePluralize}"));
			}
			sb1.Append($@"
	<!-- RESOURCES END -->
");
			return sb1.ToString();
		}



		/* privates */


		private static Dictionary<string, string> _getFaceDict(
			Dictionary<string, CrudFaceHelper> items)
		{
			return items.Select(x => new { x.Key, Value = x.Value.Face })
				.ToDictionary(x => x.Key, x => x.Value);
		}


		private static string _getResourceItemGroup(
			string name)
		{
			return $@"
	<ItemGroup>
		<Compile Update=""Resources\{name}.Designer.cs"">
			<DesignTime>True</DesignTime>
			<AutoGen>True</AutoGen>
			<DependentUpon>{name}.resx</DependentUpon>
		</Compile>
		<EmbeddedResource Update=""Resources\{name}.resx"">
			<Generator>PublicResXFileCodeGenerator</Generator>
			<LastGenOutput>{name}.Designer.cs</LastGenOutput>
		</EmbeddedResource>
	</ItemGroup>
";
		}

	}

}
