using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Basic.DataAccess;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.MySqlAccess;
using MySql.Data.MySqlClient;
using STT = System.Threading.Tasks;


namespace Basic.SqlServer
{
	/// <summary>
	/// 执行批处理的静态命令，一般执行INSERT 或 UPDATE 或 DELETE命令。
	/// </summary>
	public sealed class MySqlBatchCommand : BatchCommand
	{
		/// <summary>
		/// 初始化 MySqlBatchCommand 类的新实例。 
		/// </summary>
		/// <param name="cmd">表示 <see cref="StaticCommand"/> 实例。</param>
		public MySqlBatchCommand(StaticCommand cmd) : base(cmd) { }

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
			//MySqlCommand command = staticCommand.DbCommand as MySqlCommand;
			//using (MySql.Data.MySqlClient.MySqlCommand batch = new SqlBatch(command.Connection))
			//{
			//	foreach (TModel entity in entities)
			//	{
			//		SqlBatchCommand batchCommand = new SqlBatchCommand(staticCommand.CommandText);
			//		foreach (SqlParameter parameter in command.Parameters)
			//		{
			//			SqlParameter param = ((ICloneable)parameter).Clone() as SqlParameter;
			//			if (paramSettings != null) { paramSettings(param, entity); }
			//		}

			//		batchCommand.CommandType = staticCommand.CommandType;
			//		//batchCommand.Parameters.Add(new SqlParameter(parameterName, i));
			//		batch.BatchCommands.Add(batchCommand);
			//	}
			//	return await batch.ExecuteNonQueryAsync();
			//}
			return await Task.FromResult(0);
		}

		/// <summary>使用 SqlBatchCommand 类执行数据命令</summary>
		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		/// <param name="entities">类型 <typeparamref name='TModel'/>子类类实例，包含了需要执行参数的值。</param>
		/// <returns>受影响的行数。</returns>
		internal protected override async STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities, int timeout)
		{
			if (entities == null || entities.Any() == false) { return await Task.FromResult(0); }
			//MySqlCommand command = staticCommand.DbCommand as MySqlCommand;
			//using (SqlBatch batch = new SqlBatch(command.Connection))
			//{
			//	foreach (TModel entity in entities)
			//	{
			//		SqlBatchCommand batchCommand = new SqlBatchCommand(staticCommand.CommandText);
			//		foreach (SqlParameter parameter in command.Parameters)
			//		{
			//			SqlParameter param = ((ICloneable)parameter).Clone() as SqlParameter;
			//			if (param.Direction == ParameterDirection.Output) { continue; }
			//			if (entity.TryGetDbProperty(param.SourceColumn, out EntityPropertyMeta propertyInfo))
			//			{
			//				object value = propertyInfo.GetValue(entity);
			//				staticCommand.ResetParameterValue(param, value);
			//			}
			//		}

			//		batchCommand.CommandType = staticCommand.CommandType;
			//		//batchCommand.Parameters.Add(new SqlParameter(parameterName, i));
			//		batch.BatchCommands.Add(batchCommand);
			//	}
			//	return await batch.ExecuteNonQueryAsync();
			//}
			return await Task.FromResult(0);
		}
		//#else
		//		/// <summary>使用 SqlBatchCommand 类执行数据命令</summary>
		//		/// <typeparam name="TModel">表示 AbstractEntity 子类类型</typeparam>
		//		/// <param name="timeout">超时之前操作完成所允许的秒数。</param>
		//		/// <param name="entities">类型 <typeparamref name='TModel'/>子类类实例，包含了需要执行参数的值。</param>
		//		/// <returns>受影响的行数。</returns>
		//		internal protected override async STT.Task<int> ExecuteAsync<TModel>(IEnumerable<TModel> entities, int timeout)
		//		{
		//			if (entities == null || entities.Any() == false) { return await Task.FromResult(0); }
		//			int AffectedRows = 0;
		//			SqlCommand command = staticCommand.DbCommand as SqlCommand;
		//			EntityPropertyCollection properties = EntityPropertyProvidor.GetProperties(typeof(TModel));
		//			foreach (TModel entity in entities)
		//			{
		//				SqlCommand batchCommand = command.Clone(); ;
		//				foreach (SqlParameter parameter in batchCommand.Parameters)
		//				{
		//					if (properties.TryGetDbProperty(parameter.SourceColumn, out EntityPropertyMeta propertyInfo))
		//					{
		//						object value = propertyInfo.GetValue(entity);
		//						staticCommand.ResetParameterValue(parameter, value);
		//					}
		//				}
		//				batchCommand.CommandType = staticCommand.CommandType;
		//				AffectedRows += await batchCommand.ExecuteNonQueryAsync();
		//			}
		//			return await Task.FromResult(AffectedRows);
		//		}
#endif
	}
}
