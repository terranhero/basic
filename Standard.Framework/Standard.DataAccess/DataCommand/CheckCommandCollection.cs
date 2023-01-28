using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Basic.DataAccess;
using System.Collections.ObjectModel;
using Basic.Collections;
using Basic.Interfaces;

namespace Basic.DataAccess
{
	/// <summary>
	/// ICheckCommand 检测命令集合
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), Serializable()]
	[System.Xml.Serialization.XmlRoot(AbstractDataCommand.CheckCommandsConfig)]
	public abstract class CheckCommandCollection : ObservableCollection<CheckCommand>
	//, ICheckCommandCollection<CheckCommand>
	{
		private StaticCommand _OwnerCommand;
		/// <summary>
		/// 包含此检测命令的父命令信息
		/// </summary>
		protected StaticCommand OwnerCommand { get { return _OwnerCommand; } }

		/// <summary>
		/// 初始化 CheckCommandCollection 类的新实例。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		protected CheckCommandCollection(StaticCommand staticCommand) : base() { _OwnerCommand = staticCommand; }

		/// <summary>
		/// 初始化 CheckCommandCollection 类的新实例，该类包含从指定集合中复制的元素。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		/// <param name="collection">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		protected CheckCommandCollection(StaticCommand staticCommand, IEnumerable<CheckCommand> collection)
			: base(collection) { _OwnerCommand = staticCommand; }

		/// <summary>
		/// 初始化 CheckCommandCollection 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		/// <param name="list">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		protected CheckCommandCollection(StaticCommand staticCommand, List<CheckCommand> list)
			: base(list) { _OwnerCommand = staticCommand; }
	}
}
