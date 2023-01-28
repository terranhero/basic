using System;
using System.Collections.Generic;
using Basic.Enums;

namespace Basic.DataAccess
{
	/// <summary>
	/// 动态参数
	/// </summary>
	public sealed class DynamicParameter : System.Collections.Generic.IEnumerable<DynamicParameter>
	{
		private readonly List<DynamicParameter> childParameters;

		/// <summary>
		/// 初始化 DynamicParameter 类实例
		/// </summary>
		/// <param name="aPrefix">字段前缀，数据表别名</param>
		/// <param name="name">数据库字段返回名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="oValue">参数值</param>
		public DynamicParameter(string aPrefix, string name, DbTypeEnum dataType, object oValue)
		{
			Prefix = aPrefix; ParameterName = name;
			DataType = dataType; Size = 0;
			Precision = 0; Scale = 0;
			CompareOperator = ParameterCompare.Equal;
			LogicalOperator = LogicalOperator.AND;
			Value = oValue; childParameters = new List<DynamicParameter>(5);
		}

		/// <summary>
		/// 初始化 DynamicParameter 类实例
		/// </summary>
		/// <param name="aPrefix">字段前缀，数据表别名</param>
		/// <param name="name">数据库字段返回名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="oValue">参数值</param>
		public DynamicParameter(string aPrefix, string name, DbTypeEnum dataType, int size, object oValue)
			: this(aPrefix, name, dataType, oValue) { Size = size; }

		/// <summary>
		/// 初始化 DynamicParameter 类实例, 设置Decimal类型数据列
		/// </summary>
		/// <param name="aPrefix">字段前缀，数据表别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">数据库字段长度(decimal类型的精度)</param>
		/// <param name="scale">数据库字段的小数位数</param>
		/// <param name="oValue">参数值</param>
		public DynamicParameter(string aPrefix, string name, DbTypeEnum dataType, byte precision, byte scale, object oValue)
			: this(aPrefix, name, dataType, oValue) { Precision = precision; Scale = scale; }

		/// <summary>
		/// 当前参数与前一参数的逻辑比较符号
		/// </summary>
		public LogicalOperator LogicalOperator { get; set; }

		/// <summary>
		/// 比较符号
		/// </summary>
		public ParameterCompare CompareOperator { get; set; }

		/// <summary>
		/// 字段前缀，数据表别名
		/// </summary>
		public string Prefix { get; private set; }

		/// <summary>
		/// 需要拼接条件的数据库参数
		/// </summary>
		public string ParameterName { get; private set; }

		/// <summary>
		/// 数据库列类型
		/// </summary>
		public DbTypeEnum DataType { get; private set; }

		/// <summary>
		/// 数据库字段的小数位数
		/// </summary>
		public byte Scale { get; private set; }

		/// <summary>
		/// 数据库字段长度
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// 数据库字段长度
		/// </summary>
		public byte Precision { get; private set; }

		/// <summary>
		/// 数据库参数值
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 System.Collections.Generic.IEnumerator&lt;DynamicParameter&gt;。</returns>
		IEnumerator<DynamicParameter> IEnumerable<DynamicParameter>.GetEnumerator()
		{
			if (childParameters != null)
				return childParameters.GetEnumerator();
			return null;
		}

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			if (childParameters != null)
				return childParameters.GetEnumerator();
			return null;
		}
	}
}
