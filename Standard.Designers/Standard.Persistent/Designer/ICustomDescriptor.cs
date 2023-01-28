using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Basic.Designer
{
	/// <summary>
	/// 提供为对象提供动态自定义类型信息的接口。
	/// </summary>
	public interface ICustomDescriptor
	{
		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		string GetClassName();

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		string GetComponentName();

		/// <summary>
		/// 返回包含指定的属性描述符所描述的属性的对象。
		/// </summary>
		/// <param name="pd">表示要查找其所有者的属性的 System.ComponentModel.PropertyDescriptor。</param>
		/// <returns>表示指定属性所有者的 System.Object。</returns>
		object GetPropertyOwner(PropertyDescriptor pd);
	}
}
