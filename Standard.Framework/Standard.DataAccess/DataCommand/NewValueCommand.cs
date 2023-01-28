using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Basic.DataAccess;
using Basic.EntityLayer;
using Basic.Exceptions;
using Basic.Collections;
using Basic.Tables;
using Basic.Enums;
using System.Threading.Tasks;

namespace Basic.DataAccess
{
	/// <summary>
	/// 数据库命令执行前的检查命令
	/// </summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public abstract class NewValueCommand : AbstractDataCommand, IDbCommand, IXmlSerializable
	{
		#region 节点元素名称
		/// <summary>
		/// 表示Xml节点的元素名称 “NewValue”。
		/// </summary>
		protected internal const string XmlElementName = "NewValue";
		/// <summary>
		/// 表示Xml节点的元素名称 “NewType”。
		/// </summary>
		protected internal const string NewTypeAttribute = "NewType";
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。（元素名）
		/// </summary>
		protected internal const string CommandTextElement = "CommandText";
		#endregion

		private ExecutedStatusEnum _NewType = ExecutedStatusEnum.EveryTime;
		/// <summary>
		/// 当前命令执行类型。
		/// </summary>
		public ExecutedStatusEnum NewType { get { return _NewType; } set { _NewType = value; } }

		/// <summary>
		/// 包含此检测命令的父命令信息
		/// </summary>
		protected internal readonly StaticCommand dataCommand;

		/// <summary>
		/// 初始化 NewValueCommand 类实例
		/// </summary>
		/// <param name="staticCommand">表示执行的是数据库静态命令，包含固定的SQL 语句或存储过程。</param>
		/// <param name="command"> 表示要对数据源执行的 SQL 语句或存储过程。</param>
		protected internal NewValueCommand(StaticCommand staticCommand, DbCommand command)
			: base(command)
		{
			if (staticCommand == null)
				throw new UninitializedCommandException("Access_UninitializedCommand");
			dataCommand = staticCommand;
		}

		/// <summary>
		/// 释放数据库连接
		/// </summary>
		public virtual void ReleaseConnection()
		{
			dataDbCommand.Connection = null;
		}

		/// <summary>
		/// 重置数据库连接
		/// </summary>
		/// <param name="connection">一个 DbConnection，它表示到关系数据库实例的连接。 </param>
		public void ResetConnection(System.Data.Common.DbConnection connection)
		{
			dataDbCommand.Connection = connection;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="entities">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual async Task<Result> CreateNewValueAsync(Result result, params AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0) { return result; }
			if (_NewType == ExecutedStatusEnum.EveryTime)
			{
				foreach (AbstractEntity entity in entities)
				{
					base.ResetParameters(entity);
					using (DbDataReader reader = await dataDbCommand.ExecuteReaderAsync(CommandBehavior.SingleRow))
					{
						if (!reader.IsClosed && reader.HasRows && reader.Read())
						{
							for (int index = 0; index < reader.FieldCount; index++)
							{
								EntityPropertyMeta propertyInfo = null;
								string name = reader.GetName(index);
								if (entity.TryGetDbProperty(name, out propertyInfo))
								{
									propertyInfo.SetValue(entity, reader.GetValue(index));
									entity.RaiseCreatedValue();
								}
							}
						}
					}
				}
			}
			else if (_NewType == ExecutedStatusEnum.OnlyOnce)
			{
				AbstractEntity firstEntity = entities[0];
				if (!string.IsNullOrEmpty(dataDbCommand.CommandText))
				{
					using (DbDataReader reader = await dataDbCommand.ExecuteReaderAsync(CommandBehavior.SingleRow))
					{
						if (reader.IsClosed || !reader.HasRows || !reader.Read())
							throw new MessageException("命令 {0} 无效，此命令没有返回有效的数据。", dataDbCommand.CommandText);
						int fieldCount = reader.FieldCount;
						AbstractSequence[] sequenceArray = new AbstractSequence[fieldCount];
						EntityPropertyMeta[] propertyArray = new EntityPropertyMeta[fieldCount];
						for (int index = 0; index < fieldCount; index++)
						{
							string name = reader.GetName(index);
							sequenceArray[index] = AbstractSequence.CreateGuidStringSequence(name);
							propertyArray[index] = null;
							if (!firstEntity.TryGetDbProperty(name, out propertyArray[index]))  //实体类{0}中不存在属性{1}。
								throw new MessageException("Access_NotExistProperty", firstEntity.ToString(), name);
							if (!propertyArray[index].CanRead)  //实体类{0}的属性{1},禁止读取数据。
								throw new MessageException("Access_PropertyForbidRead", firstEntity.ToString(), propertyArray[index].Name);
							if (!propertyArray[index].CanWrite) //实体类{0}的属性{1},禁止修改数据。
								throw new MessageException("Access_PropertyForbidWrite", firstEntity.ToString(), propertyArray[index].Name);
							object resultScalar = reader.GetValue(index);
							if (resultScalar.GetType() == propertyArray[index].PropertyType)
								sequenceArray[index] = AbstractSequence.CreateSequence(name, resultScalar);
						}
						foreach (AbstractEntity entity in entities)
						{
							for (int index = 0; index < fieldCount; index++)
							{
								AbstractSequence sequence = sequenceArray[index];
								EntityPropertyMeta propertyInfo = propertyArray[index];
								if (sequence != null && propertyInfo != null)
								{
									propertyInfo.SetValue(entity, sequence.NextValue);
									entity.RaiseCreatedValue();
								}
							}
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="table">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CreateNewValue<TR>(Result result, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			if (table == null || table.Count == 0)
				return result;
			if (_NewType == ExecutedStatusEnum.EveryTime)
			{
				foreach (BaseTableRowType row in table)
				{
					dataCommand.ResetParameters(row);
					using (DbDataReader reader = dataDbCommand.ExecuteReader(CommandBehavior.SingleRow))
					{
						if (!reader.IsClosed && reader.HasRows && reader.Read())
						{
							for (int index = 0; index < reader.FieldCount; index++)
							{
								string name = reader.GetName(index);
								if (row.ContainsKey(name)) { row[name] = reader.GetValue(index); }
							}
						}
					}
				}
			}
			else if (_NewType == ExecutedStatusEnum.OnlyOnce)
			{
				if (!string.IsNullOrEmpty(dataDbCommand.CommandText))
				{
					using (DbDataReader reader = dataDbCommand.ExecuteReader(CommandBehavior.SingleRow))
					{
						if (reader.IsClosed || !reader.HasRows || !reader.Read())
							throw new MessageException("命令 {0} 无效，此命令没有返回有效的数据。", dataDbCommand.CommandText);
						int fieldCount = reader.FieldCount;
						AbstractSequence[] sequenceArray = new AbstractSequence[fieldCount];
						for (int index = 0; index < fieldCount; index++)
						{
							string name = reader.GetName(index);
							object resultScalar = reader.GetValue(index);
							if (table.Columns.Contains(name) && table.Columns[name].DataType == resultScalar.GetType())
								sequenceArray[index] = AbstractSequence.CreateSequence(name, resultScalar);
							else
								sequenceArray[index] = null;
						}
						foreach (BaseTableRowType row in table)
						{
							for (int index = 0; index < fieldCount; index++)
							{
								AbstractSequence sequence = sequenceArray[index];
								if (sequence != null) { row[sequence.PropertyName] = sequence.NextValue; }
							}
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="entities">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CreateNewValue(Result result, params AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0)
				return result;
			if (_NewType == ExecutedStatusEnum.EveryTime)
			{
				foreach (AbstractEntity entity in entities)
				{
					base.ResetParameters(entity);
					using (DbDataReader reader = dataDbCommand.ExecuteReader(CommandBehavior.SingleRow))
					{
						if (!reader.IsClosed && reader.HasRows && reader.Read())
						{
							for (int index = 0; index < reader.FieldCount; index++)
							{
								EntityPropertyMeta propertyInfo = null;
								string name = reader.GetName(index);
								if (entity.TryGetDbProperty(name, out propertyInfo))
								{
									propertyInfo.SetValue(entity, reader.GetValue(index));
									entity.RaiseCreatedValue();
								}
							}
						}
					}
				}
			}
			else if (_NewType == ExecutedStatusEnum.OnlyOnce)
			{
				AbstractEntity firstEntity = entities[0];
				if (!string.IsNullOrEmpty(dataDbCommand.CommandText))
				{
					using (DbDataReader reader = dataDbCommand.ExecuteReader(CommandBehavior.SingleRow))
					{
						if (reader.IsClosed || !reader.HasRows || !reader.Read())
							throw new MessageException("命令 {0} 无效，此命令没有返回有效的数据。", dataDbCommand.CommandText);
						int fieldCount = reader.FieldCount;
						AbstractSequence[] sequenceArray = new AbstractSequence[fieldCount];
						EntityPropertyMeta[] propertyArray = new EntityPropertyMeta[fieldCount];
						for (int index = 0; index < fieldCount; index++)
						{
							string name = reader.GetName(index);
							sequenceArray[index] = AbstractSequence.CreateGuidStringSequence(name);
							propertyArray[index] = null;
							if (!firstEntity.TryGetDbProperty(name, out propertyArray[index]))  //实体类{0}中不存在属性{1}。
								throw new MessageException("Access_NotExistProperty", firstEntity.ToString(), name);
							if (!propertyArray[index].CanRead)  //实体类{0}的属性{1},禁止读取数据。
								throw new MessageException("Access_PropertyForbidRead", firstEntity.ToString(), propertyArray[index].Name);
							if (!propertyArray[index].CanWrite) //实体类{0}的属性{1},禁止修改数据。
								throw new MessageException("Access_PropertyForbidWrite", firstEntity.ToString(), propertyArray[index].Name);
							object resultScalar = reader.GetValue(index);
							if (resultScalar.GetType() == propertyArray[index].PropertyType)
								sequenceArray[index] = AbstractSequence.CreateSequence(name, resultScalar);
						}
						foreach (AbstractEntity entity in entities)
						{
							for (int index = 0; index < fieldCount; index++)
							{
								AbstractSequence sequence = sequenceArray[index];
								EntityPropertyMeta propertyInfo = propertyArray[index];
								if (sequence != null && propertyInfo != null)
								{
									propertyInfo.SetValue(entity, sequence.NextValue);
									entity.RaiseCreatedValue();
								}
							}
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="row">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CreateNewValue(Result result, BaseTableRowType row)
		{
			base.ResetParameters(row);
			using (DbDataReader reader = dataDbCommand.ExecuteReader(CommandBehavior.SingleRow))
			{
				if (!reader.IsClosed && reader.HasRows && reader.Read())
				{
					for (int index = 0; index < reader.FieldCount; index++)
					{
						string name = reader.GetName(index);
						if (row.ContainsKey(name))
							row[name] = reader.GetValue(index);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == NewTypeAttribute)
			{
#if (NET35)
                if (Enum.IsDefined(typeof(ExecutedStatusEnum), value))
                {
                    _NewType = (ExecutedStatusEnum)Enum.Parse(typeof(ExecutedStatusEnum), value, true);
                    return true;
                }
#else
				return Enum.TryParse<ExecutedStatusEnum>(value, true, out _NewType);
#endif
			}
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.Name == CommandTextElement)//兼容5.0新版结构信息
			{
				string text = reader.ReadString();
				if (!string.IsNullOrEmpty(text))
				{
					StringBuilder builder = new StringBuilder(text);
					builder.Replace("\n", "", 0, 10).Replace("\t", "");
					builder.Replace("\n", "", builder.Length - 1, 1);
					builder.Replace("\n", "\r\n");
					dataDbCommand.CommandText = builder.ToString();
				}
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlElementName)
			{
				return true;
			}
			return base.ReadContent(reader);
		}
	}
}
