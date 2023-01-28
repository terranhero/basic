using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Basic.EntityLayer;

namespace Basic.Interfaces
{
	/// <summary>
	/// 表示实体导入的实体属性信息。
	/// </summary>
	public interface IImportProperty<TE> : INotifyPropertyChanged where TE : AbstractEntity, new()
	{
		/// <summary>
		/// 获取导入数据源的列或字段信息集合
		/// </summary>
		IImportColumnCollection<TE> Columns { get; }

		/// <summary>
		/// 如果当前字段不是必导字段，则允许修改
		/// </summary>
		bool IsEnabled { get; }

		/// <summary>
		/// 当前字段是否必导字段
		/// </summary>
		bool Required { get; set; }

		/// <summary>
		/// 当前字段是否必导字段
		/// </summary>
		bool Checked { get; set; }

		/// <summary>
		/// 导入字段名称
		/// </summary>
		string FieldName { get; set; }

		/// <summary>
		/// 导入字段描述
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// 导入字段对应的Excel表格列索引。
		/// </summary>
		int Index { get; set; }

		/// <summary>
		/// 导入字段类型
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// 导入字段类型
		/// </summary>
		IImportColumn<TE> Column { get; set; }

		/// <summary>
		/// 根据当前属性信息读取数据源对应的值。
		/// </summary>
		/// <param name="entity">需要接受此属性值的 AbstractEntity 子类实例。</param>
		/// <param name="source">需要传入数据给属性的 System.Data.DataRow 类实例。</param>
		/// <param name="rowIndex">行索引</param>
		/// <returns>如果输入赋值成功则返回 True，否则返回 False。</returns>
		bool TrySetValue(TE entity, System.Data.DataRow source, int rowIndex);
	}
}
