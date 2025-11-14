using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		/* ----------------------------------------------------------------- */
		private string TML_DbInit()
		{
			var sb1 = new StringBuilder(COM_Attention_CSharp());
			sb1.Append(@$"
using Ans.Net8.Psql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace {ProjectCommonNamespace}
{{

	public static class {DbContextName}_Init
    {{

		public static void {DbContextName}_Prepare(
			this IHost host,
			Action<IConfiguration, {DbContextName}> initData)
		{{
			using var scope1 = host.Services.CreateScope();
			var provider1 = scope1.ServiceProvider;
			var config1 = provider1.GetRequiredService<IConfiguration>();
			var context1 = provider1.GetRequiredService<{DbContextName}>();
			_ = context1.{DbContextName}_EnsureCreated(config1, initData);
		}}


		public static bool {DbContextName}_EnsureCreated(
			this {DbContextName} context,
			IConfiguration configuration,
			Action<IConfiguration, {DbContextName}> initData)
		{{
			if (!context.Database.EnsureCreated())
				return false;
			Debug.WriteLine(""[{ProjectCommonNamespace}.{DbContextName}_Init] Prepare Db"");
			initData?.Invoke(configuration, context);
			return true;
		}}

	}}

}}");
			return sb1.ToString();
		}

	}

}
