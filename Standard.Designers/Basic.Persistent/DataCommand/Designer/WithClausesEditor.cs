using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using Basic.Designer;

namespace Basic.Designer
{
	/// <summary>
	/// ��ʾ��֤���ϱ༭��
	/// </summary>
	public class WithClausesEditor : CollectionEditor
	{
		/// <summary>
		/// ʹ��ָ���ļ������ͳ�ʼ�� WithClausesEditor �����ʵ��
		/// </summary>
		public WithClausesEditor(Type type) : base(type) { }

		/// <summary>
		/// ��ȡ�˼��ϱ༭���ɰ������������͡�
		/// </summary>
		/// <returns>�˼��Ͽɰ����������������顣</returns>
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(WithClause) };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemType"></param>
		/// <returns></returns>
		protected override object CreateInstance(Type itemType)
		{
			//IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			DynamicCommandDescriptor descriptor = base.Context.Instance as DynamicCommandDescriptor;
			return TypeDescriptor.CreateInstance(null, itemType, null, new object[] { descriptor.DefinitionInfo });
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
