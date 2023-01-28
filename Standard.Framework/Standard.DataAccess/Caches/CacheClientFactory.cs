
namespace Basic.DataAccess
{
	/// <summary>
	/// 系统缓存工厂类
	/// </summary>
	public abstract class CacheClientFactory
	{
		private static CacheClientFactory _ClientFactory;
		/// <summary>
		/// 创建 ICacheClient 子类实例。
		/// </summary>
		/// <returns></returns>
		public static void SetClientFactory(CacheClientFactory factory)
		{
			_ClientFactory = factory;
		}

		/// <summary>
		/// 获取缓存 ICacheClient 接口的实例。
		/// </summary>
		/// <param name="name">数据连接名称</param>
		/// <returns>返回缓存 ICacheClient 接口的实例。</returns>
		internal static ICacheClient GetClient(string name)
		{
			if (_ClientFactory != null) { return _ClientFactory.CreateClient(name); }
			_ClientFactory = new MemoryClientFactory();
			return _ClientFactory.CreateClient(name);
		}

		/// <summary>
		/// 返回实现 DbConnection 类的提供程序的类的一个新实例。
		/// </summary>
		/// <param name="name">数据库连接信息</param>
		/// <returns>返回缓存 ICacheClient 接口的实例。</returns>
		public abstract ICacheClient CreateClient(string name);

	}
}
