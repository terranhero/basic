using Basic.EntityLayer;
using Basic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Basic.Tables
{
	/// <summary>
	/// BaseTableType&lt;TR&gt;类的Lambda表达式解析部分。
	/// </summary>
	partial class BaseTableType<TR>
	{
		/// <summary>
		/// 解析Lambda表达式
		/// </summary>
		/// <param name="expression">需要解析的Lambda表达式</param>
		/// <returns>返回解析的结果</returns>
		private string AnalyzeMemberExpression(Expression<Func<TR, object>> expression)
		{
			Expression body = expression.Body;
			StringBuilder textBuilder = new StringBuilder(1000);
			if (body is MethodCallExpression)
			{
				throw new NotImplementedException("方法不支持函数判断，只支持成员属性条件判断。");
			}
			else if (body is MemberExpression)
			{
				MemberExpression member = body as MemberExpression;
				MemberInfo mi = member.Member;
				ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnMappingAttribute));
				if (cma == null) { throw new AttributeException("ColumnAttribute_NotExists", mi.DeclaringType, mi.Name); }
				textBuilder.Append(cma.ColumnName);
			}
			return textBuilder.ToString();
		}

		/// <summary>
		/// 解析Lambda表达式
		/// </summary>
		/// <param name="expression">需要解析的Lambda表达式</param>
		/// <returns>返回解析的结果</returns>
		private string AnalyzeExpression(Expression<Func<TR, bool>> expression)
		{
			Expression body = expression.Body;
			StringBuilder textBuilder = new StringBuilder(1000);
			if (body is MethodCallExpression)
			{
				throw new NotImplementedException("方法不支持函数判断，只支持成员属性条件判断。");
			}
			else if (body is BinaryExpression)
			{
				AnalyzeBinaryExpression(textBuilder, body as BinaryExpression);
			}
			return textBuilder.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="textBuilder"></param>
		/// <param name="expression"></param>
		private void AnalyzeBinaryExpression(StringBuilder textBuilder, BinaryExpression expression)
		{
			if (expression.NodeType == ExpressionType.AndAlso)  //Lambda表达式包含 && 
			{
				textBuilder.Append("(");
				AnalyzeBinaryExpression(textBuilder, expression.Left as BinaryExpression);
				textBuilder.Append(") AND (");
				AnalyzeBinaryExpression(textBuilder, expression.Right as BinaryExpression);
				textBuilder.Append(")");
			}
			else if (expression.NodeType == ExpressionType.OrElse)  //Lambda表达式包含 || 
			{
				textBuilder.Append("(");
				AnalyzeBinaryExpression(textBuilder, expression.Left as BinaryExpression);
				textBuilder.Append(") OR (");
				AnalyzeBinaryExpression(textBuilder, expression.Right as BinaryExpression);
				textBuilder.Append(")");
			}
			else if (expression.NodeType == ExpressionType.Equal)  //Lambda表达式包含 ==
			{
				textBuilder.Append("(");
				bool result = AnalyzeConditionExpression(textBuilder, expression.Left);
				textBuilder.Append(" = ");
				if (result)
					AnalyzeExpressionResult(textBuilder, expression.Right);
				else
					AnalyzeConditionExpression(textBuilder, expression.Right);
				textBuilder.Append(")");
			}
			else if (expression.NodeType == ExpressionType.NotEqual)  //Lambda表达式包含 !=
			{
				textBuilder.Append("(");
				bool result = AnalyzeConditionExpression(textBuilder, expression.Left);
				textBuilder.Append(" <> ");
				if (result)
					AnalyzeExpressionResult(textBuilder, expression.Right);
				else
					AnalyzeConditionExpression(textBuilder, expression.Right);
				textBuilder.Append(")");
			}
			else if (expression.NodeType == ExpressionType.GreaterThan)  //Lambda表达式包含 >
			{
				textBuilder.Append("(");
				bool result = AnalyzeConditionExpression(textBuilder, expression.Left);
				textBuilder.Append(" > ");
				if (result)
					AnalyzeExpressionResult(textBuilder, expression.Right);
				else
					AnalyzeConditionExpression(textBuilder, expression.Right);
				textBuilder.Append(")");
			}
			else if (expression.NodeType == ExpressionType.GreaterThanOrEqual)  //Lambda表达式包含 >=
			{
				textBuilder.Append("(");
				bool result = AnalyzeConditionExpression(textBuilder, expression.Left);
				textBuilder.Append(" >= ");
				if (result)
					AnalyzeExpressionResult(textBuilder, expression.Right);
				else
					AnalyzeConditionExpression(textBuilder, expression.Right);
				textBuilder.Append(")");
			}
			else if (expression.NodeType == ExpressionType.LessThan)  //Lambda表达式包含 <
			{
				textBuilder.Append("(");
				bool result = AnalyzeConditionExpression(textBuilder, expression.Left);
				textBuilder.Append(" < ");
				if (result)
					AnalyzeExpressionResult(textBuilder, expression.Right);
				else
					AnalyzeConditionExpression(textBuilder, expression.Right);
				textBuilder.Append(")");
			}
			else if (expression.NodeType == ExpressionType.LessThanOrEqual)  //Lambda表达式包含 <=
			{
				textBuilder.Append("(");
				bool result = AnalyzeConditionExpression(textBuilder, expression.Left);
				textBuilder.Append(" <= ");
				if (result)
					AnalyzeExpressionResult(textBuilder, expression.Right);
				else
					AnalyzeConditionExpression(textBuilder, expression.Right);
				textBuilder.Append(")");
			}
		}

		/// <summary>
		/// 计算 Lambda 表达式结果。
		/// </summary>
		/// <param name="textBuilder"></param>
		/// <param name="expression"></param>
		private void AnalyzeExpressionResult(StringBuilder textBuilder, Expression expression)
		{
			object result = null;
			if (expression is MemberExpression)
				result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
			else if (expression is MethodCallExpression)
				result = Expression.Lambda(expression as MethodCallExpression).Compile().DynamicInvoke() ?? DBNull.Value;
			else if (expression is ConstantExpression)
				result = (expression as ConstantExpression).Value ?? DBNull.Value;
			else if (expression is UnaryExpression)
				result = Expression.Lambda((expression as UnaryExpression).Operand).Compile().DynamicInvoke() ?? DBNull.Value;
			else if (expression is BinaryExpression)
				result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
			if (result is string) { textBuilder.Append("'").Append(result).Append("'"); }
			else if (result is Guid) { textBuilder.Append("'").Append(result).Append("'"); }
			else if (result is DateTime) { textBuilder.Append("#").Append(result).Append("#"); }
			else { textBuilder.Append(result); }
		}

		/// <summary>
		/// 计算 Lambda 表达式成员变量名称。
		/// </summary>
		/// <param name="textBuilder"></param>
		/// <param name="expression"></param>
		private bool AnalyzeConditionExpression(StringBuilder textBuilder, Expression expression)
		{
			if (expression is UnaryExpression) { expression = (expression as UnaryExpression).Operand; }
			if (expression is MemberExpression)
			{
				MemberExpression member = expression as MemberExpression;
				MemberInfo mi = member.Member;
				ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnMappingAttribute));
				if (cma == null) { throw new AttributeException("ColumnAttribute_NotExists", mi.DeclaringType, mi.Name); }
				textBuilder.Append(cma.ColumnName);
				return true;
			}
			else
			{
				object result = null;
				if (expression is MemberExpression)
					result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
				else if (expression is ConstantExpression)
					result = (expression as ConstantExpression).Value ?? DBNull.Value;
				else if (expression is MethodCallExpression)
					result = Expression.Lambda(expression as MethodCallExpression).Compile().DynamicInvoke() ?? DBNull.Value;
				else if (expression is UnaryExpression)
					result = Expression.Lambda((expression as UnaryExpression).Operand).Compile().DynamicInvoke() ?? DBNull.Value;
				else if (expression is BinaryExpression)
					result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
				if (result is string) { textBuilder.Append("'").Append(result).Append("'"); }
				else if (result is Guid) { textBuilder.Append("'").Append(result).Append("'"); }
				else if (result is DateTime) { textBuilder.Append("#").Append(result).Append("#"); }
				else { textBuilder.Append(result); }
				return false;
			}
		}
	}
}
