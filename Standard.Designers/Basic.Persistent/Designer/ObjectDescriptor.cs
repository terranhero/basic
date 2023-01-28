using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Basic.Designer
{
	/// <summary>
	/// 属性包装器
	/// </summary>
	/// <typeparam name="TDD">需要包装属性的信息</typeparam>
	public class ObjectDescriptor<TDD> : ICustomTypeDescriptor where TDD : ICustomDescriptor
	{
		/// <summary>
		/// 初始化 DefinitionDescriptor 类实例
		/// </summary>
		/// <param name="dInfo">需要包装实现了 IDefinitionDescriptor 接口的对象</param>
		public ObjectDescriptor(TDD dInfo) : base() { definitionInfo = dInfo; }

		/// <summary>
		/// 包含此属性定义的类型
		/// </summary>
		private readonly TDD definitionInfo;

		/// <summary>
		/// 包含此属性定义的类型
		/// </summary>
		protected internal TDD DefinitionInfo { get { return definitionInfo; } }

		/// <summary>
		/// 返回此组件实例的自定义特性的集合
		/// </summary>
		/// <returns>包含此对象的特性的 System.ComponentModel.AttributeCollection。</returns>
		public virtual AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(definitionInfo);
		}
		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public virtual string GetClassName()
		{
			return definitionInfo.GetClassName();
		}
		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public virtual string GetComponentName()
		{
			return definitionInfo.GetComponentName();
		}
		/// <summary>
		/// 返回此组件实例的类型转换器。
		/// </summary>
		/// <returns> 表示该对象的转换器的 System.ComponentModel.TypeConverter；
		/// 如果此对象没有任何 System.ComponentModel.TypeConverter，则为null。</returns>
		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(definitionInfo);
		}
		/// <summary>
		/// 返回此组件实例的默认事件。
		/// </summary>
		/// <returns>表示该对象的默认事件的 System.ComponentModel.EventDescriptor；如果该对象没有事件，则为 null。</returns>
		public virtual EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(definitionInfo);
		}
		/// <summary>
		/// 返回此组件实例的默认属性。
		/// </summary>
		/// <returns>表示该对象的默认属性的 System.ComponentModel.PropertyDescriptor；如果此对象没有属性，则为 null。</returns>
		public virtual PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(definitionInfo);
		}
		/// <summary>
		/// 返回此组件实例的指定类型的编辑器。
		/// </summary>
		/// <param name="editorBaseType">表示该对象的编辑器的 System.Type。</param>
		/// <returns>表示该对象编辑器的指定类型的 System.Object；如果无法找到编辑器，则为 null。</returns>
		public virtual object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(definitionInfo, editorBaseType);
		}
		/// <summary>
		/// 返回此组件实例的事件。
		/// </summary>
		/// <returns>表示此组件实例的事件的 System.ComponentModel.EventDescriptorCollection。</returns>
		public virtual EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(definitionInfo);
		}
		/// <summary>
		/// 将指定的特性数组用作筛选器来返回此组件实例的事件。
		/// </summary>
		/// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
		/// <returns>表示此组件实例的已筛选事件的 System.ComponentModel.EventDescriptorCollection。</returns>
		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(definitionInfo, attributes);
		}
		/// <summary>
		/// 返回此组件实例的属性。
		/// </summary>
		/// <returns>表示此组件实例的属性的 System.ComponentModel.PropertyDescriptorCollection。</returns>
		public virtual PropertyDescriptorCollection GetProperties()
		{
			return TypeDescriptor.GetProperties(definitionInfo);
		}
		/// <summary>
		/// 返回将特性数组用作筛选器的此组件实例的属性。
		/// </summary>
		/// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
		/// <returns>表示此组件实例的已筛选属性的 System.ComponentModel.PropertyDescriptorCollection。</returns>
		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(definitionInfo, attributes);
		}

		/// <summary>
		/// 返回包含指定的属性描述符所描述的属性的对象。
		/// </summary>
		/// <param name="pd">表示要查找其所有者的属性的 System.ComponentModel.PropertyDescriptor。</param>
		/// <returns>表示指定属性所有者的 System.Object。</returns>
		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			return definitionInfo.GetPropertyOwner(pd);
		}

		/// <summary>
		/// 获取经过包装的对象。
		/// </summary>
		public virtual ICustomDescriptor WrappedObject
		{
			get { return definitionInfo; }
		}
	}
}
