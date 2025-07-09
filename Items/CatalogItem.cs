using Ans.Net8.Codegen.Schema;

namespace Ans.Net8.Codegen.Items
{

	public class CatalogItem
	{

		/* ctor */


		public CatalogItem(
			CatalogXmlElement source)
		{
			Name = source.Name;
			Title = source.Title ?? Name;
			Remark = source.Remark;
			_scan(source.Entities, null, 0);
		}


		/* readonly properties */


		public string Name { get; }
		public string Title { get; }
		public string Remark { get; }

		public List<TableItem> Tables { get; } = [];

		public IEnumerable<TableItem> TopTables
			=> Tables.Where(x => x.Level == 0);


		/* functions */


		public string GetTopTablesList()
		{
			return string.Join("\", \"", TopTables.Select(x => x.NamePluralize));
		}


		/* privates */


		private void _scan(
			IEnumerable<EntityXmlElement> entities,
			TableItem master,
			int level)
		{
			foreach (var entity1 in entities)
			{
				var table1 = new TableItem(
					this, master, entity1, level);
				Tables.Add(table1);
				level++;
				foreach (var manyref1 in entity1.Manyrefs)
				{
					var table2 = new TableItem(
						this, table1, manyref1, level);
					Tables.Add(table2);
				}
				_scan(entity1.Entities, table1, level);
				level--;
			}
		}

	}

}