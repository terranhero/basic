namespace Basic.LogInfo
{
	/// <summary>
	/// 表示系统可用请求集合
	/// </summary>
	public sealed class ActionCollection : Basic.Collections.BaseDictionary<ActionInfo>
	{
		/// <summary>
		/// 初始化 ActionCollection 类集合
		/// </summary>
		internal ActionCollection() { }

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		/// <param name="item">要添加的对象。</param>
		public override void Add(ActionInfo item)
		{
			base.AddItem(item.Url.ToLower(), item);
		}

		/// <summary>
		/// 从当前字典值集合中移除特定对象的第一个匹配项。
		/// </summary>
		/// <param name="item">要从当前字典值集合 中移除的对象。</param>
		/// <exception cref="System.NotSupportedException">当前字典集合是只读的。</exception>
		/// <returns>如果已从当前字典值集合中成功移除 item，则为 true；否则为 false。
		/// 如果在原始当前字典值集合中没有找到 item，该方法也会返回 false。</returns>
		public override bool Remove(ActionInfo item)
		{
			return base.Remove(item.Url.ToLower());
		}

		/// <summary>
		/// 添加项到集合中。
		/// </summary>
		/// <param name="items">要添加的对象数组。</param>
		public void AddRange(ActionInfo[] items)
		{
			if (items != null && items.Length > 0)
			{
				foreach (ActionInfo item in items) { base.AddItem(item.Url.ToLower(), item); }
			}
		}
	}
}
