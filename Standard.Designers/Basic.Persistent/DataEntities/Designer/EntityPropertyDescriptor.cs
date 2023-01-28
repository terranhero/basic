using System;
using System.ComponentModel;
using Basic.DataEntities;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Collections;

namespace Basic.Designer
{
    /// <summary>
    /// 属性包装器
    /// </summary>
    /// <typeparam name="TDD">需要包装属性的信息</typeparam>
    internal sealed class EntityPropertyDescriptor : ObjectDescriptor<AbstractPropertyElement>
    {
        private PropertyDescriptorCollection propertyDescriptors = null;
        /// <summary>
        /// 初始化 ParameterDescriptor 实例
        /// </summary>
        /// <param name="dInfo"></param>
        public EntityPropertyDescriptor(AbstractPropertyElement dInfo) : base(dInfo) { }

        /// <summary>
        /// 返回将特性数组用作筛选器的此组件实例的属性。
        /// </summary>
        /// <param name="attributes">用作筛选器的 System.Attribute 类型数组。</param>
        /// <returns>表示此组件实例的已筛选属性的 System.ComponentModel.PropertyDescriptorCollection。</returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            SortedPropertyDescriptorCollection properties = new SortedPropertyDescriptorCollection();
            if (propertyDescriptors == null)
                propertyDescriptors = base.GetProperties(attributes).Sort(new string[] { "Name", "Type","TypeName", "PrimaryKey", 
					"Inheritance", "Modifier", "Comment","Profix","Column","DbType","Size","Precision", "Scale","Nullable" });
            foreach (PropertyDescriptor property in propertyDescriptors)
            {
                if (string.IsNullOrWhiteSpace(DefinitionInfo.Column))
                {
                    if (property.Name == "Profix" || property.Name == "DbType" || property.Name == "Size" || property.Name == "DefaultValue" ||
                        property.Name == "Precision" || property.Name == "Scale" || property.Name == "Nullable")
                        continue;
                }
                else if (DefinitionInfo.DbType == DbTypeEnum.Binary || DefinitionInfo.DbType == DbTypeEnum.VarChar ||
                    DefinitionInfo.DbType == DbTypeEnum.Char || DefinitionInfo.DbType == DbTypeEnum.NChar ||
                    DefinitionInfo.DbType == DbTypeEnum.NVarChar || DefinitionInfo.DbType == DbTypeEnum.VarBinary)
                {
                    if (property.Name == "Precision" || property.Name == "Scale")
                        continue;
                }
                else if (DefinitionInfo.DbType == DbTypeEnum.Decimal)
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
            if (DefinitionInfo is DataEntityPropertyElement)
            {
                DataEntityPropertyElement entityProperty = DefinitionInfo as DataEntityPropertyElement;
                if (entityProperty.Attributes.Count > 0)
                {
                    foreach (AbstractAttribute aa in entityProperty.Attributes)
                    {
                        properties.Add(new AttributePropertyDescriptor(aa));
                    }
                }
            }
            return properties.ToSortedCollection();
        }
    }
}
