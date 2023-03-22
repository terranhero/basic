using Basic.EntityLayer;

using System.Reflection;
using System;
using System.Text;
using System.Data.Common;
using System.Data;
using Basic.Configuration;
using System.Linq;

namespace Basic.DataAccess
{
	/// <summary>
	/// 实体类 AbstractEntity 方法扩展。
	/// </summary>
	public static class AbstractEntityExternsion
	{
		/// <summary>
		/// 清空实体错误信息。
		/// </summary>
		/// <param name="entities">需要扩展 AbstractEntity 类数组实例。</param>
		public static void ClearError(this AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0) { return; }
			foreach (AbstractEntity entity in entities) { entity.ClearError(); }
		}
		/// <summary>
		/// 判断当前实体数组是否有异常。
		/// </summary>
		/// <param name="entities">需要扩展 AbstractEntity 类数组实例。</param>
		public static bool HasError(this AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0) { return false; }
			foreach (AbstractEntity entity in entities) { if (entity.HasError()) { return entity.HasError(); } }
			return false;
		}

		/// <summary>
		/// 设置实体属性的值
		/// </summary>
		/// <param name="entity">需要扩展 AbstractEntity 类实例。</param>
		/// <param name="name">需要指定的数据库连接名称。</param>
		/// <returns>如果找到符合条件的记录则返回true，否则返回false。</returns>
		public static bool SelectByKey(this AbstractEntity entity, string name = null)
		{
			ConnectionInfo info = ConnectionContext.DefaultConnection;
			if (ConnectionContext.Contains(name)) { info = ConnectionContext.GetConnection(name); }
			ConnectionFactory factory = ConnectionFactoryBuilder.CreateConnectionFactory(info);
			StaticCommand staticCommand = factory.CreateStaticCommand();
			entity.InitializeCommand(staticCommand);
			using (DbConnection connection = factory.CreateConnection(info))
			{
				return entity.SelectByKey(staticCommand, connection);
			}
		}

		/// <summary>
		/// 按关键字查询
		/// </summary>
		/// <param name="entity">需要扩展 AbstractEntity 类实例。</param>
		/// <param name="staticCommand">执行此查询的静态命令</param>
		/// <param name="connection">数据库连接实例。</param>
		/// <returns>如果找到符合条件的记录则返回true，否则返回false。</returns>
		private static bool SelectByKey(this AbstractEntity entity, StaticCommand staticCommand, DbConnection connection)
		{
			connection.Open();
			staticCommand.ResetConnection(connection);
			using (DbDataReader reader = staticCommand.ExecuteReader(CommandBehavior.CloseConnection))
			{
				if (reader.HasRows && reader.Read())
				{
					EntityPropertyMeta[] propertyDescriptors = entity.GetProperties();
					foreach (EntityPropertyMeta propertyInfo in propertyDescriptors)
					{
						if (propertyInfo.PrimaryKey || propertyInfo.Ignore) { continue; }
						if (propertyInfo.Mapping != null)
						{
							int index = reader.GetOrdinal(propertyInfo.Mapping.ColumnName);
							object propertyValue = reader.GetValue(index);
							propertyInfo.SetValue(entity, propertyValue);
						}
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// 创建按关键字查询命令。
		/// </summary>
		/// <param name="entity">需要扩展的实体模型实例</param>
		/// <param name="staticCommand">需要初始化的命令</param>
		/// <returns>如果初始化成功则返回true，否则返回false。</returns>
		private static bool InitializeCommand(this AbstractEntity entity, StaticCommand staticCommand)
		{
			TableMappingAttribute tma = (TableMappingAttribute)Attribute.GetCustomAttribute(entity.GetType(), typeof(TableMappingAttribute));
			if (tma == null) { return false; }
			EntityPropertyMeta[] propertyDescriptors = entity.GetProperties();
			staticCommand.CommandName = AbstractDbAccess.SelectByKeyConfig;
			StringBuilder selectBuilder = new StringBuilder("SELECT ", 1000);
			int selectLength = selectBuilder.Length;
			StringBuilder whereBuilder = new StringBuilder(" WHERE ", 500);
			int whereLength = whereBuilder.Length;
			foreach (EntityPropertyMeta propertyInfo in propertyDescriptors)
			{
				if (propertyInfo.Ignore == true) { continue; }
				ColumnMappingAttribute cma = propertyInfo.Mapping;
				if (cma == null || string.IsNullOrEmpty(cma.ColumnName)) { continue; }
				if (propertyInfo.PrimaryKey)
				{
					DbParameter parameter = staticCommand.CreateParameter(cma.ColumnName, cma.SourceColumn, cma.DataType, cma.Size, ParameterDirection.Input, cma.Nullable);
					staticCommand.ConvertParameterType(parameter, cma.DataType, cma.Precision, cma.Scale);
					parameter.Direction = ParameterDirection.Input;
					parameter.Value = propertyInfo.GetValue(entity);
					string parameterName = staticCommand.CreateParameterName(cma.ColumnName);
					staticCommand.Parameters.Add(parameter);
					if (whereLength == whereBuilder.Length)
						whereBuilder.AppendFormat("{0}={1}", cma.ColumnName, parameterName);
					else
						whereBuilder.AppendFormat(" AND {0}={1}", cma.ColumnName, parameterName);
				}
				else
				{
					if (selectLength == selectBuilder.Length)
						selectBuilder.AppendFormat("{0}", cma.ColumnName);
					else
						selectBuilder.AppendFormat(", {0}", cma.ColumnName);
				}
			}
			if (tma.AliasName == tma.TableName)
				selectBuilder.Append(" FROM ").Append(tma.TableName);
			else if (!string.IsNullOrEmpty(tma.AliasName))
				selectBuilder.Append(" FROM ").Append(tma.AliasName);
			else
				selectBuilder.Append(" FROM ").Append(tma.TableName);
			selectBuilder.Append(whereBuilder.ToString());
			staticCommand.CommandText = selectBuilder.ToString();
			staticCommand.CommandType = CommandType.Text;
			return true;
		}
	}
}
