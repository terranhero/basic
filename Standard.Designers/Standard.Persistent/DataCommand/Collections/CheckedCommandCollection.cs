using System.Collections.Generic;
using Basic.Configuration;
using System.Collections.Specialized;
using Basic.Designer;
using System.ComponentModel;
using System;

namespace Basic.Collections
{
	/// <summary>
	/// 表示检测命令集合
	/// </summary>
	[Editor(typeof(CheckCommandsEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public sealed class CheckedCommandCollection : Basic.Collections.BaseCollection<CheckedCommandElement>,
		ICollection<CheckedCommandElement>, IEnumerable<CheckedCommandElement>, INotifyCollectionChanged
	{
		private readonly StaticCommandElement staticCommand;
		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		public const string XmlElementName = "CheckCommands";

		/// <summary>
		/// 初始化 CheckCommandCollection 类的新实例。
		/// </summary>
		internal CheckedCommandCollection(StaticCommandElement element) : base() { staticCommand = element; }

        /// <summary>
        /// 引发带有提供的参数的 Basic.Collections.CheckCommandCollection.CollectionChanged 事件。
        /// </summary>
        /// <param name="e">要引发的事件的参数。</param>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			staticCommand.OnFileContentChanged(EventArgs.Empty);
		}

		/// <summary>
		/// 将一项插入集合中指定索引处。
		/// </summary>
		/// <param name="index">从零开始的索引，应在该位置插入 item。</param>
		/// <param name="item">要插入的对象。</param>
		protected override void InsertItem(int index, Basic.Configuration.CheckedCommandElement item)
        {
            if (string.IsNullOrWhiteSpace(item.Name)) { item.Name = string.Concat("CheckCommand", Convert.ToString(base.Count).PadLeft(2, '0')); }
			item.FileContentChanged += new System.EventHandler((sender, e) => { staticCommand.OnFileContentChanged(e); });
			base.InsertItem(index, item);
		}

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected internal override string GetKey(CheckedCommandElement item) { return item.Name; }
	}
}
