

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
		/// 无类型
		/// </summary>
		Unknown,

		/// <summary>
		/// 
		/// </summary>
		String,

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
		Set,

		/// <summary>
		/// 
		/// </summary>
		SortedSet,

		/*
			/// <summary>
	/// Strings are the most basic kind of Redis value. Redis Strings are binary safe, this means that
	/// a Redis string can contain any kind of data, for instance a JPEG image or a serialized Ruby object.
	/// A String value can be at max 512 Megabytes in length.
	/// </summary>
	/// <remarks><seealso href="https://redis.io/commands#string"/></remarks>
	String,

	/// <summary>
	/// Redis Lists are simply lists of strings, sorted by insertion order.
	/// It is possible to add elements to a Redis List pushing new elements on the head (on the left) or
	/// on the tail (on the right) of the list.
	/// </summary>
	/// <remarks><seealso href="https://redis.io/commands#list"/></remarks>
	List,

	/// <summary>
	/// Redis Sets are an unordered collection of Strings. It is possible to add, remove, and test for
	/// existence of members in O(1) (constant time regardless of the number of elements contained inside the Set).
	/// Redis Sets have the desirable property of not allowing repeated members.
	/// Adding the same element multiple times will result in a set having a single copy of this element.
	/// Practically speaking this means that adding a member does not require a check if exists then add operation.
	/// </summary>
	/// <remarks><seealso href="https://redis.io/commands#set"/></remarks>
	Set,

	/// <summary>
	/// Redis Sorted Sets are, similarly to Redis Sets, non repeating collections of Strings.
	/// The difference is that every member of a Sorted Set is associated with score, that is used
	/// in order to take the sorted set ordered, from the smallest to the greatest score.
	/// While members are unique, scores may be repeated.
	/// </summary>
	/// <remarks><seealso href="https://redis.io/commands#sorted_set"/></remarks>
	SortedSet,

	/// <summary>
	/// Redis Hashes are maps between string fields and string values, so they are the perfect data type
	/// to represent objects (e.g. A User with a number of fields like name, surname, age, and so forth).
	/// </summary>
	/// <remarks><seealso href="https://redis.io/commands#hash"/></remarks>
	Hash,

	/// <summary>
	/// A Redis Stream is a data structure which models the behavior of an append only log but it has more
	/// advanced features for manipulating the data contained within the stream. Each entry in a
	/// stream contains a unique message ID and a list of name/value pairs containing the entry's data.
	/// </summary>
	/// <remarks><seealso href="https://redis.io/commands#stream"/></remarks>
	Stream,

	/// <summary>
	/// The data-type was not recognised by the client library.
	/// </summary>
	Unknown,
		 */
	}

}
