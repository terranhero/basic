using System;
using System.ComponentModel;
using Basic.Configuration;
using Basic.Enums;

namespace Basic.Designer
{
	/// <summary>
	/// 属性包装器
	/// </summary>
	/// <typeparam name="TDD">需要包装属性的信息</typeparam>
	internal sealed class StaticCommandDescriptor : ObjectDescriptor<StaticCommandElement>
	{
		private PropertyDescriptorCollection propertyDescriptors = null;
		/// <summary>
		/// 初始化 StaticCommandDescriptor 实例
		/// </summary>
		/// <param name="dInfo"></param>
		public StaticCommandDescriptor(StaticCommandElement dInfo) : base(dInfo) { }

		/// <summary>
		/// 返回将特性数组用作筛选器的此组件实例的属性。
		/// </summary>
		/// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
		/// <returns>表示此组件实例的已筛选属性的 System.ComponentModel.PropertyDescriptorCollection。</returns>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			StaticMethodEnum methodEnum = base.DefinitionInfo.ExecutableMethod;
			ConfigurationTypeEnum kind = base.DefinitionInfo.Kind;
			PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
			if (propertyDescriptors == null)
				propertyDescriptors = base.GetProperties(attributes);
			foreach (PropertyDescriptor property in propertyDescriptors)
			{
				if ((methodEnum != StaticMethodEnum.FillDataSet && methodEnum != StaticMethodEnum.FillDataTable &&
					methodEnum != StaticMethodEnum.GetPagination) && property.Name == "DataCondition") { continue; }
				if ((methodEnum != StaticMethodEnum.FillDataSet && methodEnum != StaticMethodEnum.FillDataTable &&
				methodEnum != StaticMethodEnum.GetPagination) && property.Name == "Arguments") { continue; }
				else if ((kind == ConfigurationTypeEnum.AddNew || kind == ConfigurationTypeEnum.Modify ||
				 kind == ConfigurationTypeEnum.Remove || kind == ConfigurationTypeEnum.SearchTable ||
				 kind == ConfigurationTypeEnum.SelectByKey) && property.Name == "Name") { continue; }
				else if (property.IsBrowsable)
					properties.Add(property);
			}
			foreach (NewCommandElement newCommand in base.DefinitionInfo.NewCommands)
			{
				properties.Add(new CommandPropertyDescriptor<NewCommandElement>(newCommand));
			}
			foreach (CheckedCommandElement newCommand in base.DefinitionInfo.CheckCommands)
			{
				properties.Add(new CommandPropertyDescriptor<CheckedCommandElement>(newCommand));
			}
            //foreach (CommandParameter parameter in base.DefinitionInfo.Parameters)
            //{
            //    properties.Add(new CommandParameterDescriptor(parameter));
            //}
			return properties;
		}
	}
}
