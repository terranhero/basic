using System.Collections.Generic;

namespace Basic.Messages
{
	/// <summary>资源转换器接口集合</summary>
	internal sealed class MessageConverterCollection : IMessageRegister, IEnumerable<IMessageConverter>
	{
		private readonly SortedList<string, IMessageConverter> items;
		/// <summary>
		/// 初始化一个空的具有指定的初始容量的 MessageConverterCollection 类的新实例
		///，并使用默认的键比较器。
		/// </summary>
		/// <param name="capacity">集合初始容量</param>
		internal MessageConverterCollection(int capacity)
		{
			items = new SortedList<string, IMessageConverter>(capacity);
		}

		/// <summary>
		/// 初始化一个空的 MessageConverterCollection 类的新实例
		/// </summary>
		internal MessageConverterCollection()
		{
			items = new SortedList<string, IMessageConverter>();
		}

		/// <summary>获取或设置与指定的键相关联的值。</summary>
		/// <param name="key"> 要获取或设置其值的键。</param>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">已检索该属性，并且集合中不存在 key。</exception>
		/// <returns>与指定的键相关联的值。如果找不到指定的键，则 get 操作会引发 System.Collections.Generic.KeyNotFoundException，而set 操作会创建一个使用指定键的新元素。</returns>
		public IMessageConverter this[string key]
		{
			get { return items[key]; }
			set { items[key] = value; }
		}


		/// <summary>注册资源转换器。</summary>
		/// <param name="messageConverter">文本消息转换器，该转换器为实现了接口 IMessageConverter 的类实例。</param>
		public IMessageConverter Register(IMessageConverter messageConverter) { return Register(messageConverter, false); }

		/// <summary>注册资源转换器。</summary>
		/// <param name="messageConverter">文本消息转换器，该转换器为实现了接口 IMessageConverter 的类实例。</param>
		/// <param name="defaultConverter">当前消息转换器是否为默认消息转换器。</param>
		public IMessageConverter Register(IMessageConverter messageConverter, bool defaultConverter)
		{
			if (messageConverter.Name != null && !items.ContainsKey(messageConverter.Name))
				items.Add(messageConverter.Name, messageConverter);
			return defaultConverter ? messageConverter : null;
			//if (defaultConverter) { defaultMesage = messageConverter; }
		}

		///// <summary>添加项到集合中。</summary>
		///// <param name="item">要添加的对象。</param>
		//internal void Add(IMessageConverter item) { items.Add(item.Name, item); }

		///// <summary>添加项到集合中。</summary>
		///// <param name="name">转换器名称</param>
		///// <param name="item">要添加的对象。</param>
		//internal void Add(string name, IMessageConverter item) { items.Add(name, item); }

		/// <summary>
		/// 确定 Basic.Collections.BaseCollection&lt;string,T&gt; 是否包含具有指定键的元素。
		/// </summary>
		/// <param name="key">要在 Basic.Collections.BaseCollection&lt;string,T&gt; 中定位的键。</param>
		/// <returns>如果 Basic.Collections.BaseCollection&lt;string,T&gt; 包含具有指定键的元素，则为 true；否则为 false。</returns>
		/// <exception cref="System.ArgumentNullException">key 为 null。</exception>
		internal bool ContainsKey(string key)
		{
			return items.ContainsKey(key);
		}

		/// <summary>返回循环访问 当前字典值集合 的枚举数。</summary>
		/// <returns>当前字典集合中 ValueCollection 的 ValueCollection.Enumerator 结构</returns>
		IEnumerator<IMessageConverter> IEnumerable<IMessageConverter>.GetEnumerator()
		{
			return items.Values.GetEnumerator();
		}

		/// <summary>返回循环访问 当前字典值集合 的枚举数。</summary>
		/// <returns>当前字典集合中 ValueCollection 的 ValueCollection.Enumerator 结构</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return items.Values.GetEnumerator();
		}
	}
}
