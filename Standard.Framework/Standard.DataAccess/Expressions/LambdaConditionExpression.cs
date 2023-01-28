

namespace Basic.Expressions
{
	/// <summary>
	/// 抽象Lambda 表达式解析结果
	/// </summary>
	public abstract class LambdaConditionExpression
	{
		/// <summary>
		/// 
		/// </summary>
		protected readonly LambdaExpressionCollection ownerCollection = null;
		/// <summary>
		/// 初始化 LambdaConditionExpression 类实例
		/// </summary>
		/// <param name="owner">拥有此表达式参数的 LambdaExpressionCollection 类实例。</param>
		protected LambdaConditionExpression(LambdaExpressionCollection owner)
		{
			ownerCollection = owner;
		}

		///// <summary>
		///// 基于Lambda 表达式创建查询条件
		///// </summary>
		///// <param name="dynamicCommand">执行此次查询的动态命令 DynamicCommand 子类实例。</param>
		///// <param name="builder">需要创建Where条件的字符串缓冲。</param>
		//internal abstract void CreateWhere(DynamicCommand dynamicCommand, System.Text.StringBuilder builder);

		///// <summary>
		///// 基于Lambda 表达式创建查询参数
		///// </summary>
		///// <param name="dynamicCommand">执行此次查询的动态命令 DynamicCommand 子类实例。</param>
		//internal abstract void CreateParameter(DynamicCommand dynamicCommand);
	}

	///// <summary>
	///// 抽象Lambda 表达式解析结果
	///// </summary>
	//internal interface IBinaryConditionExpression : ILambdaExpression
	//{
	//    /// <summary>
	//    /// 当前 Lambda 表达式需要与下一个表达式进行逻辑操作
	//    /// </summary>
	//    ExpressionCompareEnum ExpressionCompare { get; set; }

	//    /// <summary>
	//    /// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
	//    /// </summary>
	//    /// <param name="cma">字段描述</param>
	//    /// <returns>ConditionExpression 类实例</returns>
	//    ConditionExpression AddConditionExpression(ColumnMappingAttribute cma);

	//    /// <summary>
	//    /// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
	//    /// </summary>
	//    /// <param name="tableAlias">当前字段所属表名称或别名</param>
	//    /// <param name="name">数据库字段名称</param>
	//    /// <param name="dataType">数据库字段类型</param>
	//    /// <param name="precision">表示 Value 属性的最大位数。</param>
	//    /// <param name="scale">数据库字段的小数位数</param>
	//    /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
	//    /// <returns>ConditionExpression 类实例</returns>
	//    ConditionExpression AddConditionExpression(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable);

	//    /// <summary>
	//    /// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
	//    /// </summary>
	//    /// <param name="tableAlias">当前字段所属表名称或别名</param>
	//    /// <param name="name">数据库字段名称</param>
	//    /// <param name="dataType">数据库字段类型</param>
	//    /// <param name="size">数据的最大大小，以字节为单位。</param>
	//    /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
	//    /// <returns>ConditionExpression 类实例</returns>
	//    ConditionExpression AddConditionExpression(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable);

	//    /// <summary>
	//    /// 向表达式集合中添加 BetweenExpression 类实例，添加成功后返回 BetweenExpression 类实例。
	//    /// </summary>
	//    /// <param name="cma">字段描述</param>
	//    /// <returns>BetweenExpression 类实例</returns>
	//    BetweenExpression AddBetweenExpression(ColumnMappingAttribute cma);

	//    /// <summary>
	//    /// 向表达式集合中添加 BetweenExpression 类实例，添加成功后返回 BetweenExpression 类实例。
	//    /// </summary>
	//    /// <param name="tableAlias">当前字段所属表名称或别名</param>
	//    /// <param name="name">数据库字段名称</param>
	//    /// <param name="dataType">数据库字段类型</param>
	//    /// <param name="precision">表示 Value 属性的最大位数。</param>
	//    /// <param name="scale">数据库字段的小数位数</param>
	//    /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
	//    /// <returns>BetweenExpression 类实例</returns>
	//    BetweenExpression AddBetweenExpression(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable);

	//    /// <summary>
	//    /// 向表达式集合中添加 BetweenExpression 类实例，添加成功后返回 BetweenExpression 类实例。
	//    /// </summary>
	//    /// <param name="tableAlias">当前字段所属表名称或别名</param>
	//    /// <param name="name">数据库字段名称</param>
	//    /// <param name="dataType">数据库字段类型</param>
	//    /// <param name="size">数据的最大大小，以字节为单位。</param>
	//    /// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
	//    /// <returns>BetweenExpression 类实例</returns>
	//    BetweenExpression AddBetweenExpression(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable);

	//    /// <summary>
	//    /// 向表达式集合中添加ExpressionCollection 类实例，添加成功后返回 ExpressionCollection 类实例。
	//    /// </summary>
	//    /// <returns>ExpressionCollection 类实例</returns>
	//    IBinaryConditionExpression AddExpressionCollection();
	//}
}
