using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.EntityLayer;
using System.ComponentModel;
using System.Data;
using System.Collections.ObjectModel;
using Basic.Collections;
using Basic.Imports;

namespace Basic.Interfaces
{
    /// <summary>
    /// 表示当前实体的导入数据的上下文信息。
    /// </summary>
    /// <typeparam name="TE">表示 AbstractEntity 子类类型信息</typeparam>
    public interface IImportContext<TE> : INotifyPropertyChanged where TE : AbstractEntity, new()
    {
        /// <summary>
        /// 清理所有数据包含架构
        /// </summary>
        void Clear();

        /// <summary>
        /// 导入的列集合
        /// </summary>
        IImportColumnCollection<TE> Columns { get; }

        /// <summary>
        /// 创建 IImportColumn 接口子类实例。
        /// </summary>
        /// <param name="column">DataTable 类列信息。</param>
        /// <returns>返回创建成功的 IImportColumn 接口子类实例。</returns>
        IImportColumn<TE> CreateColumn(DataColumn column);

        /// <summary>
        /// 创建 IImportColumn 接口子类实例。
        /// </summary>
        /// <param name="collection">DataTable 类中Columns列信息。</param>
        void CreateColumns(DataColumnCollection collection);

        /// <summary>
        /// 当前需要导入实体的属性集合。
        /// </summary>
        IImportPropertyCollection<TE> Properties { get; }

        /// <summary>
        /// 表示调用 IImportProperty&lt;TE&gt;.TrySetValue方法引发的事件。
        /// </summary>
        event EventHandler<HandlerDataRowEventArgs<TE>> HandlerDataRow;

        /// <summary>
        /// 表示调用 IImportContext&lt;TE&gt;.ToEntities方法引发的事件。
        /// </summary>
        event EventHandler<HandlerEntityEventArgs<TE>> HandlerEntity;

        /// <summary>
        /// 属性 SheetName 是否不为空。
        /// </summary>
        bool SheetNameNotEmpty { get; }

        /// <summary>
        /// 属性 SheetName 是否为空。
        /// </summary>
        bool SheetNameEmpty { get; }

        /// <summary>
        /// 表示Excel表格中的Sheet列表
        /// </summary>
        ObservableCollection<string> Sheets { get; }

        /// <summary>
        ///  获取或设置导入的数据源 Excel 文件中的 Sheet 名称。
        /// </summary>
        string SheetName { get; set; }

        /// <summary>
        /// 获取或设置需要导入的 Excel 文件名称(含路径信息)。
        /// </summary>
        string ExcelFileName { get; set; }

        /// <summary>
        /// 根据当前导入上下文配置信息，将 DataTable 实例数据转换成 TE 表示的实体类数组。
        /// </summary>
        /// <param name="table">表示将要转换的 DataTable 类实例。</param>
        /// <returns>如果转换成功，则表示 TE 类实例数组。</returns>
        TE[] ToEntities(DataTable table);

        /// <summary>
        /// 根据当前导入上下文配置信息，将 DataTable 实例数据转换成 TE 表示的实体类数组。
        /// </summary>
        /// <param name="list">表示需要接收转换的实体结果集合。</param>
        /// <param name="table">表示将要转换的 DataTable 类实例。</param>
        /// <returns>如果转换成功，则表示 TE 类实例数组。</returns>
        void ToEntities(Pagination<TE> list, DataTable table);
    }
}
