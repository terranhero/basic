using System;
using System.Security.Cryptography;
using System.Text;

namespace Basic.Configuration
{
    /// <summary>
    /// 系统加密类。
    /// </summary>
    public static class ConfigurationAlgorithm
    {
        private static readonly byte[] assemblyKey;
        static ConfigurationAlgorithm() { assemblyKey = typeof(ConfigurationAlgorithm).Assembly.GetName().GetPublicKey(); }

        #region 16进制和字节互换
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

        #region 对称加密算法
        /// <summary>
        /// 使用对称加密算法解密字符串(AES)
        /// </summary>
        /// <param name="pwdkey">解密密码</param>
        /// <param name="original">需要解密字符串</param>
        /// <returns>解密字符串</returns>
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
        /// 使用对称加密算法解密字符串(AES)
        /// </summary>
        /// <param name="password">解密密码</param>
        /// <param name="original">需要解密字符串</param>
        /// <returns>解密字符串</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Decryption(string password, string original)
        {
            byte[] buffer1 = Encoding.Unicode.GetBytes(password);
            return Decryption(buffer1, original);
        }

        /// <summary>
        /// 使用对称加密算法解密字符串(DES3)
        /// </summary>
        /// <param name="original">待解密字符串</param>
        /// <returns>解密字符串</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Decryption(string original)
        {
            return Decryption(assemblyKey, original);
        }

        /// <summary>
        /// 使用对称加密算法加密字符串(AES)
        /// </summary>
        /// <param name="password">加密密码</param>
        /// <param name="original">需要加密字符串</param>
        /// <returns>加密字符串</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Encryption(string password, string original)
        {
            byte[] buffer1 = Encoding.Unicode.GetBytes(password);
            return Encryption(buffer1, original);
        }

        /// <summary>
        /// 使用对称加密算法加密字符串(AES)
        /// </summary>
        /// <param name="pwdkey">加密密码</param>
        /// <param name="original">需要加密字符串</param>
        /// <returns>加密字符串</returns>
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
        /// 使用对称加密算法加密字符串(DES3)
        /// </summary>
        /// <param name="original">待加密字符串</param>
        /// <returns>加密字符串</returns>
        [System.Security.SecuritySafeCritical()]
        public static string Encryption(string original)
        {
            return Encryption(assemblyKey, original);
        }
        #endregion
    }
}
