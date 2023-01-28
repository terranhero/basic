using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Basic.DataAccess;

using Basic.Collections;

namespace Basic.SqlServer
{
	/// <summary>
	/// SqlCheckCommand 检测命令集合
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), Serializable()]
	[System.Xml.Serialization.XmlRoot(AbstractDataCommand.CheckCommandsConfig)]
	internal sealed class SqlCheckCommandCollection : Basic.DataAccess.CheckCommandCollection
	{
		private readonly SqlStaticCommand ownerCommand;
		/// <summary>
		/// 初始化 SqlCheckCommandCollection 类的新实例。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		public SqlCheckCommandCollection(SqlStaticCommand staticCommand) : base(staticCommand) { ownerCommand = staticCommand; }

		/// <summary>
		/// 初始化 SqlCheckCommandCollection 类的新实例，该类包含从指定集合中复制的元素。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		/// <param name="collection">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		public SqlCheckCommandCollection(SqlStaticCommand staticCommand, IEnumerable<CheckCommand> collection)
			: base(staticCommand, collection) { ownerCommand = staticCommand; }

		/// <summary>
		/// 初始化 SqlCheckCommandCollection 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		/// <param name="list">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		public SqlCheckCommandCollection(SqlStaticCommand staticCommand, List<CheckCommand> list)
			: base(staticCommand, list) { ownerCommand = staticCommand; }

		/// <summary>
		/// 将新项添加到集合末尾。
		/// </summary>
		/// <returns>已添加到集合中的项。</returns>
		public object AddNewCore()
		{
			return new SqlCheckCommand(ownerCommand);
		}
	}
}
