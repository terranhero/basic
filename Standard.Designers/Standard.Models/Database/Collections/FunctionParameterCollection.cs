using System.Collections.Specialized;
using Basic.Database;

namespace Basic.Collections
{
	/// <summary>
	/// 表示 FunctionParameterInfo 类的集合。
	/// </summary>
	public sealed class FunctionParameterCollection : BaseCollection<FunctionParameterInfo>, INotifyCollectionChanged
	{
		private readonly TableFunctionInfo tableDesignerInfo;
		/// <summary>
		/// 初始化 FunctionParameterCollection 类的新实例。
		/// </summary>
		/// <param name="table">需要通知 FunctionParameterInfo 类实例当前类的属性已更改。</param>
		internal FunctionParameterCollection(TableFunctionInfo table) : base() { tableDesignerInfo = table; }

		/// <summary>
		/// 获取集合的键属性
		/// </summary>
		/// <param name="item">需要获取键的集合子元素</param>
		/// <returns>返回元素的键</returns>
		protected override string GetKey(FunctionParameterInfo item) { return item.Name; }
	}
}
