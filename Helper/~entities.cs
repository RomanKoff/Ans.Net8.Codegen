using Ans.Net8.Codegen.Items;
using Ans.Net8.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;

namespace Ans.Net8.Codegen.Helper
{

	public partial class CodegenHelper
	{

		/* ----------------------------------------------------------------- */
		private string TML_CommonEnums()
		{
			var sb1 = new StringBuilder(COM_Attention_CSharp());
			sb1.Append($@"
namespace {ProjectCommonNamespace}.Entities
{{
");
			foreach (var key1 in CommonEnums.Keys)
			{
				var data1 = CommonEnums[key1].Data;
				var localization1 = CommonEnums[key1].Localization;
				sb1.Append($@"
	public enum {key1}Enum
		: int
	{{");
				foreach (var key2 in data1.Keys)
				{
					var value2 = data1[key2];
					string s1 = null;
					_ = localization1?.TryGetValue(key2, out s1);
					var l1 = s1?.Make(" // {0}");
					sb1.Append($@"
		{value2} = {key2},{l1}");
				}
				sb1.Append($@"
	}}
");
			}
			sb1.Append($@"
}}");
			return sb1.ToString();
		}





		/* ----------------------------------------------------------------- */
		private string TML_Entities(
			TableItem table)
		{
			var sb1 = new StringBuilder(COM_Attention_CSharp());
			sb1.Append($@"
using Ans.Net8.Common;
using Ans.Net8.Common.Attributes;
using Ans.Net8.Common.Crud;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace {ProjectCommonNamespace}.Entities
{{

	{TML_Entities_Enums(table)}public interface {table.InterfaceName}
		: {table.BaseInterfaceName}
	{{{TML_Entities_InterfaceFields(table)}
	}}



	public class {table.BaseClassName}
		: {table.Interfaces}
	{{

		/* ctors */
		

		public {table.BaseClassName}()
		{{
			// defaults
		}}


		public {table.BaseClassName}(
			{table.InterfaceName} source)
			: this()
		{{
			if (source != null)
				this.Import(source);
		}}


		/* fields */

		
		{TML_Entities_Fields(table)}
	}}



	{TML_Entities_Attributes(table)}public partial class {table.Name}
		: {table.BaseClassName},
		{table.Interfaces}
	{{

		/* ctors */


		public {table.Name}()
            : base()
        {{
        }}


		public {table.Name}(
            {table.InterfaceName} source)
            : base(source)
        {{
        }}
		{TML_Entities_Navigation(table)}
	}}



	public static partial class _e
	{{
		public static void Import(
			this {table.InterfaceName} item,
			{table.InterfaceName} source)
		{{
			item.Id = source.Id;{TML_Entities_Imports(table)}
		}}
	}}

}}
");
			return sb1.ToString();
		}





		/* ----------------------------------------------------------------- */
		private static string TML_Entities_Enums(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.EnumFields.Where(x => !x.IsEnumGlobal))
			{
				var data1 = item1.EnumData;
				var localization1 = item1.EnumLocalization;
				sb1.Append($@"public enum {table.Name}{item1.Name}Enum
		: int
	{{");
				foreach (var key1 in item1.EnumData.Keys)
				{
					var value1 = data1[key1];
					string s1 = null;
					_ = localization1?.TryGetValue(key1, out s1);
					var l1 = s1?.Make(" // {0}");
					sb1.Append($@"
		{value1} = {key1},{l1}");
				}
				sb1.Append($@"
	}}



	");
			}
			return sb1.ToString();
		}





		/* ----------------------------------------------------------------- */
		private static string TML_Entities_InterfaceFields(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.Fields.Where(x => x.Name != "MasterPtr"))
			{
				sb1.Append($@"
		{item1.CSharpDeclareString}");
			}
			return sb1.ToString();
		}





		/* ----------------------------------------------------------------- */
		private static string TML_Entities_Fields(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			sb1.Append($@"[Key]
        public int Id {{ get; set; }}
");
			foreach (var item1 in table.Fields)
			{
				sb1.Append($@"
		[AnsField(CrudFieldTypeEnum.{item1.Type})]{item1.GetCSharpAttributes().MakeFromCollection(null, "\n\t\t{0}", null)}
        public {item1.CSharpDeclareString}
");
			}
			return sb1.ToString();
		}





		/* ----------------------------------------------------------------- */
		private static string TML_Entities_Attributes(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.ConstraintFields)
			{
				if (table.HasMaster && !item1.IsAbsoluteUnique)
				{
					sb1.Append($@"[Index(nameof(MasterPtr), nameof({item1.Name}), IsUnique = true)]
	");
				}
				else
				{
					sb1.Append($@"[Index(nameof({item1.Name}), IsUnique = true)]
	");
				}
			}
			return sb1.ToString();
		}





		/* ----------------------------------------------------------------- */
		private static string TML_Entities_Navigation(
			TableItem table)
		{
			var sb1 = new StringBuilder();

			if (table.HasNavigation)
			{

				if (table.HasReferencesTo)
				{
					sb1.Append(@"

        /* navigation to */
");
					foreach (var item1 in table.ReferencesTo)
					{
						var name1
							= $"Ref_{item1.Table.Name}";
						sb1.Append($@"

		[NotMapped, JsonIgnore, XmlIgnore]
		public virtual {item1.Table.Name}
			 {name1} {{ get; set; }}
");
					}
				}

				if (table.HasReferencesFrom)
				{
					sb1.Append(@"

        /* navigation from */
");
					foreach (var item1 in table.ReferencesFrom)
					{
						var name1
							= $"Slave_{item1.Table.NamePluralize}";
						sb1.Append($@"

		[NotMapped, JsonIgnore, XmlIgnore]
		public virtual ICollection<{item1.Table.Name}>
			{name1} {{ get; set; }}
");
					}
				}

				if (table.HasMaster || table.IsTree)
				{
					sb1.Append(@"

        /* navigation aliases */
");
					if (table.HasMaster)
					{
						sb1.Append($@"

		[NotMapped, JsonIgnore, XmlIgnore]
		public virtual {table.Master.Name} Master
			=> Ref_{table.Master.Name};
");
					}
					if (table.IsTree)
					{
						sb1.Append($@"

		[NotMapped, JsonIgnore, XmlIgnore]
		public virtual {table.Name} Parent
			=> Ref_{table.Name};


		[NotMapped, JsonIgnore, XmlIgnore]
		public virtual ICollection<{table.Name}> Childs
			=> Slave_{table.NamePluralize};
");
					}
				}

				if (table.HasSlaveSimpleManyrefs)
				{
					sb1.Append(@"

        /* simple manyrefs */

");
					foreach (var item1 in table.SlaveSimpleManyrefs)
					{
						var s1 = $"{item1.ManyrefField.ReferenceTable.NamePluralize}";
						sb1.Append($@"
        [NotMapped, JsonIgnore, XmlIgnore]
		public virtual int[] DataSM_{s1} {{ get; set; }}
");
					}
				}
			}
			return sb1.ToString();
		}





		/* ----------------------------------------------------------------- */
		private static string TML_Entities_Imports(
			TableItem table)
		{
			var sb1 = new StringBuilder();
			foreach (var item1 in table.Fields)
			{
				sb1.Append($@"
			item.{item1.Name} = source.{item1.Name};");
			}
			return sb1.ToString();
		}

	}

}
