using System.Collections.Generic;
using System.Text;

namespace Basic.Cryptography
{
	/// <summary>产品编码</summary>
	public struct ProductCode
	{
		private readonly byte[] _Buffer;
		private ProductCode(byte[] bytes)
		{
			if (bytes.Length != 32) { throw new System.ArgumentException(nameof(bytes), "参数不符合系统要求"); }
			_Buffer = bytes;
		}

		/// <summary>获取本机唯一标识，一般为有效网卡的MAC地址</summary>
		/// <returns>返回新的产品编码</returns>
		public static ProductCode New(byte[] code, byte[] key)
		{
			List<byte> bytes = new List<byte>(code);
			bytes.AddRange(key);
			return new ProductCode(MachineCode.Hash(bytes.ToArray()));
		}

		/// <summary></summary>
		/// <returns></returns>
		public byte[] ToByteArray() { return _Buffer; }

		//private const string emptyCode = "000000-000000-000000-00000000";
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
