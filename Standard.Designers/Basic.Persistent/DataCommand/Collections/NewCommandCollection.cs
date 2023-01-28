using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Basic.Configuration;
using Basic.Designer;

namespace Basic.Collections
{
	/// <summary>
	/// 表示新值命令集合
	/// </summary>
	[Editor(typeof(NewCommandsEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public sealed class NewCommandCollection : Basic.Collections.BaseCollection<NewCommandElement>,
		ICollection<NewCommandElement>, IEnumerable<NewCommandElement>, INotifyCollectionChanged
	{
		private readonly StaticCommandElement staticCommand;
		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		public const string XmlElementName = "NewValues";
		/// <summary>
		/// 初始化 NewCommandCollection 类的新实例。
		/// </summary>
		internal NewCommandCollection(StaticCommandElement element) : base() { staticCommand = element; }

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, Basic.Configuration.NewCommandElement item)
        {
            if (string.IsNullOrWhiteSpace(item.Name)) { item.Name = string.Concat("NewCommand", Convert.ToString(base.Count).PadLeft(2, '0')); }
			item.FileContentChanged += new System.EventHandler((sender, e) => { staticCommand.OnFileContentChanged(e); });
			base.InsertItem(index, item);
		}

		/// <summary>
        /// 引发带有提供的参数的 Basic.Collections.NewCommandCollection.CollectionChanged 事件。
		/// </summary>
        /// <param name="e">要引发的事件的参数。</param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			staticCommand.OnFileContentChanged(EventArgs.Empty);
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(NewCommandElement item) { return item.Name; }
	}
}
