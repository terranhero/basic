using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Basic.Cryptography
{
	/// <summary>表示计算机标识，一般为有效网卡的MAC地址</summary>
	public struct MachineCode
	{
		#region 静态方法
		private static readonly byte[] keys = typeof(MachineCode).Assembly.GetName().GetPublicKey();
		private static readonly SHA256 _sha256 = SHA256.Create();

		/// <summary>获取本机唯一标识，一般为有效网卡的MAC地址</summary>
		/// <returns></returns>
		public static MachineCode New()
		{
			List<byte> bytes = new List<byte>();
			foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (nic.OperationalStatus == OperationalStatus.Up)
				{
					bytes.AddRange(nic.GetPhysicalAddress().GetAddressBytes());
				}
			}
			return new MachineCode(Hash(bytes.ToArray()));
		}

		/// <summary>使用不对称加密算法加密字符串</summary>
		/// <param name="original">源加密字节流</param>
		/// <returns>加密字符串</returns>
		[System.Security.SecuritySafeCritical()]
		internal static byte[] Hash(byte[] original)
		{
			int length = original.Length;
			Array.Resize(ref original, original.Length + keys.Length);
			Array.Copy(keys, 0, original, length, keys.Length);
			return _sha256.ComputeHash(original);
		}
		#endregion

		private readonly byte[] _Buffer;
		private MachineCode(byte[] bytes)
		{
			if (bytes.Length != 32) { throw new System.ArgumentException(nameof(bytes), "参数不符合系统要求"); }
			_Buffer = bytes;
		}

		/// <summary>返回byte[] </summary>
		/// <returns></returns>
		public byte[] ToByteArray() { return _Buffer; }

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder(28);
			foreach (byte bt in _Buffer)
			{
				builder.AppendFormat("{0:X2}", bt);
				if (builder.Length == 8) { builder.Append("-"); }
				else if (builder.Length == 17) { builder.Append("-"); }
				else if (builder.Length == 26) { builder.Append("-"); }
				else if (builder.Length == 35) { builder.Append("-"); }
				else if (builder.Length == 44) { builder.Append("-"); }
				else if (builder.Length == 53) { builder.Append("-"); }
				else if (builder.Length == 62) { builder.Append("-"); }
			}
			return builder.ToString();
		}
	}
}
