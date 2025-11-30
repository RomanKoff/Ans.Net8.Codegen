using Ans.Net8.Codegen.Schema;

namespace Ans.Net8.Codegen.Items
{

	public class EnumItem
	{

		/* ctor */


		public EnumItem(
			EnumXmlElement source)
		{
			Name = source.Name;
			Data = source.Data;
			Localization = source.Localization;
		}


		/* properties */


		public string Name { get; set; }
		public string Data { get; set; }
		public string Localization { get; set; }

	}

}