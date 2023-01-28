using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Interfaces;
using Basic.EntityLayer;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using Basic.Collections;
using System.Collections.Specialized;

namespace Basic.Imports
{
	/// <summary>
	/// 数据导入上下文
	/// </summary>
	/// <typeparam name="TE">表示具有默认构造函数的 AbstractEntity 子类类型。</typeparam>
	internal sealed class ImportContext<TE> : IImportContext<TE>, INotifyPropertyChanged where TE : AbstractEntity, new()
	{
		private readonly ObservableCollection<string> sheets = new ObservableCollection<string>();
		private readonly ImportColumnCollection<TE> importColumns;
		private readonly ImportPropertyCollection<TE> importProperties;

		/// <summary>
		/// 初始化 ImportContext 类实例
		/// </summary>
		internal ImportContext()
		{
			importColumns = new ImportColumnCollection<TE>();
			importProperties = new ImportPropertyCollection<TE>();
		}

		/// <summary>
		/// 在更改属性值时发生。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 引发 PropertyChanged 事件的保护方法。
		/// </summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
		}

		/// <summary>
		/// 表示Excel表格中的Sheet列表
		/// </summary>
		public ObservableCollection<string> Sheets { get { return sheets; } }

		/// <summary>
		/// 清理所有数据包含架构。
		/// </summary>
		public void Clear() { sheets.Clear(); importColumns.Clear(); }

		/// <summary>
		/// 通知数据表已经更改。
		/// </summary>
		public void NotifyColumnsChanged()
		{
			OnPropertyChanged("SheetNameEmpty");
			OnPropertyChanged("SheetNameNotEmpty");
			foreach (ImportProperty<TE> item in importProperties)
			{
				item.OnPropertyChanged("Columns");
			}
		}
		private string m_SheetName;

		private string m_ExcelFileName;

		/// <summary>
		/// 导入的列集合
		/// </summary>
		public IImportColumnCollection<TE> Columns { get { return importColumns; } }

		/// <summary>
		/// 创建 IImportColumn 接口子类实例。
		/// </summary>
		/// <param name="column">DataTable 类列信息。</param>
		/// <returns>返回创建成功的 IImportColumn 接口子类实例。</returns>
		public IImportColumn<TE> CreateColumn(DataColumn column)
		{
			return new ImportColumn<TE>(this, column);
		}

		/// <summary>
		/// 创建 IImportColumn 接口子类实例。
		/// </summary>
		/// <param name="collection">DataTable 类列信息。</param>
		public void CreateColumns(DataColumnCollection collection)
		{
			importColumns.Clear();
			foreach (DataColumn column in collection)
			{
				importColumns.Add(new ImportColumn<TE>(this, column));
			}
			foreach (ImportProperty<TE> property in importProperties)
			{
				property.InitializationIndexColumn();
			}
		}

		/// <summary>
		/// 当前需要导入实体的属性集合。
		/// </summary>
		public IImportPropertyCollection<TE> Properties { get { return importProperties; } }

		/// <summary>
		/// 属性 SheetName 是否不为空。
		/// </summary>
		public bool SheetNameNotEmpty { get { return !string.IsNullOrEmpty(m_SheetName); } }

		/// <summary>
		/// 属性 SheetName 是否为空。
		/// </summary>
		public bool SheetNameEmpty { get { return string.IsNullOrEmpty(m_SheetName); } }

		/// <summary>
		///  获取或设置导入的数据源 Excel 文件中的 Sheet 名称。
		/// </summary>
		public string SheetName
		{
			get { return m_SheetName; }
			set
			{
				if (m_SheetName != value)
				{
					m_SheetName = value;
					OnPropertyChanged("SheetName");
					OnPropertyChanged("SheetNameEmpty");
					OnPropertyChanged("SheetNameNotEmpty");
				}
			}
		}

		/// <summary>
		/// 获取或设置需要导入的 Excel 文件名称(含路径信息)。
		/// </summary>
		public string ExcelFileName
		{
			get { return m_ExcelFileName; }
			set
			{
				if (m_ExcelFileName != value)
				{
					m_ExcelFileName = value;
					OnPropertyChanged("ExcelFileName");
				}
			}
		}

		/// <summary>
		/// 表示调用 IImportProperty&lt;TE&gt;.TrySetValue方法引发的事件。
		/// </summary>
		public event EventHandler<HandlerDataRowEventArgs<TE>> HandlerDataRow;

		/// <summary>
		/// 引发 HandlerDataRow 事件的方法。
		/// </summary>
		/// <param name="entity">表示 TE 类型的实例。</param>
		/// <param name="property">表示当前正在处理的 IImportProperty&lt;TE&gt; 属性信息</param>
		/// <param name="row">表示当前正在导入的 DataRow 类实例。</param>
		/// <param name="rowIndex">行索引</param>
		/// <param name="defaultHandledValue">Handled 属性的默认值。</param>
		internal void OnHandlerDataRow(TE entity, IImportProperty<TE> property, DataRow row, int rowIndex, bool defaultHandledValue)
		{
			if (HandlerDataRow != null) { HandlerDataRow(this, new HandlerDataRowEventArgs<TE>(entity, property, row, rowIndex, defaultHandledValue)); }
		}

		/// <summary>
		/// 引发 HandlerDataRow 事件的方法。
		/// </summary>
		/// <param name="eventArgs">引发事件 HandlerDataRow 的参数。</param>
		internal void OnHandlerDataRow(HandlerDataRowEventArgs<TE> eventArgs)
		{
			if (HandlerDataRow != null) { HandlerDataRow(this, eventArgs); }
		}

		/// <summary>
		/// 表示调用 IImportContext&lt;TE&gt;.ToEntities方法引发的事件。
		/// </summary>
		public event EventHandler<HandlerEntityEventArgs<TE>> HandlerEntity;

		/// <summary>
		/// 引发 HandlerEntity 事件的方法。
		/// </summary>
		/// <param name="eventArgs">引发事件 HandlerEntity 的参数。</param>
		internal void OnHandlerEntity(HandlerEntityEventArgs<TE> eventArgs)
		{
			if (HandlerEntity != null) { HandlerEntity(this, eventArgs); }
		}

		/// <summary>
		/// 根据当前导入上下文配置信息，将 DataTable 实例数据转换成 TE 表示的实体类数组。
		/// </summary>
		/// <param name="table">表示将要转换的 DataTable 类实例。</param>
		/// <returns>如果转换成功，则表示 TE 类实例数组。</returns>
		public TE[] ToEntities(DataTable table)
		{
			Pagination<TE> list = new Pagination<TE>();
			ToEntities(list, table);
			return list.ToArray();
		}

		/// <summary>
		/// 根据当前导入上下文配置信息，将 DataTable 实例数据转换成 TE 表示的实体类数组。
		/// </summary>
		/// <param name="list">表示需要接收转换的实体结果集合。</param>
		/// <param name="table">表示将要转换的 DataTable 类实例。</param>
		/// <returns>如果转换成功，则表示 TE 类实例数组。</returns>
		public void ToEntities(Pagination<TE> list, DataTable table)
		{
			int rowIndex = 0;
			foreach (DataRow row in table.Rows)
			{
				rowIndex++;
				TE entity = new TE();
				foreach (IImportProperty<TE> property in importProperties)
				{
					if (property.Checked) { property.TrySetValue(entity, row, rowIndex); }
				}
				HandlerEntityEventArgs<TE> eventArgs = new HandlerEntityEventArgs<TE>(entity, row, rowIndex, false);
				OnHandlerEntity(eventArgs); list.Add(entity);
				if (eventArgs.Handled) { continue; }
			}
		}
	}
}
