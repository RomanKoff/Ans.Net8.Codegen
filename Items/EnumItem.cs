using Ans.Net8.Codegen.Schema;
using Ans.Net8.Common;

namespace Ans.Net8.Codegen.Items
{

	public class EnumItem
	{

		/* ctor */


		public EnumItem(
			EnumXmlElement source)
		{
			Name = source.Name;
			Data = SuppDictionary.GetDictInt(source.Data);
			Localization = SuppDictionary.GetDictInt(source.Localization);
		}


		/* properties */


		public string Name { get; set; }
		public DictInt Data { get; set; }
		public DictInt Localization { get; set; }

	}

}