﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Basic.Collections;
using Basic.DataAccess;
using Basic.EntityLayer;
using Basic.Enums;
using Microsoft.Data.SqlClient;
using STT = System.Threading.Tasks;

namespace Basic.SqlServer
{
	/// <summary>
	/// 执行批处理的静态命令，一般执行INSERT 或 UPDATE 或 DELETE命令。
	/// </summary>
	public sealed class SqlServerBatchCommand : BatchCommand
	{
		/// <summary>
		/// 初始化 SqlServerBatchCommand 类的新实例。 
		/// </summary>
		/// <param name="cmd">表示 <see cref="StaticCommand"/> 实例。</param>
		public SqlServerBatchCommand(StaticCommand cmd) : base(cmd) { }

		/// <summary>使用 SqlBatchCommand 类执行数据命令</summary>
		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <param name="entities">类型 <typeparamref name='TModel'/>子类类实例，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected override async STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities, int timeout)
		{
			if (entities == null || entities.Any() == false) { return await Task.FromResult(0); }
#if NET6_0_OR_GREATER
			SqlCommand command = staticCommand.DbCommand as SqlCommand;
			using (SqlBatch batch = new SqlBatch(command.Connection))
			{
				EntityPropertyCollection properties = EntityPropertyProvidor.GetProperties(typeof(TModel));
				foreach (TModel entity in entities)
				{
					SqlBatchCommand batchCommand = new SqlBatchCommand(staticCommand.CommandText);
					SqlParameterCollection parameters = batchCommand.Parameters;
					foreach (SqlParameter parameter in command.Parameters)
					{
						SqlParameter param = ((ICloneable)parameter).Clone() as SqlParameter;
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
#else
			int AffectedRows = 0;
			SqlCommand command = staticCommand.DbCommand as SqlCommand;
			EntityPropertyCollection properties = EntityPropertyProvidor.GetProperties(typeof(TModel));
			foreach (TModel entity in entities)
			{
				SqlCommand batchCommand = command.Clone(); ;
				foreach (SqlParameter parameter in batchCommand.Parameters)
				{
					if (properties.TryGetDbProperty(parameter.SourceColumn, out EntityPropertyMeta propertyInfo))
					{
						object value = propertyInfo.GetValue(entity);
						staticCommand.ResetParameterValue(parameter, value);
					}
				}
				batchCommand.CommandType = staticCommand.CommandType;
				AffectedRows += await batchCommand.ExecuteNonQueryAsync();
			}
			return await Task.FromResult(AffectedRows);
#endif
		}
	}
}
