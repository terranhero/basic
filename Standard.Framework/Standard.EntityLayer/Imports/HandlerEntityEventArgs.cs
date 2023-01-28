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
	public sealed class HandlerEntityEventArgs<TE> : HandledEventArgs where TE : Basic.EntityLayer.AbstractEntity, new()
	{
		private readonly TE m_AbstractEntity;
		private readonly DataRow m_DataRow;
		/// <summary>
		/// 用 Handled 属性的默认值 false 初始化 HandlerPropertyEventArgs&lt;TE&gt; 类的新实例。
		/// </summary>
		/// <param name="entity">表示 TE 类型的实例。</param>
		/// <param name="row">表示当前正在导入的 DataRow 类实例。</param>
		/// <param name="rowIndex">行索引</param>
		public HandlerEntityEventArgs(TE entity, DataRow row, int rowIndex) : this(entity, row, rowIndex, false) { }

		/// <summary>
		/// 用 Handled 属性的指定默认值初始化 HandlerPropertyEventArgs&lt;TE&gt; 类的新实例。
		/// </summary>
		/// <param name="entity">表示 TE 类型的实例。</param>
		/// <param name="row">表示当前正在导入的 DataRow 类实例。</param>
		/// <param name="rowIndex">行索引</param>
		/// <param name="defaultHandledValue">Handled 属性的默认值。</param>
		public HandlerEntityEventArgs(TE entity, DataRow row, int rowIndex, bool defaultHandledValue)
			: base(defaultHandledValue)
		{
			m_AbstractEntity = entity;
			m_DataRow = row; m_RowIndex = rowIndex;
		}

		/// <summary>
		/// 表示当前正在处理的 TE 实体类实例。
		/// </summary>
		public TE Entity { get { return m_AbstractEntity; } }

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
