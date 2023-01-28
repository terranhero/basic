using System.Data;
using Basic.Enums;

namespace Basic.Database
{
	/// <summary>
	/// 存储过程参数信息
	/// </summary>
	public sealed class ProcedureParameter
	{
		/// <summary>
		/// 所返回的数据库表中每列的列名。
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// ODBC 数据类型的整数代码。
		/// 如果该数据类型无法映射到 ODBC 类型，则为 NULL。
		/// 本机数据类型名称在 TYPE_NAME 列中返回。
		/// </summary>
		public DbTypeEnum DbType { get; set; }

		/// <summary>
		/// 数据的传输大小。
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// 数据的传输大小。
		/// </summary>
		public int Precision { get; set; }

		/// <summary>
		/// 小数点后的数字位数。 
		/// </summary>
		public short Scale { get; set; }

		/// <summary>
		/// 当前字段是否允许为空。指定为 Null 性。1 = 可以为 NULL。0 = NOT NULL。
		/// </summary>
		public bool Nullable { get; set; }

		/// <summary>
		/// 存储过程参数值
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// 存储过程参数输入输出类型。
		/// </summary>
		public ParameterDirection Direction { get; set; }

		///// <summary>
		///// 字段大小是否显示
		///// </summary>
		//[System.ComponentModel.Browsable(false)]
		//public Visibility BracketVisibility
		//{
		//	get
		//	{
		//		switch (DbType)
		//		{
		//			case DbTypeEnum.Binary:
		//			case DbTypeEnum.Char:
		//			case DbTypeEnum.NChar:
		//			case DbTypeEnum.NVarChar:
		//			case DbTypeEnum.VarBinary:
		//			case DbTypeEnum.VarChar:
		//			case DbTypeEnum.Decimal:
		//				return Visibility.Visible;
		//			default:
		//				return Visibility.Collapsed;
		//		}
		//	}
		//}

		///// <summary>
		///// 字段大小是否显示
		///// </summary>
		//[System.ComponentModel.Browsable(false)]
		//public Visibility SizeVisibility
		//{
		//	get
		//	{
		//		switch (DbType)
		//		{
		//			case DbTypeEnum.Binary:
		//			case DbTypeEnum.Char:
		//			case DbTypeEnum.NChar:
		//			case DbTypeEnum.NVarChar:
		//			case DbTypeEnum.VarBinary:
		//			case DbTypeEnum.VarChar:
		//				return Visibility.Visible;
		//			default:
		//				return Visibility.Collapsed;
		//		}
		//	}
		//}

		///// <summary>
		///// 字段精度是否显示
		///// </summary>
		//[System.ComponentModel.Browsable(false)]
		//public Visibility PrecisionVisibility { get { return DbType == DbTypeEnum.Decimal ? Visibility.Visible : Visibility.Collapsed; } }

		///// <summary>
		///// 字段精度是否显示
		///// </summary>
		//[System.ComponentModel.Browsable(false)]
		//public Visibility ScaleVisibility { get { return DbType == DbTypeEnum.Decimal && Scale > 0 ? Visibility.Visible : Visibility.Collapsed; } }
	}
}
