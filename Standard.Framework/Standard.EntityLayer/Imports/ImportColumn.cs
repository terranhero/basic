using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Basic.Interfaces;
using System.Data;
using Basic.EntityLayer;

namespace Basic.Imports
{
    /// <summary>
    /// 导入的Excel列信息
    /// </summary>
    internal sealed class ImportColumn<TE> : IImportColumn<TE> where TE : AbstractEntity, new()
    {
        private readonly ImportContext<TE> importContext;
        private readonly DataColumn dataColumn;
        /// <summary>
        /// 在更改属性值时发生。
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 初始化 ImportProperty 类实例。
        /// </summary>
        /// <param name="context">表示导入的上下文信息。</param>
        /// <param name="column">DataTable 中的 DataColumn 信息。</param>
        /// <exception cref="System.ArgumentNullException">context 参数不能为 null。</exception>
        /// <exception cref="System.ArgumentNullException">propertyInfo 参数不能为 null。</exception>
        internal ImportColumn(ImportContext<TE> context, DataColumn column)
        {
            if (context == null) { throw new ArgumentNullException("context", "参数\"context\"不能为null"); }
            if (column == null) { throw new ArgumentNullException("column", "参数\"column\"不能为null"); }
            importContext = context;
            dataColumn = column;
        }

        /// <summary>
        /// 引发 PropertyChanged 事件的保护方法。
        /// </summary>
        /// <param name="propertyName"></param>
        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }

        /// <summary>
        /// 表示当前导入列对应的 DataTable 中的 DataColumn 信息。
        /// </summary>
        public DataColumn Column { get { return dataColumn; } }

        /// <summary>
        /// 获取或设置字段或列名称
        /// </summary>
        public string ColumnName
        {
            get { return dataColumn.ColumnName; }
        }

        /// <summary>
        /// 获取或设置字段或列的描述
        /// </summary>
        public string Description
        {
            get { return dataColumn.Caption; }
        }

        /// <summary>
        /// 获取或设置字段或列所在集合的索引(从0开始)。
        /// </summary>
        public int Index
        {
            get { return dataColumn.Ordinal; }
        }

        /// <summary>
        /// 获取或设置导入字段类型
        /// </summary>
        public Type Type
        {
            get { return dataColumn.DataType; }
        }
    }
}
