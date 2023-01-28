using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Basic.Configuration;

namespace Basic.Designer
{
	/// <summary>
	/// ��ʾ��֤���ϱ༭��
	/// </summary>
	public class CommandParametersEditor : CollectionEditor
	{
		/// <summary>
		/// ʹ��ָ���ļ������ͳ�ʼ�� ValidationAttributesEditor �����ʵ��
		/// </summary>
		public CommandParametersEditor(Type type) : base(type) { }

		/// <summary>
		/// ��ȡ�˼��ϱ༭���ɰ������������͡�
		/// </summary>
		/// <returns>�˼��Ͽɰ����������������顣</returns>
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(CommandParameter) };
		}

		/// <summary>
		///  ����ָ���ļ��������͵���ʵ����
		/// </summary>
		/// <param name="itemType">Ҫ�����������͡�</param>
		/// <returns>ָ���������ʵ����</returns>
		protected override object CreateInstance(Type itemType)
		{
			if (base.Context.Instance is StaticCommandDescriptor)
			{
				StaticCommandDescriptor entityPropertyDescriptor = base.Context.Instance as StaticCommandDescriptor;
				return TypeDescriptor.CreateInstance(null, itemType, null, new object[] { entityPropertyDescriptor.DefinitionInfo });
			}
			else if (base.Context.Instance is DynamicCommandDescriptor)
			{
				DynamicCommandDescriptor entityPropertyDescriptor1 = base.Context.Instance as DynamicCommandDescriptor;
				return TypeDescriptor.CreateInstance(null, itemType, null, new object[] { entityPropertyDescriptor1.DefinitionInfo });
			}
			return TypeDescriptor.CreateInstance(null, itemType, null, new object[] { base.Context.Instance });
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
