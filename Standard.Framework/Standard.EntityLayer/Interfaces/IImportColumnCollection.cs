using Basic.EntityLayer;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Basic.Interfaces
{
	/// <summary>
	/// 表示 IImportColumn 接口的集合
	/// </summary>
	public interface IImportColumnCollection<TE> : ICollection<IImportColumn<TE>>, IList<IImportColumn<TE>>, INotifyCollectionChanged
        where TE : AbstractEntity, new()
    {
    }
}
