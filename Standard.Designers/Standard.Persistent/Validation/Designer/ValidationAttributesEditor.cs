using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using Basic.Designer;

namespace Basic.DataEntities
{
    /// <summary>
    /// 表示验证集合编辑器
    /// </summary>
    public class ValidationAttributesEditor : CollectionEditor
    {
        /// <summary>
        /// 使用指定的集合类型初始化 ValidationAttributesEditor 类的新实例
        /// </summary>
        public ValidationAttributesEditor(Type type) : base(type) { }

        /// <summary>
        /// 获取此集合编辑器可包含的数据类型。
        /// </summary>
        /// <returns>此集合可包含的数据类型数组。</returns>
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(DisplayFormat),typeof(ImportPorpertyAttribute),typeof(RequiredValidation),typeof(BoolRequiredValidation),typeof(CompareValidation),
				typeof(RangeValidation) ,typeof(RegularExpressionValidation),typeof(MaxLengthValidation),typeof(StringLengthValidation)	};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        protected override object CreateInstance(Type itemType)
        {
            //IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
            EntityPropertyDescriptor entityPropertyDescriptor = base.Context.Instance as EntityPropertyDescriptor;
            return TypeDescriptor.CreateInstance(null, itemType, null, new object[] { entityPropertyDescriptor.DefinitionInfo });
        }

        /// <summary>
        /// 指示是否可一次选择多个集合项。
        /// </summary>
        /// <returns></returns>
        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }
    }
}
