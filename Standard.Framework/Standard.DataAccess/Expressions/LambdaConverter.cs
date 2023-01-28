using System.Linq.Expressions;

namespace Basic.DataAccess
{
	/// <summary>lambda表达式解析器</summary>
	internal sealed class LambdaConverter
	{
		private readonly Expression lambda;
		/// <summary></summary>
		/// <param name="expression"></param>
		public LambdaConverter(Expression expression)
		{
			lambda = expression;
		}

		/// <summary>将Lambda表达式解析成静态命令</summary>
		/// <param name="cmd"></param>
		public void ToWhere(StaticCommand cmd)
		{
			if (lambda is BinaryExpression binary) { }
			else if (lambda is MethodCallExpression) { }
		}
	}
}
