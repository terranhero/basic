

using System;

namespace Basic.Caches
{

	/// <summary>缓存键信息</summary>
	public sealed class KeyInfo
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public KeyInfo(string key)
		{
			KeyName = key;
		}

		/// <summary>
		/// 缓存键名称
		/// </summary>
		public string KeyName { get; private set; }

		/// <summary>
		/// 缓存键类型
		/// </summary>
		public KeyTypes KeyType { get; set; }

		/// <summary>
		/// 缓存键对应的过期时间
		/// </summary>
		public DateTimeOffset Expiration { get; set; }

		/// <summary>
		/// 缓存键存储数据的大小，字节
		/// </summary>
		public int Size { get; set; }
	}

	/// <summary>
	/// 缓存键类型
	/// </summary>
	public enum KeyTypes
	{
		/// <summary>
		/// 无类型
		/// </summary>
		None,
		/// <summary>
		/// 
		/// </summary>
		List,
		/// <summary>
		/// 
		/// </summary>
		Hash,
		/// <summary>
		/// 
		/// </summary>
		Set
	}

}
