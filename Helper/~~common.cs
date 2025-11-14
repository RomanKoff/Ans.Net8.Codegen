using Ans.Net8.Common;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		private static string COM_Attention_CSharp()
		{
			return $@"/*
 * Внимание!
 * Этот код сгенерирован автоматически {DateTime.Now}.
 * Внесенные изменения будут утеряны при следующей генерации.
 */
";
		}


		private static string COM_Attention_Razor()
		{
			return $@"@*
	Внимание!
	Этот код сгенерирован автоматически {DateTime.Now}.
	Внесенные изменения будут утеряны при следующей генерации.
*@
";
		}


		private static string COM_Attention_Xml()
		{
			return $@"	<!--
	Внимание!
	Этот код сгенерирован автоматически {DateTime.Now}.
	Внесенные изменения будут утеряны при следующей генерации.
	-->
";
		}


		private static void _logFile(
			string filename)
		{
			SuppConsole.WriteLineParam("Add file", filename[(SolutionPath.Length)..]);
		}

	}

}
