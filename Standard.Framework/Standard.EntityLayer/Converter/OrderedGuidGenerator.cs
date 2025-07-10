using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Basic.EntityLayer
{
	/// <summary>有序 Guid 生成器</summary>
	public static class OrderedGuidGenerator
	{
		private static readonly byte[] keys = new byte[] { 0x84, 0x73, 0xE5, 0x59, 0x51, 0x6F, 0x11, 0xF0, 0x99, 0x25, 0x8A, 0x78, 0xAD, 0x61, 0x4D, 0xC9 };
		private static readonly long ticksPerDay = TimeSpan.TicksPerDay;

		private static int Fnv1a32Hash(string input)
		{
			const uint fnv32Offset = 2166136261;
			const uint fnv32Prime = 16777619;

			uint hash = fnv32Offset;
			byte[] bytes = Encoding.UTF8.GetBytes(input);

			foreach (byte b in bytes)
			{
				hash ^= b;
				hash *= fnv32Prime;
			}

			return (int)hash;
		}

		/// <summary></summary>
		/// <param name="input"></param>
		/// <returns></returns>
		private static long Fnv1a64Hash(string input)
		{
			const ulong FnvOffsetBasis = 14695981039346656037UL;
			const ulong FnvPrime = 1099511628211UL;

			ulong hash = FnvOffsetBasis;
			byte[] bytes = Encoding.UTF8.GetBytes(input);

			foreach (byte b in bytes)
			{
				hash ^= b;
				hash *= FnvPrime;
			}

			return (long)hash;
		}

		/// <summary>
		/// 使用指定的整数和前四个字节, 再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highString">GUID 的前 4 个字节</param>
		public static Guid NewGuid(string highString)
		{
			return NewGuid(Fnv1a64Hash(highString), DateTimeOffset.UtcNow.UtcTicks);
		}

		/// <summary>
		/// 使用指定的整数和前四个字节,再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highString">GUID 的前 4 个字节</param>
		/// <param name="lows">GUID 的后 8 个字节</param>
		/// <returns>返回生成新的GUID值</returns>
		public static Guid NewGuid(string highString, long lows)
		{
			return NewGuid(Fnv1a64Hash(highString), lows);
		}

		/// <summary>
		/// 使用指定的字符串和日期部分, 再使用时间戳字节数组, 初始化 Guid 类的新实例<br/>
		/// 字符串一般使用数据库表名称，这样能够保证每次单表保存时大概率产生唯一值<br/>
		/// 日期部分则表示表中日期字段方便索引查询
		/// </summary>
		/// <param name="highString">GUID 的前 4 个字节</param>
		/// <param name="date">GUID 中5,6,7,8四个直接的值，仅适用日期部分，时间部分舍弃</param>
		/// <returns>返回生成新的GUID值</returns>
		public static Guid NewGuid(string highString, DateTime date)
		{
			return NewGuid(highString, date, DateTimeOffset.UtcNow.UtcTicks);
		}

		/// <summary>
		/// 使用指定的字符串和日期部分, 再使用时间戳字节数组, 初始化 Guid 类的新实例<br/>
		/// 字符串一般使用数据库表名称，这样能够保证每次单表保存时大概率产生唯一值<br/>
		/// 日期部分则表示表中日期字段方便索引查询
		/// </summary>
		/// <param name="highString">GUID 的前 4 个字节</param>
		/// <param name="date">GUID 中5,6,7,8四个直接的值，仅适用日期部分，时间部分舍弃</param>
		/// <param name="lows">GUID 的后 8 个字节</param>
		/// <returns>返回生成新的GUID值</returns>
		public static Guid NewGuid(string highString, DateTime date, long lows)
		{
			int days = (int)(date.Ticks / ticksPerDay);
			return NewGuid(Fnv1a32Hash(highString), (short)(days & 0xFFFF), (short)((days >> 16) & 0xFFFF), lows);
		}

		/// <summary>
		/// 使用指定的整数和前四个字节,再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highA">GUID 的前 4 个字节</param>
		/// <param name="highBC">GUID 的下 4 个字节</param>
		/// <param name="lows">GUID 的后 8 个字节</param>
		/// <returns>返回生成新的GUID值</returns>
		public static Guid NewGuid(int highA, int highBC, long lows)
		{
			byte[] bytes = BitConverter.GetBytes(lows); Array.Reverse(bytes);
			return new Guid(highA, (short)(highBC & 0xFFFF), (short)((highBC >> 16) & 0xFFFF), bytes);
		}

		/// <summary>
		/// 使用指定的整数和前四个字节,再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highA">GUID 的前 4 个字节</param>
		/// <param name="highB">GUID 的下两个字节</param>
		/// <param name="highC">GUID 的下两个字节</param>
		/// <param name="lows">GUID 的后 8 个字节</param>
		/// <returns>返回生成新的GUID值</returns>
		public static Guid NewGuid(int highA, short highB, short highC, long lows)
		{
			byte[] bytes = BitConverter.GetBytes(lows);
			Array.Reverse(bytes); return new Guid(highA, highB, highC, bytes);
		}

		/// <summary>
		/// 使用指定的整数和前四个字节,再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highA">GUID 的前 4 个字节</param>
		/// <param name="highB">GUID 的下两个字节</param>
		/// <param name="highC">GUID 的下两个字节</param>
		/// <returns>返回生成新的GUID值</returns>
		public static Guid NewGuid(int highA, short highB, short highC)
		{
			byte[] bytes = BitConverter.GetBytes(DateTimeOffset.UtcNow.UtcTicks);
			Array.Reverse(bytes); return new Guid(highA, highB, highC, bytes);
		}

		/// <summary>
		/// 使用指定的整数和前四个字节,再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highs">GUID 的前 8 个字节</param>
		/// <param name="lows">GUID 的后 8 个字节</param>
		public static Guid NewGuid(long highs, long lows)
		{
			byte[] guid = BitConverter.GetBytes(highs);
			Array.Reverse(guid); Array.Resize(ref guid, 16);
			byte[] bytes = BitConverter.GetBytes(lows);
			//guid[8] = bytes[0]; guid[9] = bytes[1]; guid[10] = bytes[2]; guid[11] = bytes[3];
			//guid[12] = bytes[4]; guid[13] = bytes[5]; guid[14] = bytes[6]; guid[15] = bytes[7];

			guid[8] = bytes[7]; guid[9] = bytes[6]; guid[10] = bytes[5]; guid[11] = bytes[4];
			guid[12] = bytes[3]; guid[13] = bytes[2]; guid[14] = bytes[1]; guid[15] = bytes[0];
			return new Guid(guid);
		}

		/// <summary>
		/// 使用指定的整数和前四个字节,再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highs">GUID 的前 8 个字节</param>
		public static Guid NewGuid(long highs)
		{
			return NewGuid(highs, DateTimeOffset.UtcNow.UtcTicks);
		}

		/// <summary>
		/// 使用指定的整数和前四个字节,再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highs">GUID 的前 8 个字节</param>
		/// <param name="lows">GUID 的后 8 个字节</param>
		public static Guid NewGuid(int highs, long lows)
		{
			byte[] guid = BitConverter.GetBytes(highs);
			Array.Reverse(guid); Array.Resize(ref guid, 16);
			byte[] bytes = BitConverter.GetBytes(lows);
			//guid[8] = bytes[0]; guid[9] = bytes[1]; guid[10] = bytes[2]; guid[11] = bytes[3];
			//guid[12] = bytes[4]; guid[13] = bytes[5]; guid[14] = bytes[6]; guid[15] = bytes[7];

			guid[8] = bytes[7]; guid[9] = bytes[6]; guid[10] = bytes[5]; guid[11] = bytes[4];
			guid[12] = bytes[3]; guid[13] = bytes[2]; guid[14] = bytes[1]; guid[15] = bytes[0];
			return new Guid(guid);
		}

		/// <summary>
		/// 使用指定的整数和前四个字节,再使用时间戳字节数组, 初始化 Guid 类的新实例
		/// </summary>
		/// <param name="highs">GUID 的前 8 个字节</param>
		public static Guid NewGuid(int highs)
		{
			return NewGuid(highs, DateTimeOffset.UtcNow.UtcTicks);
		}
	}
}
