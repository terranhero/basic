using System.Collections.Generic;
using System.Collections.Specialized;
using Basic.Configuration;
using Basic.Designer;
using System.ComponentModel;
using System;

namespace Basic.Collections
{
    /// <summary>
    /// 表示命令参数信息
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Basic.Designer.PersistentCategory(PersistentCategoryAttribute.CategoryDataCommand)]
    [Basic.Designer.PersistentDescription("PersistentDescription_ParameterCollection")]
    [Editor(typeof(CommandParametersEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public sealed class CommandParameterCollection : Basic.Collections.BaseCollection<CommandParameter>,
         ICollection<CommandParameter>, IEnumerable<CommandParameter>, INotifyCollectionChanged
    {
        private readonly AbstractCommandElement _Command;
        /// <summary>
        /// 初始化 CommandParameterCollection 类的新实例。
        /// </summary>
        internal CommandParameterCollection(AbstractCommandElement command)
            : base() { _Command = command; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _Command.OnFileContentChanged(e);
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// 将一项插入集合中指定索引处。
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 item。</param>
        /// <param name="item">要插入的对象。</param>
        protected override void InsertItem(int index, CommandParameter item)
        {
            if (string.IsNullOrWhiteSpace(item.Name)) { item.Name = string.Concat("Parameter", Convert.ToString(base.Count).PadLeft(2, '0')); }
            item.FileContentChanged += new System.EventHandler((sender, e) => { _Command.OnFileContentChanged(e); });
            base.InsertItem(index, item);
        }

        /// <summary>
        /// 获取集合的键属性
        /// </summary>
        /// <param name="item">需要获取键的集合子元素</param>
        /// <returns>返回元素的键</returns>
        protected internal override string GetKey(CommandParameter item) { return item.Name; }
    }
}
