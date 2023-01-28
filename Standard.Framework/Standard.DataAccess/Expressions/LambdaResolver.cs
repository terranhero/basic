using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Exceptions;

namespace Basic.Expressions
{
	/// <summary>
	/// 一元 Lambda 表达式解析器
	/// </summary>
	internal static class LambdaResolver
	{
		/// <summary>
		/// 解析Lambda表达式
		/// </summary>
		/// <param name="lambdaCollection">Lambda结果集合。</param>
		/// <param name="expression">需要解析的Lambda表达式</param>
		public static void AnalyzeExpression(LambdaExpressionCollection lambdaCollection, Expression expression)
		{
			if (expression is MethodCallExpression)
			{
				MethodCallExpression methodCall = expression as MethodCallExpression;
				AnalyzeMethodCallExpression(lambdaCollection, methodCall);
			}
			else if (expression is BinaryExpression)
			{
				BinaryExpression be = expression as BinaryExpression;
				//二元表达式是否包含逻辑比较符号 && 或 ||
				if (be.NodeType == ExpressionType.AndAlso || be.NodeType == ExpressionType.OrElse)
				{
					BinaryConditionExpression bce = lambdaCollection.AddBinaryExpression();
					AnalyzeBinaryExpression(bce, be.Left, be.NodeType, be.Right);
				}
				else
				{
					AnalyzeConditionExpression(lambdaCollection, be);
				}
			}
			else if (expression is MemberExpression)
			{
			}
		}

		#region 单表达式分析
		/// <summary>
		/// 解析静态方法调用lambda表达式
		/// </summary>
		/// <param name="lambdaCollection">Lambda表达式对应数据库参数集合。</param>
		/// <param name="expression">表示调用了 LambdaMethod 中方法的 Lambda 表达式</param>
		private static void AnalyzeMethodCallExpression(LambdaExpressionCollection lambdaCollection, MethodCallExpression expression)
		{
			MethodCallExpression methodCall = expression as MethodCallExpression;
			string methodName = methodCall.Method.Name;
			if (methodName == LambdaExtension.LikeFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.Like);
				if (condition.Value != DBNull.Value && condition.Value != null)
				{
					string temp = (string)condition.Value;
					if (!temp.EndsWith("%")) { temp = string.Concat(temp, "%"); }
					if (!temp.StartsWith("%")) { temp = string.Concat("%", temp); }
					condition.Value = temp;
				}
			}
			else if (methodName == LambdaExtension.NotLikeFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.NotLike);
				if (condition.Value != DBNull.Value && condition.Value != null)
				{
					string temp = (string)condition.Value;
					if (!temp.EndsWith("%")) { temp = string.Concat(temp, "%"); }
					if (!temp.StartsWith("%")) { temp = string.Concat("%", temp); }
					condition.Value = temp;
				}
			}
			else if (methodName == LambdaExtension.LessThanFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.LessThan);
			}
			else if (methodName == LambdaExtension.LessThanOrEqualFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.LessThanEqual);
			}
			else if (methodName == LambdaExtension.GreaterThanFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.GreaterThan);
			}
			else if (methodName == LambdaExtension.GreaterThanOrEqualFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.GreaterThanEqual);
			}
			else if (methodName == LambdaExtension.InFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeInExpression(condition, methodCall, ExpressionTypeEnum.In);
			}
			else if (methodName == LambdaExtension.NotInFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeInExpression(condition, methodCall, ExpressionTypeEnum.NotIn);
			}
			else if (methodName == LambdaExtension.BetweenFunction)
			{
				BetweenExpression condition = lambdaCollection.AddBetweenExpression();
				AnalyzeBetweenExpression(condition, methodCall);
			}
			else if (methodName == LambdaExtension.IsNullFunction)
			{
				ConditionExpression condition = lambdaCollection.AddConditionExpression();
				AnalyzeIsNullExpression(condition, methodCall, ExpressionTypeEnum.IsNull);
			}

		}

		/// <summary>
		/// 解析静态方法调用转换成SQL语句
		/// </summary>
		/// <param name="condition">Lambda表达式对应数据库参数集合。</param>
		/// <param name="mce">表示调用了LambdaMethod.In 或 LambdaMethod.NotIn 的Lambda表达式</param>
		/// <param name="type">表达式方法名称对应的比较符号。</param>
		private static void AnalyzeIsNullExpression(ConditionExpression condition, MethodCallExpression mce, ExpressionTypeEnum type)
		{
			if (mce.Arguments.Count != 1) { throw new ArgumentException("传入参数异常！", "source"); }
			Expression expArg1 = mce.Arguments[0];
			if (!(expArg1 is MemberExpression)) { throw new ArgumentException("传入参数异常！", "source"); }
			MemberExpression memberLeft = expArg1 as MemberExpression;
			ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(memberLeft.Member, typeof(ColumnMappingAttribute));
			if (cma != null)
			{
				condition.SetConditionExpression(cma);
				condition.ExpressionType = type;
			}
			else
			{
				ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(memberLeft.Member, typeof(ColumnAttribute));
				if (ca == null) { throw new AttributeException("ColumnAttribute_NotExists", memberLeft.Member.DeclaringType, memberLeft.Member.Name); }
				DbTypeEnum dataType = DbTypeEnumConverter.ConvertFrom(ca.DataType);
				condition.SetConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Size, ca.Nullable);
				condition.ExpressionType = type;
				return;
			}
		}

		/// <summary>
		/// 解析静态方法调用转换成SQL语句
		/// </summary>
		/// <param name="condition">Lambda表达式对应数据库参数集合。</param>
		/// <param name="mce">表示调用了LambdaMethod.In 或 LambdaMethod.NotIn 的Lambda表达式</param>
		/// <param name="type">表达式方法名称对应的比较符号。</param>
		private static void AnalyzeInExpression(ConditionExpression condition, MethodCallExpression mce, ExpressionTypeEnum type)
		{
			if (mce.Arguments.Count != 2) { throw new ArgumentException("传入参数异常！", "source,likePattern"); }
			Expression expArg1 = mce.Arguments[0];
			Expression expArg2 = mce.Arguments[1];
			if (!(expArg1 is MemberExpression)) { throw new ArgumentException("传入参数异常！", "source"); }
			MemberExpression memberLeft = expArg1 as MemberExpression;
			ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(memberLeft.Member, typeof(ColumnMappingAttribute));
			if (cma != null)
			{
				condition.SetConditionExpression(cma);
				condition.ExpressionType = type;
				condition.Value = AnalyzeExpressionResult(expArg2);
			}
			else
			{
				ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(memberLeft.Member, typeof(ColumnAttribute));
				if (ca == null) { throw new AttributeException("ColumnAttribute_NotExists", memberLeft.Member.DeclaringType, memberLeft.Member.Name); }
				DbTypeEnum dataType = DbTypeEnumConverter.ConvertFrom(ca.DataType);
				condition.SetConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Size, ca.Nullable);
				condition.ExpressionType = type;
				condition.Value = AnalyzeExpressionResult(expArg2);
				return;
			}
		}

		/// <summary>
		/// 解析静态方法调用转换成SQL语句
		/// </summary>
		/// <param name="condition">Lambda 条件表达式。</param>
		/// <param name="mce">表示调用了LambdaMethod.Like 或 LambdaMethod.NotLike 的Lambda表达式</param>
		/// <param name="type">表达式方法名称对应的比较符号。</param>
		private static void AnalyzeStringExpression(ConditionExpression condition, MethodCallExpression mce, ExpressionTypeEnum type)
		{
			if (mce.Arguments.Count < 2) { throw new ArgumentException("传入参数异常！", "source,target"); }
			Expression expArg1 = mce.Arguments[0];
			Expression expArg2 = mce.Arguments[1];
			if (!(expArg1 is MemberExpression)) { throw new ArgumentException("传入参数异常！", "source"); }
			MemberExpression memberLeft = expArg1 as MemberExpression;
			ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(memberLeft.Member, typeof(ColumnMappingAttribute));
			if (cma != null)
			{
				condition.SetConditionExpression(cma);
				condition.ExpressionType = type;
				condition.Value = AnalyzeExpressionResult(expArg2);
			}
			else
			{
				ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(memberLeft.Member, typeof(ColumnAttribute));
				if (ca == null) { throw new AttributeException("ColumnAttribute_NotExists", memberLeft.Member.DeclaringType, memberLeft.Member.Name); }
				DbTypeEnum dataType = DbTypeEnumConverter.ConvertFrom(ca.DataType);
				condition.SetConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Size, ca.Nullable);
				condition.ExpressionType = type;
				condition.Value = AnalyzeExpressionResult(expArg2);
			}
		}

		/// <summary>
		/// 解析静态方法调用转换成SQL语句
		/// </summary>
		/// <param name="condition">Lambda 条件表达式。</param>
		/// <param name="mce">表示调用了LambdaMethod.Between 的Lambda表达式</param>
		private static void AnalyzeBetweenExpression(BetweenExpression condition, MethodCallExpression mce)
		{
			if (mce.Arguments.Count != 3) { throw new ArgumentException("传入参数异常！", "source, fromValue, toValue"); }
			Expression expArg1 = mce.Arguments[0];
			Expression expArg2 = mce.Arguments[1];
			Expression expArg3 = mce.Arguments[2];
			if (!(expArg1 is MemberExpression)) { throw new ArgumentException("传入参数异常！", "source"); }
			MemberExpression memberLeft = expArg1 as MemberExpression;
			ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(memberLeft.Member, typeof(ColumnMappingAttribute));
			if (cma != null)
			{
				condition.SetConditionExpression(cma);
				object bValue = AnalyzeExpressionResult(expArg2);
				object eValue = AnalyzeExpressionResult(expArg3);
				if (bValue == null) { throw new ArgumentNullException(); }
				if (eValue == null) { throw new ArgumentNullException(); }
				condition.Value = bValue;
				condition.ToValue = eValue;
			}
			else
			{
				ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(memberLeft.Member, typeof(ColumnAttribute));
				if (ca == null) { throw new AttributeException("ColumnAttribute_NotExists", memberLeft.Member.DeclaringType, memberLeft.Member.Name); }
				DbTypeEnum dataType = DbTypeEnumConverter.ConvertFrom(ca.DataType);
				condition.SetConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Size, ca.Nullable);
				object arg2Value = AnalyzeExpressionResult(expArg2);
				object arg3Value = AnalyzeExpressionResult(expArg3);
				if (arg2Value == null) { throw new ArgumentNullException(); }
				if (arg3Value == null) { throw new ArgumentNullException(); }
				condition.Value = arg2Value;
				condition.ToValue = arg3Value;
			}
		}
		#endregion

		#region 分析二元 Lambda 表达式

		/// <summary>
		/// 解析静态方法调用lambda表达式
		/// </summary>
		/// <param name="condition">Lambda表达式对应数据库参数集合。</param>
		/// <param name="expression">表示调用了 LambdaMethod 中方法的 Lambda 表达式</param>
		private static void AnalyzeMethodCallExpression(ConditionExpression condition, MethodCallExpression expression)
		{
			MethodCallExpression methodCall = expression as MethodCallExpression;
			string methodName = methodCall.Method.Name;
			if (methodName == LambdaExtension.LikeFunction)
			{
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.Like);
				if (condition.Value != DBNull.Value && condition.Value != null)
				{
					string temp = (string)condition.Value;
					if (!string.IsNullOrEmpty(temp) && !temp.EndsWith("%"))
						condition.Value = string.Concat(temp, "%");
				}
			}
			else if (methodName == LambdaExtension.NotLikeFunction)
			{
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.NotLike);
			}
			else if (methodName == LambdaExtension.LessThanFunction)
			{
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.LessThan);
			}
			else if (methodName == LambdaExtension.LessThanOrEqualFunction)
			{
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.LessThanEqual);
			}
			else if (methodName == LambdaExtension.GreaterThanFunction)
			{
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.GreaterThan);
			}
			else if (methodName == LambdaExtension.GreaterThanOrEqualFunction)
			{
				AnalyzeStringExpression(condition, methodCall, ExpressionTypeEnum.GreaterThanEqual);
			}
			else if (methodName == LambdaExtension.InFunction)
			{
				AnalyzeInExpression(condition, methodCall, ExpressionTypeEnum.In);
			}
			else if (methodName == LambdaExtension.NotInFunction)
			{
				AnalyzeInExpression(condition, methodCall, ExpressionTypeEnum.NotIn);
			}
			else if (methodName == LambdaExtension.IsNullFunction)
			{
				AnalyzeIsNullExpression(condition, methodCall, ExpressionTypeEnum.IsNull);
			}
		}

		/// <summary>
		/// 分析二元 Lambda 表达式
		/// </summary>
		/// <param name="bce">Lambda表达式对应数据库参数集合。</param>
		/// <param name="left">Lambda表达式左边部分</param>
		/// <param name="right">Lambda表达式右边部分</param>
		/// <param name="type">Lambda表达式节点类型</param>
		private static void AnalyzeBinaryExpression(BinaryConditionExpression bce, Expression left, ExpressionType type, Expression right)
		{
			if (left is BinaryExpression)
			{
				BinaryExpression be = left as BinaryExpression;
				if (be.NodeType == ExpressionType.AndAlso || be.NodeType == ExpressionType.OrElse)
				{
					BinaryConditionExpression leftbe = bce.LeftBinaryExpression();
					AnalyzeBinaryExpression(leftbe, be.Left, be.NodeType, be.Right);
				}
				else
				{
					if (be.Left is UnaryExpression) //Lambda 计算表达式，+-*/ & |
					{
						Expression uel = ReduceUnaryExpression(be.Left as UnaryExpression);
						AnalyzeCalculateExpression(bce.LeftCalculateExpression(), uel, be.NodeType, be.Right);
					}
					else if (be.Left is BinaryExpression)
						AnalyzeCalculateExpression(bce.LeftCalculateExpression(), be.Left, be.NodeType, be.Right);
					else
						AnalyzeConditionExpression(bce.LeftConditionExpression(), be.Left, be.NodeType, be.Right);
				}
			}
			else if (left is MethodCallExpression)
			{
				MethodCallExpression mce = left as MethodCallExpression;
				if (mce.Method.Name == LambdaExtension.BetweenFunction)
					AnalyzeBetweenExpression(bce.LeftBetweenExpression(), mce);
				else
					AnalyzeMethodCallExpression(bce.LeftConditionExpression(), mce);
			}
			AnalyzeLogicalExpression(bce, type);
			if (right is BinaryExpression)
			{
				BinaryExpression be = right as BinaryExpression;
				if (be.NodeType == ExpressionType.AndAlso || be.NodeType == ExpressionType.OrElse)
				{
					BinaryConditionExpression rightbe = bce.RightBinaryExpression();
					AnalyzeBinaryExpression(rightbe, be.Left, be.NodeType, be.Right);
				}
				else
				{
					if (be.Left is UnaryExpression) //Lambda 计算表达式，+-*/ & |
					{
						Expression uel = ReduceUnaryExpression(be.Left as UnaryExpression);
						AnalyzeCalculateExpression(bce.RightCalculateExpression(), uel, be.NodeType, be.Right);
					}
					else if (be.Left is BinaryExpression)
						AnalyzeCalculateExpression(bce.RightCalculateExpression(), be.Left, be.NodeType, be.Right);
					else
						AnalyzeConditionExpression(bce.RightConditionExpression(), be.Left, be.NodeType, be.Right);
				}
			}
			else if (right is MethodCallExpression)
			{
				MethodCallExpression mce = right as MethodCallExpression;
				if (mce.Method.Name == LambdaExtension.BetweenFunction)
					AnalyzeBetweenExpression(bce.RightBetweenExpression(), mce);
				else
					AnalyzeMethodCallExpression(bce.RightConditionExpression(), mce);
			}
		}

		/// <summary>
		/// 分析Lambda表达式(二元表达式(m=>m.MemberName==""))
		/// </summary>
		/// <param name="condition">Lambda表达式对应数据库参数集合。</param>
		/// <param name="left">二元表达式左半部分</param>
		/// <param name="type">二元表达式类型</param>
		/// <param name="right">二元表达式右半部分</param>
		private static void AnalyzeConditionExpression(ConditionExpression condition, Expression left, ExpressionType type, Expression right)//, BinaryExpression be)
		{
			if (left is MemberExpression)
			{
				MemberExpression memberLeft = left as MemberExpression;
				MemberInfo mi = memberLeft.Member;
				ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnMappingAttribute));
				ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnAttribute));
				if (cma == null && ca == null) { throw new AttributeException("ColumnAttribute_NotExists", mi.DeclaringType, mi.Name); }
				if (cma != null)
				{
					condition.SetConditionExpression(cma);
					AnalyzeConditionExpressionType(condition, type);
					condition.Value = AnalyzeExpressionResult(right);
				}
				else
				{
					DbTypeEnum dataType = DbTypeEnumConverter.ConvertFrom(ca.DataType);
					if (ca.Precision > 0 || ca.Scale > 0)
						condition.SetConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Precision, ca.Scale, ca.Nullable);
					else
						condition.SetConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Size, ca.Nullable);
					AnalyzeConditionExpressionType(condition, type);
					condition.Value = AnalyzeExpressionResult(right);
				}
			}
			else if (left is MethodCallExpression)
			{
				MethodCallExpression methodCall = left as MethodCallExpression;
				AnalyzeMethodCallExpression(condition, methodCall);
			}
			else if (left is UnaryExpression)
			{
				left = ReduceUnaryExpression(left as UnaryExpression);
				AnalyzeConditionExpression(condition, left, type, right);
			}
		}

		/// <summary>
		/// 分析Lambda表达式(二元表达式(m=>m.MemberName==""))
		/// </summary>
		/// <param name="condition">Lambda 计算表达式。</param>
		/// <param name="left">二元表达式左半部分</param>
		/// <param name="type">二元表达式类型</param>
		/// <param name="right">二元表达式右半部分</param>
		private static void AnalyzeCalculateExpression(CalculateExpression condition, Expression left, ExpressionType type, Expression right)
		{
			if (left is MemberExpression)
			{
				AnalyzeConditionExpression(condition, left, type, right);
			}
			else if (left is BinaryExpression)
			{
				BinaryExpression beLeft = left as BinaryExpression;
				AnalyzeConditionExpression(condition, beLeft.Left, type, right);
				//condition.Constant = condition.Value;
				AnalyzeCalculateExpressionType(condition, beLeft.NodeType);
				condition.Constant = AnalyzeExpressionResult(beLeft.Right);
			}
		}
		#endregion

		/// <summary>
		/// 解析表达式的常量值
		/// </summary>
		/// <param name="expression">需要解析的表达式</param>
		/// <returns>返回解析结果</returns>
		private static object AnalyzeExpressionResult(Expression expression)
		{
			object result = DBNull.Value;
			if (expression is MemberExpression)
				result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
			else if (expression is ConstantExpression)
				result = (expression as ConstantExpression).Value ?? DBNull.Value;
			else if (expression is UnaryExpression)
				result = AnalyzeUnaryExpression(expression as UnaryExpression);
			else if (expression is BinaryExpression)
				result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
			else if (expression is NewExpression)
				result = Expression.Lambda(expression as NewExpression).Compile().DynamicInvoke();
			else
				result = Expression.Lambda(expression).Compile().DynamicInvoke() ?? DBNull.Value;
			if (result != null && result != DBNull.Value && result is string[])
				return string.Concat("'", string.Join("','", result as string[]), "'");
			else if (result != null && result != DBNull.Value && result is System.Guid[])
				return string.Concat("'", string.Join("','", (result as Guid[]).Select(t => t.ToString()).ToArray()), "'");
			else if (result != null && result != DBNull.Value && result is byte[])
				return string.Join(",", result as byte[]);
			else if (result != null && result != DBNull.Value && result is int[])
				return string.Join(",", result as int[]);
			else if (result != null && result != DBNull.Value && result is long[])
				return string.Join(",", result as long[]);
			else if (result != null && result != DBNull.Value && result is short[])
				return string.Join(",", result as short[]);
			else if (result != null && result != DBNull.Value && result is Array)
				return string.Join(",", (result as Array));
			return result;
		}

		/// <summary>
		/// 分析Lambda表达式(二元表达式(m=>m.MemberName==""))
		/// </summary>
		/// <param name="lambdaCollection">Lambda表达式对应数据库参数集合。</param>
		/// <param name="be">Lambda 二元表达式。</param>
		private static void AnalyzeConditionExpression(LambdaExpressionCollection lambdaCollection, BinaryExpression be)
		{
			if (be.Left is MemberExpression)
			{
				MemberExpression memberLeft = be.Left as MemberExpression;
				MemberInfo mi = memberLeft.Member;
				ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnMappingAttribute));
				ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnAttribute));
				if (cma == null && ca == null) { throw new AttributeException("ColumnAttribute_NotExists", mi.DeclaringType, mi.Name); }
				if (cma != null)
				{
					ConditionExpression pe = lambdaCollection.AddConditionExpression(cma);
					AnalyzeConditionExpressionType(pe, be.NodeType);
					pe.Value = AnalyzeExpressionResult(be.Right);
				}
				else
				{
					DbTypeEnum dataType = DbTypeEnumConverter.ConvertFrom(ca.DataType);
					ConditionExpression pe = null;
					if (ca.Size > 0)
						pe = lambdaCollection.AddConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Size, ca.Nullable);
					else
						pe = lambdaCollection.AddConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Precision, ca.Scale, ca.Nullable);
					AnalyzeConditionExpressionType(pe, be.NodeType);
					pe.Value = AnalyzeExpressionResult(be.Right);
				}
				return;
			}
			if (be.Left is UnaryExpression)
			{
				MemberExpression memberLeft = (be.Left as UnaryExpression).Operand as MemberExpression;
				if (memberLeft != null)
				{
					MemberInfo mi = memberLeft.Member;
					ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnMappingAttribute));
					ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnAttribute));
					if (cma == null && ca == null) { throw new AttributeException("ColumnAttribute_NotExists", mi.DeclaringType, mi.Name); }
					if (cma != null)
					{
						ConditionExpression pe = lambdaCollection.AddConditionExpression(cma);
						AnalyzeConditionExpressionType(pe, be.NodeType);
						pe.Value = AnalyzeExpressionResult(be.Right);
					}
					else
					{
						DbTypeEnum dataType = DbTypeEnumConverter.ConvertFrom(ca.DataType);
						ConditionExpression pe = null;
						if (ca.Size > 0)
							pe = lambdaCollection.AddConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Size, ca.Nullable);
						else
							pe = lambdaCollection.AddConditionExpression(ca.TableName, ca.ColumnName, dataType, ca.Precision, ca.Scale, ca.Nullable);
						AnalyzeConditionExpressionType(pe, be.NodeType);
						pe.Value = AnalyzeExpressionResult(be.Right);
					}
					return;
				}
			}
			if (be.Left is MethodCallExpression)
			{
				MethodCallExpression methodCall = be.Left as MethodCallExpression;
				AnalyzeMethodCallExpression(lambdaCollection, methodCall);
			}
			if (be.Left is BinaryExpression)
			{
				CalculateExpression ce = lambdaCollection.AddCalculateExpression();
				BinaryExpression left = be.Left as BinaryExpression;
				AnalyzeConditionExpression(ce, left.Left, left.NodeType, left.Right);
				AnalyzeConditionExpressionType(ce, be.NodeType);
				ce.Value = AnalyzeExpressionResult(be.Right);
			}
		}


		/// <summary>
		/// 解析一元运算符表达式
		/// </summary>
		/// <param name="ue">包含一元运算符的表达式</param>
		/// <returns>返回运算结果</returns>
		private static object AnalyzeUnaryExpression(UnaryExpression ue)
		{
			return Expression.Lambda(ue.Operand).Compile().DynamicInvoke() ?? DBNull.Value;
		}

		/// <summary>
		/// 解析一元运算符表达式
		/// </summary>
		/// <param name="ue">包含一元运算符的表达式</param>
		/// <returns>返回运算结果</returns>
		private static Expression ReduceUnaryExpression(UnaryExpression ue)
		{
			if (ue.Operand is UnaryExpression) { return ReduceUnaryExpression(ue.Operand as UnaryExpression); }
			return ue.Operand;
		}

		/// <summary>
		/// 解析计算符号
		/// </summary>
		/// <param name="ce">解析表达式结果</param>
		/// <param name="expressionType">Lambda表达式节点类型</param>
		private static void AnalyzeCalculateExpressionType(CalculateExpression ce, ExpressionType expressionType)
		{
			switch (expressionType)
			{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
					ce.CalculateType = CalculateTypeEnum.Add;
					break;
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					ce.CalculateType = CalculateTypeEnum.Subtract;
					break;
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					ce.CalculateType = CalculateTypeEnum.Multiply;
					break;
				case ExpressionType.Divide:
					ce.CalculateType = CalculateTypeEnum.Divide;
					break;
				case ExpressionType.And:
					ce.CalculateType = CalculateTypeEnum.And;
					break;
				case ExpressionType.Or:
					ce.CalculateType = CalculateTypeEnum.Or;
					break;
				case ExpressionType.ExclusiveOr:
					ce.CalculateType = CalculateTypeEnum.ExclusiveOr;
					break;
				case ExpressionType.LeftShift:
					ce.CalculateType = CalculateTypeEnum.LeftShift;
					break;
				case ExpressionType.RightShift:
					ce.CalculateType = CalculateTypeEnum.RightShift;
					break;
			}
		}

		/// <summary>
		/// 解析比较条件符号
		/// </summary>
		/// <param name="ce">解析表达式结果</param>
		/// <param name="expressionType">Lambda表达式节点类型</param>
		private static void AnalyzeConditionExpressionType(ConditionExpression ce, ExpressionType expressionType)
		{
			switch (expressionType)
			{
				case ExpressionType.Equal:
					ce.ExpressionType = ExpressionTypeEnum.Equal;
					break;
				case ExpressionType.GreaterThan:
					ce.ExpressionType = ExpressionTypeEnum.GreaterThan;
					break;
				case ExpressionType.GreaterThanOrEqual:
					ce.ExpressionType = ExpressionTypeEnum.GreaterThanEqual;
					break;
				case ExpressionType.LessThan:
					ce.ExpressionType = ExpressionTypeEnum.LessThan;
					break;
				case ExpressionType.LessThanOrEqual:
					ce.ExpressionType = ExpressionTypeEnum.LessThanEqual;
					break;
				case ExpressionType.NotEqual:
					ce.ExpressionType = ExpressionTypeEnum.NotEqual;
					break;
			}
		}

		/// <summary>
		/// 分析逻辑比较符号
		/// </summary>
		/// <param name="lbe">解析表达式结果</param>
		/// <param name="expressionType">Lambda表达式节点类型</param>
		private static void AnalyzeLogicalExpression(BinaryConditionExpression lbe, ExpressionType expressionType)
		{
			switch (expressionType)
			{
				case ExpressionType.AndAlso:
					lbe.ExpressionCompare = ExpressionCompareEnum.AndAlso;
					break;
				case ExpressionType.OrElse:
					lbe.ExpressionCompare = ExpressionCompareEnum.OrElse;
					break;
			}
		}
	}
}
