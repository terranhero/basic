using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using Basic.DataAccess;

namespace Basic.OracleAccess
{
	/// <summary>
	/// SqlCheckCommand 检测命令集合
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), Serializable()]
	internal sealed class OracleNewValueCommandCollection : NewValueCommandCollection
	{
		private readonly OracleStaticCommand dbStaticCommand;
		/// <summary>
		/// 初始化 SqlNewValueCommandCollection 类的新实例。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		public OracleNewValueCommandCollection(OracleStaticCommand staticCommand) : base(staticCommand) { dbStaticCommand = staticCommand; }

		/// <summary>
		/// 初始化 SqlNewValueCommandCollection 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		/// <param name="list">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		public OracleNewValueCommandCollection(OracleStaticCommand staticCommand, IList<NewValueCommand> list)
			: base(staticCommand, list) { dbStaticCommand = staticCommand; }

		/// <summary>
		/// 将新项添加到集合末尾。
		/// </summary>
		/// <returns>已添加到集合中的项。</returns>
		public object AddNewCore()
		{
			return new OracleNewValueCommand(dbStaticCommand);
		}
	}
}
