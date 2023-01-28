using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示当前属性是数据库表中关键字。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class PrimaryKeyAttribute : Attribute
	{
		/// <summary>
		/// 初始化PrimaryKeyAttribute类实例, 设置字符类型数据列
		/// </summary>
		public PrimaryKeyAttribute() { }
	}
}
