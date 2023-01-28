using Basic.Configuration;
using Basic.DataEntities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Collections
{
	/// <summary>
	/// 表示 AbstractEntityElement 类实例的集合
	/// </summary>
	public sealed class AbstractEntityColllection : Basic.Collections.BaseCollection<AbstractEntityElement>
	{
		private readonly PersistentConfiguration persistentConfiguration;

		/// <summary>
		/// 初始化 AbstractCustomTypeDescriptor 类实例。
		/// </summary>
		internal AbstractEntityColllection(PersistentConfiguration persistent) { persistentConfiguration = persistent; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected internal override string GetKey(AbstractEntityElement item) { return item.ClassName; }
	}
}
