using System;

namespace Basic.Caches
{
	/// <summary>缓存键信息</summary>
	public sealed class KeyInfo
	{
		/// <summary>
		/// 初始化 KeyInfo 累实例
		/// </summary>
		/// <param name="key"></param>
		public KeyInfo(string key) => KeyName = key;

		/// <summary>缓存键名称</summary>
		public string KeyName { get; private set; }

		/// <summary>缓存键类型</summary>
		public KeyTypes KeyType { get; set; }

		/// <summary>缓存键类型,文本</summary>
		public string KeyTypeText
		{
			get
			{
				switch (KeyType)
				{
					case KeyTypes.SortedSet: return "ZSET";
					default: return KeyType.ToString().ToUpper();
				}

			}
		}

		/// <summary>
		/// 缓存键对应的过期时间
		/// </summary>
		public DateTimeOffset Expiration { get; set; }

		/// <summary>
		/// 缓存键对应的过期时间
		/// </summary>
		public string TTL
		{
			get
			{
				if (Expiration == DateTimeOffset.MinValue) { return "No limit"; }
				else
				{
					TimeSpan span = Expiration - DateTimeOffset.Now;
					if (span.TotalMinutes > 0) { return string.Concat(Math.Ceiling(span.TotalMinutes), " min"); }
					else { return string.Concat(Math.Ceiling(span.TotalSeconds), " sec"); }
				}
			}
		}

		/// <summary>
		/// 缓存键存储数据的大小，字节
		/// </summary>
		public long Size { get; set; }

		/// <summary>缓存键存储数据的大小
		/// 显示文本，例如：1KB，1MB</summary>
		public string SizeText
		{
			get
			{
				if (Size >= 1073741824) { return string.Concat(Math.Floor(Size / 1073741824M), " GB"); }
				else if (Size >= 1048576) { return string.Concat(Math.Floor(Size / 1048576M), " MB"); }
				else if (Size >= 1024) { return string.Concat(Math.Floor(Size / 1024M), " KB"); }
				else if (Size >= 512) { string.Concat(Math.Floor(Size / 1024M), " KB"); }
				else if (Size == 0) { return "0 KB"; }
				return string.Concat(Size, " B");
			}
		}
	}

	/// <summary>
	/// 缓存键类型
	/// </summary>
	public enum KeyTypes
	{
		/// <summary>
		/// 客户端库无法识别该数据类型
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// 字符串是缓存中最基本的数据类型。
		/// 字符串是二进制安全的，这意味着一个字符串可以包含任何类型的数据，
		/// 例如JPEG图像或序列化的Ruby对象。字符串值的最大长度可达512兆字节。
		/// </summary>
		/// <remarks><seealso href="https://redis.io/commands#string"/></remarks>
		String,

		/// <summary>
		/// 列表只是按插入顺序排序的字符串列表。
		/// 可以向列表添加元素，将新元素推送到列表的头部（左侧）或尾部（右侧）
		/// </summary>
		/// <remarks><seealso href="https://redis.io/commands#list"/></remarks>
		List,

		/// <summary>
		/// 哈希（Hash）是字符串类型的字段（field）与字符串类型的值（value）之间的映射表，因此它是表示对象的理想数据类型
		/// （例如：一个用户对象可以包含多个字段，如姓名、姓氏、年龄等等）。
		/// </summary>
		Hash,

		/// <summary>
		/// 集合（Set）是字符串（String）类型的无序集合。可以在 O(1) 时间复杂度内完成成员的添加、删除
		/// 以及存在性检测（该常量时间复杂度不受集合中包含的元素数量影响）。
		/// Redis 集合具备一个实用特性：不允许存在重复成员。
		/// 对同一个元素进行多次添加，最终集合中只会保留该元素的一份副本。
		/// 从实际应用角度来说，这意味着添加成员时，无需先执行「存在性检测」再执行「添加」的操作流程。
		/// </summary>
		Set,

		/// <summary>
		/// 有序集合（Sorted Set）与集合（Set）类似，同样是字符串类型的不重复集合。
		/// 两者的区别在于，有序集合中的每个成员都会关联一个分数（score），该分数被用于
		/// 对有序集合进行排序，排序规则为按照分数从低到高排列。
		/// 需要注意的是，有序集合的成员具有唯一性，但分数可以重复。
		/// </summary>
		SortedSet,

		/// <summary>
		/// 流（Stream）是一种数据结构，其设计模仿了**仅追加日志（append only log）**的行为模式，同时还具备更丰富的高级特性，可用于对存储在流中的数据进行操作。
		/// 流中的每一条记录都包含一个唯一的消息 ID，以及一个存储该记录数据的键值对列表。
		/// </summary>
		Stream
	}

}
