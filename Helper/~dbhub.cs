using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		public void Gen_DbHub()
		{
			var path1 = $"{ProjectCommonPath}";
			SuppIO.CreateDirectoryIfNotExists(path1);

			var filename1 = $"{path1}/DbHub.cs";
			SuppIO.FileWrite(filename1, TML_DbHub());
			_logFile(filename1);

			Console.WriteLine();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbHub()
		{
			var sb1 = new StringBuilder(_getAttention_CSharp());
			sb1.Append($@"
using Ans.Net8.Common;
using {ProjectCommonNamespace}.Repositories;
using Microsoft.EntityFrameworkCore;

namespace {ProjectCommonNamespace}
{{

	public partial class DbHub(
		DbContext db)
	{{
{TML_DbHub_Resources()}{TML_DbHub_Reps()}{TML_DbHub_Enums()}
	}}

}}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbHub_Resources()
		{
			var sb1 = new StringBuilder();
			sb1.Append(@$"
		/* resources */


		public static ResourcesHelper Res__Catalogs
			=> new(Resources._Res_Catalogs.ResourceManager);

		public static ResourcesHelper Res__Faces
			=> new(Resources._Res_Faces.ResourceManager);

");
			foreach (var item1 in Tables)
			{
				sb1.Append(@$"
		public static ResourcesHelper Res_{item1.NamePluralize}
			=> new(Resources.Res_{item1.NamePluralize}.ResourceManager);
");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbHub_Reps()
		{
			var sb1 = new StringBuilder();
			sb1.Append(@$"

		/* repositories */

");
			foreach (var item1 in Tables)
			{
				sb1.Append(@$"
		public Rep_{item1.NamePluralize}
			Rep_{item1.NamePluralize} {{ get; }} = new(db);
");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbHub_Enums()
		{
			if (Enums.Count == 0)
				return null;
			var sb1 = new StringBuilder();
			sb1.Append(@$"

		/* enums */

");
			foreach (var item1 in Enums)
			{
				sb1.Append(@$"
		public RegistryList Enum_{item1.Key} {{ get; }}
			= new(""{item1.Value}"");
");
			}
			return sb1.ToString();
		}

	}

}
