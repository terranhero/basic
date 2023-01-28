using Basic.Properties;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Basic.Validations
{
    /// <summary>
    /// 提供一个带判断条件的必输验证特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class BoolRequiredAttribute : RequiredAttribute
    {
        /// <summary>
        /// 初始化 WebCompareAttribute 类实例
        /// </summary>
        /// <param name="otherProperty">需要比较的属性名称</param>
        public BoolRequiredAttribute(string otherProperty) : this(otherProperty, true) { }

        /// <summary>
        /// 初始化 WebCompareAttribute 类实例
        /// </summary>
        /// <param name="otherProperty">需要比较的属性名称</param>
        /// <param name="requiredValue">判断目标属性为何值时，必输！</param>
        public BoolRequiredAttribute(string otherProperty, object requiredValue)
            : base()
        {
            OtherProperty = otherProperty;
            RequiredValue = requiredValue;
        }
#if(NET35)
        /// <summary>
        /// 确定对象的指定值是否有效。
        /// </summary>
        /// <param name="value">要验证的对象的值。</param>
        /// <returns>如果指定的值有效，则为 true；否则，为 false。</returns>
        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }
#else

		/// <summary>
		/// 根据当前的验证特性来验证指定的值。
		/// </summary>
		/// <param name="value">要验证的值。</param>
		/// <param name="validationContext">有关验证操作的上下文信息。</param>
		/// <returns> System.ComponentModel.DataAnnotations.ValidationResult 类的实例。</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			string[] memberNames = new string[] { validationContext.MemberName };
			PropertyInfo property = validationContext.ObjectType.GetProperty(this.OtherProperty);
			if (property == null)
			{
				return new ValidationResult(string.Format(Strings.UnknownProperty,
					new object[] { this.OtherProperty }), memberNames);
			}
			object otherValue = property.GetValue(validationContext.ObjectInstance, null);
			if (property.PropertyType != RequiredValue.GetType())
			{
				return new ValidationResult(string.Format(Strings.ErrorPropertyType,
					new object[] { this.OtherProperty, "RequiredValue" }), memberNames);
			}
			if (object.Equals(otherValue, RequiredValue))
			{
				return base.IsValid(value, validationContext);
			}
			return null;
		}
#endif
        /// <summary>
        /// 需要与当前属性比较的属性
        /// </summary>
        public string OtherProperty { get; private set; }

        /// <summary>
        /// 目标属性为何值时，必输！
        /// </summary>
        public object RequiredValue { get; private set; }
    }
}
