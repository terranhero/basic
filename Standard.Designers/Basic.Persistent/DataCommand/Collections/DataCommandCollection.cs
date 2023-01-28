using System.Collections.Generic;
using Basic.Configuration;
using System.Collections.Specialized;
using Basic.Enums;

namespace Basic.Collections
{
	/// <summary>
	/// 表示执行数据库命令的集合
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class DataCommandCollection : Basic.Collections.BaseCollection<DataCommandElement>,
		ICollection<DataCommandElement>, IEnumerable<DataCommandElement>, INotifyCollectionChanged
	//System.Collections.ObjectModel.ObservableCollection<DataCommandElement>
	{
		internal const string XmlElementName = "DataCommands";
		private readonly PersistentConfiguration persistentConfiguration;
		/// <summary>
		/// 初始化 DataCommandCollection 类的新实例。
		/// </summary>
		internal DataCommandCollection(PersistentConfiguration persistent) : base() { persistentConfiguration = persistent; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			persistentConfiguration.OnFileContentChanged(System.EventArgs.Empty);
			base.OnCollectionChanged(e);
		}

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, DataCommandElement item)
		{
			item.FileContentChanged += new System.EventHandler((sender, e) => { persistentConfiguration.OnFileContentChanged(e); });
			string name = this.GetKey(item);
			if (string.IsNullOrWhiteSpace(item.Name))
				item.Name = string.Concat("DataCommand_", index);
			if (base.ContainsKey(name)) { item.Name = string.Concat(item.Name, "_", this.Count); }
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(DataCommandElement item) { return item.Name; }
	}
}
