using System;
using System.Collections.Generic;
using System.ComponentModel;
using Basic.DataAccess;

namespace Basic.DB2Access
{
	/// <summary>
	/// DB2CheckCommand 检测命令集合
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), Serializable()]
	internal sealed class DB2CheckCommandCollection : Basic.DataAccess.CheckCommandCollection
	{
		private readonly DB2StaticCommand ownerCommand;
		/// <summary>
		/// 初始化 DB2CheckCommandCollection 类的新实例。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		public DB2CheckCommandCollection(DB2StaticCommand staticCommand) : base(staticCommand) { ownerCommand = staticCommand; }

		/// <summary>
		/// 初始化 DB2CheckCommandCollection 类的新实例，该类包含从指定集合中复制的元素。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		/// <param name="collection">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		public DB2CheckCommandCollection(DB2StaticCommand staticCommand, IEnumerable<CheckCommand> collection)
			: base(staticCommand, collection) { ownerCommand = staticCommand; }

		/// <summary>
		/// 初始化 DB2CheckCommandCollection 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		/// <param name="list">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		public DB2CheckCommandCollection(DB2StaticCommand staticCommand, List<CheckCommand> list)
			: base(staticCommand, list) { ownerCommand = staticCommand; }

		/// <summary>
		/// 将新项添加到集合末尾。
		/// </summary>
		/// <returns>已添加到集合中的项。</returns>
		public object AddNewCore()
		{
			return new DB2CheckCommand(ownerCommand);
		}
	}
}
