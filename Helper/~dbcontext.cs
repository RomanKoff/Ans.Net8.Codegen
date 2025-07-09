using Ans.Net8.Common;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		public void Gen_DbContext()
		{
			var path1 = $"{ProjectCommonPath}";
			SuppIO.CreateDirectoryIfNotExists(path1);

			var filename1 = $"{path1}/{DbContextName}.cs";
			SuppIO.FileWrite(filename1, TML_DbContext());
			_logFile(filename1);

			Console.WriteLine();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbContext()
		{
			var sb1 = new StringBuilder(_getAttention_CSharp());
			sb1.Append(@$"
using Ans.Net8.Common;
using {ProjectCommonNamespace}.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace {ProjectCommonNamespace}
{{

	public class {DbContextName}
        : DbContext
    {{

		/* ctors */


		public {DbContextName}()
        {{
        }}


		public {DbContextName}(
            DbContextOptions<{DbContextName}> options)
            : base(options)
        {{
        }}


		/* readonly properties */


		public static string[] DbTables
			=> [{TML_DbContext_DbTables()}];


		/* dbsets */

{TML_DbContext_DbSets()}


		/* methods */


		protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {{
			Debug.WriteLine(""[{ProjectCommonNamespace}.{DbContextName}] OnModelCreating()"");
{TML_DbContext_Mapping()}{TML_DbContext_Requireds()}{TML_DbContext_Defaults()}{TML_DbContext_References()}
		}}

	}}

}}
");
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbContext_DbTables()
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in Tables)
				sb1.Insert(0, $", \"{item1.NamePluralize}\"");
			return sb1.ToString()[2..];
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbContext_DbSets()
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in Tables)
			{
				sb1.Append(@$"
        public DbSet<{item1.Name}> {item1.NamePluralize} {{ get; set; }}");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbContext_Mapping()
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in Tables)
			{
				sb1.Append(@$"
			modelBuilder.Entity<{item1.Name}>()
				.ToTable(""{item1.NamePluralize}"");");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbContext_Requireds()
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in Tables)
			{
				foreach (var item2 in item1.RequiredFields)
				{
					sb1.Append(@$"

			modelBuilder.Entity<{item1.Name}>()
				.Property(x => x.{item2.Name})
				.IsRequired(true);");
				}
			}
			if (sb1.Length > 0)
			{
				sb1.Insert(0, @$"

            // requireds");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbContext_Defaults()
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in Tables)
			{
				foreach (var item2 in item1.FuncSqlFields)
				{
					sb1.Append(@$"

			modelBuilder.Entity<{item1.Name}>()
				.Property(x => x.{item2.Name})
				.HasDefaultValueSql(""{item2.FuncSql}"");");
				}
			}
			if (sb1.Length > 0)
			{
				sb1.Insert(0, @$"

            // defaults");
			}
			return sb1.ToString();
		}



		/* ----------------------------------------------------------------- */
		private string TML_DbContext_References()
		{
			var sb1 = new StringBuilder();
			if (ReferenceTables.Any())
			{
				sb1.Append(@$"

            // references ");
				foreach (var item1 in ReferenceTables)
				{
					foreach (var item2 in item1.ReferencesTo)
					{
						sb1.Append(@$"

			modelBuilder.Entity<{item1.Name}>()
				.HasOne(x => x.Ref_{item2.Table.Name})
				.WithMany(x => x.Slave_{item1.NamePluralize})
				.HasForeignKey(x => x.{item2.Field.Name})
				.OnDelete(DeleteBehavior.NoAction);");
					}
				}
			}
			return sb1.ToString();
		}

	}

}
