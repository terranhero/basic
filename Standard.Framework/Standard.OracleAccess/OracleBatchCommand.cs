using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Basic.DataAccess;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.OracleAccess;
using Basic.Tables;
using Oracle.ManagedDataAccess.Client;
using STT = System.Threading.Tasks;


namespace Basic.OracleAccess
{
	/// <summary>
	/// 执行批处理的静态命令，一般执行INSERT 或 UPDATE 或 DELETE命令。
	/// </summary>
	internal sealed class OracleBatchCommand : BatchCommand
	{
		/// <summary>
		/// 初始化 OracleBatchCommand 类的新实例。 
		/// </summary>
		/// <param name="cmd">表示 <see cref="StaticCommand"/> 实例。</param>
		public OracleBatchCommand(StaticCommand cmd) : base(cmd) { }

		/// <summary>使用 SqlBatchCommand 类执行数据命令</summary>
		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <param name="entities">类型 <typeparamref name='TModel'/>子类类实例，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected override async STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities, int timeout)
		{
			if (entities == null || entities.Any() == false) { return await Task.FromResult(0); }
#if NET6_0_OR_GREATER
			using (OracleCommand command = staticCommand.DbCommand as OracleCommand)
			{

				//foreach (TModel entity in entities)
				//{
				//	foreach (OracleParameter parameter in command.Parameters)
				//	{
				//		parameter.
				//		SqlParameter param = batchCommand.Parameters.Add(parameter.ParameterName, parameter.SqlDbType, parameter.Size);
				//		param.Precision = parameter.Precision;
				//		param.Scale = parameter.Scale;
				//		staticCommand.ResetParameterValue(param, null);
				//	}
				//	batchCommand.CommandType = staticCommand.CommandType;
				//	//batchCommand.Parameters.Add(new SqlParameter(parameterName, i));
				//	batch.BatchCommands.Add(batchCommand);
				//}


				return await command.ExecuteNonQueryAsync();
			}

#else
			return await Task.FromResult(0);
#endif
		}
	}
}
