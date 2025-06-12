using Basic.EntityLayer;
using Basic.Enums;
using Basic.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Xml.Serialization;
using STT = System.Threading.Tasks;

namespace Basic.DataAccess
{
	/// <summary>
	/// 执行批处理的静态命令，一般执行INSERT 或 UPDATE 或 DELETE命令。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public abstract class BatchCommand : Component, IDisposable
	{
		/// <summary>需要执行批处理可的静态命令，一般执行INSERT 或 UPDATE 或 DELETE命令</summary>
		internal protected StaticCommand staticCommand;
		/// <summary>初始化 BatchCommand 类的新实例。 </summary>
		/// <param name="cmd">表示 <see cref="StaticCommand"/> 实例。</param>
		protected BatchCommand(StaticCommand cmd) { staticCommand = cmd; }

		/// <summary>
		/// 针对 .NET Framework 数据提供程序的 Connection 对象执行 SQL 语句，并返回受影响的行数。
		/// </summary>
		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		/// <param name="entities">类型 <see cref="AbstractEntity"/> 子类类实例，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected virtual STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities) where TModel : AbstractEntity
		{
			return this.ExecuteAsync<TModel>(entities, 30);
		}

		/// <summary>使用 BatchCommand 类执行数据命令</summary>
		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <param name="entities">类型 <typeparamref name='TModel'/>子类类实例，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected abstract STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities, int timeout) where TModel : AbstractEntity;
	}
}
