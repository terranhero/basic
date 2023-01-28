using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using Basic.EntityLayer;
using System.Collections.Specialized;

namespace Basic.Interfaces
{
    /// <summary>
    /// 表示 IImportProperty 接口集合
    /// </summary>
    public interface IImportPropertyCollection<TE> : ICollection, INotifyCollectionChanged where TE : AbstractEntity, new()
    {
        /// <summary>
        /// 获取指定索引处的元素。
        /// </summary>
        /// <param name="index">要获取的元素的从零开始的索引。</param>
        /// <returns>指定索引处的元素。</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index 小于零。- 或 -index 等于或大于 Count。</exception>
        IImportProperty<TE> this[int index] { get; }

        /// <summary>
        /// 确定某元素是否在 IImportPropertyCollection 中。
        /// </summary>
        /// <param name="value">要在 IImportPropertyCollection 中定位的对象。对于引用类型，该值可以为 null。</param>
        /// <returns>如果在 IImportPropertyCollection 中找到 value，则为 true；否则为 false。</returns>
        bool Contains(IImportProperty<TE> value);
    }
}
