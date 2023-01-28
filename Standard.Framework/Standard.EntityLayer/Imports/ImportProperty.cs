using Basic.EntityLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Basic.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Basic.Imports
{
	/// <summary>
	/// 导入数据项，指每个字段信息。
	/// </summary>
	internal sealed class ImportProperty<TE> : IImportProperty<TE> where TE : AbstractEntity, new()
	{
		private readonly EntityPropertyMeta m_PropertyInfo;
		private readonly ImportContext<TE> importContext;
		/// <summary>
		/// 在更改属性值时发生。
		/// </summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 引发 PropertyChanged 事件的保护方法。
		/// </summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
		}

		/// <summary>
		/// 初始化 ImportProperty 类实例。
		/// </summary>
		/// <param name="context">表示导入的上下文信息。</param>
		/// <param name="propertyInfo">实体属性的反射信息。</param>
		/// <exception cref="System.ArgumentNullException">context 参数不能为 null。</exception>
		/// <exception cref="System.ArgumentNullException">propertyInfo 参数不能为 null。</exception>
		internal ImportProperty(ImportContext<TE> context, EntityPropertyMeta propertyInfo)
		{
			if (context == null) { throw new ArgumentNullException("context", "参数\"context\"不能为null"); }
			if (propertyInfo == null) { throw new ArgumentNullException("propertyInfo", "参数\"propertyInfo\"不能为null"); }
			importContext = context;
			m_PropertyInfo = propertyInfo;
			foreach (Attribute attribute in m_PropertyInfo.Attributes)
			{
				if (attribute is ColumnMappingAttribute)
				{
					ColumnMappingAttribute cma = (ColumnMappingAttribute)attribute;
					m_FieldName = cma.ColumnName;
				}
				else if (attribute is ImportAttribute)
				{
					ImportAttribute importAttribute = (ImportAttribute)attribute;
					m_Index = importAttribute.Index;
					m_Required = importAttribute.Required;
					m_Checked = importAttribute.Checked;
				}
				else if (attribute is DescriptionAttribute)
				{
					DescriptionAttribute da = (DescriptionAttribute)attribute;
					m_Description = da.Description;
				}
				else if (attribute is WebDisplayAttribute)
				{
					WebDisplayAttribute wda = (WebDisplayAttribute)attribute;
					m_Description = wda.DisplayName;
				}
#if(NET35)
#else
				else if (attribute is DisplayAttribute)
				{
					DisplayAttribute display = (DisplayAttribute)attribute;
					m_Description = display.Name;
				}
#endif
			}
		}

		/// <summary>
		/// 如果当前字段不是必导字段，则允许修改
		/// </summary>
		public bool IsEnabled { get { return !m_Required; } }

		private bool m_Required;
		/// <summary>
		/// 当前字段是否必导字段
		/// </summary>
		public bool Required
		{
			get { return m_Required; }
			set
			{
				if (m_Required != value)
				{
					m_Required = value;
					OnPropertyChanged("Required");
					OnPropertyChanged("IsEnabled");
				}
			}
		}

		private bool m_Checked = true;
		/// <summary>
		/// 当前字段是否必导字段
		/// </summary>
		public bool Checked
		{
			get { return m_Checked; }
			set { if (m_Checked != value) { m_Checked = value; OnPropertyChanged("Checked"); } }
		}

		private string m_FieldName;
		/// <summary>
		/// 导入字段名称
		/// </summary>
		public string FieldName
		{
			get { return m_FieldName; }
			set { if (m_FieldName != value) { m_FieldName = value; OnPropertyChanged("FieldName"); } }
		}

		private string m_Description;
		/// <summary>
		/// 导入字段描述
		/// </summary>
		public string Description
		{
			get { return m_Description; }
			set { if (m_Description != value) { m_Description = value; OnPropertyChanged("Description"); } }
		}

		private int m_Index;
		/// <summary>
		/// 导入字段对应的Excel表格列索引。
		/// </summary>
		public int Index
		{
			get { return m_Index; }
			set
			{
				if (m_Index != value)
				{
					m_Index = value; OnPropertyChanged("Index");
					OnPropertyChanged("Position");
				}
			}
		}

		/// <summary>
		/// 当前导入属性对应的列位置
		/// </summary>
		public int Position { get { return m_Index + 1; } }

		/// <summary>
		/// 根据当前列索引初始化 Column 信息
		/// </summary>
		public void InitializationIndexColumn()
		{
			OnPropertyChanged("Columns");
			if (m_Index < importContext.Columns.Count && m_Index >= 0)
			{
				m_ImportColumn = importContext.Columns[m_Index];
			}
			else
			{
				m_ImportColumn = null;
			}
			OnPropertyChanged("Index");
			OnPropertyChanged("Column");
		}

		/// <summary>
		/// 导入字段类型
		/// </summary>
		public Type Type
		{
			get
			{
				Type type = m_PropertyInfo.PropertyType;
				if (type.IsGenericType && type.GUID == typeof(Nullable<>).GUID) { return type.GetGenericArguments()[0]; }
				return type;
			}
		}

		/// <summary>
		/// 获取导入数据源的列或字段信息集合，用户数据源绑定。
		/// </summary>
		public IImportColumnCollection<TE> Columns
		{
			get { return importContext.Columns; }
		}

		private IImportColumn<TE> m_ImportColumn;
		/// <summary>
		/// 导入字段数据源列信息
		/// </summary>
		public IImportColumn<TE> Column
		{
			get { return m_ImportColumn; }
			set
			{
				if (m_ImportColumn != value)
				{
					m_ImportColumn = value;
					m_Index = m_ImportColumn.Index;
					OnPropertyChanged("Index");
					OnPropertyChanged("Column");
				}
			}
		}

		/// <summary>
		/// 根据当前属性信息读取数据源对应的值。
		/// </summary>
		/// <param name="entity">需要接受此属性值的 AbstractEntity 子类实例。</param>
		/// <param name="source">需要传入数据给属性的 System.Data.DataRow 类实例。</param>
		/// <param name="rowIndex">行索引</param>
		/// <returns>如果输入赋值成功则返回 True，否则返回 False。</returns>
		public bool TrySetValue(TE entity, DataRow source, int rowIndex)
		{
			HandlerDataRowEventArgs<TE> eventArgs = new HandlerDataRowEventArgs<TE>(entity, this, source, rowIndex, false);
			importContext.OnHandlerDataRow(eventArgs);
			if (!eventArgs.Handled)
			{
				if (m_ImportColumn != null)
				{
					m_PropertyInfo.SetValue(entity, source[m_ImportColumn.Index]);
				}
			}
			return true;
		}

		/// <summary>
		/// 根据当前属性信息读取数据源对应的值。
		/// </summary>
		/// <param name="entity">需要接受此属性值的 AbstractEntity 子类实例。</param>
		/// <param name="source">需要传入数据给属性的 System.Data.DataRowView 类实例。</param>
		/// <returns>如果输入赋值成功则返回 True，否则返回 False。</returns>
		public bool TrySetValue(TE entity, DataRowView source)
		{
			m_PropertyInfo.SetValue(entity, source[this.m_Index]);
			return true;
		}
	}
}
