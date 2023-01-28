using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Basic.Collections
{
    /// <summary>
    /// 提供StringObjectArray类上的属性。
    /// </summary>
    internal sealed class StringPropertyDescriptor : PropertyDescriptor
    {
        private string keyName = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pKeyName"></param>
        internal StringPropertyDescriptor(string pKeyName)
            : base(pKeyName, null)
        {
            keyName = pKeyName;
        }

        /// <summary>
        /// 返回重置对象时是否更改其值。
        /// </summary>
        /// <param name="component">要测试重置功能的组件。</param>
        /// <returns>如果重置组件更改其值，则为 true；否则为 false。</returns>
        public override bool CanResetValue(object component)
        {
            return true;
        }

        /// <summary>
        /// 获取该属性绑定到的组件的类型。
        /// </summary>
        public override Type ComponentType
        {
            get { return typeof(KeyValuePair<string, object>); }
        }

        /// <summary>
        /// 获取组件上的属性的当前值。
        /// </summary>
        /// <param name="component">具有为其检索值的属性的组件。</param>
        /// <returns>给定组件的属性的值。</returns>
        public override object GetValue(object component)
        {
            return (component as IStringObjectArray).GetObject(keyName);
        }

        /// <summary>
        /// 获取指示该属性是否为只读的值。
        /// </summary>
        public override bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 获取该属性的类型。
        /// </summary>
        public override Type PropertyType
        {
            get { return typeof(object); }
        }

        /// <summary>
        /// 将组件的此属性的值重置为默认值。
        /// </summary>
        /// <param name="component">具有要重置为默认值的属性值的组件。</param>
        public override void ResetValue(object component)
        {
            (component as IStringObjectArray).SetObject(keyName, null);
        }

        /// <summary>
        /// 将组件的值设置为一个不同的值。
        /// </summary>
        /// <param name="component">具有要进行设置的属性值的组件。</param>
        /// <param name="value">新值。</param>
        public override void SetValue(object component, object value)
        {
            (component as IStringObjectArray).SetObject(keyName, value);
        }

        /// <summary>
        /// 确定一个值，该值指示是否需要永久保存此属性的值。
        /// </summary>
        /// <param name="component">具有要检查其持久性的属性的组件。</param>
        /// <returns>如果属性应该被永久保存，则为 true；否则为 false。</returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
