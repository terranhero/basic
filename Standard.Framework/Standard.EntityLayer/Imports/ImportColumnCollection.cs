using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Interfaces;
using System.Collections.ObjectModel;
using Basic.EntityLayer;

namespace Basic.Imports
{
    /// <summary>
    /// 导入的列信息集合
    /// </summary>
    internal sealed class ImportColumnCollection<TE> : ObservableCollection<IImportColumn<TE>>, IImportColumnCollection<TE>
        where TE : AbstractEntity, new()
    {
        /// <summary>
        /// 初始化 ImportColumnCollection 类的新实例。
        /// </summary>
        public ImportColumnCollection() : base() { }

        /// <summary>
        /// 初始化 ImportColumnCollection 类的新实例，该类包含从指定集合中复制的元素。
        /// </summary>
        /// <param name="collection">从中复制元素的集合。</param>
        /// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
        public ImportColumnCollection(IEnumerable<IImportColumn<TE>> collection) : base(collection) { }

        /// <summary>
        /// 初始化 ImportColumnCollection 类的新实例，该类包含从指定列表中复制的元素。
        /// </summary>
        /// <param name="list">从中复制元素的列表。</param>
        /// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
        public ImportColumnCollection(List<IImportColumn<TE>> list) : base(list) { }
    }
}
