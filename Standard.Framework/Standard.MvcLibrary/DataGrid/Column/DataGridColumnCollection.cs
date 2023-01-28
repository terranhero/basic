using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Basic.EasyLibrary
{
	/// <summary>表示 DataGrid 数据列和标题列的集合</summary>
	/// <typeparam name="T"></typeparam>
	[SuppressMessage("CodeQuality", "IDE0052:删除未读的私有成员", Justification = "<挂起>")]
	public sealed class DataGridColumnCollection<T> : IEnumerable<DataGridColumn<T>> where T : class
	{
		private readonly DataGridColumn<T> mOwner;
		private readonly int _RowIndex = 0;
		private readonly List<DataGridColumn<T>> _Columns = new List<DataGridColumn<T>>(20);

		/// <summary>初始化 DataGridColumnCollection 类实例</summary>
		/// <param name="column"><![CDATA[拥有此实例的 DataGridColumn<T> 对象。]]></param>
		public DataGridColumnCollection(DataGridColumn<T> column) { mOwner = column; }

		/// <summary>
		/// <![CDATA[将对象添加到 DataGridRow<T> 的结尾处。]]>
		/// </summary>
		/// <param name="item"><![CDATA[要添加到 DataGridRow<T> 的末尾处的对象。]]></param>
		/// <returns><![CDATA[返回添加成功的 DataGridColumn<T> 对象实例。]]></returns>
		internal DataGridColumn<T> Append(DataGridColumn<T> item) { _Columns.Add(item); return item; }

		/// <summary>判断是否存在列信息</summary>
		public bool HasColumns { get { return _Columns.Count == 0; } }

		/// <summary>
		/// <![CDATA[将元素 DataGridColumn<T> 插入 DataGridColumnCollection<T> 的指定索引处。]]> 
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		internal void Insert(int index, DataGridColumn<T> item)
		{
			_Columns.Insert(index, item);
		}

		/// <summary>获取样式名称</summary>
		public string CssClass { get { return string.Join(" ", cssClasses.ToArray()); } }
		private readonly List<string> cssClasses = new List<string>(10);

		/// <summary>设置数据单元格样式名称 属性值</summary>
		/// <param name="calssName">设置当前单元格样式</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridColumnCollection<T> SetCssClass(string calssName)
		{
			if (cssClasses.Contains(calssName) == false) { cssClasses.Add(calssName); }
			return this;
		}

		/// <summary>获取当前数据单元格样式名称是否为空。</summary>
		public bool HasCssClass { get { return cssClasses.Count > 0; } }

		/// <summary>判断当前行是否包含指定的样式类。</summary>
		public bool HasClass(string className)
		{
			return cssClasses.Contains(className);
		}

		/// <summary>
		/// 表示当前行索引
		/// </summary>
		public int Index { get { return _RowIndex; } }

		/// <summary>
		/// <![CDATA[返回循环访问 DataGridRow<DataGridColumn<T>> 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 DataGridRow<T> 的 DataGridRow<T>.Enumerator。]]></returns>
		public IEnumerator<DataGridColumn<T>> GetEnumerator()
		{
			return _Columns.GetEnumerator();
		}

		/// <summary>
		/// <![CDATA[返回循环访问 DataGridRow 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 DataGridRow 的 DataGridRow.Enumerator。]]></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _Columns.GetEnumerator();
		}
	}
}
