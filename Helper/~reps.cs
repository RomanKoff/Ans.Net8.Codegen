using Ans.Net8.Codegen.Items;
using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		public void Gen_Reps()
		{
			var path1 = $"{ProjectCommonPath}/Repositories";
			SuppIO.CreateDirectoryIfNotExists(path1);

			foreach (var item1 in Tables)
			{
				var filename2 = $"{path1}/Rep_{item1.NamePluralize}.cs";
				SuppIO.FileWrite(filename2, TML_Reps(item1));
				_logFile(filename2);
			}

			Console.WriteLine();
		}



		/* ----------------------------------------------------------------- */
		private string TML_Reps(
			TableItem table)
		{
			var sb1 = new StringBuilder(_getAttention_CSharp());
			sb1.Append($@"
using Ans.Net8.Common;
using Ans.Net8.Common.Crud;
using Ans.Net8.Web;
using {ProjectCommonNamespace}.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace {ProjectCommonNamespace}.Repositories
{{

	public partial class Rep_{table.NamePluralize}(
		DbContext db)
		: _Crud{table.EntityPrefixString}Repository_Proto<{table.Name}>(db),
		ICrud{table.EntityPrefixString}Repository<{table.Name}>
	{{
{TML_Reps_GetNew(table)}{TML_Reps_GetItem(table)}{TML_Reps_GetRegistry(table)}{TML_Reps_SuppManyrefs(table)}
	}}

}}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Reps_GetNew(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.HasMaster)
			{
				sb1.Append($@"
		public override {table.Name} GetNew(
			int masterPtr)
		{{
			var model1 = new {table.Name}
			{{
				MasterPtr = masterPtr
			}};
			return model1;
		}}
");
			}
			else
			{
				sb1.Append($@"
		public override {table.Name} GetNew()
		{{
			var model1 = new {table.Name}();
			return model1;
		}}
");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Reps_GetItem(
			TableItem table)
		{
			if (!table.HasSlaves)
				return null;
			var sb1 = new StringBuilder();
			sb1.Append($@"

		public override {table.Name} GetItem(
			int id)
		{{
			var item1 = DbSet
				.AsNoTracking(){TML_Reps_GetItem_Includes(table)}
				.FirstOrDefault(x => x.Id == id);{TML_Reps_GetItem_SimpleManyrefs(table)}
			return item1;
		}}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Reps_GetItem_Includes(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.ReferencesFrom.Select(x => x.Table))
			{
				sb1.Append($@"
				.Include(x => x.Slave_{item1.NamePluralize})");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Reps_GetItem_SimpleManyrefs(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.SlaveSimpleManyrefs?.Count() > 0)
			{
				sb1.Append($@"
			// simple manyrefs");
				foreach (var item1 in table.SlaveSimpleManyrefs)
				{
					var s1 = item1.ManyrefField.ReferenceTable.NamePluralize;
					sb1.Append($@"
			item1.DataSM_{s1} = item1.Slave_{item1.NamePluralize}?
				.Select(x => x.{item1.ManyrefField.Name}).ToArray();");
				}
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private static string TML_Reps_GetRegistry(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			if (table.HasMaster)
			{
				sb1.Append($@"

		public RegistryList GetRegistry(
			Func<{table.Master.Name}, string> funcMasterTitle,
			Func<{table.Name}, string> funcTitle,
			Expression<Func<{table.Name}, bool>> filter,
			string order,
			bool offNullItem)
		{{
			var query1 = GetItemsAsQueryable(filter);
			if (!string.IsNullOrEmpty(order))
				query1 = query1.ApplyOrder(new OrderBuilder(order));
			var data1 = query1
				.Include(x => x.Ref_{table.Master.Name})
				.GroupBy(x => x.Ref_{table.Master.Name})
				.AsEnumerable();
			var reg1 = new RegistryList();
			foreach (var item1 in data1)
			{{
				reg1.AddLabel(funcMasterTitle(item1.Key));
				foreach (var item2 in item1)
					reg1.Add(new RegistryItem(
						item2.Id.ToString(),
						funcTitle(item2),
						0, false));
			}}
			if (!offNullItem)
				reg1.AddNullItem();
			return reg1;
		}}


		public RegistryList GetRegistry(
			bool offNullItem = false)
		{{
			return GetRegistry(
				x => {table.Master.FuncTitle},
				x => {table.FuncTitle},
				null,
				{table.RegistrySorting},
				offNullItem);
		}}
");
			}
			else
			{
				sb1.Append($@"

		public RegistryList GetRegistry(
			Func<{table.Name}, string> funcTitle,
			Expression<Func<{table.Name}, bool>> filter,
			string order,
			bool offNullItem)
		{{
			var query1 = GetItemsAsQueryable(filter);
			if (!string.IsNullOrEmpty(order))
				query1 = query1.ApplyOrder(new OrderBuilder(order));
			var reg1 = new RegistryList(
				query1?.Select(
					x => new RegistryItem(
						x.Id.ToString(),
						funcTitle(x),
						0, false)));
			if (!offNullItem)
				reg1.AddNullItem();
			return reg1;
		}}


		public RegistryList GetRegistry(
			bool offNullItem = false)
		{{
			return GetRegistry(
				x => {table.FuncTitle},
				null,
				{table.RegistrySorting},
				offNullItem);
		}}
");
			}
			if (table.IsTree)
			{
				if (table.HasMaster)
				{
					sb1.Append($@"

		// reg slave tree


		public TreeHelper<{table.Name}> GetTree(
			int masterPtr,
			int? id,
			Expression<Func<{table.Name}, bool>> filter)
		{{
			var query1 = GetItemsAsQueryable(masterPtr, filter);
			var items1 = query1.AsEnumerable();
			// order: {table.DefaultSorting}
			return new TreeHelper<{table.Name}>(
				items1,
				id,
				o => o{new OrderBuilder(table.DefaultSorting).GetLinqCode("\t\t\t\t\t")});
		}}


		public RegistryList GetRegistryTree(
			int masterPtr,
			int? id,
			Func<{table.Name}, string> funcTitle,
			Expression<Func<{table.Name}, bool>> filter,
			bool offNullItem = false)
		{{
			var tree1 = GetTree(masterPtr, id, filter);
			var reg1 = new RegistryList(
				tree1?.AllItems?.Select(
					x => new RegistryItem(
						x.Id.ToString(),
						funcTitle(x.Value),
						x.Level, false)));
			if (!offNullItem)
				reg1.AddNullItem();
			return reg1;
		}}
");
				}
				else
				{
					sb1.Append($@"

		// reg tree


		public TreeHelper<{table.Name}> GetTree(
			int? id,
			Expression<Func<{table.Name}, bool>> filter)
		{{
			var query1 = GetItemsAsQueryable(filter);
			var items1 = query1.AsEnumerable();
			// order: {table.DefaultSorting}
			return new TreeHelper<{table.Name}>(
				items1,
				id,
				o => o{new OrderBuilder(table.DefaultSorting).GetLinqCode("\t\t\t\t\t")});
		}}


		public RegistryList GetRegistryTree(
			int? id,
			Func<{table.Name}, string> funcTitle,
			Expression<Func<{table.Name}, bool>> filter,
			bool offNullItem = false)
		{{
			var tree1 = GetTree(id, filter);
			var reg1 = new RegistryList(
				tree1?.AllItems?.Select(
					x => new RegistryItem(
						x.Id.ToString(),
						funcTitle(x.Value),
						x.Level, false)));
			if (!offNullItem)
				reg1.AddNullItem();
			return reg1;
		}}
");
				}
			}
			return sb1.ToString();
		}



		private static string TML_Reps_SuppManyrefs(
			TableItem table)
		{
			if (!table.IsManyref)
				return null;
			var sb1 = new StringBuilder();
			sb1.Append($@"

		public override void AddManyrefs(
			int masterPtr,
			IEnumerable<int> keys)
		{{
			foreach (var item1 in keys)
				Add(new {table.Name}
				{{
					MasterPtr = masterPtr,
					{table.ManyrefField.Name} = item1,
				}});
			DbContext.SaveChanges();
		}}


		public override void RemoveManyrefs(
			int masterPtr,
			IEnumerable<int> keys)
		{{
			foreach (var item1 in keys)
				Remove(GetItem(
					x => x.MasterPtr == masterPtr
					&& x.{table.ManyrefField.Name} == item1));
			DbContext.SaveChanges();
		}}


		public override void ManyrefUpdate(
			int masterPtr,
			IEnumerable<int> oldKeys,
			IEnumerable<int> newKeys)
		{{
			var c1 = new KeysComparer(oldKeys, newKeys);
			if (c1.HasDeleted)
				RemoveManyrefs(masterPtr, c1.Deleted);
			if (c1.HasAdded)
				AddManyrefs(masterPtr, c1.Added);
		}}
");
			return sb1.ToString();
		}

	}

}
