using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic.EntityLayer;

namespace Basic.Caches
{
	/// <summary>
	/// 表示缓存接口
	/// </summary>
	public interface ICacheClient : IDisposable
	{
		#region 缓存异步方法 - 获取缓存键信息
		/// <summary>获取所有缓存的键</summary>
		/// <returns>如果存在则返回键列表，否则返回 null。</returns>
		IEnumerable<KeyInfo> GetKeyInfos();

		/// <summary>获取所有缓存的键</summary>
		/// <returns>如果存在则返回键列表，否则返回 null。</returns>
		IEnumerable<KeyInfo> GetKeyInfosAsync();

		/// <summary>获取所有缓存的键</summary>
		/// <returns>如果存在则返回键列表，否则返回 null。</returns>
		IEnumerable<string> GetKeys();
		#endregion

		#region 缓存同步方法 - 移除缓存键及其数据
		/// <summary>根据传入的键移除一条记录</summary>
		/// <param name="key">需要移除记录的键</param>
		/// <returns>移除成功则返回true，否则返回false。</returns>
		bool KeyDelete(string key);

		/// <summary>从缓存中移除指定键的缓存项</summary>
		/// <param name="keys">需要移除记录的键</param>
		/// <returns>移除成功则返回true，否则返回false。</returns>
		void KeyDelete(string[] keys);
		#endregion

		#region 缓存异步方法 - 移除缓存键及其数据
		/// <summary>根据传入的键移除一条记录</summary>
		/// <param name="key">需要移除记录的键</param>
		/// <returns>移除成功则返回true，否则返回false。</returns>
		Task<bool> KeyDeleteAsync(string key);

		/// <summary>从缓存中移除指定键的缓存项</summary>
		/// <param name="keys">需要移除记录的键</param>
		/// <returns>移除成功则返回true，否则返回false。</returns>
		Task KeyDeleteAsync(string[] keys);
		#endregion

		#region 缓存同步方法 - 判断缓存键是否存在
		/// <summary>判断键是否存在</summary>
		/// <param name="key">需要检查的缓存键.</param>
		/// <returns><see langword="true"/>如果键存在. <see langword="false"/> 如果键不存在.</returns>
		bool KeyExists(string key);
		#endregion

		#region 缓存异步方法 - 判断缓存键是否存在
		/// <summary>使用异步方法判断键是否存在</summary>
		/// <param name="key">需要检查的缓存键.</param>
		/// <returns><see langword="true"/>如果键存在. <see langword="false"/> 如果键不存在.</returns>
		Task<bool> KeyExistsAsync(string key);
		#endregion

		#region 缓存同步方法 - 设置缓存键过期
		/// <summary>设置键绝对过期策略 ，
		/// 指定 <paramref name="key"/> 键的过期的时间点，
		/// 超时后，密钥将自动删除。
		/// </summary>
		/// <remarks>
		/// 如果在超时到期之前更新了密钥，则超时将被删除，就像在密钥上调用PERSIST命令一样
		/// <para>对于Redis版本 2.1.3，现有的超时不能被覆盖。因此，如果键已经有相关的超时，它将不做任何事情并返回0。</para>
		/// <para>从Redis 2.1.3开始，您可以更新密钥的超时时间。也可以使用PERSIST命令删除超时。有关更多信息，请参阅密钥过期页面。</para>
		/// </remarks>
		/// <param name="key">需要设置绝对过期时间的缓存键.</param>
		/// <param name="expiry">绝对过期时间</param>
		/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
		bool KeyExpire(string key, DateTime expiry);

		/// <summary>设置键滑动过期策略</summary>
		/// <param name="key">需要设置绝对过期时间的缓存键.</param>
		/// <param name="expiry">绝对过期时间</param>
		/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
		bool KeyExpire(string key, TimeSpan expiry);
		#endregion

		#region 缓存异步方法 - 设置缓存键过期
		/// <summary>设置键滑动过期策略</summary>
		/// <param name="key">需要设置绝对过期时间的缓存键.</param>
		/// <param name="expiry">绝对过期时间</param>
		/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
		Task<bool> KeyExpireAsync(string key, TimeSpan expiry);

		/// <summary>设置键绝对过期策略</summary>
		/// <param name="key">需要设置绝对过期时间的缓存键.</param>
		/// <param name="expiry">绝对过期时间</param>
		/// <returns>缓存键过期时间设置成功返回 <see langword="true"/>. 如果缓存键不存在或无法设置过期时间返回<see langword="false"/></returns>
		Task<bool> KeyExpireAsync(string key, DateTime expiry);
		#endregion

		#region 缓存同步方法 - 获取或设置缓存数据
		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="value">该缓存项的数据。</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool Set<T>(string key, T value, DateTime expiresAt);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="value">该缓存项的数据。</param>
		/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool Set<T>(string key, T value, TimeSpan expiresIn);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		T Get<T>(string key);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
		/// <returns>与指定的键对应的一组缓存项。</returns>
		IDictionary<string, T> Get<T>(params string[] keys);
		#endregion

		#region 缓存异步方法 - 获取或设置缓存数据
		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="value">该缓存项的数据。</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> SetAsync<T>(string key, T value, DateTime expiresAt);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要获取的缓存项的唯一标识符。</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		Task<T> GetAsync<T>(string key);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="keys"> 要返回的缓存项的一组唯一标识符。</param>
		/// <returns>与指定的键对应的一组缓存项。</returns>
		Task<IDictionary<string, T>> GetAsync<T>(string[] keys);
		#endregion

		#region 缓存同步方法 - 列表操作，设置，插入
		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool List<T>(string key, IList<T> values);

		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool List<T>(string key, IList<T> values, DateTime expiresAt);

		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool List<T>(string key, IList<T> values, TimeSpan expiresIn);

		/// <summary>
		/// 返回存储在键处的列表的长度。如果键不存在，则将其解释为空列表，并返回0
		/// </summary>
		/// <param name="key">列表的键.</param>
		/// <returns>键对应的列表的长度.</returns>
		long ListLength<T>(string key);

		/// <summary>
		///  通过使用键、值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		IList<T> List<T>(string key);
		#endregion

		#region 缓存异步方法 - 列表操作，设置，插入
		/// <summary>
		/// 通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="item">该缓存项的数据列表。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> ListPushAsync<T>(string key, T item);

		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> ListAsync<T>(string key, IList<T> values);

		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> ListAsync<T>(string key, IList<T> values, DateTime expiresAt);

		/// <summary>
		///  通过使用键、列标值和逐出设置，将某个缓存项插入缓存中。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <param name="values">该缓存项的数据列表。</param>
		/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> ListAsync<T>(string key, IList<T> values, TimeSpan expiresIn);

		/// <summary>
		/// 返回存储在键处的列表的长度。如果键不存在，则将其解释为空列表，并返回0
		/// </summary>
		/// <param name="key">列表的键.</param>
		/// <returns>键对应的列表的长度.</returns>
		Task<long> ListLengthAsync<T>(string key);

		/// <summary>通过使用键</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key"> 要插入的缓存项的唯一标识符。</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		Task<IList<T>> ListAsync<T>(string key);
		#endregion

		#region 缓存同步方法 - 哈希表操作
		/// <summary>存储数据到哈希表</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <param name="value">哈希表值</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool HashSet<T>(string hashId, string key, T value);

		/// <summary>存储数据到哈希表</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <param name="value">哈希表值</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool HashSet<T>(string hashId, string key, T value, DateTime expiresAt);

		/// <summary>存储数据到哈希表</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <param name="value">哈希表值</param>
		/// <param name="expiresIn">一个TimeSpan 类型的值，该值指示键过期的相对时间。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool HashSet<T>(string hashId, string key, T value, TimeSpan expiresIn);

		/// <summary>移除哈希表中的某值</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>移除成功则返回true，否则返回false。</returns>
		bool HashDelete(string hashId, string key);

		/// <summary>确定哈希表中是否存在某个缓存项。</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		bool HashExists(string hashId, string key);

		/// <summary>从哈希表获取数据</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		T HashGet<T>(string hashId, string key);

		/// <summary>返回存储在key处的哈希中包含的字段数</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <returns>哈希中的字段数，当键不存在时为 0</returns>
		long HashLength<T>(string hashId);

		/// <summary>获取整个哈希表的数据</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		List<T> HashGetAll<T>(string hashId);
		#endregion

		#region 缓存异步方法 - 哈希表操作
		/// <summary>确定哈希表中是否存在某个缓存项。</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		Task<bool> HashDeleteAsync(string hashId, string key);

		/// <summary>确定哈希表中是否存在某个缓存项。</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		Task<bool> HashExistsAsync(string hashId, string key);

		/// <summary>从哈希表获取数据。</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <param name="value">哈希表值</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		Task<bool> HashSetAsync<T>(string hashId, string key, T value);

		/// <summary>存储数据到哈希表</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <param name="value">哈希表值</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> HashSetAsync<T>(string hashId, string key, T value, DateTime expiresAt);

		/// <summary>从哈希表获取数据。</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <param name="key">哈希表键</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		Task<T> HashGetAsync<T>(string hashId, string key);

		/// <summary>返回存储在key处的哈希中包含的字段数</summary>
		/// <param name="hashId">哈希表缓存键</param>
		/// <returns>哈希中的字段数，当键不存在时为 0</returns>
		Task<long> HashLengthAsync<T>(string hashId);

		/// <summary>获取整个哈希表的数据</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="hashId">哈希表缓存键</param>
		/// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
		Task<IList<T>> HashGetAllAsync<T>(string hashId);
		#endregion

		#region 缓存同步方法 - 集合和有序集合操作 
		/// <summary>
		/// 将指定的成员添加到存储在key处的集合中。
		/// 已是此集合成员的指定成员将被忽略。
		/// 如果键不存在，则在添加指定成员之前创建一个新的集合。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合的键</param>
		/// <param name="value">要添加到集合中的 <typeparamref name="T"/> 类型的对象实例</param>
		/// <returns>如果指定的成员不在集合中返回，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
		bool SetAdd<T>(string key, T value);

		/// <summary>
		/// 将指定的成员添加到存储在key处的集合中。
		/// 已是此集合成员的指定成员将被忽略。
		/// 如果键不存在，则在添加指定成员之前创建一个新的集合。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合的键</param>
		/// <param name="value">要添加到集合中的 <typeparamref name="T"/> 类型的对象实例</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>如果指定的成员不在集合中返回<see langword="true"/>，否则返回<see langword="false"/></returns>
		bool SetAdd<T>(string key, T value, DateTime expiresAt);

		/// <summary>
		///将指定的成员添加到存储在 <paramref name="key"/> 处的集合中。<br/>
		///已是此集合成员的指定成员将被忽略。<br/>
		///如果键不存在，则在添加指定成员之前创建一个新的集合。
		/// </summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="items">要添加到集合中的 <typeparamref name="T"/> 类型的可枚举对象实例</param>
		/// <returns>添加到集合中的元素数，不包括已存在于集合中的所有元素。</returns>
		long SetAdd<T>(string key, IEnumerable<T> items);

		/// <summary>存储数据到集合。</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="items">要添加到集合中的 <typeparamref name="T"/> 类型的可枚举对象实例</param>
		/// <param name="expiresAt"></param>
		/// <returns>添加到集合中的元素数，不包括已存在于集合中的所有元素。</returns>
		long SetAdd<T>(string key, IEnumerable<T> items, DateTime expiresAt);

		/// <summary>返回存储在key处的集合的集合基数（元素数）</summary>
		/// <param name="key">集合的键</param>
		/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
		long SetLength<T>(string key);

		///<summary>返回存储在键处的集合值的所有成员。</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合的键</param>
		///<returns>集合中的所有元素。</returns>
		ICollection<T> SetMembers<T>(string key);

		/// <summary>存储数据到有序集合。</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="value">哈希表值</param>
		/// <param name="score">与元素关联的分数，用于排序。</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		bool ZSetAdd<T>(string key, T value, double score);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="value">哈希表值</param>
		/// <param name="score">与元素关联的分数，用于排序。</param>
		/// <param name="expiresAt">指定键过期的时间点，如果键已经存在则此参数忽略。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool ZSetAdd<T>(string key, T value, double score, DateTime expiresAt);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="value">哈希表值</param>
		/// <param name="score">与元素关联的分数，用于排序。</param>
		/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		bool ZSetAdd<T>(string key, T value, double score, TimeSpan expiresIn);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="values">哈希表值</param>
		/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
		/// <returns>添加到集合中的元素数，不包括已存在于集合中的所有元素。</returns>
		long ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="values">哈希表值</param>
		/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
		/// <param name="expiresAt">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
		/// <returns>添加到集合中的元素数，不包括已存在于集合中的所有元素。</returns>
		long ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, DateTime expiresAt);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="values">哈希表值</param>
		/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
		/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
		/// <returns>添加到集合中的元素数，不包括已存在于集合中的所有元素。</returns>
		long ZSetAdd<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, TimeSpan expiresIn);

		/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
		/// <param name="key">集合的键</param>
		/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
		long ZSetLength<T>(string key);

		/// <summary>从有序集合中读取所有数据。</summary>
		/// <param name="key">哈希表键</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		ICollection<T> ZSetMembers<T>(string key);
		#endregion

		#region 缓存异步方法 - 集合和有序集合操作
		/// <summary>存储数据到集合。</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="value">哈希表值</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		Task<bool> SetAddAsync<T>(string key, T value);

		/// <summary>存储数据到集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="value">哈希表值</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> SetAddAsync<T>(string key, T value, DateTime expiresAt);

		/// <summary>存储数据到集合。</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="items">需要添加到集合的项目列表</param>
		/// <returns>返回添加成功的集合项数量。</returns>
		Task<long> SetAddAsync<T>(string key, IEnumerable<T> items);

		/// <summary>存储数据到集合。</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="items">需要添加到集合的项目列表</param>
		/// <param name="expiresAt">指定键过期的时间点。</param>
		/// <returns>返回添加成功的集合项数量。</returns>
		Task<long> SetAddAsync<T>(string key, IEnumerable<T> items, DateTime expiresAt);

		/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
		/// <param name="key">集合的键</param>
		/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
		Task<long> SetLengthAsync<T>(string key);

		/// <summary>获取集合中所有成员</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<ICollection<T>> SetMembersAsync<T>(string key);

		/// <summary>存储数据到有序集合。</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="value">哈希表值</param>
		/// <param name="score">与元素关联的分数，用于排序。</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		Task<bool> ZSetAddAsync<T>(string key, T value, double score);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="value">哈希表值</param>
		/// <param name="score">与元素关联的分数，用于排序。</param>
		/// <param name="expiresAt">指定键过期的时间点，如果键已经存在则此参数忽略。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> ZSetAddAsync<T>(string key, T value, double score, DateTime expiresAt);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="value">哈希表值</param>
		/// <param name="score">与元素关联的分数，用于排序。</param>
		/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<bool> ZSetAddAsync<T>(string key, T value, double score, TimeSpan expiresIn);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="values">哈希表值</param>
		/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<long> ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="values">哈希表值</param>
		/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
		/// <param name="expiresAt">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<long> ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, DateTime expiresAt);

		/// <summary>存储数据到有序集合</summary>
		/// <typeparam name="T">缓存值类型</typeparam>
		/// <param name="key">集合键名</param>
		/// <param name="values">哈希表值</param>
		/// <param name="scoreFunc">与元素关联的分数，用于排序。</param>
		/// <param name="expiresIn">指定键从现在开始过期的时间，如果键已经存在则此参数忽略。</param>
		/// <returns>创建成功则为true，否则为false。</returns>
		Task<long> ZSetAddAsync<T>(string key, IEnumerable<T> values, Func<T, double> scoreFunc, TimeSpan expiresIn);

		/// <summary>返回存储在key处的有序集合的集合基数（元素数）</summary>
		/// <param name="key">集合的键</param>
		/// <returns>集合的基数（元素数），如果键不存在，则为0。</returns>
		Task<long> ZSetLengthAsync<T>(string key);

		/// <summary>从有序集合中读取所有数据。</summary>
		/// <param name="key">哈希表键</param>
		/// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
		Task<ICollection<T>> ZSetMembersAsync<T>(string key);
		#endregion
	}
}
