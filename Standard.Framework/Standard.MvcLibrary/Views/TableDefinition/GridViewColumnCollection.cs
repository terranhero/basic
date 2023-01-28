using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Basic.Collections;
using Basic.EntityLayer;

namespace Basic.MvcLibrary
{
	/// <summary>表示 el-table 列集合信息定义</summary>
	/// <typeparam name="T"></typeparam>
	public sealed class GridViewColumnCollection<T> : IEnumerable<GridViewColumn<T>>, IColumnsProvider<T> where T : class
	{
		private readonly GridViewColumn<T> mOwner;
		private readonly EntityPropertyCollection mProperties;
		private readonly List<GridViewColumn<T>> mColumns = new List<GridViewColumn<T>>(20);
		private readonly IBasicContext mBasic;
		/// <summary>初始化 GridViewColumnCollection 类实例。</summary>
		/// <param name="bh"></param>
		/// <param name="column">拥有此集合的列信息</param>
		/// <param name="props"></param>
		internal GridViewColumnCollection(IBasicContext bh, EntityPropertyCollection props, GridViewColumn<T> column)
		{
			mProperties = props; mOwner = column; mBasic = bh;
		}

		/// <summary>表示基础扩展类，包装有Http请求的上下文信息和用户信息</summary>
		public IBasicContext Basic { get { return mBasic; } }

		/// <summary>
		/// <![CDATA[返回循环访问 GridViewColumnCollection<T> 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 GridViewColumnCollection<T> 的 IEnumerable<T>.Enumerator。]]></returns>
		IEnumerator<GridViewColumn<T>> IEnumerable<GridViewColumn<T>>.GetEnumerator()
		{
			return mColumns.GetEnumerator();
		}

		/// <summary>
		/// <![CDATA[返回循环访问 GridViewColumnCollection 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 GridViewColumnCollection 的 GridViewColumnCollection.Enumerator。]]></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return mColumns.GetEnumerator();
		}

		GridViewHeaderColumn<T> IColumnsProvider<T>.HeaderFor<TR>(Expression<System.Func<T, TR>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body);
			string name = memberExpression == null ? null : memberExpression.Member.Name;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			GridViewHeaderColumn<T> header = new GridViewHeaderColumn<T>(mBasic, mProperties);
			mColumns.Add(header);


			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					header.Title(wda.DisplayName);
				else
					header.Title(converterName, wda.DisplayName);
			}
			else if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				header.Title(propertyInfo.Name);
			else
				header.Title(propertyInfo.DisplayName);

			return header;
		}

		GridViewHeaderColumn<T> IColumnsProvider<T>.HeaderFor(string expression)
		{
			GridViewHeaderColumn<T> header = new GridViewHeaderColumn<T>(mBasic, mProperties);
			header.Title(expression);
			mColumns.Add(header);
			return header;
		}

		GridViewHeaderColumn<T> IColumnsProvider<T>.HeaderFor(string converterName, string name)
		{
			GridViewHeaderColumn<T> header = new GridViewHeaderColumn<T>(mBasic, mProperties);
			header.Title(converterName, name);
			mColumns.Add(header);
			return header;
		}

		GridViewColumn<T, RT> IColumnsProvider<T>.LabelFor<RT>(Expression<System.Func<T, RT>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			GridViewColumn<T, RT> column = new GridViewColumn<T, RT>(mBasic, mProperties, name, expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.Title(propertyInfo.Name);
			else
				column.Title(propertyInfo.DisplayName);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.Title(wda.DisplayName);
				else
					column.Title(converterName, wda.DisplayName);
			}
			mColumns.Add(column);
			return column;
		}

		GridViewColumn<T, Enum> IColumnsProvider<T>.EnumFor(Expression<System.Func<T, Enum>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			GridViewColumn<T, Enum> column = new GridViewColumn<T, Enum>(mBasic, mProperties, name + "Text", expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.Title(propertyInfo.Name);
			else
				column.Title(propertyInfo.DisplayName);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.Title(wda.DisplayName);
				else
					column.Title(converterName, wda.DisplayName);
			}
			mColumns.Add(column);
			return column;
		}

		GridViewColumn<T, bool> IColumnsProvider<T>.BooleanFor(Expression<System.Func<T, bool>> expression)
		{
			MemberExpression memberExpression = LambdaHelper.GetMember(expression.Body) as MemberExpression;
			string name = memberExpression == null ? null : memberExpression.Member.Name;
			Type declaringType = memberExpression == null ? null : memberExpression.Expression.Type;

			mProperties.TryGetProperty(name, out EntityPropertyMeta propertyInfo);

			GridViewColumn<T, bool> column = new GridViewColumn<T, bool>(mBasic, mProperties, name + "Text", expression.Compile(), propertyInfo);
			if (string.IsNullOrWhiteSpace(propertyInfo.DisplayName))
				column.Title(propertyInfo.Name);
			else
				column.Title(propertyInfo.DisplayName);

			if (propertyInfo.Display != null)
			{
				WebDisplayAttribute wda = propertyInfo.Display;
				string converterName = wda.ConverterName;
				if (string.IsNullOrWhiteSpace(converterName))
					column.Title(wda.DisplayName);
				else
					column.Title(converterName, wda.DisplayName);
			}
			mColumns.Add(column);
			return column;
		}

		GridViewButtonColumn<T> IColumnsProvider<T>.ButtonsFor(Action<IButtonsProvider<T>> expression, params int[] codes)
		{
			bool isAuthorization = true; if (codes == null) { codes = new int[0]; }
			foreach (int code in codes) { isAuthorization = isAuthorization || AuthorizeContext.CheckAuthorizationCode(mBasic, code); }
			if (isAuthorization == false) { return new GridViewButtonColumn<T>(mBasic, mProperties); }
			GridViewButtonColumn<T> column = new GridViewButtonColumn<T>(mBasic, mProperties);
			column.ClassName("el-table-column--buttons");
			expression.Invoke(column);
			mColumns.Add(column);
			return column;
		}

		GridViewButtonColumn<T> IColumnsProvider<T>.ButtonsFor(Action<IButtonsProvider<T>> expression)
		{
			GridViewButtonColumn<T> column = new GridViewButtonColumn<T>(mBasic, mProperties);
			column.ClassName("el-table-column-btns");
			expression.Invoke(column);
			mColumns.Add(column);
			return column;
		}

		/// <summary>使用 Lambda 表达式创建列字段。</summary>
		/// <param name="authorized">是否授权显示此列</param>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns>返回生成的列对象</returns>
		GridViewButtonColumn<T> IColumnsProvider<T>.ButtonsFor(bool authorized, Action<IButtonsProvider<T>> expression)
		{
			if (authorized == false) { return new GridViewButtonColumn<T>(mBasic, mProperties); }
			GridViewButtonColumn<T> column = new GridViewButtonColumn<T>(mBasic, mProperties);
			column.ClassName("el-table-column-btns");
			expression.Invoke(column);
			mColumns.Add(column);
			return column;
		}

		///// <summary>使用 Lambda 表达式创建列字段。</summary>
		///// <param name="authorizeCode">是否根据授权码确定显示此列</param>
		///// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		///// <returns>返回生成的列对象</returns>
		//GridViewButtonColumn<T> IColumnsProvider<T>.ButtonsFor(int authorizeCode, Action<IButtonsProvider<T>> expression)
		//{
		//	if (authorized == false) { return new GridViewButtonColumn<T>(mBasic, mProperties); }
		//	GridViewButtonColumn<T> column = new GridViewButtonColumn<T>(mBasic, mProperties);
		//	column.ClassName("el-table-column-btns");
		//	expression.Invoke(column);
		//	mColumns.Add(column);
		//	return column;
		//}

	}

}
