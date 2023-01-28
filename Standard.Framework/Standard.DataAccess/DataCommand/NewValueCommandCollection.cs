using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Basic.DataAccess
{
	/// <summary>
	/// ICheckCommand 检测命令集合
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), Serializable()]
	public abstract class NewValueCommandCollection : Collection<NewValueCommand>
	{
		private readonly StaticCommand _OwnerCommand;
		/// <summary>
		/// 包含此新值命令的父命令信息
		/// </summary>
		internal protected StaticCommand Command { get { return _OwnerCommand; } }

		/// <summary>
		/// 初始化 NewValueCommandCollection 类的新实例。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		protected NewValueCommandCollection(StaticCommand staticCommand) : base() { _OwnerCommand = staticCommand; }

		/// <summary>
		/// 初始化 NewValueCommandCollection 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="staticCommand">包含此检测命令集合的静态命令</param>
		/// <param name="list">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		protected NewValueCommandCollection(StaticCommand staticCommand, IList<NewValueCommand> list)
			: base(list) { _OwnerCommand = staticCommand; }
	}
}
