using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime;
using Basic.Interfaces;
using System.Data;

namespace Basic.Imports
{
	/// <summary>
	/// 为可在事件处理程序中完整处理的事件提供数据。
	/// </summary>
	/// <typeparam name="TE">表示 AbstractEntity 子类类型信息</typeparam>
	public sealed class HandlerDataRowEventArgs<TE> : HandledEventArgs where TE : Basic.EntityLayer.AbstractEntity, new()
	{
		private readonly TE m_AbstractEntity;
		private readonly IImportProperty<TE> m_ImportProperty;
		private readonly DataRow m_DataRow;
		/// <summary>
		/// 用 Handled 属性的默认值 false 初始化 HandlerPropertyEventArgs&lt;TE&gt; 类的新实例。
		/// </summary>
		/// <param name="entity">表示 TE 类型的实例。</param>
		/// <param name="property">表示当前正在处理的 IImportProperty&lt;TE&gt; 属性信息</param>
		/// <param name="row">表示当前正在导入的 DataRow 类实例。</param>
		/// <param name="rowIndex">行索引</param>
		public HandlerDataRowEventArgs(TE entity, IImportProperty<TE> property, DataRow row, int rowIndex) : this(entity, property, row, rowIndex, false) { }

		/// <summary>
		/// 用 Handled 属性的指定默认值初始化 HandlerPropertyEventArgs&lt;TE&gt; 类的新实例。
		/// </summary>
		/// <param name="entity">表示 TE 类型的实例。</param>
		/// <param name="property">表示当前正在处理的 IImportProperty&lt;TE&gt; 属性信息</param>
		/// <param name="row">表示当前正在导入的 DataRow 类实例。</param>
		/// <param name="rowIndex">行索引</param>
		/// <param name="defaultHandledValue">Handled 属性的默认值。</param>
		public HandlerDataRowEventArgs(TE entity, IImportProperty<TE> property, DataRow row, int rowIndex, bool defaultHandledValue)
			: base(defaultHandledValue)
		{
			m_AbstractEntity = entity;
			m_ImportProperty = property;
			m_DataRow = row; m_RowIndex = rowIndex;
		}

		/// <summary>
		/// 表示当前正在处理的 TE 实体类实例。
		/// </summary>
		public TE Entity { get { return m_AbstractEntity; } }

		/// <summary>
		/// 表示当前正在处理的 TE 实体类实例属性信息。
		/// </summary>
		public IImportProperty<TE> Property { get { return m_ImportProperty; } }

		/// <summary>
		/// 获取或设置字段或列名称
		/// </summary>
		public string ColumnName { get { return m_ImportProperty.Column.ColumnName; } }

		/// <summary>
		/// 获取导入字段名称
		/// </summary>
		public string FieldName { get { return m_ImportProperty.FieldName; } }

		/// <summary>
		/// 获取字段或列所在集合从0开始的索引。
		/// </summary>
		public int ColumnIndex { get { return m_ImportProperty.Column.Index; } }

		/// <summary>
		/// 表示当前正在处理的 DataRow 实体类实例。
		/// </summary>
		public DataRow Row { get { return m_DataRow; } }

		/// <summary>
		/// 表示当前正在处理的 DataRow 所在行索引号。
		/// </summary>
		public int RowIndex { get { return m_RowIndex; } }
		private readonly int m_RowIndex;
	}
}
