using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Basic.Collections;

using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>
	/// ����������ӿ�
	/// </summary>
	public abstract class CommandBuilder : IDisposable
	{
		///// <summary>
		///// �����������ƴ������ݿ�����
		///// </summary>
		///// <param name="tableCache">��ǰ�����ļ��Ļ���</param>
		//public abstract void CreateDataCommand(DatabaseConfiguration tableCache);

		///// <summary>
		///// �����������ƴ������ݿ�����
		///// </summary>
		///// <param name="dbCommands">��ǰ�����ļ��Ļ���</param>
		///// <param name="configFileName">�����ļ�����</param>
		//public abstract void CreateDataCommand(DatabaseCommands dbCommands, string configFileName);

		/// <summary>
		/// �����������ƴ������ݿ�����
		/// </summary>
		/// <param name="tableInfo">��ǰ�����ļ��Ļ���</param>
		internal abstract void CreateDataCommand(TableConfiguration tableInfo);

		/// <summary>
		/// ִ�����ͷŻ����÷��й���Դ��ص�Ӧ�ó����������
		/// </summary>
		public virtual void Dispose() { }
	}
}
