
using Basic.Enums;

namespace Basic.DataAccess
{
	/// <summary>数据库基础数据表列信息</summary>
	public sealed class TableColumnEntity : global::Basic.EntityLayer.AbstractEntity
	{
		/// <summary>初始化 TableColumnEntity 类实例</summary>
		public TableColumnEntity() : base() { }

		/// <summary>初始化 TableColumnEntity 类实例</summary>
		/// <param name="enabledValidation">是否启用实体类中 IDataErrorInfo 的验证特性</param>
		public TableColumnEntity(bool enabledValidation) : base(enabledValidation) { }

		/// <summary>获取或设置数据库表中的列的名称。</summary>
		[System.ComponentModel.DefaultValue("")]
		public string Name { get; set; }

		/// <summary>获取或设置数据库表中的列的名称。</summary>
		[System.ComponentModel.DefaultValue("")]
		public string PropertyName { get; set; }

		/// <summary>属性对应数据库字段描述</summary>
		[System.ComponentModel.DefaultValue("")]
		public string Comment { get; set; }

		/// <summary>属性类型名称</summary>
		[System.ComponentModel.DefaultValue(typeof(DbTypeEnum), "NVarChar")]
		public DbTypeEnum DbType { get; set; }

		/// <summary>获取或设置列中数据的最大大小（以字节为单位）。</summary>
		/// <value>列中数据的最大大小（以字节为单位）。默认值是从参数值推导出的。</value>
		[System.ComponentModel.DefaultValue(0)]
		public int Size { get; set; }

		/// <summary>获取或设置用来表示 Value 属性的最大位数。 </summary>
		/// <value>用于表示 Value 属性的最大位数。 默认值为 0。这指示数据提供程序设置 Value 的精度。 </value>
		[System.ComponentModel.DefaultValue(typeof(byte), "0")]
		public byte Precision { get; set; }

		/// <summary>获取或设置 Value 解析为的小数位数。</summary>
		/// <value>要将 Value 解析为的小数位数。默认值为 0。</value>
		[System.ComponentModel.DefaultValue(typeof(byte), "0")]
		public byte Scale { get; set; }

		/// <summary>属性是否允许为空</summary>
		[System.ComponentModel.DefaultValue(typeof(bool), "false")]
		public bool Nullable { get; set; }
	}
}
