using System;
using System.Security.Cryptography;
using System.Text;

namespace Basic.EntityLayer
{
	/// <summary>系统加密类。</summary>
	[System.Security.SecuritySafeCritical()]
	public static class SecurityAlgorithm
	{
		private static readonly byte[] assemblyKey;
		static SecurityAlgorithm() { assemblyKey = typeof(SecurityAlgorithm).Assembly.GetName().GetPublicKey(); }
		private static readonly SHA256 _sha256 = System.Security.Cryptography.SHA256.Create();
		private static readonly MD5 _md5 = MD5.Create();
		private static readonly SHA512 _sha512 = System.Security.Cryptography.SHA512.Create();

		#region Hash MD5算法
		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static string HashToString(string original, int key)
		{
			byte[] bytes = HashToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static string HashToString(string original, Guid key)
		{
			byte[] bytes = HashToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static string HashToString(byte[] original, int key)
		{
			byte[] bytes = HashToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static string HashToString(string original)
		{
			byte[] bytes = _md5.ComputeHash(Encoding.Unicode.GetBytes(original));
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">源加密字节流</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] HashToBytes(byte[] original, int key)
		{
			byte[] keyArray = BitConverter.GetBytes(key);
			int length = keyArray.Length;
			Array.Resize<byte>(ref keyArray, length + original.Length);
			Array.Copy(original, 0, keyArray, length, original.Length);
			length += original.Length;
			Array.Resize<byte>(ref keyArray, length + assemblyKey.Length);
			Array.Copy(assemblyKey, 0, keyArray, length, assemblyKey.Length);
			return _md5.ComputeHash(keyArray);
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">源加密字节流</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] HashToBytes(byte[] original, Guid key)
		{
			byte[] keyArray = key.ToByteArray();
			int length = keyArray.Length;
			Array.Resize<byte>(ref keyArray, length + original.Length);
			Array.Copy(original, 0, keyArray, length, original.Length);
			length += original.Length;
			Array.Resize<byte>(ref keyArray, length + assemblyKey.Length);
			Array.Copy(assemblyKey, 0, keyArray, length, assemblyKey.Length);
			return _md5.ComputeHash(keyArray);
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] HashToBytes(string original, int key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return HashToBytes(obj, key);
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] HashToBytes(string original, Guid key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return HashToBytes(obj, key);
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] HashToBytes(string original)
		{
			return _md5.ComputeHash(Encoding.Unicode.GetBytes(original));
		}
		#endregion

		#region SHA256ToStr 算法
		/// <summary>SHA256 密码加密算法，计算输入数据的 SHA256 哈希值</summary>
		/// <remarks>使用 SHA256 算法计算哈希值，并返回计算结果的 base64 单行编码</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密后的字符串，用 base64 单行编码格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToStr(string original, Guid key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return Convert.ToBase64String(SHA256ToBytes(obj, key), Base64FormattingOptions.None);
		}

		/// <summary>SHA256 密码加密算法，计算输入数据的 SHA256 哈希值</summary>
		/// <remarks>使用 SHA256 算法计算哈希值，并返回计算结果的 base64 单行编码</remarks>
		/// <param name="original">原字符串</param>
		/// <returns>加密后的字符串，用 base64 单行编码格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToStr(string original)
		{
			byte[] bytes = SHA256ToBytes(original);
			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}

		/// <summary>SHA256 密码加密算法，计算输入数据的 SHA256 哈希值</summary>
		/// <remarks>使用 SHA256 算法计算哈希值，并返回计算结果的 base64 单行编码</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密后的字符串，用 base64 单行编码格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToStr(string original, int key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return Convert.ToBase64String(SHA256ToBytes(obj, key), Base64FormattingOptions.None);
		}
		#endregion

		#region SHA256 算法
		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">源加密字节流</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		private static byte[] SHA256ToBytes(byte[] original, int key)
		{
			byte[] keyArray = BitConverter.GetBytes(key);
			int length = keyArray.Length;
			Array.Resize<byte>(ref keyArray, length + original.Length);
			Array.Copy(original, 0, keyArray, length, original.Length);
			length += original.Length;
			Array.Resize<byte>(ref keyArray, length + assemblyKey.Length);
			Array.Copy(assemblyKey, 0, keyArray, length, assemblyKey.Length);
			return _sha256.ComputeHash(keyArray);
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">源加密字节流</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		private static byte[] SHA256ToBytes(byte[] original, Guid key)
		{
			byte[] keyArray = key.ToByteArray();
			int length = keyArray.Length;
			Array.Resize<byte>(ref keyArray, length + original.Length);
			Array.Copy(original, 0, keyArray, length, original.Length);
			length += original.Length;
			Array.Resize<byte>(ref keyArray, length + assemblyKey.Length);
			Array.Copy(assemblyKey, 0, keyArray, length, assemblyKey.Length);
			return _sha256.ComputeHash(keyArray);
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA256ToBytes(string original)
		{
			return _sha256.ComputeHash(Encoding.Unicode.GetBytes(original));
		}
		#endregion

		#region SHA256 算法
		/// <summary>SHA256 密码加密算法，计算输入数据的 SHA256 哈希值</summary>
		/// <remarks>使用 SHA256 算法计算哈希值，并返回计算结果的十六进制字符串</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密后的字符串，用十六进制字符串格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToBits(string original, int key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			byte[] bytes = SHA256ToBytes(obj, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>SHA256 密码加密算法，计算输入数据的 SHA256 哈希值</summary>
		/// <remarks>使用 SHA256 算法计算哈希值，并返回计算结果的十六进制字符串</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密后的字符串，用十六进制字符串格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToBits(string original, Guid key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			byte[] bytes = SHA256ToBytes(obj, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>SHA256 密码加密算法，计算输入数据的 SHA256 哈希值</summary>
		/// <remarks>使用 SHA256 算法计算哈希值，并返回计算结果的十六进制字符串</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密后的字符串，用十六进制字符串格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToBits(byte[] original, int key)
		{
			byte[] bytes = SHA256ToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>SHA256 密码加密算法，计算输入数据的 SHA256 哈希值</summary>
		/// <remarks>使用 SHA256 算法计算哈希值，并返回计算结果的十六进制字符串</remarks>
		/// <param name="original">原字符串</param>
		/// <returns>加密后的字符串，用十六进制字符串格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToBits(string original)
		{
			byte[] bytes = _sha256.ComputeHash(Encoding.Unicode.GetBytes(original));
			return BitConverter.ToString(bytes).Replace("-", "");
		}
		#endregion

		#region SHA512ToBytes 算法
		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">源加密字节流</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		private static byte[] SHA512ToBytes(byte[] original, int key)
		{
			byte[] keyArray = BitConverter.GetBytes(key);
			int length = keyArray.Length;
			Array.Resize<byte>(ref keyArray, length + original.Length);
			Array.Copy(original, 0, keyArray, length, original.Length);
			length += original.Length;
			Array.Resize<byte>(ref keyArray, length + assemblyKey.Length);
			Array.Copy(assemblyKey, 0, keyArray, length, assemblyKey.Length);
			return _sha512.ComputeHash(keyArray);
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">源加密字节流</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		private static byte[] SHA512ToBytes(byte[] original, Guid key)
		{
			byte[] keyArray = key.ToByteArray();
			int length = keyArray.Length;
			Array.Resize<byte>(ref keyArray, length + original.Length);
			Array.Copy(original, 0, keyArray, length, original.Length);
			length += original.Length;
			Array.Resize<byte>(ref keyArray, length + assemblyKey.Length);
			Array.Copy(assemblyKey, 0, keyArray, length, assemblyKey.Length);
			return _sha512.ComputeHash(keyArray);
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">原字符串</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		private static byte[] SHA512ToBytes(string original)
		{
			return _sha512.ComputeHash(Encoding.Unicode.GetBytes(original));
		}
		#endregion

		#region SHA512ToStr 算法
		/// <summary>SHA512 密码加密算法，计算输入数据的 SHA512 哈希值</summary>
		/// <remarks>使用 SHA512 算法计算哈希值，并返回计算结果的 base64 单行编码</remarks>
		/// <param name="original">原字符串</param>
		/// <returns>加密后的字符串，用 base64 单行编码格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToStr(string original)
		{
			byte[] bytes = SHA512ToBytes(original);
			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}

		/// <summary>SHA512 密码加密算法，计算输入数据的 SHA512 哈希值</summary>
		/// <remarks>使用 SHA512 算法计算哈希值，并返回计算结果的 base64 单行编码</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密后的字符串，用 base64 单行编码格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToStr(string original, Guid key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return Convert.ToBase64String(SHA512ToBytes(obj, key), Base64FormattingOptions.None);
		}

		/// <summary>SHA512 密码加密算法，计算输入数据的 SHA512 哈希值</summary>
		/// <remarks>使用 SHA512 算法计算哈希值，并返回计算结果的 base64 单行编码</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">源加密关键字</param>
		/// <returns>加密后的字符串，用 base64 单行编码格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToStr(string original, int key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return Convert.ToBase64String(SHA512ToBytes(obj, key), Base64FormattingOptions.None);
		}
		#endregion

		#region SHA512 算法
		/// <summary>SHA512 密码加密算法，计算输入数据的 SHA512 哈希值</summary>
		/// <remarks>使用 SHA512 算法计算哈希值，并返回计算结果的十六进制字符串</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密后的字符串，用十六进制字符串格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToBits(string original, int key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			byte[] bytes = SHA512ToBytes(obj, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>SHA512 密码加密算法，计算输入数据的 SHA512 哈希值</summary>
		/// <remarks>使用 SHA512 算法计算哈希值，并返回计算结果的十六进制字符串</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密后的字符串，用十六进制字符串格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToBits(string original, Guid key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return BitConverter.ToString(SHA512ToBytes(obj, key)).Replace("-", "");
		}

		/// <summary>SHA512 密码加密算法，计算输入数据的 SHA512 哈希值</summary>
		/// <remarks>使用 SHA512 算法计算哈希值，并返回计算结果的十六进制字符串</remarks>
		/// <param name="original">原字符串</param>
		/// <param name="key">加密额外关键字</param>
		/// <returns>加密后的字符串，用十六进制字符串格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToBits(byte[] original, int key)
		{
			return BitConverter.ToString(SHA512ToBytes(original, key)).Replace("-", "");
		}

		/// <summary>SHA512 密码加密算法，计算输入数据的 SHA512 哈希值</summary>
		/// <remarks>使用 SHA512 算法计算哈希值，并返回计算结果的十六进制字符串</remarks>
		/// <param name="original">原字符串</param>
		/// <returns>加密后的字符串，用十六进制字符串格式返回</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToBits(string original)
		{
			byte[] bytes = _sha512.ComputeHash(Encoding.Unicode.GetBytes(original));
			return BitConverter.ToString(bytes).Replace("-", "");
		}
		#endregion

		#region 将Byte数组转换成16进制字符串
		/// <summary>
		/// 将Byte数组转换成16进制字符串 
		/// </summary>
		/// <param name="source">需要转换的 byte[] leixing </param>
		/// <returns>反悔转换成功的16进制字符串</returns>
		public static string ToHexString(byte[] source)
		{
			if (source.Length == 0) { return string.Empty; }
			StringBuilder strB = new StringBuilder(20);
			for (int index = 0; index < source.Length; index++)
			{
				strB.AppendFormat("{0:X2}", source[index]);
			}
			return strB.ToString();
		}

		/// <summary>
		/// 将16进制字符串转换成Byte数组
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static byte[] ToByteArray(string source)
		{
			if (string.IsNullOrEmpty(source)) { return new byte[0]; }
			if ((source.Length % 2) != 0) { source += " "; }
			byte[] returnBytes = new byte[source.Length / 2];
			for (int index = 0; index < source.Length; index++)
			{
				returnBytes[index] = Convert.ToByte(source.Substring(index * 2, 2), 0x10);
			}
			return returnBytes;
		}


		#endregion

		#region 对称加密算法
		///// <summary>
		///// 使用对称加密算法解密字符串(AES)
		///// </summary>
		///// <param name="pwdkey">解密密码</param>
		///// <param name="original">需要解密字符串</param>
		///// <returns>解密字符串</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricDecryption(byte[] pwdkey, string original)
		//{
		//    if (original == null || original == string.Empty)
		//        return string.Empty;

		//    using (AesCryptoServiceProvider provider1 = new AesCryptoServiceProvider())
		//    {
		//        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
		//        {
		//            provider1.Key = md5.ComputeHash(pwdkey);
		//        }
		//        provider1.Mode = CipherMode.ECB;
		//        provider1.GenerateIV();
		//        byte[] buffer2 = Convert.FromBase64String(original);
		//        byte[] result = provider1.CreateDecryptor().TransformFinalBlock(buffer2, 0, buffer2.Length);
		//        return Encoding.Unicode.GetString(result);
		//    }
		//}

		///// <summary>
		///// 使用对称加密算法解密字符串(AES)
		///// </summary>
		///// <param name="password">解密密码</param>
		///// <param name="original">需要解密字符串</param>
		///// <returns>解密字符串</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricDecryption(string password, string original)
		//{
		//    byte[] buffer1 = Encoding.Unicode.GetBytes(password);
		//    return SymmetricDecryption(buffer1, original);
		//}

		///// <summary>
		///// 使用对称加密算法解密字符串(DES3)
		///// </summary>
		///// <param name="original">待解密字符串</param>
		///// <returns>解密字符串</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricDecryption(string original)
		//{
		//    return SymmetricDecryption(assemblyKey, original);
		//}

		///// <summary>
		///// 使用对称加密算法加密字符串(AES)
		///// </summary>
		///// <param name="password">加密密码</param>
		///// <param name="original">需要加密字符串</param>
		///// <returns>加密字符串</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricEncryption(string password, string original)
		//{
		//    byte[] buffer1 = Encoding.Unicode.GetBytes(password);
		//    return SymmetricEncryption(buffer1, original);
		//}

		///// <summary>
		///// 使用对称加密算法加密字符串(AES)
		///// </summary>
		///// <param name="pwdkey">加密密码</param>
		///// <param name="original">需要加密字符串</param>
		///// <returns>加密字符串</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricEncryption(byte[] pwdkey, string original)
		//{
		//    if (original == null || original == string.Empty)
		//        return string.Empty;

		//    using (AesCryptoServiceProvider provider1 = new AesCryptoServiceProvider())
		//    {
		//        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
		//        {
		//            provider1.Key = md5.ComputeHash(pwdkey);
		//        }
		//        provider1.Mode = CipherMode.ECB;
		//        byte[] buffer2 = Encoding.Unicode.GetBytes(original);
		//        byte[] result = provider1.CreateEncryptor().TransformFinalBlock(buffer2, 0, buffer2.Length);
		//        return Convert.ToBase64String(result);
		//    }
		//}

		///// <summary>
		///// 使用对称加密算法加密字符串(DES3)
		///// </summary>
		///// <param name="original">待加密字符串</param>
		///// <returns>加密字符串</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricEncryption(string original)
		//{
		//    return SymmetricEncryption(assemblyKey, original);
		//}
		#endregion
	}
}
