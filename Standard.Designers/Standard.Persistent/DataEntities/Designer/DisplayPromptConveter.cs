using Basic.Configuration;
using Basic.DataEntities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basic.Designer
{
	/// <summary>
	/// 提供数据库当前命令所在实体模型的异常代码转换器。
	/// 提供在字符串对象与其他表示形式之间实现相互转换的类型转换器。
	/// </summary>
	public sealed class DisplayPromptConveter : StringConverter
	{      /// <summary>
		/// 使用指定的上下文返回此对象是否支持可以从列表中选取的标准值集。
		/// </summary>
		/// <param name="context">一个提供格式上下文的 System.ComponentModel.ITypeDescriptorContext。</param>
		/// <returns>如果应调用 System.ComponentModel.TypeConverter.GetStandardValues() 来查找对象支持的一组公共值，则为 true；否则，为 false。</returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

		/// <summary>
		/// 当与格式上下文一起提供时，返回此类型转换器设计用于的数据类型的标准值集合。
		/// </summary>
		/// <param name="context">提供格式上下文的 System.ComponentModel.ITypeDescriptorContext，可用来提取有关从中调用此转换器的环境的附加信息。此参数或其属性可以为 null。</param>
		/// <returns>包含标准有效值集的 System.ComponentModel.TypeConverter.StandardValuesCollection；如果数据类型不支持标准值集，则为 null。</returns>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			StringCollection strings = new StringCollection();
			DisplayNameElement display = context.Instance as DisplayNameElement;
			if (display.Property.Owner is DataEntityElement)
			{
				DataEntityElement dataEntityElement = display.Property.Owner as DataEntityElement;
				string groupName = dataEntityElement.Persistent.GroupName;
				foreach (DataEntityPropertyElement property in dataEntityElement.Properties)
				{
					if (!string.IsNullOrWhiteSpace(property.DisplayName.DisplayName))
						strings.Add(string.Concat(property.DisplayName.DisplayName, "_Prompt"));
					else
						strings.Add(string.Concat(groupName, "_", property.Name, "_Prompt"));
				}
			}
			else if (display.Property.Owner is DataConditionElement)
			{
				DataConditionElement dataConditionElement = display.Property.Owner as DataConditionElement;
				string groupName = dataConditionElement.Persistent.GroupName;
				foreach (DataConditionPropertyElement property in dataConditionElement.Arguments)
				{
					if (!string.IsNullOrWhiteSpace(property.DisplayName.DisplayName))
						strings.Add(string.Concat(property.DisplayName.DisplayName, "_Prompt"));
					else
						strings.Add(string.Concat(groupName, "_", property.Name, "_Prompt"));
				}
			}
			return new StandardValuesCollection(strings);
		}
	}
}
