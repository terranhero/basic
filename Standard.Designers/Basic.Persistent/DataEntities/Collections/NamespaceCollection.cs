using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Designer;

namespace Basic.Collections
{
	/// <summary>
	/// 表示引入的命名空间集合
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	[System.ComponentModel.TypeConverter(typeof(NamespacesConverter))]
	public sealed class NamespaceCollection : System.Collections.ObjectModel.ObservableCollection<string>
	{
		/// <summary>
		/// 初始化为空的 NamespaceCollection 类的新实例。
		/// </summary>
		public NamespaceCollection() : base() { }

		/// <summary>
		/// 将 NamespaceCollection 类的新实例初始化为指定列表的包装。
		/// </summary>
		/// <param name="list">由新的集合包装的列表。</param>
		/// <exception cref="System.ArgumentNullException">list 为 null。</exception>
		public NamespaceCollection(IList<string> list) : base(list) { }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Concat("Namespaces of ", base.Count);
		}
	}
}
