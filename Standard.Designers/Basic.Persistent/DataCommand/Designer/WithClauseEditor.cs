using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using Basic.Designer;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Basic.Designer
{
	/// <summary>
	/// ��ʾ��֤���ϱ༭��
	/// </summary>
	public class WithClauseEditor : System.Drawing.Design.UITypeEditor
	{
		/// <summary>
		/// ʹ��ָ���ļ������ͳ�ʼ�� WithClauseEditor �����ʵ��
		/// </summary>
		public WithClauseEditor() : base() { }

		/// <summary>
		/// ��ȡ�� EditValue ����ʹ�õı༭����ʽ��
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			//ָ��Ϊģʽ�������Ա༭������
			return UITypeEditorEditStyle.Modal;
		}

		/// <summary>
		/// ʹ�� System.Drawing.Design.UITypeEditor.GetEditStyle() ������ָʾ�ı༭����ʽ�༭ָ�������ֵ��
		/// </summary>
		/// <param name="context">�����ڻ�ȡ������������Ϣ�� System.ComponentModel.ITypeDescriptorContext��</param>
		/// <param name="provider">System.IServiceProvider���˱༭������������ȡ����</param>
		/// <param name="value">Ҫ�༭�Ķ���</param>
		/// <returns>�µĶ���ֵ����������ֵ��δ���ģ��������صĶ���Ӧ�봫�ݸ����Ķ�����ͬ��</returns>
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (editorService == null) { return value; }
				if (value is WithClause && context.Instance is DynamicCommandDescriptor) //�û�ѡ����WithClause���͵�Ԫ��
				{
					WithClauseWindow window = new WithClauseWindow(value as WithClause);
					if (window.ShowModal() == true)
					{
						DynamicCommandDescriptor dcd = context.Instance as DynamicCommandDescriptor;
						dcd.DefinitionInfo.WithClauses.Remove(value as WithClause);
					}
				}
				else if (value is string && context.Instance is WithClause)
				{
					WithClauseWindow window = new WithClauseWindow(context.Instance as WithClause);
					window.ShowModal();
				}
			}
			return base.EditValue(context, provider, value);
		}

	}
}
