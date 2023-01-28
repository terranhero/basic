using System.Collections.Generic;
using System.Collections.Specialized;
using Basic.Database;

namespace Basic.Collections
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class DesignColumnCollection : BaseCollection<DesignColumnInfo>,
		ICollection<DesignColumnInfo>, IEnumerable<DesignColumnInfo>, INotifyCollectionChanged
	{
		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		internal const string XmlElementName = "TableColumns";
		private readonly DesignTableInfo tableInfo;

		/// <summary>
		/// 初始化 DesignColumnCollection 类的新实例。
		/// </summary>
		internal DesignColumnCollection(DesignTableInfo table) : base() { tableInfo = table; }

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		/// <param name="name">要添加的对象。</param>
		public DesignColumnInfo CreateColumn(string name)
		{
			return new DesignColumnInfo(tableInfo, name);
		}

		/// <summary>添加项到集合中。</summary>
		public DesignColumnInfo CreateColumn()
		{
			return new DesignColumnInfo(tableInfo);
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected override string GetKey(DesignColumnInfo item) { return item.Name; }
	}
}
