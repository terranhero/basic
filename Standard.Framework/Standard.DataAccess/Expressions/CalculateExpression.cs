using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.EntityLayer;

using Basic.Enums;

namespace Basic.Expressions
{
	/// <summary>
	/// 计算 Lambda 表达式
	/// </summary>
	public sealed class CalculateExpression : ConditionExpression
	{
		/// <summary>
		/// 初始化CalculateExpression类实例
		/// </summary>
		/// <param name="owner">拥有此表达式参数的 LambdaExpressionCollection 类实例。</param>
		public CalculateExpression(LambdaExpressionCollection owner) : base(owner) { }

		/// <summary>
		/// 初始化CalculateExpression类实例
		/// </summary>
		/// <param name="owner">拥有此表达式参数的 LambdaExpressionCollection 类实例。</param>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">表示 Value 属性的最大位数。</param>
		/// <param name="scale">数据库字段的小数位数</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public CalculateExpression(LambdaExpressionCollection owner,
			string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
			: base(owner, tableAlias, name, dataType, precision, scale, nullable) { }

		/// <summary>
		/// 初始化CalculateExpression类实例
		/// </summary>
		/// <param name="owner">拥有此表达式参数的 LambdaExpressionCollection 类实例。</param>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public CalculateExpression(LambdaExpressionCollection owner,
			string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable)
			: base(owner, tableAlias, name, dataType, size, nullable) { }

		/// <summary>
		/// 计算类型
		/// </summary>
		public  CalculateTypeEnum CalculateType { get; set; }

		/// <summary>
		/// 计算常量
		/// </summary>
		public  object Constant { get; set; }
	}
}
