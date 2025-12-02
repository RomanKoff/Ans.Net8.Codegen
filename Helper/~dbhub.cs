using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		/* ----------------------------------------------------------------- */
		private string TML_DbHub()
		{
			var sb1 = new StringBuilder(COM_Attention_CSharp());
			sb1.Append($@"
using Ans.Net8.Common;
using {ProjectCommonNamespace}.Repositories;
using Microsoft.EntityFrameworkCore;

namespace {ProjectCommonNamespace}
{{

	public partial class DbHub(
		DbContext db)
	{{
{TML_DbHub_Resources()}{TML_DbHub_Reps()}{TML_DbHub_RegistryLists()}
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
		private string TML_DbHub_RegistryLists()
		{
			if (CommonEnums.Count == 0)
				return null;
			var sb1 = new StringBuilder();
			sb1.Append(@$"

		/* enums */

");
			foreach (var item1 in CommonEnums)
			{
				sb1.Append(@$"
		public RegistryList Enum_{item1.Key} {{ get; }}
			= new(""{item1.Value.Localization ?? item1.Value.Data}"");
");
			}
			return sb1.ToString();
		}

	}

}
