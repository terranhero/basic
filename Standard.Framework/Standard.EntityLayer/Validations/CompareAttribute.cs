using Basic.EntityLayer;
using Basic.Enums;
using Basic.Messages;
using Basic.Properties;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Basic.Validations
{
	/// <summary>
	/// 提供一个比较模型的两个属性的特性。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class CompareAttribute : ValidationAttribute
	{
		/// <summary>
		/// 初始化 WebCompareAttribute 类实例
		/// </summary>
		/// <param name="otherProperty">需要比较的属性名称</param>
		/// <param name="op">比较运算符</param>
		public CompareAttribute(string otherProperty, ValidationCompareOperator op)
			: this(otherProperty, ValidationDataType.String, op) { }

		/// <summary>
		/// 初始化 WebCompareAttribute 类实例
		/// </summary>
		/// <param name="otherProperty">需要比较的属性名称</param>
		/// <param name="type">比较类型</param>
		public CompareAttribute(string otherProperty, ValidationDataType type)
			: this(otherProperty, type, ValidationCompareOperator.Equal) { }

		/// <summary>
		/// 初始化 WebCompareAttribute 类实例
		/// </summary>
		/// <param name="otherProperty">需要比较的属性名称</param>
		/// <param name="type">比较类型</param>
		/// <param name="op">比较运算符</param>
		public CompareAttribute(string otherProperty, ValidationDataType type = ValidationDataType.String,
			ValidationCompareOperator op = ValidationCompareOperator.Equal)
			: base()
		{
			OtherProperty = otherProperty;
			Operator = op;
			ValidationDataType = type;
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
		/// 基于发生错误的数据字段对错误消息应用格式设置。
		/// </summary>
		/// <param name="validationContext"> 有关验证操作的上下文信息。</param>
		/// <returns>带有格式的错误消息的实例。</returns>
		private string FormatErrorMessage(ValidationContext validationContext)
		{
			GroupNameAttribute gna = (GroupNameAttribute)Attribute.GetCustomAttribute(validationContext.ObjectType, typeof(GroupNameAttribute));
			if (gna != null)
			{
				string keyName = string.Concat(gna.Name, "_", validationContext.MemberName, "_Compare");
				string value = MessageContext.GetString(gna.ConverterName, keyName, CultureInfo.CurrentUICulture);
				if (value != keyName) { return value; }
			}
			return base.FormatErrorMessage(validationContext.DisplayName);
		}

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
				return new ValidationResult(string.Format(Strings.UnknownProperty, new object[] { this.OtherProperty }), memberNames);
			}
			object otherValue = property.GetValue(validationContext.ObjectInstance, null);
			bool result = Compare(value, otherValue);
			if (!result)
			{
				return new ValidationResult(this.FormatErrorMessage(validationContext), memberNames);
			}
			return null;
		}
#endif

		private bool Compare(object value, object otherValue)
		{
			int num = 0;
			if (value != null && otherValue != null)
			{
				switch (ValidationDataType)
				{
					case ValidationDataType.String:
						num = string.Compare((string)value, (string)otherValue, false, CultureInfo.CurrentCulture);
						break;
					case ValidationDataType.Short:
						num = ((short)value).CompareTo(otherValue);
						break;
					case ValidationDataType.Long:
						num = ((long)value).CompareTo(otherValue);
						break;
					case ValidationDataType.Integer:
						num = ((int)value).CompareTo(otherValue);
						break;
					case ValidationDataType.Double:
						num = ((double)value).CompareTo(otherValue);
						break;
					case ValidationDataType.Date:
					case ValidationDataType.DateTime:
						num = ((DateTime)value).CompareTo(otherValue);
						break;
					case ValidationDataType.TimeSpan:
						num = ((TimeSpan)value).CompareTo(otherValue);
						break;
					case ValidationDataType.Decimal:
						num = ((decimal)value).CompareTo(otherValue);
						break;
				}
			}
			switch (this.Operator)
			{
				case ValidationCompareOperator.Equal:
					return (num == 0);
				case ValidationCompareOperator.NotEqual:
					return (num != 0);
				case ValidationCompareOperator.GreaterThan:
					return (num > 0);
				case ValidationCompareOperator.GreaterThanEqual:
					return (num >= 0);
				case ValidationCompareOperator.LessThan:
					return (num < 0);
				case ValidationCompareOperator.LessThanEqual:
					return (num <= 0);
			}
			return true;

		}

		/// <summary>
		/// 表示需要比较的数据类型
		/// </summary>
		public ValidationDataType ValidationDataType { get; private set; }
		/// <summary>
		/// 需要与当前属性比较的属性
		/// </summary>
		public string OtherProperty { get; private set; }
		/// <summary>
		/// 比较运算符
		/// </summary>
		public ValidationCompareOperator Operator { get; private set; }
	}
}
