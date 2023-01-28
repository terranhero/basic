using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Basic.Configuration;

namespace Basic.Designer
{
	/// <summary>
	/// ��ʾ��֤���ϱ༭��
	/// </summary>
	public class CheckCommandsEditor : CollectionEditor
	{
		/// <summary>
		/// ʹ��ָ���ļ������ͳ�ʼ�� ValidationAttributesEditor �����ʵ��
		/// </summary>
		public CheckCommandsEditor(Type type) : base(type) { }

		/// <summary>
		/// ��ȡ�˼��ϱ༭���ɰ������������͡�
		/// </summary>
		/// <returns>�˼��Ͽɰ����������������顣</returns>
		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(CheckedCommandElement) };
		}

		/// <summary>
		///  ����ָ���ļ��������͵���ʵ����
		/// </summary>
		/// <param name="itemType">Ҫ�����������͡�</param>
		/// <returns>ָ���������ʵ����</returns>
		protected override object CreateInstance(Type itemType)
		{
			StaticCommandDescriptor entityPropertyDescriptor = base.Context.Instance as StaticCommandDescriptor;
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
