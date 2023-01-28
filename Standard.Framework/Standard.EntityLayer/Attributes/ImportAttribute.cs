using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示实体类属性导入特性信息。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ImportAttribute : Attribute
	{
		/// <summary>
		/// 初始化 ImportAttribute 类实例
		/// </summary>
		/// <param name="index">当前属性导入时指定导入数据源的索引。</param>
		public ImportAttribute(int index) : this(index, false, true) { }

		/// <summary>
		/// 初始化 ImportAttribute 类实例
		/// </summary>
		/// <param name="index">当前属性导入时指定导入数据源的索引。</param>
		/// <param name="required">当前属性是否必须导入。</param>
		public ImportAttribute(int index, bool required) : this(index, required, true) { }

		/// <summary>
		/// 初始化 ImportAttribute 类实例
		/// </summary>
		/// <param name="index">当前属性导入时指定导入数据源的索引。</param>
		/// <param name="required">当前属性是否必须导入。</param>
		/// <param name="isChecked">当前属性是否需要导入，默认值为 true。</param>
		public ImportAttribute(int index, bool required, bool isChecked) { Index = index; Required = required; Checked = isChecked; }

		/// <summary>
		/// 当前属性导入时指定导入数据源的索引。
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// 当前属性是否必须导入。
		/// </summary>
		public bool Required { get; private set; }

		/// <summary>
		/// 当前属性是否需要导入，默认值为 true。
		/// </summary>
		public bool Checked { get; private set; }
	}
}
