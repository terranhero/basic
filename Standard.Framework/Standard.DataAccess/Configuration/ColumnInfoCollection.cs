using System.Collections;
using System.Collections.Generic;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示数据库列信息
	/// </summary>
	public sealed class ColumnInfoCollection : IEnumerable<ColumnInfo>
	{
		private readonly List<ColumnInfo> _List;
		private readonly TableConfiguration _Table;

		/// <summary>
		/// 初始化 ColumnInfoCollection 了实例。
		/// </summary>
		/// <param name="ti">表示当前拥有此集合的 TableConfiguration 类实例。</param>
		internal ColumnInfoCollection(TableConfiguration ti) { _Table = ti; _List = new List<ColumnInfo>(); }

		/// <summary>
		/// 确定是否存在行记录。
		/// </summary>
		public bool HasRows { get { return _List.Count > 0; } }

		/// <summary>
		/// 创建 ColumnInfo 类实例。
		/// </summary>
		/// <returns>返回创建成功的 ColumnInfo 类实例。</returns>
		internal ColumnInfo CreateColumn()
		{
			return new ColumnInfo(this);
		}

		/// <summary>
		/// 将 ColumnInfo 类实例添加到集合的末尾。
		/// </summary>
		/// <returns>返回添加成功的 ColumnInfo 类实例。</returns>
		internal ColumnInfo Add(ColumnInfo ci)
		{
			_List.Add(ci);
			return ci;
		}

		/// <summary>
		/// <![CDATA[返回循环访问 IEnumerator<ColumnInfo> 的枚举器。]]>
		/// </summary>
		/// <returns><![CDATA[用于 ColumnInfoCollection 的 IEnumerator<ColumnInfo>。]]></returns>
		IEnumerator<ColumnInfo> IEnumerable<ColumnInfo>.GetEnumerator() { return _List.GetEnumerator(); }

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 System.Collections.IEnumerator 对象。</returns>
		IEnumerator IEnumerable.GetEnumerator() { return _List.GetEnumerator(); }
	}
}
