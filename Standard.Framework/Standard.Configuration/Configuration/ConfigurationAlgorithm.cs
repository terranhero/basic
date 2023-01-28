using System;
using System.Security.Cryptography;
using System.Text;

namespace Basic.Configuration
{
    /// <summary>
    /// ϵͳ�����ࡣ
    /// </summary>
    public static class ConfigurationAlgorithm
    {
        private static readonly byte[] assemblyKey;
        static ConfigurationAlgorithm() { assemblyKey = typeof(ConfigurationAlgorithm).Assembly.GetName().GetPublicKey(); }

        #region 16���ƺ��ֽڻ���
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
			if (string.IsNullOrWhiteSpace(source)) { return new byte[0]; }
            if ((source.Length % 2) != 0) { source = source + " "; }
            byte[] returnBytes = new byte[source.Length / 2];
            for (int index = 0; index < source.Length; index++)
            {
                returnBytes[index] = Convert.ToByte(source.Substring(index * 2, 2), 0x10);
            }
            return returnBytes;
        }


        #endregion

        #region �ԳƼ����㷨
        /// <summary>
        /// ʹ�öԳƼ����㷨�����ַ���(AES)
        /// </summary>
        /// <param name="pwdkey">��������</param>
        /// <param name="original">��Ҫ�����ַ���</param>
        /// <returns>�����ַ���</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Decryption(byte[] pwdkey, string original)
        {
            if (original == null || original == string.Empty)
                return string.Empty;

            using (AesCryptoServiceProvider provider1 = new AesCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    provider1.Key = md5.ComputeHash(pwdkey);
                }
                provider1.Mode = CipherMode.ECB;
                provider1.GenerateIV();
                byte[] buffer2 = Convert.FromBase64String(original);
                byte[] result = provider1.CreateDecryptor().TransformFinalBlock(buffer2, 0, buffer2.Length);
                return Encoding.Unicode.GetString(result);
            }
        }

        /// <summary>
        /// ʹ�öԳƼ����㷨�����ַ���(AES)
        /// </summary>
        /// <param name="password">��������</param>
        /// <param name="original">��Ҫ�����ַ���</param>
        /// <returns>�����ַ���</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Decryption(string password, string original)
        {
            byte[] buffer1 = Encoding.Unicode.GetBytes(password);
            return Decryption(buffer1, original);
        }

        /// <summary>
        /// ʹ�öԳƼ����㷨�����ַ���(DES3)
        /// </summary>
        /// <param name="original">�������ַ���</param>
        /// <returns>�����ַ���</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Decryption(string original)
        {
            return Decryption(assemblyKey, original);
        }

        /// <summary>
        /// ʹ�öԳƼ����㷨�����ַ���(AES)
        /// </summary>
        /// <param name="password">��������</param>
        /// <param name="original">��Ҫ�����ַ���</param>
        /// <returns>�����ַ���</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Encryption(string password, string original)
        {
            byte[] buffer1 = Encoding.Unicode.GetBytes(password);
            return Encryption(buffer1, original);
        }

        /// <summary>
        /// ʹ�öԳƼ����㷨�����ַ���(AES)
        /// </summary>
        /// <param name="pwdkey">��������</param>
        /// <param name="original">��Ҫ�����ַ���</param>
        /// <returns>�����ַ���</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Encryption(byte[] pwdkey, string original)
        {
            if (original == null || original == string.Empty)
                return string.Empty;

            using (AesCryptoServiceProvider provider1 = new AesCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    provider1.Key = md5.ComputeHash(pwdkey);
                }
                provider1.Mode = CipherMode.ECB;
                byte[] buffer2 = Encoding.Unicode.GetBytes(original);
                byte[] result = provider1.CreateEncryptor().TransformFinalBlock(buffer2, 0, buffer2.Length);
                return Convert.ToBase64String(result);
            }
        }

        /// <summary>
        /// ʹ�öԳƼ����㷨�����ַ���(DES3)
        /// </summary>
        /// <param name="original">�������ַ���</param>
        /// <returns>�����ַ���</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Encryption(string original)
        {
            return Encryption(assemblyKey, original);
        }
        #endregion
    }
}
