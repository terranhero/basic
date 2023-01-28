using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using Basic.Designer;

namespace Basic.DataEntities
{
    /// <summary>
    /// ��ʾ��֤���ϱ༭��
    /// </summary>
    public class ValidationAttributesEditor : CollectionEditor
    {
        /// <summary>
        /// ʹ��ָ���ļ������ͳ�ʼ�� ValidationAttributesEditor �����ʵ��
        /// </summary>
        public ValidationAttributesEditor(Type type) : base(type) { }

        /// <summary>
        /// ��ȡ�˼��ϱ༭���ɰ������������͡�
        /// </summary>
        /// <returns>�˼��Ͽɰ����������������顣</returns>
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
        /// ָʾ�Ƿ��һ��ѡ���������
        /// </summary>
        /// <returns></returns>
        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }
    }
}
