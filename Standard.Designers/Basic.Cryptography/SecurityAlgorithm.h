// GoldSoft.Basic.Cryptography.h

#pragma once

using namespace System;
using namespace System::Reflection;
using namespace System::Collections::Generic;
namespace Basic {
	namespace Cryptography {
		/// <summary>
		/// MD5加密算法帮助类
		/// </summary>
		public ref class SecurityAlgorithm abstract sealed
		{
		private:
			/// <summary>
			/// 表示可安全调用的程序集列表
			/// </summary>
			static SortedSet<String^>^ SafeAssemblies = gcnew SortedSet<String^>(
				gcnew array<String^> {"Name", "SourceColumn", "ParameterType", "Direction", "Size", "Precision", "Scale", "Nullable"});

			/// <summary>
			/// 测试当前调用是否合法
			/// </summary>
			/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
			/// <returns>如果验证成功则返回 true，否则返回 false。</returns>
			static bool CheckAssemblies(AssemblyName^ assemblyName);

			/// <summary>
			/// 将Byte数组转换成16进制字符串
			/// </summary>
			/// <param name="source">需要转换的 Byte 数组 。</param>
			/// <returns>返回转换成功的加密16进制字符串。</returns>
			static String^ ToHexString(array<unsigned char>^ source);

			/// <summary>
			/// 将16进制字符串转换成Byte数组
			/// </summary>
			/// <param name="source">需要转换的字符串。</param>
			/// <returns>返回转换成功的Byte数组。</returns>
			static array<unsigned char>^ ToByteArray(String^ source);

			/// <summary>
			/// 创建新数组
			/// </summary>
			/// <param name="byteArray">需要验证的程序集唯一标识。</param>
			/// <param name="otherArray">需要加密的 Byte 数组。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ CreateNewArray(array<unsigned char>^ byteArray, array<unsigned char>^ otherArray);

			/// <summary>
			/// 标准加密算法
			/// </summary>
			/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
			/// <param name="source">需要加密的 Byte 数组。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
			/// <param name="source">需要加密的 Byte 数组。</param>
			/// <param name="otherKey">需要合并加密的 Int32 类型数值。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, int otherKey);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
			/// <param name="source">需要加密的 Byte 数组。</param>
			/// <param name="otherKey">需要合并加密的 String 类型数值。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, String^ otherKey);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
			/// <param name="source">需要加密的 Byte 数组。</param>
			/// <param name="otherKey">需要合并加密的 Byte 数组。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, array<unsigned char>^ otherKey);

		public: //加密算法
			/// <summary>
			/// 标准加密算法
			/// </summary>
			/// <param name="source">需要加密的 Byte 数组。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(array<unsigned char>^ source);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="source">需要加密的 Byte 数组。</param>
			/// <param name="otherKey">需要合并加密的 Int32 类型数值。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(array<unsigned char>^ source, int otherKey);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="source">需要加密的 Byte 数组。</param>
			/// <param name="otherKey">需要合并加密的 String 类型数值。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(array<unsigned char>^ source, String^ otherKey);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="source">需要加密的 Byte 数组。</param>
			/// <param name="otherKey">需要合并加密的 Byte 数组。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(array<unsigned char>^ source, array<unsigned char>^ otherKey);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="source">需要加密的字符串。</param>
			/// <param name="otherKey">需要合并加密的 Int32 类型数值。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(String^ source, int otherKey);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="source">需要加密的字符串。</param>
			/// <param name="otherKey">需要合并加密的 String 类型数值。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(String^ source, String^ otherKey);

			/// <summary>
			/// 用户密码加密算法
			/// </summary>
			/// <param name="source">需要加密的字符串。</param>
			/// <param name="otherKey">需要合并加密的 Byte 数组。</param>
			/// <returns>返回加密成功的 Byte 数组。</returns>
			static array<unsigned char>^ HashToBytes(String^ source, array<unsigned char>^ otherKey);
		};
	}
}
