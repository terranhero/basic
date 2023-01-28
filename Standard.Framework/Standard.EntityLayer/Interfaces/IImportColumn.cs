using System;
using System.ComponentModel;
using Basic.EntityLayer;
using System.Data;

namespace Basic.Interfaces
{
    /// <summary>
    /// 表示导入实体对应的数据源字段或属性信息。
    /// </summary>
    public interface IImportColumn<TE> : INotifyPropertyChanged where TE : AbstractEntity, new()
    {
        /// <summary>
        /// 获取或设置字段或列名称
        /// </summary>
        string ColumnName { get; }

        /// <summary>
        /// 获取或设置字段或列的描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 获取或设置字段或列所在集合的索引(从0开始)。
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 获取或设置导入字段类型
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// 表示当前导入列对应的DataTable中的DataColumn信息。
        /// </summary>
        DataColumn Column { get; }
    }
}
