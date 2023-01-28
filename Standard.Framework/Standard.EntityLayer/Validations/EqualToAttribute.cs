using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel;

namespace Basic.Validations
{
    /// <summary>
    /// 提供一个比较模型的两个属性的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EqualToAttribute : ValidationAttribute
    {
        /// <summary>
		/// 初始化 EqualToAttribute 类实例
        /// </summary>
        /// <param name="otherProperty"></param>
        public EqualToAttribute(string otherProperty)
        {
            if (otherProperty == null)
            {
                throw new ArgumentNullException("otherProperty");
            }
            this.OtherProperty = otherProperty;
            this.OtherPropertyDisplayName = null;
        }

        /// <summary>
        /// 基于出现比较错误的数据字段对错误消息应用格式设置。
        /// </summary>
        /// <param name="name">导致验证失败的字段的名称。</param>
        /// <returns>带有格式的错误消息。</returns>
        public override string FormatErrorMessage(string name)
        {
            if ((base.ErrorMessage == null) && (base.ErrorMessageResourceName == null))
            {
                base.ErrorMessage = "'{0}' and '{1}' do not match.";
            }
            string str = this.OtherPropertyDisplayName ?? this.OtherProperty;
            return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, new object[] { name, str });
        }
#if(NET35)
        /// <summary>
        /// 确定对象的指定值是否有效。
        /// </summary>
        /// <param name="value">要验证的对象的值。</param>
        /// <returns>如果指定的值有效，则为 true；否则，为 false。</returns>
        public override bool IsValid(object value)
        {
            return true;
        }
#else

		/// <summary>
		/// 根据当前的验证特性来验证指定的值。
		/// </summary>
		/// <param name="value">要验证的值。</param>
		/// <param name="validationContext">有关验证操作的上下文信息。</param>
		/// <returns>System.ComponentModel.DataAnnotations.ValidationResult 类的实例。</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			string[] memberNames = new string[] { validationContext.MemberName };
			PropertyInfo property = validationContext.ObjectType.GetProperty(this.OtherProperty);
			if (property == null)
			{
				return new ValidationResult(string.Format(CultureInfo.CurrentCulture, "Could not find a property named {0}.", new object[] { this.OtherProperty }), memberNames);
			}
			DisplayAttribute attribute = (DisplayAttribute)Attribute.GetCustomAttribute(property, typeof(DisplayAttribute));
			if ((attribute != null) && !string.IsNullOrWhiteSpace(attribute.Name))
			{
				this.OtherPropertyDisplayName = attribute.Name;
			}
			object objB = property.GetValue(validationContext.ObjectInstance, null);
			if (!object.Equals(value, objB))
			{
				return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), memberNames);
			}
			return null;
		}
#endif

        /// <summary>
        /// 获取要与当前属性进行比较的属性。
        /// </summary>
        /// <value>要与当前属性进行比较的属性。</value>
        public string OtherProperty { get; private set; }

        /// <summary>
        ///  获取或设置要与当前属性进行比较的属性显示名称。
        /// </summary>
        public string OtherPropertyDisplayName { get; set; }
    }


}
