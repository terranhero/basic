using System;
using System.Data;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 系统帮助类提供辅助方法
	/// </summary>
	public static class EnumHelp
	{
		/// <summary>
		/// 将所有枚举项拼接成使用逗号连接的字符串
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		/// <returns>如果参数类型为枚举类型则，返回拼接成功的字符串。</returns>
		public static string GetEnumItemString(Type enumType)
		{
			if (enumType.IsEnum)
				return string.Join(",", Enum.GetNames(enumType));
			return string.Empty;
		}

		/// <summary>
		/// 枚举名称列名
		/// </summary>
		public const string EnumTableNameColumn = "EnumName";

		/// <summary>
		/// 枚举值列名
		/// </summary>
		public const string EnumTableValueColumn = "EnumValue";

		/// <summary>
		/// 根据枚举类型
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		/// <returns>返回枚举表</returns>
		public static DataTable CreateDataTable(Type enumType)
		{
			int[] enumValues = (int[])Enum.GetValues(enumType);
			DataTable table = new DataTable("EnumTable");
			table.Columns.Add(EnumTableValueColumn, typeof(int));
			table.Columns.Add(EnumTableNameColumn, typeof(string));
			for (int index = 0; index < enumValues.Length; index++)
			{
				DataRow row = table.NewRow();
				row[EnumTableValueColumn] = Enum.ToObject(enumType, enumValues[index]);
				row[EnumTableNameColumn] = Enum.GetName(enumType, enumValues[index]);
				table.Rows.Add(row);
			}
			return table;
		}

		/// <summary>
		/// 根据枚举类型
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		/// <returns>返回枚举表</returns>
		public static DataView CreateDataView(Type enumType)
		{
			int[] enumValues = (int[])Enum.GetValues(enumType);
			DataTable table = new DataTable("EnumTable");
			table.Columns.Add(EnumTableValueColumn, typeof(int));
			table.Columns.Add(EnumTableNameColumn, typeof(string));
			for (int index = 0; index < enumValues.Length; index++)
			{
				DataRow row = table.NewRow();
				row[EnumTableValueColumn] = Enum.ToObject(enumType, enumValues[index]);
				row[EnumTableNameColumn] = Enum.GetName(enumType, enumValues[index]);
				table.Rows.Add(row);
			}
			return table.DefaultView;
		}

		/// <summary>
		/// 根据枚举类型
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		/// <returns>返回枚举表</returns>
		public static Array CreateEnumArray(Type enumType)
		{
			return Enum.GetValues(enumType);
		}
	}
}
