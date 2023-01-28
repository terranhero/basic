using System;
using System.ComponentModel;
using Basic.Configuration;

namespace Basic.Designer
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class PersistentDescriptor : ObjectDescriptor<PersistentConfiguration>
	{
		/// <summary>
		/// 初始化 ParameterDescriptor 实例
		/// </summary>
		/// <param name="dInfo"></param>
		public PersistentDescriptor(PersistentConfiguration dInfo) : base(dInfo) { }

		private PropertyDescriptorCollection propertyDescriptors = null;
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
				if (property.IsBrowsable)
					properties.Add(property);
			}
			//if (DefinitionInfo.TableInfo != null && DefinitionInfo.TableInfo.Columns.Count > 0)
			//{
			//   foreach (TableColumnElement column in DefinitionInfo.TableInfo.Columns)
			//   {
			//      properties.Add(new TableColumnDescriptor(column));
			//   }
			//}
			return properties;
		}
	}
}
