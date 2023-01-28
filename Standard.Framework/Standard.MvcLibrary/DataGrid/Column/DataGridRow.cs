using System.Collections.Generic;
using System.Linq;
using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示 DataGrid 数据列和标题列的集合
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class DataGridRow<T> : IEnumerable<DataGridColumn<T>> where T : class
	{
		private readonly DataGrid<T> _DataGrid;
		private readonly int _RowIndex = 0;
		private readonly List<DataGridColumn<T>> _Columns = new List<DataGridColumn<T>>(20);
		/// <summary>
		///  初始化 DataGridRow 类实例。
		/// </summary>
		/// <param name="dg"><![CDATA[拥有此实例的 DataGrid<T> 对象。]]></param>
		/// <param name="rowIndex"><![CDATA[当前 DataGridRow<T> 类型实例的索引号。]]></param>
		public DataGridRow(DataGrid<T> dg, int rowIndex) { _DataGrid = dg; _RowIndex = rowIndex; Height = Unit.Empty; }

		/// <summary><![CDATA[获取当前列数组中是否存在冻结列。]]></summary>
		/// <returns>如果存在则返回true，否则返回false。</returns>
		public bool HasFrozenColumns() { return _Columns.Any(m => m.Frozen == true); }

		/// <summary><![CDATA[获取当前列数组中是否存在非冻结列。]]></summary>
		/// <returns>如果存在则返回true，否则返回false。</returns>
		public bool HasNoneFrozenColumns() { return _Columns.Any(m => m.Frozen == false); }

		/// <summary>
		/// <![CDATA[将对象添加到 DataGridRow<T> 的结尾处。]]>
		/// </summary>
		/// <param name="item"><![CDATA[要添加到 DataGridRow<T> 的末尾处的对象。]]></param>
		/// <returns><![CDATA[返回添加成功的 DataGridColumn<T> 对象实例。]]></returns>
		internal DataGridColumn<T> Append(DataGridColumn<T> item) { _Columns.Add(item); return item; }

		/// <summary>显示一个行高度。</summary>
		internal Unit Height { get; private set; }

		/// <summary>用厘米为高度的数字初始化 Height 的新值。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public DataGridRow<T> SetHeight(float height) { Height = new Unit(height, UnitType.Cm); return this; }

		/// <summary>用指定的长度初始化 System.Web.UI.WebControls.Unit 结构的新实例。</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public DataGridRow<T> SetHeight(string height) { Height = new Unit(height); return this; }

		/// <summary>用像素为高度的数字初始化 Height 的新值</summary>
		/// <param name="height">要设置的属性新值</param>
		/// <returns>返回当前对象。</returns>
		public DataGridRow<T> SetHeight(int height) { Height = new Unit(height); return this; }

		/// <summary>
		/// 获取样式名称。
		/// </summary>
		public string CssClass { get { return string.Join(" ", cssClasses.ToArray()); } }
		private readonly List<string> cssClasses = new List<string>(10);
		/// <summary>
		/// 设置数据单元格样式名称 属性值
		/// </summary>
		/// <param name="calssName">设置当前单元格样式</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridRow<T> SetCssClass(string calssName)
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

		/// <summary></summary>
		/// <param name="index"></param>
		/// <returns></returns>
		internal DataGridColumn<T> this[int index]
		{
			get
			{
				if (_Columns.Count == 0) { return null; }
				return _Columns[index];
			}
		}

		/// <summary>
		/// <![CDATA[搜索指定的对象，并返回整个 DataGridRow<T> 中第一个匹配项的从零开始的索引。]]> 
		/// </summary>
		/// <param name="item">要插入的对象。</param>
		internal int IndexOf(DataGridColumn<T> item)
		{
			return _Columns.IndexOf(item);
		}

		/// <summary>
		/// <![CDATA[将元素 DataGridColumn<T> 插入 DataGridRow<T> 的指定索引处。]]> 
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		internal void Insert(int index, DataGridColumn<T> item)
		{
			_Columns.Insert(index, item);
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

	/// <summary>
	/// 表示 DataGridRow 数据头行的集合
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class DataGridRowCollection<T> : IEnumerable<DataGridRow<T>> where T : class
	{
		private readonly DataGrid<T> _DataGrid;
		private readonly List<DataGridColumn<T>> _Columns = new List<DataGridColumn<T>>(100);
		private readonly List<DataGridRow<T>> _Rows = new List<DataGridRow<T>>(50);
		private DataGridRow<T> _CurrentRow;
		/// <summary>
		///  初始化 DataGridRowCollection 类实例。
		/// </summary>
		/// <param name="dg"><![CDATA[拥有此实例的 DataGrid<T> 对象。]]></param>
		public DataGridRowCollection(DataGrid<T> dg)
		{
			_DataGrid = dg; _Rows = new List<DataGridRow<T>>(50);
			_CurrentRow = new DataGridRow<T>(dg, 0);
			_Rows.Add(_CurrentRow);
		}

		/// <summary><![CDATA[获取当前集合中包含的 DataGridColumn<T> 中实际包含的元素数。]]></summary>
		public int Count { get { return _Columns.Count; } }

		/// <summary>
		/// <![CDATA[将元素 DataGridColumn<T> 插入 DataGridRow<T> 的指定索引处。]]> 
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		public void Insert(int index, DataGridColumn<T> item)
		{
			DataGridColumn<T> oldItem = _CurrentRow[index];
			_CurrentRow.Insert(index, item);
			if (oldItem != null)
			{
				int oldIndex = _Columns.IndexOf(oldItem);
				_Columns.Insert(oldIndex, item);
			}
			else
			{
				_Columns.Insert(index, item);
			}
		}

		/// <summary>设置 CssClass 属性值</summary>
		/// <param name="cssName">样式class名称</param>
		/// <returns>返回当前列对象。</returns>
		public DataGridRow<T> SetCssClass(string cssName) { _CurrentRow.SetCssClass(cssName); return _CurrentRow; }

		/// <summary>
		/// <![CDATA[获取当前实例中保存的所有 DataGridColumn<T> 类实例。由多维数组转换成一维数组。]]>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DataGridColumn<T>> GetColumns() { return _Columns; }

		/// <summary><![CDATA[获取当前列数组中是否存在冻结列。]]></summary>
		/// <returns>如果存在则返回true，否则返回false。</returns>
		public bool HasFrozenColumns() { return _Columns.Any(m => m.Frozen == true); }

		/// <summary><![CDATA[获取当前列数组中是否存在非冻结列。]]></summary>
		/// <returns>如果存在则返回true，否则返回false。</returns>
		public bool HasNoneFrozenColumns() { return _Columns.Any(m => m.Frozen == false); }

		/// <summary>
		/// <![CDATA[将对象添加到 DataGridRow<T> 的结尾处。]]>
		/// </summary>
		/// <param name="row"></param>
		/// <param name="item"><![CDATA[要添加到 DataGridRow<T> 的末尾处的对象。]]></param>
		/// <returns><![CDATA[返回添加成功的 DataGridColumn<T> 对象实例。]]></returns>
		internal DataGridColumn<T> Append(DataGridRow<T> row, DataGridColumn<T> item)
		{
			if (!(item is DataGridHeaderColumn<T>)) { _Columns.Add(item); }
			row.Append(item); return item;
		}

		/// <summary>
		/// <![CDATA[将对象添加到 DataGridRow<T> 的结尾处。]]>
		/// </summary>
		/// <param name="item"><![CDATA[要添加到 DataGridRow<T> 的末尾处的对象。]]></param>
		/// <returns><![CDATA[返回添加成功的 DataGridColumn<T> 对象实例。]]></returns>
		internal DataGridColumn<T> Append(DataGridColumn<T> item)
		{
			if (!(item is DataGridHeaderColumn<T>)) { _Columns.Add(item); }
			_CurrentRow.Append(item); return item;
		}

		/// <summary>
		/// <![CDATA[获取一个 DataGridRow<T>对象,该对象表示当前行。]]>
		/// </summary>
		/// <returns><![CDATA[一个 DataGridRow<T>对象,该对象表示当前行。]]></returns>
		internal DataGridRow<T> GetCurrentRow() { return _CurrentRow; }

		/// <summary>
		/// <![CDATA[将对象添加到 DataGridRowCollection<T> 的结尾处。]]>
		/// </summary>
		/// <returns><![CDATA[返回添加成功的 DataGridRow<T> 对象实例。]]></returns>
		internal DataGridRow<T> CreateRow()
		{
			_CurrentRow = new DataGridRow<T>(_DataGrid, _Rows.Count);
			_Rows.Add(_CurrentRow); return _CurrentRow;
		}

		/// <summary>
		/// <![CDATA[返回循环访问 DataGridRowCollection<Row<T>> 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 DataGridRowCollection<T> 的 DataGridRowCollection<T>.Enumerator。]]></returns>
		public IEnumerator<DataGridRow<T>> GetEnumerator()
		{
			return _Rows.GetEnumerator();
		}

		/// <summary>
		/// <![CDATA[返回循环访问 DataGridRowCollection 的枚举数。]]> 
		/// </summary>
		/// <returns><![CDATA[用于 DataGridRowCollection 的 DataGridRowCollection.Enumerator。]]></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _Rows.GetEnumerator();
		}
	}
}
