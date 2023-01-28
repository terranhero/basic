using System;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;

namespace GoldSoft.Basic.Exceptions
{
	/// <summary>
	/// �Զ����쳣�Ļ���,����ʱ������������ʱ�������Ѵ�����Ϣ
	/// </summary>
	[Serializable()]
	public class RuntimeException : ApplicationException
	{
		private string _Message = string.Empty;
		/// <summary>
		/// ��ȡ�쳣��Ϣ�Ĵ������
		/// </summary>
		public virtual string ErrorCode { get; private set; }

		/// <summary>
		/// ��ȡ������ǰ�쳣����Ϣ��
		/// </summary>
		public new string Message
		{
			get { return _Message; }
			set { _Message = value; }
		}

		/// <summary>
		/// ʹ��ָ�����쳣�����ʼ�� GoldSoftException �����ʵ���� 
		/// </summary>
		/// <param name="errorCode">�ض��쳣������Ϣ����Դ��</param>
		public RuntimeException(string errorCode) : this(errorCode, null, null) { }

		/// <summary>
		/// ʹ��ָ�����쳣�����ʼ�� GoldSoftException �����ʵ���� 
		/// </summary>
		/// <param name="errorCode">�ض��쳣������Ϣ����Դ��</param>
		/// <param name="paramArray">��ʽ����Դ�Ĳ�������</param>
		public RuntimeException(string errorCode, params object[] paramArray) : this(errorCode, null, paramArray) { }


		/// <summary>
		/// ʹ��ָ�����쳣����ʹ�����Ϣ,����Ϊ���쳣ԭ����ڲ��쳣�����ó�ʼ�� RuntimeException �����ʵ���� 
		/// </summary>
		/// <param name="errorCode">�ض��쳣������Ϣ�Ķ����Ա���</param>
		/// <param name="innerException">���µ�ǰ�쳣���쳣�����δָ���ڲ��쳣������һ�������á�</param>
		/// <param name="paramArray">��������Ϣ�Ĳ�������</param>
		private RuntimeException(string errorCode, Exception innerException, params object[] paramArray)
			: base(null, innerException)
		{
			ErrorCode = errorCode;
		}

		/// <summary>
		/// �����л����ݳ�ʼ�� GoldSoftException �����ʵ���� 
		/// </summary>
		/// <param name="info">�������й��������쳣�����л��Ķ������ݡ�</param>
		/// <param name="context">�������й�Դ��Ŀ�����������Ϣ�� </param>
		protected RuntimeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			ErrorCode = info.GetString("ErrorCode");
			_Message = info.GetString("CustomMessage");
		}

		/// <summary>
		/// ʹ�ý� GoldSoftException �������л������������� SerializationInfo��
		/// </summary>
		/// <param name="info">Ҫ������ݵ� SerializationInfo��</param>
		/// <param name="context">�����л���Ŀ�꣨��μ� StreamingContext����</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ErrorCode", ErrorCode);
			info.AddValue("CustomMessage", _Message);
		}
	}
}
