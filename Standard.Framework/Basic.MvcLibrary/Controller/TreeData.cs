using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Text;

namespace Basic.MvcLibrary
{
	/// <summary></summary>
	/// <typeparam name="T"></typeparam>
	public sealed class TreeData<T> : IEnumerable<T>
	{
		private readonly List<T> datas;
		private readonly JsonConverter converter;
		/// <summary>初始化 TreeData 类实例</summary>
		/// <param name="list">一个集合，其元素被复制到新列表中。</param>
		/// <param name="request"></param>
		public TreeData(HttpRequestBase request, IEnumerable<T> list) { converter = new JsonConverter(request); datas = new List<T>(list); }

		private Func<T, object> selectPredicate;
		/// <summary>设置提取子节点的 Lambda 表达式</summary>
		/// <param name="predicate">一个 Lambda 表达式。</param>
		/// <returns>返回当前对象</returns>
		public TreeData<T> Where(Func<IEnumerable<T>, T, IEnumerable<T>> predicate)
		{
			wherePredicate = predicate;
			return this;
		}

		private Func<IEnumerable<T>, T, IEnumerable<T>> wherePredicate;
		/// <summary>设置提取子节点的 Lambda 表达式</summary>
		/// <param name="predicate">一个 Lambda 表达式。</param>
		/// <returns>返回当前对象</returns>
		public TreeData<T> Select(Func<T, object> predicate)
		{
			selectPredicate = predicate;
			return this;
		}

		/// <summary>将对象序列化为符合tree组件需要的数据格式</summary>
		/// <returns>返回tree 组件数据格式</returns>
		public string ToJson()
		{
			IEnumerable<T> tops = wherePredicate.Invoke(datas, default);
			if (tops == null || tops.Any() == false) { return "[]"; }
			StringBuilder builder = new StringBuilder(10000);
			builder.Append("[");
			int length = builder.Length;
			foreach (T row in tops)
			{
				if (builder.Length > length) { builder.Append(","); }
				builder.Append("{");
				if (selectPredicate == null) { builder.Append(converter.Serialize(row, false)); }
				else { builder.Append(converter.Serialize(selectPredicate(row), false)); }
				builder.Append(",\"children\":[");
				SerializeChildren(builder, row);
				builder.Append("]");
				builder.Append("}");
			}
			builder.Append("]");
			return builder.ToString();
		}

		private void SerializeChildren(StringBuilder builder, T item)
		{
			IEnumerable<T> children = wherePredicate.Invoke(datas, item);
			if (children == null || children.Any() == false) { return; }
			int length = builder.Length;
			foreach (T row in children)
			{
				if (builder.Length > length) { builder.Append(","); }
				builder.Append("{");
				if (selectPredicate == null) { builder.Append(converter.Serialize(row, false)); }
				else { builder.Append(converter.Serialize(selectPredicate(row), false)); }
				builder.Append(",\"children\":[");
				SerializeChildren(builder, row);
				builder.Append("]");
				builder.Append("}");
			}
		}

		/// <summary>返回一个循环访问集合的枚举器。</summary>
		/// <returns>用于循环访问集合的枚举数。</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() { return datas.GetEnumerator(); }

		/// <summary>返回一个循环访问集合的枚举器。</summary>
		/// <returns>用于循环访问集合的枚举数。</returns>
		IEnumerator IEnumerable.GetEnumerator() { return datas.GetEnumerator(); }
	}
}
