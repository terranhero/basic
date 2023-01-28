// 这是主 DLL 文件。

#include "stdafx.h"
#include "HashAlgorithm.h"
#include "SecurityAlgorithm.h"

using namespace System;
using namespace System::Text;
using namespace System::Reflection;
using namespace System::Collections::Generic;
using namespace System::Security::Permissions;
using namespace System::Runtime::InteropServices;
/// <summary>
/// 将Byte数组转换成16进制字符串
/// </summary>
/// <param name="source">需要转换的 Byte 数组 。</param>
/// <returns>返回转换成功的加密16进制字符串。</returns>
String^ Basic::Cryptography::SecurityAlgorithm::ToHexString(array<unsigned char>^ source)
{
	if (source -> Length == 0)
		return String::Empty;
	StringBuilder^ strB = gcnew StringBuilder(20);
	for(int index = 0; index<source -> Length; index++)
	{
		strB->AppendFormat("{0:X2}",source[index]);
	}
	return strB->ToString();
}

/// <summary>
/// 将16进制字符串转换成Byte数组
/// </summary>
/// <param name="source">需要转换的字符串。</param>
/// <returns>返回转换成功的Byte数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::ToByteArray(String^ source)
{
	if (String::IsNullOrWhiteSpace(source))
	{
		return gcnew array<Byte>(0);
	}
	if ((source->Length % 2) != 0)
	{
		source = String::Concat(source, " ");
	}
	array<Byte>^ returnBytes = gcnew array<Byte>(source->Length / 2);
	for (int i = 0; (i < returnBytes->Length); i++)
	{
		returnBytes[i] = Convert::ToByte(source->Substring((i * 2), 2), 0x10);
	}
	return returnBytes;
}

/// <summary>
/// 测试当前调用是否合法
/// </summary>
/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
/// <returns>如果验证成功则返回 true，否则返回 false。</returns>
bool Basic::Cryptography::SecurityAlgorithm::CheckAssemblies(AssemblyName^ assemblyName)
{	
	/*if(assemblyName->Name=="GoldSoft.Basic.WinTest")
	throw gcnew Exception("测试");*/
	array<unsigned char>^ thisKey= {
		0x00,0x24,0x00,0x00,0x04,0x80,0x00,0x00,0x94,0x00,0x00,0x00,0x06,0x02,0x00,0x00,
		0x00,0x24,0x00,0x00,0x52,0x53,0x41,0x31,0x00,0x04,0x00,0x00,0x01,0x00,0x01,0x00,
		0x0F,0x49,0x02,0x9E,0x94,0xC7,0x4B,0x2A,0x8A,0x63,0x9D,0x50,0xFD,0x2F,0x36,0xC0,
		0xBA,0xA0,0x1B,0x5D,0xD3,0x43,0x63,0xDD,0x7A,0xB9,0xF7,0x4E,0xD0,0x20,0xA3,0xFD,
		0x60,0xD5,0x90,0xBA,0xF4,0x54,0x45,0x25,0x08,0x3C,0x68,0xF9,0x54,0x4D,0x1F,0xD9,
		0x34,0xA4,0xC5,0x50,0x81,0xE2,0xA5,0x57,0x58,0x9F,0x3E,0xEF,0xBE,0x74,0xDF,0x56,
		0xDF,0xAF,0x98,0xE8,0xFD,0xB2,0xEF,0x33,0x45,0xC5,0xE9,0xE1,0x1B,0x8C,0x16,0xCD,
		0x06,0x37,0x9C,0x3B,0xAC,0xEA,0x28,0x77,0x55,0x94,0x5E,0x3B,0x7F,0x97,0x07,0xD2,
		0x58,0x1A,0xEF,0x3E,0x74,0xF4,0x23,0x94,0x97,0x4A,0xD7,0x8A,0x99,0xB1,0x6B,0x1D,
		0xA9,0xFF,0xB2,0xFE,0xBA,0x85,0x3A,0x5D,0x4F,0xD1,0x67,0x75,0x79,0x6D,0x85,0x91
	};
	array<unsigned char>^ publicKey= assemblyName->GetPublicKey();
	/*if ( publicKey == null) return false;
	if (b1.Length != b2.Length) return false;
	for (int i = 0; i < b1.Length; i++)
	if (b1[i] != b2[i])
	return false;
	return true;*/
	//StrongNamePublicKeyBlob^ snpb = gcnew StrongNamePublicKeyBlob(publicKey);
	//StrongNameIdentityPermission^ snip = gcnew StrongNameIdentityPermission(snpb,"GoldSoft.*", gcnew Version("5.0.0.0"));
	//snip->Demand();
	return false;
}

/// <summary>
/// 创建新数组
/// </summary>
/// <param name="byteArray">需要验证的程序集唯一标识。</param>
/// <param name="otherArray">需要加密的 Byte 数组。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::CreateNewArray(array<unsigned char>^ byteArray, array<unsigned char>^ otherArray)
{
	int keySize = otherArray -> Length;
	int sourceSize = byteArray->Length;

	int newSize = HashAlgorithm_CalculateNewSize(sourceSize, keySize);

	array<unsigned char>^ sourceArray = gcnew array<unsigned char>(newSize);
	Array::Copy(byteArray, 0, sourceArray, 0, sourceSize);
	pin_ptr<unsigned char> sourcePointer = &sourceArray[0];
	if(keySize>0)
	{
		pin_ptr<unsigned char> keyPointer = &otherArray[0];
		HashAlgorithm_InitializePassword(sourcePointer, newSize, keyPointer, keySize);
	}
	else	
	{
		HashAlgorithm_InitializePassword(sourcePointer, newSize);
	}
	return  sourceArray;
}

/// <summary>
/// 标准加密算法
/// </summary>
/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
/// <param name="source">需要加密的 Byte 数组。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(AssemblyName^ assemblyName,array<unsigned char>^ source)
{
	array<unsigned char>^ resultArray = gcnew array<unsigned char>(16);
	if(CheckAssemblies(assemblyName)){
		int sourceLength = source->Length;
		pin_ptr<unsigned char> sourcePointer = &source[0];
		HashAlgorithm_Update(sourcePointer, sourceLength);

		pin_ptr<unsigned char> resultPointer = &resultArray[0];
		HashAlgorithm_Final(resultPointer);
	}
	return  resultArray;
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
/// <param name="source">需要加密的 Byte 数组。</param>
/// <param name="otherKey">需要合并加密的 Int32 类型数值。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, int otherKey)
{	
	array<unsigned char>^ keyArray = BitConverter::GetBytes(otherKey);
	array<unsigned char>^ sourceArray = CreateNewArray(source, keyArray);
	return  HashToBytes(assemblyName, sourceArray);
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
/// <param name="source">需要加密的 Byte 数组。</param>
/// <param name="otherKey">需要合并加密的 String 类型数值。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, String^ otherKey)
{
	array<unsigned char>^ keyArray = Encoding::Unicode->GetBytes(otherKey);
	array<unsigned char>^ sourceArray = CreateNewArray(source, keyArray);
	return  HashToBytes(assemblyName, sourceArray);
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="assemblyName">需要验证的程序集唯一标识。</param>
/// <param name="source">需要加密的 Byte 数组。</param>
/// <param name="otherKey">需要合并加密的 Byte 数组。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(AssemblyName^ assemblyName, array<unsigned char>^ source, array<unsigned char>^ otherKey)
{
	array<unsigned char>^ sourceArray = CreateNewArray(source, otherKey);
	return  HashToBytes(assemblyName, sourceArray);
}

/// <summary>
/// 标准加密算法
/// </summary>
/// <param name="source">需要加密的 Byte 数组。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(array<unsigned char>^ source)
{
	Assembly^ assembly=Assembly::GetCallingAssembly();
	return  HashToBytes(assembly->GetName(),source);
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="source">需要加密的 Byte 数组。</param>
/// <param name="otherKey">需要合并加密的 Int32 类型数值。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(array<unsigned char>^ source, int otherKey)
{
	Assembly^ assembly=Assembly::GetCallingAssembly();
	return  HashToBytes(assembly->GetName(), source, otherKey);
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="source">需要加密的 Byte 数组。</param>
/// <param name="otherKey">需要合并加密的 String 类型数值。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(array<unsigned char>^ source, String^ otherKey)
{
	Assembly^ assembly=Assembly::GetCallingAssembly();
	return  HashToBytes(assembly->GetName(), source, otherKey);
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="source">需要加密的 Byte 数组。</param>
/// <param name="otherKey">需要合并加密的 Byte 数组。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(array<unsigned char>^ source, array<unsigned char>^ otherKey)
{
	Assembly^ assembly=Assembly::GetCallingAssembly();
	return  HashToBytes(assembly->GetName(), source, otherKey);
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="source">需要加密的字符串。</param>
/// <param name="otherKey">需要合并加密的 Int32 类型数值。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(String^ source, int otherKey)
{
	array<unsigned char>^ byteArray=	Encoding::Unicode->GetBytes(source);
	Assembly^ assembly=Assembly::GetCallingAssembly();
	return  HashToBytes(assembly->GetName(), byteArray, otherKey);
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="source">需要加密的字符串。</param>
/// <param name="otherKey">需要合并加密的 String 类型数值。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(String^ source, String^ otherKey)
{
	array<unsigned char>^ byteArray=	Encoding::Unicode->GetBytes(source);
	Assembly^ assembly=Assembly::GetCallingAssembly();
	return  HashToBytes(assembly->GetName(), byteArray, otherKey);
}

/// <summary>
/// 用户密码加密算法
/// </summary>
/// <param name="source">需要加密的字符串。</param>
/// <param name="otherKey">需要合并加密的 Byte 数组。</param>
/// <returns>返回加密成功的 Byte 数组。</returns>
array<unsigned char>^ Basic::Cryptography::SecurityAlgorithm::HashToBytes(String^ source, array<unsigned char>^ otherKey)
{
	array<unsigned char>^ byteArray=	Encoding::Unicode->GetBytes(source);
	Assembly^ assembly=Assembly::GetCallingAssembly();
	return  HashToBytes(assembly->GetName(), byteArray, otherKey);
}
