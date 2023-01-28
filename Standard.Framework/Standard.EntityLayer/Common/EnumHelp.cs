using System;
using System.Data;

namespace Basic.EntityLayer
{
	/// <summary>
	/// ϵͳ�������ṩ��������
	/// </summary>
	public static class EnumHelp
	{
		/// <summary>
		/// ������ö����ƴ�ӳ�ʹ�ö������ӵ��ַ���
		/// </summary>
		/// <param name="enumType">ö������</param>
		/// <returns>�����������Ϊö�������򣬷���ƴ�ӳɹ����ַ�����</returns>
		public static string GetEnumItemString(Type enumType)
		{
			if (enumType.IsEnum)
				return string.Join(",", Enum.GetNames(enumType));
			return string.Empty;
		}

		/// <summary>
		/// ö����������
		/// </summary>
		public const string EnumTableNameColumn = "EnumName";

		/// <summary>
		/// ö��ֵ����
		/// </summary>
		public const string EnumTableValueColumn = "EnumValue";

		/// <summary>
		/// ����ö������
		/// </summary>
		/// <param name="enumType">ö������</param>
		/// <returns>����ö�ٱ�</returns>
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
		/// ����ö������
		/// </summary>
		/// <param name="enumType">ö������</param>
		/// <returns>����ö�ٱ�</returns>
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
		/// ����ö������
		/// </summary>
		/// <param name="enumType">ö������</param>
		/// <returns>����ö�ٱ�</returns>
		public static Array CreateEnumArray(Type enumType)
		{
			return Enum.GetValues(enumType);
		}
	}
}
