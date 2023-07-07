using System;
using System.Security.Cryptography;
using System.Text;

namespace Basic.EntityLayer
{
	/// <summary>ϵͳ�����ࡣ</summary>
	[System.Security.SecuritySafeCritical()]
	public static class SecurityAlgorithm
	{
		private static readonly byte[] assemblyKey;
		static SecurityAlgorithm() { assemblyKey = typeof(SecurityAlgorithm).Assembly.GetName().GetPublicKey(); }
		private static readonly SHA256 _sha256 = System.Security.Cryptography.SHA256.Create();
		private static readonly MD5 _md5 = MD5.Create();
		private static readonly SHA512 _sha512 = System.Security.Cryptography.SHA512.Create();

		#region Hash MD5�㷨
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string HashToString(string original, int key)
		{
			byte[] bytes = HashToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string HashToString(string original, Guid key)
		{
			byte[] bytes = HashToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string HashToString(byte[] original, int key)
		{
			byte[] bytes = HashToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string HashToString(string original)
		{
			byte[] bytes = _md5.ComputeHash(Encoding.Unicode.GetBytes(original));
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">Դ�����ֽ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
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

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">Դ�����ֽ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
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

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] HashToBytes(string original, int key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return HashToBytes(obj, key);
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] HashToBytes(string original, Guid key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return HashToBytes(obj, key);
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] HashToBytes(string original)
		{
			return _md5.ComputeHash(Encoding.Unicode.GetBytes(original));
		}
		#endregion

		#region SHA256ToBase64 �㷨
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToBase64(string original, Guid key)
		{
			byte[] bytes = SHA256ToBytes(original, key);
			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToBase64(string original)
		{
			byte[] bytes = SHA256ToBytes(original);
			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToBase64(string original, int key)
		{
			byte[] bytes = SHA256ToBytes(original, key);
			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}
		#endregion

		#region SHA256 �㷨
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToString(string original, int key)
		{
			byte[] bytes = SHA256ToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToString(string original, Guid key)
		{
			byte[] bytes = SHA256ToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToString(byte[] original, int key)
		{
			byte[] bytes = SHA256ToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA256ToString(string original)
		{
			byte[] bytes = _sha256.ComputeHash(Encoding.Unicode.GetBytes(original));
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">Դ�����ֽ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA256ToBytes(byte[] original, int key)
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

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">Դ�����ֽ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA256ToBytes(byte[] original, Guid key)
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

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA256ToBytes(string original, int key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return SHA256ToBytes(obj, key);
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA256ToBytes(string original, Guid key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return SHA256ToBytes(obj, key);
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA256ToBytes(string original)
		{
			return _sha256.ComputeHash(Encoding.Unicode.GetBytes(original));
		}
		#endregion

		#region SHA512 �㷨
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToBase64(string original, Guid key)
		{
			byte[] bytes = SHA512ToBytes(original, key);
			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToBase64(string original)
		{
			byte[] bytes = SHA512ToBytes(original);
			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToBase64(string original, int key)
		{
			byte[] bytes = SHA512ToBytes(original, key);
			return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		}
		#endregion

		#region SHA512 �㷨
		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToString(string original, int key)
		{
			byte[] bytes = SHA512ToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToString(string original, Guid key)
		{
			byte[] bytes = SHA512ToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">���ܶ���ؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToString(byte[] original, int key)
		{
			byte[] bytes = SHA512ToBytes(original, key);
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static string SHA512ToString(string original)
		{
			byte[] bytes = _sha512.ComputeHash(Encoding.Unicode.GetBytes(original));
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">Դ�����ֽ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA512ToBytes(byte[] original, int key)
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

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">Դ�����ֽ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA512ToBytes(byte[] original, Guid key)
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

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA512ToBytes(string original, int key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return SHA512ToBytes(obj, key);
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <param name="key">Դ���ܹؼ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA512ToBytes(string original, Guid key)
		{
			byte[] obj = Encoding.Unicode.GetBytes(original);
			return SHA512ToBytes(obj, key);
		}

		/// <summary>ʹ�ò��ԳƼ����㷨�����ַ���</summary>
		/// <param name="original">ԭ�ַ���</param>
		/// <returns>�����ַ���</returns>
		[System.Security.SecuritySafeCritical()]
		public static byte[] SHA512ToBytes(string original)
		{
			return _sha512.ComputeHash(Encoding.Unicode.GetBytes(original));
		}
		#endregion

		#region ��Byte����ת����16�����ַ���
		/// <summary>
		/// ��Byte����ת����16�����ַ��� 
		/// </summary>
		/// <param name="source">��Ҫת���� byte[] leixing </param>
		/// <returns>����ת���ɹ���16�����ַ���</returns>
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
		/// ��16�����ַ���ת����Byte����
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

		#region �ԳƼ����㷨
		///// <summary>
		///// ʹ�öԳƼ����㷨�����ַ���(AES)
		///// </summary>
		///// <param name="pwdkey">��������</param>
		///// <param name="original">��Ҫ�����ַ���</param>
		///// <returns>�����ַ���</returns>
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
		///// ʹ�öԳƼ����㷨�����ַ���(AES)
		///// </summary>
		///// <param name="password">��������</param>
		///// <param name="original">��Ҫ�����ַ���</param>
		///// <returns>�����ַ���</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricDecryption(string password, string original)
		//{
		//    byte[] buffer1 = Encoding.Unicode.GetBytes(password);
		//    return SymmetricDecryption(buffer1, original);
		//}

		///// <summary>
		///// ʹ�öԳƼ����㷨�����ַ���(DES3)
		///// </summary>
		///// <param name="original">�������ַ���</param>
		///// <returns>�����ַ���</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricDecryption(string original)
		//{
		//    return SymmetricDecryption(assemblyKey, original);
		//}

		///// <summary>
		///// ʹ�öԳƼ����㷨�����ַ���(AES)
		///// </summary>
		///// <param name="password">��������</param>
		///// <param name="original">��Ҫ�����ַ���</param>
		///// <returns>�����ַ���</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricEncryption(string password, string original)
		//{
		//    byte[] buffer1 = Encoding.Unicode.GetBytes(password);
		//    return SymmetricEncryption(buffer1, original);
		//}

		///// <summary>
		///// ʹ�öԳƼ����㷨�����ַ���(AES)
		///// </summary>
		///// <param name="pwdkey">��������</param>
		///// <param name="original">��Ҫ�����ַ���</param>
		///// <returns>�����ַ���</returns>
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
		///// ʹ�öԳƼ����㷨�����ַ���(DES3)
		///// </summary>
		///// <param name="original">�������ַ���</param>
		///// <returns>�����ַ���</returns>
		//[System.Security.SecuritySafeCritical()]
		//public static string SymmetricEncryption(string original)
		//{
		//    return SymmetricEncryption(assemblyKey, original);
		//}
		#endregion
	}
}
