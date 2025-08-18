using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basic.Collections;
using Basic.DataAccess;
using Basic.EntityLayer;
using STT = System.Threading.Tasks;
using Npgsql;
using System.Data.Common;

namespace Basic.PostgreSql
{
	/// <summary>
	/// 执行批处理的静态命令，一般执行INSERT 或 UPDATE 或 DELETE命令。
	/// </summary>
	public sealed class NpgBatchCommand : BatchCommand
	{
		/// <summary>
		/// 初始化 SqlServerBatchCommand 类的新实例。 
		/// </summary>
		/// <param name="cmd">表示 <see cref="StaticCommand"/> 实例。</param>
		public NpgBatchCommand(StaticCommand cmd) : base(cmd) { }

#if NET6_0_OR_GREATER
		/// <summary>使用 BatchCommand 类执行数据命令</summary>
		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <param name="paramSettings">表示执行命令前，自定义初始化参数值的方法。</param>
		/// <param name="entities">类型 <typeparamref name='TModel'/>子类类实例，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected override async STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities, Action<DbParameter, TModel> paramSettings, int timeout)
		{
			if (entities == null || entities.Any() == false) { return await Task.FromResult(0); }
			NpgsqlCommand command = staticCommand.DbCommand as NpgsqlCommand;
			using (NpgsqlBatch batch = new NpgsqlBatch(command.Connection))
			{
				foreach (TModel entity in entities)
				{
					NpgsqlBatchCommand batchCommand = new NpgsqlBatchCommand(staticCommand.CommandText);
					NpgsqlParameterCollection parameters = batchCommand.Parameters;
					foreach (NpgsqlParameter parameter in command.Parameters)
					{
						NpgsqlParameter param = ((ICloneable)parameter).Clone() as NpgsqlParameter;
						if (paramSettings != null) { paramSettings(param, entity); }
					}
					batchCommand.CommandType = staticCommand.CommandType;
					batch.BatchCommands.Add(batchCommand);
				}

				return await batch.ExecuteNonQueryAsync();
			}
		}

		/// <summary>使用 SqlBatchCommand 类执行数据命令</summary>
		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <param name="entities">类型 <typeparamref name='TModel'/>子类类实例，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected override async STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities, int timeout)
		{
			if (entities == null || entities.Any() == false) { return await Task.FromResult(0); }
			NpgsqlCommand command = staticCommand.DbCommand as NpgsqlCommand;
			using (NpgsqlBatch batch = new NpgsqlBatch(command.Connection))
			{
				EntityPropertyCollection properties = EntityPropertyProvidor.GetProperties(typeof(TModel));
				foreach (TModel entity in entities)
				{
					NpgsqlBatchCommand batchCommand = new NpgsqlBatchCommand(staticCommand.CommandText);
					NpgsqlParameterCollection parameters = batchCommand.Parameters;
					foreach (NpgsqlParameter parameter in command.Parameters)
					{
						NpgsqlParameter param = ((ICloneable)parameter).Clone() as NpgsqlParameter;
						if (properties.TryGetDbProperty(param.SourceColumn, out EntityPropertyMeta propertyInfo))
						{
							object value = propertyInfo.GetValue(entity);
							staticCommand.ResetParameterValue(param, value);
						}
					}
					batchCommand.CommandType = staticCommand.CommandType;
					batch.BatchCommands.Add(batchCommand);
				}

				return await batch.ExecuteNonQueryAsync();
			}
		}
#endif
	}
}
