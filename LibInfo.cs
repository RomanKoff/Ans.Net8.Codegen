using Ans.Net8.Common;

namespace Ans.Net8.Codegen
{

	public static class LibInfo
	{
		public static string GetName() => SuppApp.GetName();
		public static string GetVersion() => SuppApp.GetVersion();
		public static string GetDescription() => SuppApp.GetDescription();
	}

}
