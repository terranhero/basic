using System;
using System.ComponentModel;
using Basic.Configuration;
using Basic.EntityLayer;
using Basic.Enums;

namespace Basic.Designer
{
	/// <summary>
	/// 属性包装器
	/// </summary>
	/// <typeparam name="TDD">需要包装属性的信息</typeparam>
	internal sealed class ParameterDescriptor : ObjectDescriptor<CommandParameter>
	{
		private PropertyDescriptorCollection propertyDescriptors = null;
		/// <summary>
		/// 初始化 ParameterDescriptor 实例
		/// </summary>
		/// <param name="dInfo"></param>
		public ParameterDescriptor(CommandParameter dInfo) : base(dInfo) { }

		/// <summary>
		/// 返回将特性数组用作筛选器的此组件实例的属性。
		/// </summary>
		/// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
		/// <returns>表示此组件实例的已筛选属性的 System.ComponentModel.PropertyDescriptorCollection。</returns>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
			if (propertyDescriptors == null)
				propertyDescriptors = base.GetProperties(attributes);
			foreach (PropertyDescriptor property in propertyDescriptors)
			{
				if (string.IsNullOrWhiteSpace(DefinitionInfo.Name))
				{
					if (property.Name == "SourceColumn" || property.Name == "Size" || property.Name == "Nullable" || property.Name == "Direction" ||
						property.Name == "Precision" || property.Name == "Scale" || property.Name == "ParameterType")
						continue;
				}
				else if (DefinitionInfo.ParameterType == DbTypeEnum.Binary || DefinitionInfo.ParameterType == DbTypeEnum.VarChar ||
					DefinitionInfo.ParameterType == DbTypeEnum.Char || DefinitionInfo.ParameterType == DbTypeEnum.NChar ||
					DefinitionInfo.ParameterType == DbTypeEnum.NVarChar || DefinitionInfo.ParameterType == DbTypeEnum.VarBinary)
				{
					if (property.Name == "Precision" || property.Name == "Scale")
						continue;
				}
				else if (DefinitionInfo.ParameterType == DbTypeEnum.Decimal)
				{
					if (property.Name == "Size")
						continue;
				}
				else if (property.Name == "Size" || property.Name == "Precision" || property.Name == "Scale")
				{
					continue;
				}
				if (property.IsBrowsable)
					properties.Add(property);
			}
			return properties.Sort(new string[] { "Name", "SourceColumn", "ParameterType", "Direction", "Nullable", "Size", "Precision", "Scale" });
		}
	}
}
