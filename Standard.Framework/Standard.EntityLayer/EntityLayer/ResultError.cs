using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示命令执行过程中出现的错误。
	/// </summary>
	public sealed class ResultError
	{
		private readonly int _Index = -1;
		private readonly string _PropertyName = null;
		private readonly string _Message = null;

		/// <summary>
		/// 根据属性，错误信息初始化 ResultError 类实例。
		/// </summary>
		/// <param name="index">表示模型数组的位置索引。如果不是数组则为 -1。</param>
		/// <param name="propertyName">表示当前属性名称。</param>
		/// <param name="errorMessage">表示错误信息。</param>
		internal ResultError(int index, string propertyName, string errorMessage)
		{
			_Index = index; _PropertyName = propertyName;
			_Message = errorMessage;
		}

		/// <summary>
		/// 根据属性，错误信息初始化 ResultError 类实例。
		/// </summary>
		/// <param name="propertyName">表示当前属性名称。</param>
		/// <param name="errorMessage">表示错误信息。</param>
		internal ResultError(string propertyName, string errorMessage)
			: this(-1, propertyName, errorMessage) { }

		/// <summary>
		/// 获取表示模型数组的位置索引，如果不是数组则为 -1。
		/// </summary>
		/// <value>有关由 CheckCommand 生成的关于位置索引。</value>
		public int Index { get { return _Index; } }

		/// <summary>
		/// 获取引发错误信息的属性名称。
		/// </summary>
		/// <value>有关由 CheckCommand 生成的关于属性名称。</value>
		public string Name { get { return _PropertyName; } }

		/// <summary>
		/// 获取对错误进行描述的文本。
		/// </summary>
		/// <value>有关由 CheckCommand 生成的错误信息。</value>
		public string Message { get { return _Message; } }
	}

	/// <summary>
	/// 表示 ResultError 类集合。
	/// </summary>
	public sealed class ResultErrorCollection : IEnumerable<ResultError>
	{
		private readonly Result _Result = null;
		private readonly List<ResultError> _Errors;
		/// <summary>
		/// 初始化 ResultErrorCollection 类实例
		/// </summary>
		internal ResultErrorCollection(Result result) { _Result = result; _Errors = new List<ResultError>(); }

		/// <summary>将指定 ResultError 集合的元素添加到 ResultErrorCollection 的末尾。</summary>
		/// <param name="errors">表示 ResultError 类型的集合。</param>
		internal void AddErrors(IEnumerable<ResultError> errors) { _Errors.AddRange(errors); }

		/// <summary>
		/// 根据索引号、属性名称、错误信息初始化 ResultError 类实例，并添加到集合末尾。
		/// </summary>
		/// <param name="index">表示模型数组的位置索引。如果不是数组则为 -1。</param>
		/// <param name="propertyName">表示当前属性名称。</param>
		/// <param name="errorMessage">表示错误信息。</param>
		internal ResultError AddError(int index, string propertyName, string errorMessage)
		{
			ResultError error = new ResultError(index, propertyName, errorMessage);
			_Errors.Add(error); return error;
		}

		/// <summary>
		/// 根据属性名称、错误信息初始化 ResultError 类实例，并添加到集合末尾。
		/// </summary>
		/// <param name="propertyName">表示当前属性名称。</param>
		/// <param name="errorMessage">表示错误信息。</param>
		internal ResultError AddError(string propertyName, string errorMessage)
		{
			ResultError error = new ResultError(propertyName, errorMessage);
			_Errors.Add(error); return error;
		}

		/// <summary>
		/// 获取或设置位于指定索引处的元素。
		/// </summary>
		/// <param name="index">要获得或设置的元素从零开始的索引。</param>
		/// <exception cref="System.ArgumentOutOfRangeException">index 小于 0。 - 或 - index 等于或大于 ResultErrorCollection.Count。</exception>
		/// <returns>位于指定索引处的元素。</returns>
		public ResultError this[int index] { get { return _Errors[index]; } set { _Errors[index] = value; } }

		/// <summary>
		/// 从 ResultErrorCollection 中移除所有元素。
		/// </summary>
		public void Clear() { _Errors.Clear(); }

		/// <summary>
		/// 获取 ResultErrorCollection 中实际包含的元素数。
		/// </summary>
		public int Count { get { return _Errors.Count; } }

		/// <summary>
		/// <![CDATA[返回循环访问 IEnumerable<ResultError> 的枚举数。]]>
		/// </summary>
		/// <returns><![CDATA[用于 IEnumerable<ResultError> 的 IEnumerator<ResultError>。]]></returns>
		IEnumerator<ResultError> IEnumerable<ResultError>.GetEnumerator()
		{
			return _Errors.GetEnumerator();
		}

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _Errors.GetEnumerator();
		}
	}
}
