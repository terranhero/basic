
namespace Basic.Expressions
{
	/// <summary>
	/// 表达式逻辑比较符号
	/// </summary>
	public enum ExpressionCompareEnum
	{
		/// <summary>
		/// 条件 AND 运算，它仅在第一个操作数的计算结果为 true 时才计算第二个操作数，如：a AND b。
		/// </summary>
		AndAlso = 1,
		/// <summary>
		/// 短路条件 OR 运算，如：a OR b。
		/// </summary>
		OrElse,
	}

	internal static class CompareEnumExtersion
	{
		internal static void AppendCompare(this System.Text.StringBuilder builder, ExpressionCompareEnum compare)
		{
			if (compare == ExpressionCompareEnum.AndAlso) { builder.Append(" AND "); }
			else if (compare == ExpressionCompareEnum.OrElse) { builder.Append(" OR "); }
		}

		internal static void AppendExpressionType(this System.Text.StringBuilder builder, ExpressionTypeEnum expressionType)
		{
			if (expressionType == ExpressionTypeEnum.Equal) { builder.Append(" = "); }
			else if (expressionType == ExpressionTypeEnum.NotEqual) { builder.Append(" <> "); }
			else if (expressionType == ExpressionTypeEnum.GreaterThan) { builder.Append(" > "); }
			else if (expressionType == ExpressionTypeEnum.GreaterThanEqual) { builder.Append(" >= "); }
			else if (expressionType == ExpressionTypeEnum.LessThan) { builder.Append(" < "); }
			else if (expressionType == ExpressionTypeEnum.LessThanEqual) { builder.Append(" <= "); }
			else if (expressionType == ExpressionTypeEnum.In) { builder.Append(" IN "); }
			else if (expressionType == ExpressionTypeEnum.NotIn) { builder.Append(" NOT IN "); }
			else if (expressionType == ExpressionTypeEnum.Like) { builder.Append(" LIKE "); }
			else if (expressionType == ExpressionTypeEnum.NotLike) { builder.Append(" NOT LIKE "); }
		}

		internal static void AppendExpressionTypeLeftBracket(this System.Text.StringBuilder builder, ExpressionTypeEnum expressionType)
		{
			if (expressionType == ExpressionTypeEnum.In) { builder.Append("("); }
			else if (expressionType == ExpressionTypeEnum.NotIn) { builder.Append("("); }
			else if (expressionType == ExpressionTypeEnum.Like) { builder.Append("("); }
			else if (expressionType == ExpressionTypeEnum.NotLike) { builder.Append("("); }
		}

		internal static void AppendExpressionTypeRightBracket(this System.Text.StringBuilder builder, ExpressionTypeEnum expressionType)
		{
			if (expressionType == ExpressionTypeEnum.In) { builder.Append(")"); }
			else if (expressionType == ExpressionTypeEnum.NotIn) { builder.Append(")"); }
			else if (expressionType == ExpressionTypeEnum.Like) { builder.Append(")"); }
			else if (expressionType == ExpressionTypeEnum.NotLike) { builder.Append(")"); }
		}
	}
}
