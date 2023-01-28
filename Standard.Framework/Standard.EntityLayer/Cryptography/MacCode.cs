using System;
using System.Net.NetworkInformation;

namespace Basic.Cryptography
{
	/// <summary>表示计算机标识，一般为有效网卡的MAC地址</summary>
	public struct MacCode
	{
		private readonly byte[] _Buffer;
		private MacCode(byte[] bytes) { _Buffer = bytes; }

		/// <summary>获取本机唯一标识，一般为有效网卡的MAC地址</summary>
		/// <returns></returns>
		public static MacCode New()
		{
			MacCode ma = new MacCode(new byte[6] { 0, 0, 0, 0, 0, 0 });
			foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (nic.OperationalStatus == OperationalStatus.Up)
				{
					ma.Add(nic.GetPhysicalAddress().GetAddressBytes());
				}
			}
			return ma;
		}

		private void Add(byte[] bytes)
		{
			if (bytes.Length < 6) { return; }
			for (int index = 0; index < _Buffer.Length; index++)
			{
				_Buffer[index] = (byte)(_Buffer[index] + bytes[index]);
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		public byte[] ToByteArray() { return _Buffer; }

		/// <summary></summary>
		/// <returns></returns>
		public override string ToString()
		{
			return BitConverter.ToString(_Buffer).Replace('-', ':');
		}
	}
}
