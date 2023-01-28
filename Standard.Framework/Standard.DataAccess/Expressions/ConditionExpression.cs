using Basic.EntityLayer;

using Basic.Enums;

namespace Basic.Expressions
{
	/// <summary>
	/// lambda 表达式参数解析结果。
	/// </summary>
	public class ConditionExpression : LambdaConditionExpression
	{
		/// <summary>
		/// 初始化 ConditionExpression 类实例
		/// </summary>
		/// <param name="owner">拥有此表达式参数的 LambdaExpressionCollection 类实例。</param>
		public ConditionExpression(LambdaExpressionCollection owner)
			: base(owner)
		{
			ExpressionType = ExpressionTypeEnum.Equal;
			DataType = DbTypeEnum.Int32;
			Size = 0;
			Precision = 0;
			Scale = 0;
		}

		/// <summary>
		/// 初始化 ConditionExpression 类实例
		/// </summary>
		/// <param name="owner">拥有此表达式参数的 LambdaExpressionCollection 类实例。</param>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">表示 Value 属性的最大位数。</param>
		/// <param name="scale">数据库字段的小数位数</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public ConditionExpression(LambdaExpressionCollection owner,
			string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
			: base(owner)
		{
			ExpressionType = ExpressionTypeEnum.Equal;
			SetConditionExpression(tableAlias, name, dataType, nullable);
			Size = 0;
			Precision = precision;
			Scale = scale;
		}

		/// <summary>
		/// 初始化 ConditionExpression 类实例
		/// </summary>
		/// <param name="owner">拥有此表达式参数的 LambdaExpressionCollection 类实例。</param>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public ConditionExpression(LambdaExpressionCollection owner,
			string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable)
			: base(owner)
		{
			ExpressionType = ExpressionTypeEnum.Equal;
			SetConditionExpression(tableAlias, name, dataType, nullable);
			Size = size;
			Precision = 0;
			Scale = 0;
		}

		private void SetConditionExpression(string tableAlias, string name, DbTypeEnum dataType, bool nullable)
		{
			TableAlias = tableAlias;
			ColumnName = name;
			ParameterName = ownerCollection.GetParameterName(name);
			DataType = dataType;
			Nullable = nullable;
		}

		/// <summary>
		/// 初始化ExpressionParemeter类实例
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">表示 Value 属性的最大位数。</param>
		/// <param name="scale">数据库字段的小数位数</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public void SetConditionExpression(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
		{
			SetConditionExpression(tableAlias, name, dataType, nullable);
			Size = 0;
			Precision = precision;
			Scale = scale;
		}

		/// <summary>
		/// 初始化ExpressionParemeter类实例
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		public void SetConditionExpression(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable)
		{
			SetConditionExpression(tableAlias, name, dataType, nullable);
			Size = size;
			Precision = 0;
			Scale = 0;
		}

		/// <summary>
		/// 初始化ExpressionParemeter类实例
		/// </summary>
		/// <param name="ca">当前字段信息。</param>
		public void SetConditionExpression(ColumnAttribute ca)
		{
			DbTypeEnum dataType = DbTypeEnumConverter.ConvertFrom(ca.DataType);
			SetConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Nullable);
			Size = ca.Size;
			Precision = ca.Precision;
			Scale = ca.Scale;
		}

		/// <summary>
		/// 初始化ExpressionParemeter类实例
		/// </summary>
		/// <param name="cma">当前字段信息。</param>
		public void SetConditionExpression(ColumnMappingAttribute cma)
		{
			SetConditionExpression(cma.TableAlias, cma.SourceColumn, cma.DataType, cma.Nullable);
			Size = cma.Size;
			Precision = cma.Precision;
			Scale = cma.Scale;
		}

		/// <summary>
		/// 当前 Lambda 表达式的比较操作符号
		/// </summary>
		public virtual ExpressionTypeEnum ExpressionType { get; set; }

		/// <summary>
		/// 数据库表名称或别名
		/// </summary>
		public virtual string TableAlias { get; private set; }

		/// <summary>
		/// 数据库列名称
		/// </summary>
		public virtual string ColumnName { get; private set; }

		/// <summary>
		/// 是否是否允许为空
		/// </summary>
		public virtual bool Nullable { get; private set; }

		/// <summary>
		/// 数据库列类型
		/// </summary>
		public virtual DbTypeEnum DataType { get; private set; }

		/// <summary>
		/// 数据库字段的小数位数
		/// </summary>
		public virtual byte Scale { get; private set; }

		/// <summary>
		/// 数据库字段长度
		/// </summary>
		public virtual int Size { get; private set; }

		/// <summary>
		/// 数据库字段长度
		/// </summary>
		public virtual byte Precision { get; private set; }

		/// <summary>
		/// 参数名称
		/// </summary>
		public virtual string ParameterName { get; set; }

		/// <summary>
		/// 参数值
		/// </summary>
		public virtual object Value { get; set; }
	}

}
