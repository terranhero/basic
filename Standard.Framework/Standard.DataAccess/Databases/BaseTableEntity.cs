using System.Collections.Generic;

namespace Basic.DataAccess
{
	/// <summary>数据库基础数据表(含视图)</summary>
	public sealed class BaseTableEntity : global::Basic.EntityLayer.AbstractEntity
	{
		/// <summary>初始化 BaseTableEntity 类实例</summary>
		public BaseTableEntity() : base() { }

		/// <summary>初始化 BaseTableEntity 类实例</summary>
		/// <param name="enabledValidation">是否启用实体类中 IDataErrorInfo 的验证特性</param>
		public BaseTableEntity(bool enabledValidation) : base(enabledValidation) { }

		private readonly List<TableColumnEntity> _columns = new List<TableColumnEntity>(50);
		/// <summary>基础数据表所有用户</summary>
		public string Owner { get; internal set; }

		/// <summary>基础数据表名称</summary>
		public string ObjectName { get; internal set; }

		/// <summary>基础数据表描述</summary>
		public string Description { get; internal set; }

		/// <summary></summary>
		public IReadOnlyCollection<TableColumnEntity> Columns { get { return _columns; } }
	}
}
