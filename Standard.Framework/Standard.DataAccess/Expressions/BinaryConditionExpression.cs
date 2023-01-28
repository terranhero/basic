using System.Collections.Generic;
using System.Collections.ObjectModel;
using Basic.EntityLayer;


namespace Basic.Expressions
{
	/// <summary>
	/// lambda 表达式参数解析结果。
	/// </summary>
	public sealed class BinaryConditionExpression : LambdaConditionExpression
	{
		/// <summary>
		/// 初始化 ExpressionCollection 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		/// <param name="owner">拥有此表达式参数的 LambdaExpressionCollection 类实例。</param>
		public BinaryConditionExpression(LambdaExpressionCollection owner)
			: base(owner) { ExpressionCompare = ExpressionCompareEnum.AndAlso; }

		/// <summary>
		/// 当前 Lambda 表达式需要与下一个表达式进行逻辑操作
		/// </summary>
		public ExpressionCompareEnum ExpressionCompare { get; set; }

		/// <summary>
		/// 当前 Lambda 表达式需要与下一个表达式进行逻辑操作
		/// </summary>
		public LambdaConditionExpression LeftExpression { get; set; }

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <returns>ConditionExpression 类实例</returns>
		public ConditionExpression LeftConditionExpression()
		{
			ConditionExpression binaryExpression = new ConditionExpression(ownerCollection);
			LeftExpression = binaryExpression;
			return binaryExpression;
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <returns>ConditionExpression 类实例</returns>
		public CalculateExpression LeftCalculateExpression()
		{
			CalculateExpression binaryExpression = new CalculateExpression(ownerCollection);
			LeftExpression = binaryExpression;
			return binaryExpression;
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <returns>ConditionExpression 类实例</returns>
		public BetweenExpression LeftBetweenExpression()
		{
			BetweenExpression binaryExpression = new BetweenExpression(ownerCollection);
			LeftExpression = binaryExpression;
			return binaryExpression;
		}

		/// <summary>
		/// 向表达式集合中添加 BinaryConditionExpression 类实例，添加成功后返回 ExpressionCollection 类实例。
		/// </summary>
		/// <returns>ExpressionCollection 类实例</returns>
		public BinaryConditionExpression LeftBinaryExpression()
		{
			BinaryConditionExpression binaryExpression = new BinaryConditionExpression(ownerCollection);
			LeftExpression = binaryExpression;
			return binaryExpression;
		}

		/// <summary>
		/// 当前 Lambda 表达式需要与下一个表达式进行逻辑操作
		/// </summary>
		public LambdaConditionExpression RightExpression { get; set; }

		/// <summary>
		/// 向表达式集合中添加 BinaryConditionExpression 类实例，添加成功后返回 ExpressionCollection 类实例。
		/// </summary>
		/// <returns>ExpressionCollection 类实例</returns>
		public BinaryConditionExpression RightBinaryExpression()
		{
			BinaryConditionExpression binaryExpression = new BinaryConditionExpression(ownerCollection);
			RightExpression = binaryExpression;
			return binaryExpression;
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <returns>ConditionExpression 类实例</returns>
		public ConditionExpression RightConditionExpression()
		{
			ConditionExpression binaryExpression = new ConditionExpression(ownerCollection);
			RightExpression = binaryExpression;
			return binaryExpression;
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <returns>ConditionExpression 类实例</returns>
		public CalculateExpression RightCalculateExpression()
		{
			CalculateExpression binaryExpression = new CalculateExpression(ownerCollection);
			RightExpression = binaryExpression;
			return binaryExpression;
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <returns>ConditionExpression 类实例</returns>
		public BetweenExpression RightBetweenExpression()
		{
			BetweenExpression binaryExpression = new BetweenExpression(ownerCollection);
			RightExpression = binaryExpression;
			return binaryExpression;
		}
	}
}
