using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Basic.Collections
{
    /// <summary>
    /// IStringObjectArray接口的默认实现
    /// 表示键/值对的集合，提供为对象提供动态自定义类型信息的接口。
    /// </summary>
    public sealed class StringObjectArray : Dictionary<string, object>, IStringObjectArray, ISerializable, IEnumerable, ICustomTypeDescriptor
    {
        #region 静态方法
        /// <summary>
        /// 初始化 StringObjectArray 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
        /// </summary>
        /// <param name="key1">要添加的元素的键</param>
        /// <param name="value1">要添加的元素的值。对于引用类型，该值可以为 null。</param>
        public static StringObjectArray Create(string key1, object value1)
        {
            StringObjectArray objArray = new StringObjectArray(1);
            objArray.Add(key1, value1);
            return objArray;
        }
        /// <summary>
        /// 初始化 StringObjectArray 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
        /// </summary>
        /// <param name="key1">要添加的元素的键</param>
        /// <param name="value1">要添加的元素的值。对于引用类型，该值可以为 null。</param>
        /// <param name="key2">要添加的元素的键</param>
        /// <param name="value2">要添加的元素的值。对于引用类型，该值可以为 null。</param>
        public static StringObjectArray Create(string key1, object value1, string key2, object value2)
        {
            StringObjectArray objArray = new StringObjectArray(2);
            objArray.Add(key1, value1);
            objArray.Add(key2, value2);
            return objArray;
        }
        /// <summary>
        /// 初始化 StringObjectArray 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
        /// </summary>
        /// <param name="key1">要添加的元素的键</param>
        /// <param name="value1">要添加的元素的值。对于引用类型，该值可以为 null。</param>
        /// <param name="key2">要添加的元素的键</param>
        /// <param name="value2">要添加的元素的值。对于引用类型，该值可以为 null。</param>
        /// <param name="key3">要添加的元素的键</param>
        /// <param name="value3">要添加的元素的值。对于引用类型，该值可以为 null。</param>
        public static StringObjectArray Create(string key1, object value1, string key2, object value2, string key3, object value3)
        {
            StringObjectArray objArray = new StringObjectArray(3);
            objArray.Add(key1, value1);
            objArray.Add(key2, value2);
            objArray.Add(key3, value3);
            return objArray;
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化 StringObjectArray 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
        /// </summary>
        public StringObjectArray() : this(5) { }

        /// <summary>
        /// 初始化 StringObjectArray 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;TKey,TValue&gt;中复制的元素并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="dictionary">System.Collections.Generic.IDictionary&lt;TKey,TValue&gt;，它的元素被复制到新的 StringObjectArray 中</param>
        public StringObjectArray(IDictionary<string, object> dictionary) : base(dictionary) { }

        /// <summary>
        /// 初始化StringObjectArray类的新实例，该实例使用指定的 IEqualityComparer。
        /// </summary>
        /// <param name="comparer"></param>
        public StringObjectArray(StringComparer comparer) : this(5) { }

        /// <summary>
        /// 初始化StringObjectArray类的新实例，该实例为空且具有指定的初始容量，并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="capacity">StringObjectArray可包含的初始元素数。</param>
        public StringObjectArray(int capacity) : base(capacity) { }

        /// <summary>
        ///  初始化 Basic.Common.StringObjectArray类的新实例，该实例为空且具有指定的初始容量，并使用指定的System.StringComparer。
        /// </summary>
        /// <param name="capacity">Basic.Common.StringObjectArray 可包含的初始元素数。</param>
        /// <param name="comparer">比较键时要使用的 System.StringComparer，以便为键类型使用默认的System.StringComparer</param>
        public StringObjectArray(int capacity, StringComparer comparer) : base(capacity, comparer) { }

        /// <summary>
        /// 用序列化数据初始化StringObjectArray 类的新实例。
        /// </summary>
        /// <param name="info">一个SerializationInfo对象，它包含序列化StringObjectArray所需的信息。</param>
        /// <param name="context">StreamingContext 结构，该结构包含与StringObjectArray相关联的序列化流的源和目标。</param>
        internal StringObjectArray(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion

        #region IStringObjectArray Members
        /// <summary>
        /// 获取或设置具有指定键的元素。
        /// </summary>
        /// <param name="key">要获取或设置的元素的键。</param>
        /// <returns>带有指定键的元素。</returns>
        public new object this[string key]
        {
            get
            {
                if (ContainsKey(key))
                    return base[key];
                return null;
            }
            set
            {
                if (ContainsKey(key))
                    base[key] = value;
                else
                    base.Add(key, value);
            }
        }

        /// <summary>
        /// 设置指定键的值
        /// </summary>
        /// <param name="keyName">字典中键的名称</param>
        /// <param name="value">需要设置的值</param>
        public void SetObject(string keyName, object value)
        {
            base.Add(keyName, value);
        }

        /// <summary>
        /// 获取指定键的Object值。
        /// </summary>
        /// <param name="keyName">字典中键的名称</param>
        /// <returns>指定键的Object值。</returns>
        public object GetObject(string keyName)
        {
            return this[keyName];
        }
        #endregion

        #region ICustomTypeDescriptor Members

        /// <summary>
        /// 返回此组件实例的自定义属性的集合。
        /// </summary>
        /// <returns>包含此对象的属性的 AttributeCollection。</returns>
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return new AttributeCollection(null);
        }

        /// <summary>
        /// 返回此组件实例的类名。
        /// </summary>
        /// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }

        /// <summary>
        /// 返回此组件实例的名称。
        /// </summary>
        /// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        /// <summary>
        /// 返回此组件实例的类型转换器。
        /// </summary>
        /// <returns>表示该对象的转换器的 TypeConverter；如果此对象没有任何 TypeConverter，则为 null。</returns>
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        /// <summary>
        /// 返回此组件实例的默认事件。
        /// </summary>
        /// <returns>表示该对象的默认事件的 EventDescriptor；如果该对象没有事件，则为 null。</returns>
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        /// <summary>
        /// 返回此组件实例的默认属性。
        /// </summary>
        /// <returns>表示该对象的默认属性的 PropertyDescriptor；如果此对象没有属性，则为null。</returns>
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        /// <summary>
        /// 返回此组件实例的指定类型的编辑器。
        /// </summary>
        /// <param name="editorBaseType">表示该对象的编辑器的 Type。</param>
        /// <returns>表示该对象编辑器的指定类型的 Object；如果无法找到编辑器，则为null。</returns>
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        /// <summary>
        /// 将指定的属性数组用作筛选器来返回此组件实例的事件。
        /// </summary>
        /// <param name="attributes">用作筛选器的 Attribute 类型数组。</param>
        /// <returns>表示此组件实例的已筛选事件的 EventDescriptorCollection。</returns>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(null);
        }

        /// <summary>
        /// 返回此组件实例的事件。
        /// </summary>
        /// <returns>表示此组件实例的事件的 EventDescriptorCollection。</returns>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return new EventDescriptorCollection(null);
        }

        private PropertyDescriptorCollection propertyDescriptorCollectionCache = null;

        /// <summary>
        /// 返回将属性 (Attribute) 数组用作筛选器的此组件实例的属性 (Property)。
        /// </summary>
        /// <param name="attributes">用作筛选器的 Attribute 类型数组。</param>
        /// <returns>表示此组件实例的已筛选属性的 PropertyDescriptorCollection。</returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            if (Count <= 0)
            {
                return new PropertyDescriptorCollection(null);
            }

            if (propertyDescriptorCollectionCache == null)
            {
                propertyDescriptorCollectionCache = new PropertyDescriptorCollection(null);
                foreach (string keyName in base.Keys)
                {
                    propertyDescriptorCollectionCache.Add(new StringPropertyDescriptor(keyName));
                }
            }
            return propertyDescriptorCollectionCache;

        }

        /// <summary>
        /// 返回此组件实例的属性。
        /// </summary>
        /// <returns>表示此组件实例的属性的 PropertyDescriptorCollection。</returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        /// <summary>
        /// 返回包含指定的属性描述符所描述的属性的对象。
        /// </summary>
        /// <param name="pd">表示要查找其所有者的属性的 PropertyDescriptor。</param>
        /// <returns>表示指定属性所有者的 Object。</returns>
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region ISerializable Members

        /// <summary>
        /// 使用将目标对象序列化所需的数据填充 SerializationInfo。
        /// </summary>
        /// <param name="info">要填充数据的 SerializationInfo。</param>
        /// <param name="context">此序列化的目标。</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// 返回循环访问StringObjectArray的枚举数
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion
    }
}
