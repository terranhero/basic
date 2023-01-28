using Basic.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 提供用于从表达式中获取模型名称的帮助器类。
	/// </summary>
	public static class LambdaHelper
	{
		/// <summary>
		/// 从 lambda 表达式中获取 EntityPropertyDescriptor 属性对象
		/// </summary>
		/// <param name="expression">Lambda 表达式。</param>
		/// <returns>返回 Lambda 表达式指定的属性对象。</returns>
		public static EntityPropertyMeta GetProperty(LambdaExpression expression)
		{
			MemberExpression member = GetMember(expression.Body);
			EntityPropertyProvidor.TryGetProperty(member.Member.DeclaringType, member.Member.Name, out EntityPropertyMeta propertyInfo);
			return propertyInfo;
		}

		/// <summary>
		/// 获取 Lambda 表示式属性成员表达式。
		/// </summary>
		/// <param name="body">Lambda 表达式主体。</param>
		/// <returns>返回解析结果，表示成员 Lambda 表达式</returns>
		public static MemberExpression GetMember(Expression body)
		{
			if (body is UnaryExpression)
			{
				return (body as UnaryExpression).Operand as MemberExpression;
			}
			else if (body is ConditionalExpression)
			{
				return (body as ConditionalExpression).Test as MemberExpression;
			}
			else if (body is BinaryExpression)
			{
				return (body as BinaryExpression).ReduceExtensions() as MemberExpression;
			}
			else if (body is MethodCallExpression)
			{
				MethodCallExpression mce = body as MethodCallExpression;
				return mce.ReduceExtensions() as MemberExpression;
			}
			return body as MemberExpression;
		}

		/// <summary>
		/// 获取 Lambda 表示式属性成员表达式。
		/// </summary>
		/// <param name="body">Lambda 表达式主体。</param>
		/// <returns>返回解析结果，表示成员 Lambda 表达式</returns>
		public static string GetMemberName(Expression body)
		{
			MemberExpression member;
			if (body is UnaryExpression)
			{
				member = ((body as UnaryExpression).Operand as MemberExpression);
			}
			else if (body is ConditionalExpression)
			{
				member = (body as ConditionalExpression).Test as MemberExpression;
			}
			else if (body is System.Linq.Expressions.BinaryExpression)
			{
				member = (body as BinaryExpression).ReduceExtensions() as MemberExpression;
			}
			else if (body is MethodCallExpression)
			{
				MethodCallExpression mce = body as MethodCallExpression;
				member = mce.ReduceExtensions() as MemberExpression;
			}
			member = body as MemberExpression;
			if (member != null) { return member.Member.Name; }
			return body.ToString();
		}
	}
}
