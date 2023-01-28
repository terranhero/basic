// GoldSoft.Basic.Cryptography.h

#pragma once

using namespace System;
using namespace System::Reflection;
using namespace System::Collections::Generic;
namespace Basic {
	namespace Cryptography {
		/// <summary>
		/// MD5�����㷨������
		/// </summary>
		public ref class SecurityAlgorithm abstract sealed
		{
		private:
			/// <summary>
			/// ��ʾ�ɰ�ȫ���õĳ����б�
			/// </summary>
			static SortedSet<String^>^ SafeAssemblies = gcnew SortedSet<String^>(
				gcnew array<String^> {"Name", "SourceColumn", "ParameterType", "Direction", "Size", "Precision", "Scale", "Nullable"});

			/// <summary>
			/// ���Ե�ǰ�����Ƿ�Ϸ�
			/// </summary>
			/// <param name="assemblyName">��Ҫ��֤�ĳ���Ψһ��ʶ��</param>
			/// <returns>�����֤�ɹ��򷵻� true�����򷵻� false��</returns>
			static bool CheckAssemblies(AssemblyName^ assemblyName);

			/// <summary>
			/// ��Byte����ת����16�����ַ���
			/// </summary>
			/// <param name="source">��Ҫת���� Byte ���� ��</param>
			/// <returns>����ת���ɹ��ļ���16�����ַ�����</returns>
			static String^ ToHexString(array<unsigned char>^ source);

			/// <summary>
			/// ��16�����ַ���ת����Byte����
			/// </summary>
			/// <param name="source">��Ҫת�����ַ�����</param>
			/// <returns>����ת���ɹ���Byte���顣</returns>
			static array<unsigned char>^ ToByteArray(String^ source);

			/// <summary>
			/// ����������
			/// </summary>
			/// <param name="byteArray">��Ҫ��֤�ĳ���Ψһ��ʶ��</param>
			/// <param name="otherArray">��Ҫ���ܵ� Byte ���顣</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ CreateNewArray(array<unsigned char>^ byteArray, array<unsigned char>^ otherArray);

			/// <summary>
			/// ��׼�����㷨
			/// </summary>
			/// <param name="assemblyName">��Ҫ��֤�ĳ���Ψһ��ʶ��</param>
			/// <param name="source">��Ҫ���ܵ� Byte ���顣</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="assemblyName">��Ҫ��֤�ĳ���Ψһ��ʶ��</param>
			/// <param name="source">��Ҫ���ܵ� Byte ���顣</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� Int32 ������ֵ��</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, int otherKey);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="assemblyName">��Ҫ��֤�ĳ���Ψһ��ʶ��</param>
			/// <param name="source">��Ҫ���ܵ� Byte ���顣</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� String ������ֵ��</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, String^ otherKey);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="assemblyName">��Ҫ��֤�ĳ���Ψһ��ʶ��</param>
			/// <param name="source">��Ҫ���ܵ� Byte ���顣</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� Byte ���顣</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, array<unsigned char>^ otherKey);

		public: //�����㷨
			/// <summary>
			/// ��׼�����㷨
			/// </summary>
			/// <param name="source">��Ҫ���ܵ� Byte ���顣</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(array<unsigned char>^ source);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="source">��Ҫ���ܵ� Byte ���顣</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� Int32 ������ֵ��</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(array<unsigned char>^ source, int otherKey);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="source">��Ҫ���ܵ� Byte ���顣</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� String ������ֵ��</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(array<unsigned char>^ source, String^ otherKey);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="source">��Ҫ���ܵ� Byte ���顣</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� Byte ���顣</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(array<unsigned char>^ source, array<unsigned char>^ otherKey);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="source">��Ҫ���ܵ��ַ�����</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� Int32 ������ֵ��</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(String^ source, int otherKey);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="source">��Ҫ���ܵ��ַ�����</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� String ������ֵ��</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(String^ source, String^ otherKey);

			/// <summary>
			/// �û���������㷨
			/// </summary>
			/// <param name="source">��Ҫ���ܵ��ַ�����</param>
			/// <param name="otherKey">��Ҫ�ϲ����ܵ� Byte ���顣</param>
			/// <returns>���ؼ��ܳɹ��� Byte ���顣</returns>
			static array<unsigned char>^ HashToBytes(String^ source, array<unsigned char>^ otherKey);
		};
	}
}
