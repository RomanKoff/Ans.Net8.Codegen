namespace Ans.Net8.Codegen.Items
{

	public class ExtentionItemDict
		: Dictionary<string, string>
	{
	}



	public class ExtentionsDict
		: Dictionary<string, ExtentionItemDict>
	{

		public string Get(
			string target,
			string item)
		{
			return (TryGetValue(target, out var item1)
				&& item1.TryGetValue(item, out var item2))
					? item2 : null;
		}


		public string Get(
			string target,
			string item,
			string template)
		{
			var ext1 = Get(target, item);
			return ext1 == null
				? null : string.Format(template, ext1);
		}

	}

}