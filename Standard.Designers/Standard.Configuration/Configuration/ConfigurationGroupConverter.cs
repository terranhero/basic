using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basic.Configuration
{
    /// <summary>
    /// 提供 ConfigurationGroup 类型的转换器。
    /// </summary>
    public sealed class ConfigurationGroupTypeProvider : System.ComponentModel.TypeDescriptionProvider
    {
        /// <summary>
        /// 获取给定类型和对象的自定义类型说明符。
        /// </summary>
        /// <param name="objectType">要为其检索类型说明符的对象的类型。</param>
        /// <param name="instance">该类型的实例。如果没有向 System.ComponentModel.TypeDescriptor 传递任何实例，则可以是 null。</param>
        /// <returns>可以为该类型提供元数据的 System.ComponentModel.ICustomTypeDescriptor。</returns>
        public override System.ComponentModel.ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ConfigurationGroupTypeDescriptor(instance as ConfigurationGroup);
        }
    }

    /// <summary>
    /// 提供 ConfigurationGroup 类型的转换器。
    /// </summary>
    public sealed class ConfigurationGroupTypeDescriptor : System.ComponentModel.ICustomTypeDescriptor
    {
        /// <summary>
        /// 初始化 DefinitionDescriptor 类实例
        /// </summary>
        /// <param name="dInfo">需要包装实现了 IDefinitionDescriptor 接口的对象</param>
        public ConfigurationGroupTypeDescriptor(ConfigurationGroup dInfo) : base() { definitionInfo = dInfo; }

        /// <summary>
        /// 包含此属性定义的类型
        /// </summary>
        private readonly ConfigurationGroup definitionInfo;

        /// <summary>
        /// 返回此组件实例的自定义特性的集合
        /// </summary>
        /// <returns>包含此对象的特性的 System.ComponentModel.AttributeCollection。</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(definitionInfo);
        }
        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(definitionInfo);
        }
        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(definitionInfo);
        }
        /// <summary>
        /// 返回此组件实例的类型转换器。
        /// </summary>
        /// <returns> 表示该对象的转换器的 System.ComponentModel.TypeConverter；
        /// 如果此对象没有任何 System.ComponentModel.TypeConverter，则为null。</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(definitionInfo);
        }
        /// <summary>
        /// 返回此组件实例的默认事件。
        /// </summary>
        /// <returns>表示该对象的默认事件的 System.ComponentModel.EventDescriptor；如果该对象没有事件，则为 null。</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(definitionInfo);
        }
        /// <summary>
        /// 返回此组件实例的默认属性。
        /// </summary>
        /// <returns>表示该对象的默认属性的 System.ComponentModel.PropertyDescriptor；如果此对象没有属性，则为 null。</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(definitionInfo);
        }
        /// <summary>
        /// 返回此组件实例的指定类型的编辑器。
        /// </summary>
        /// <param name="editorBaseType">表示该对象的编辑器的 System.Type。</param>
        /// <returns>表示该对象编辑器的指定类型的 System.Object；如果无法找到编辑器，则为 null。</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(definitionInfo, editorBaseType);
        }
        /// <summary>
        /// 返回此组件实例的事件。
        /// </summary>
        /// <returns>表示此组件实例的事件的 System.ComponentModel.EventDescriptorCollection。</returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(definitionInfo);
        }
        /// <summary>
        /// 将指定的特性数组用作筛选器来返回此组件实例的事件。
        /// </summary>
        /// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
        /// <returns>表示此组件实例的已筛选事件的 System.ComponentModel.EventDescriptorCollection。</returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(definitionInfo, attributes);
        }
        /// <summary>
        /// 返回此组件实例的属性。
        /// </summary>
        /// <returns>表示此组件实例的属性的 System.ComponentModel.PropertyDescriptorCollection。</returns>
        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(definitionInfo);
        }
        /// <summary>
        /// 返回将特性数组用作筛选器的此组件实例的属性。
        /// </summary>
        /// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
        /// <returns>表示此组件实例的已筛选属性的 System.ComponentModel.PropertyDescriptorCollection。</returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(definitionInfo, attributes);
        }

        /// <summary>
        /// 返回包含指定的属性描述符所描述的属性的对象。
        /// </summary>
        /// <param name="pd">表示要查找其所有者的属性的 System.ComponentModel.PropertyDescriptor。</param>
        /// <returns>表示指定属性所有者的 System.Object。</returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return definitionInfo;
        }
    }

    /// <summary>
    /// 提供 ConfigurationGroup 类型的转换器。
    /// </summary>
    public sealed class ConfigurationGroupConverter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        /// 使用指定的上下文和特性返回由 value 参数指定的数组类型的属性的集合。
        /// </summary>
        /// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
        /// <param name="value">一个 System.Object，指定要为其获取属性的数组类型。</param>
        /// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
        /// <returns>具有为此数据类型公开的属性的 System.ComponentModel.PropertyDescriptorCollection；或者，如果没有属性，则为 null。</returns>
        public override System.ComponentModel.PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
            System.ComponentModel.PropertyDescriptorCollection returnProperties = new System.ComponentModel.PropertyDescriptorCollection(null);
            foreach (System.ComponentModel.PropertyDescriptor property in properties)
            {
                if (property.Name == "Name")
                    returnProperties.Add(property);
            }
            return returnProperties;
        }
    }
}
