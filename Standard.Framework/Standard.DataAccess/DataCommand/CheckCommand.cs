using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Basic.DataAccess;
using Basic.Exceptions;
using Basic.Collections;
using Basic.EntityLayer;
using Basic.Interfaces;
using Basic.Tables;
using System.Text;
using Basic.Enums;
using System.Linq;
using Basic.Messages;
using System.Globalization;
using System.Threading.Tasks;

namespace Basic.DataAccess
{
	/// <summary>数据库命令执行前的检查命令</summary>
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never), System.Serializable()]
	[System.ComponentModel.ToolboxItem(false)]
	public abstract class CheckCommand : AbstractDataCommand, IDbCommand, IXmlSerializable//, ICheckCommand
	{
		#region Xml 节点名称常量
		/// <summary>
		/// 表示 Converter 属性
		/// </summary>
		protected internal const string ConverterAttribute = "Converter";

		/// <summary>
		/// 表示 ErrorCode 属性
		/// </summary>
		protected internal const string ErrorCodeAttribute = "ErrorCode";

		/// <summary>
		/// 表示 CheckExist 属性
		/// </summary>
		protected internal const string CheckExistAttribute = "CheckExist";

		/// <summary>
		/// 表示 SourceColumn 属性
		/// </summary>
		protected internal const string SourceColumnAttribute = "SourceColumn";

		/// <summary>
		/// 表示 PropertyName 属性
		/// </summary>
		protected internal const string PropertyNameAttribute = "PropertyName";

		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		public const string CheckCommandsElement = "CheckCommands";

		/// <summary>
		/// 数据库表中所有列配置节名称
		/// </summary>
		public const string XmlElementName = "CheckCommand";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。（元素名）
		/// </summary>
		public const string CommandTextElement = "CommandText";

		/// <summary>
		/// 表示 Parameter 属性。
		/// </summary>
		protected internal const string ParameterAttribute = "Parameter";
		#endregion

		/// <summary>包含此检测命令的父命令信息</summary>
		protected internal readonly StaticCommand dataCommand;

		/// <summary>初始化 CheckDataCommand 类实例</summary>
		/// <param name="staticCommand">表示执行的是数据库静态命令，包含固定的SQL 语句或存储过程。</param>
		/// <param name="command"> 表示要对数据源执行的 SQL 语句或存储过程。</param>
		protected internal CheckCommand(StaticCommand staticCommand, DbCommand command)
			: base(command)
		{
			dataCommand = staticCommand ?? throw new UninitializedCommandException("Access_UninitializedCommand");
			CheckExist = true;
		}

		/// <summary>
		/// 释放数据库连接
		/// </summary>
		internal protected virtual void ReleaseConnection()
		{
			dataDbCommand.Connection = null;
		}

		#region ICheckCommand 成员
		/// <summary>
		/// 获取或设置检查命令执行失败时的错误编码
		/// </summary>
		/// <value>需要返回的错误编码，默认值为null</value>
		public string ErrorCode { get; set; }

		/// <summary>
		/// 获取或设置检查命令执行失败时的错误消息。
		/// </summary>
		/// <value>需要返回的错误编码，默认值为null</value>
		public string ErrorMessage { get; internal protected set; }

		/// <summary>
		/// 获取或设置检查命令执行失败时的错误消息指定的消息转换器
		/// </summary>
		/// <value>需要返回的错误编码，默认值为null</value>
		public string Converter { get; internal protected set; }

		/// <summary>
		/// 当前检测命令是检测存在还是检测不存在。
		/// </summary>
		public bool CheckExist { get; internal protected set; }

		/// <summary>
		/// 获取检测Transact-SQL 命令需要使用的参数名称。
		/// 必须和IDataCommand的DbParameters中存在的参数名对应。
		/// 如果有多个请使用英文状态下的逗号(,)分割。
		/// </summary>
		/// <value>返回参数名</value>
		protected internal string Parameter { get; set; }

		/// <summary>
		/// 检查命令如果失败则需要将失败信息对应的实体类属性
		/// </summary>
		public string PropertyName { get; internal protected set; }

		/// <summary>
		/// 重置数据库连接
		/// </summary>
		/// <param name="connection">一个 DbConnection，它表示到关系数据库实例的连接。 </param>
		public void ResetConnection(System.Data.Common.DbConnection connection)
		{
			dataDbCommand.Connection = connection;
		}
		#endregion

		#region CheckData 方法
		/// <summary>
		/// 执行命令进行检测，判断当前命令是否允许继续执行！
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData(Result result)
		{
			base.ResetParameters();
			object resultValue = dataDbCommand.ExecuteScalar();
			string message = ErrorCode;
			if (CheckExist && resultValue != null && resultValue != DBNull.Value)
			{
				if (Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
					message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
				result.AddError(PropertyName, message);
			}
			else if (!CheckExist && (resultValue == null || resultValue == DBNull.Value))
			{
				if (Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
					message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
				result.AddError(PropertyName, ErrorCode);
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="table">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData<TR>(Result result, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			if (table == null || table.Count == 0) { return result; }
			int rowIndex = -1;
			foreach (TR row in table)
			{
				base.ResetParameters(row); rowIndex++;
				object resultValue = dataDbCommand.ExecuteScalar();
				if (CheckExist == true && resultValue != null && resultValue != DBNull.Value)
				{
					string message = row.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
					message = row.FormatMessage(PropertyName, message);
					result.AddError(rowIndex, PropertyName, message);
					row.SetColumnError(PropertyName, message);
				}
				else if (CheckExist == false && (resultValue == null || resultValue == DBNull.Value))
				{
					string message = row.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
					message = row.FormatMessage(PropertyName, message);
					result.AddError(rowIndex, PropertyName, message);
					row.SetColumnError(PropertyName, message);
				}
			}
			//if (table.HasErrors) { return result.SetError(string.Join(",", table.GetErrors().Select(r => r.RowError).ToArray()), PropertyName); }
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="entities">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData(Result result, params AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0) { return result; }
			bool entitiesArray = entities.Length >= 2; int rowIndex = -1;
			foreach (AbstractEntity entity in entities)
			{
				base.ResetParameters(entity);
				if (entitiesArray) { rowIndex++; }
				object resultValue = dataDbCommand.ExecuteScalar();
				if (CheckExist == true && resultValue != null && resultValue != DBNull.Value)
				{
					string message = entity.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);

					message = entity.FormatMessage(PropertyName, message);
					result.AddError(rowIndex, PropertyName, message);
#if (!NET35)
					entity.AddError(PropertyName, message);
#endif
				}
				else if (CheckExist == false && (resultValue == null || resultValue == DBNull.Value))
				{
					string message = entity.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);

					message = entity.FormatMessage(PropertyName, message);
					result.AddError(rowIndex, PropertyName, message);
#if (!NET35)
					entity.AddError(PropertyName, message);
#endif
				}
			}
			//if (hasError) { return result.SetError(string.Join(",", entities.Select(r => r.GetErrorMessage()).ToArray()), PropertyName); }
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="row">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData(Result result, BaseTableRowType row)
		{
			base.ResetParameters(row);
			object resultValue = dataDbCommand.ExecuteScalar();
			if (CheckExist == true && resultValue != null && resultValue != DBNull.Value)
			{
				string message = row.RaiseChecked(Converter, ErrorCode);
				if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
					message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);

				message = row.FormatMessage(PropertyName, message);
				result.AddError(PropertyName, message);
				row.SetColumnError(PropertyName, message);
			}
			else if (CheckExist == false && (resultValue == null || resultValue == DBNull.Value))
			{
				string message = row.RaiseChecked(Converter, ErrorCode);
				if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
					message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);

				message = row.FormatMessage(PropertyName, message);
				result.AddError(PropertyName, message);
				row.SetColumnError(PropertyName, message);
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="anonObject">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Result CheckData(Result result, object anonObject)
		{
			base.ResetParameters(anonObject);
			object resultValue = dataDbCommand.ExecuteScalar();
			string message = ErrorMessage;
			if (CheckExist == true && resultValue != null && resultValue != DBNull.Value)
			{
				if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
					message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
				else
					message = ErrorCode;
				result.AddError(PropertyName, message);
			}
			else if (CheckExist == false && (resultValue == null || resultValue == DBNull.Value))
			{
				if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
					message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
				else
					message = ErrorCode;
				result.AddError(PropertyName, message);
			}
			return result;
		}
		#endregion

		#region CheckDataAsync 方法
		/// <summary>
		/// 执行命令进行检测，判断当前命令是否允许继续执行！
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Task<Result> CheckDataAsync(Result result)
		{
			base.ResetParameters();
			return dataDbCommand.ExecuteScalarAsync().ContinueWith(task =>
			{
				object resultValue = task.Result; string message = ErrorCode;
				if (CheckExist && resultValue != null && resultValue != DBNull.Value)
				{
					if (Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
					result.AddError(PropertyName, message);
				}
				else if (!CheckExist && (resultValue == null || resultValue == DBNull.Value))
				{
					if (Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
					result.AddError(PropertyName, ErrorCode);
				}
				return result;
			});
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="table">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual async Task<Result> CheckDataAsync<TR>(Result result, BaseTableType<TR> table) where TR : BaseTableRowType
		{
			if (table == null || table.Count == 0) { return result; }
			int rowIndex = -1;
			foreach (TR row in table)
			{
				base.ResetParameters(row); rowIndex++;
				object resultValue = await dataDbCommand.ExecuteScalarAsync();
				if (CheckExist == true && resultValue != null && resultValue != DBNull.Value)
				{
					string message = row.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
					message = row.FormatMessage(PropertyName, message);
					result.AddError(rowIndex, PropertyName, message);
					row.SetColumnError(PropertyName, message);
				}
				else if (CheckExist == false && (resultValue == null || resultValue == DBNull.Value))
				{
					string message = row.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
					message = row.FormatMessage(PropertyName, message);
					result.AddError(rowIndex, PropertyName, message);
					row.SetColumnError(PropertyName, message);
				}
			}
			//if (table.HasErrors) { return result.SetError(string.Join(",", table.GetErrors().Select(r => r.RowError).ToArray()), PropertyName); }
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="entities">需要检测的实体数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual async Task<Result> CheckDataAsync(Result result, params AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0) { return result; }
			bool entitiesArray = entities.Length >= 2; int rowIndex = -1;
			foreach (AbstractEntity entity in entities)
			{
				base.ResetParameters(entity);
				if (entitiesArray) { rowIndex++; }
				object resultValue = await dataDbCommand.ExecuteScalarAsync();
				if (CheckExist == true && resultValue != null && resultValue != DBNull.Value)
				{
					string message = entity.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);

					message = entity.FormatMessage(PropertyName, message);
					result.AddError(rowIndex, PropertyName, message);
					entity.AddError(PropertyName, message);
				}
				else if (CheckExist == false && (resultValue == null || resultValue == DBNull.Value))
				{
					string message = entity.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);

					message = entity.FormatMessage(PropertyName, message);
					result.AddError(rowIndex, PropertyName, message);
					entity.AddError(PropertyName, message);
				}
			}
			return result;
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="row">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Task<Result> CheckDataAsync(Result result, BaseTableRowType row)
		{
			base.ResetParameters(row);
			return dataDbCommand.ExecuteScalarAsync().ContinueWith(task =>
			{
				object resultValue = task.Result;
				if (CheckExist == true && resultValue != null && resultValue != DBNull.Value)
				{
					string message = row.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);

					message = row.FormatMessage(PropertyName, message);
					result.AddError(PropertyName, message);
					row.SetColumnError(PropertyName, message);
				}
				else if (CheckExist == false && (resultValue == null || resultValue == DBNull.Value))
				{
					string message = row.RaiseChecked(Converter, ErrorCode);
					if (!string.IsNullOrEmpty(ErrorCode) && message == ErrorCode && Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);

					message = row.FormatMessage(PropertyName, message);
					result.AddError(PropertyName, message);
					row.SetColumnError(PropertyName, message);
				}
				return result;
			});
		}

		/// <summary>
		/// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
		/// </summary>
		/// <param name="result">表示数据库执行结果。</param>
		/// <param name="anonObject">需要检测的键值对数据源</param>
		/// <returns>执行ransact-SQL语句的返回值。</returns>
		public virtual Task<Result> CheckDataAsync(Result result, object anonObject)
		{
			base.ResetParameters(anonObject);
			return dataDbCommand.ExecuteScalarAsync().ContinueWith(task =>
			{
				object resultValue = task.Result; string message = ErrorCode;
				if (CheckExist && resultValue != null && resultValue != DBNull.Value)
				{
					if (Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
					result.AddError(PropertyName, message);
				}
				else if (!CheckExist && (resultValue == null || resultValue == DBNull.Value))
				{
					if (Regex.IsMatch(ErrorCode, "^[A-Za-z0-9_]+$"))
						message = MessageContext.GetString(Converter, ErrorCode, CultureInfo.CurrentUICulture);
					result.AddError(PropertyName, ErrorCode);
				}
				return result;
			});
		}

		#endregion

		#region IXmlSerializable 成员
		/// <summary>
		/// 根据父命令读取当前检测命令的参数信息
		/// </summary>
		public void ReadOwnerParameter()
		{
			if (!string.IsNullOrEmpty(Parameter))
			{
				if (Parameter.IndexOf(',') >= 0)
					ReadOwnerCommandParameter(Parameter.Split(','));
				else
					ReadOwnerCommandParameter(new string[] { Parameter });
			}
		}

		/// <summary>
		/// 根据父命令读取当前检测命令的参数信息
		/// </summary>
		/// <param name="strArray">此命令需要的参数名称数组</param>
		internal protected abstract void ReadOwnerCommandParameter(string[] strArray);

		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		internal protected override bool ReadAttribute(string name, string value)
		{
			if (name == ParameterAttribute) { Parameter = value; return true; }
			else if (name == CheckExistAttribute) { CheckExist = Convert.ToBoolean(value); return true; }
			else if (name == SourceColumnAttribute) { PropertyName = value; return true; }
			else if (name == PropertyNameAttribute) { PropertyName = value; return true; }
			else if (name == ConverterAttribute) { Converter = value; return true; }
			else if (name == ErrorCodeAttribute) { ErrorCode = value; return true; }
			return base.ReadAttribute(name, value);
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		internal protected override bool ReadContent(System.Xml.XmlReader reader)
		{
			if (reader.NodeType == XmlNodeType.Text && reader.LocalName == string.Empty)//兼容5.0旧版结构信息
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
			else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == CommandTextElement)//兼容5.0新版结构信息
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
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == XmlElementName)
			{
				return true;
			}
			else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == CheckCommandsElement)
			{
				return true;
			}
			return base.ReadContent(reader);
		}

		///// <summary>
		///// 从对象的 XML 表示形式生成该对象。
		///// </summary>
		///// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		//protected internal override void ReadXml(System.Xml.XmlReader reader)
		//{
		//    reader.MoveToContent();
		//    ReadCheckCommandAttribute(reader);
		//    while (reader.Read())
		//    {
		//        if ((reader.NodeType == XmlNodeType.Text && reader.Name == string.Empty) ||
		//         (reader.NodeType == XmlNodeType.Element && reader.Name == CommandTextElement))
		//        {
		//            string text = reader.ReadString();
		//            if (!string.IsNullOrEmpty(text))
		//            {
		//                StringBuilder builder = new StringBuilder(text);
		//                builder.Replace("\n", "", 0, 3);
		//                builder.Replace("\t", "");
		//                builder.Replace("\n", "", builder.Length - 1, 1);
		//                builder.Replace("\n", "\r\n");
		//                CommandText = builder.ToString();
		//            }
		//        }
		//        else if (reader.NodeType == XmlNodeType.Element && reader.Name == ConfigConst.Parameter.ParametersConfig)
		//        {
		//            System.Xml.XmlReader reader2 = reader.ReadSubtree();
		//            while (reader2.Read())	//读取所有静态命令节点信息
		//            {
		//                if (reader2.NodeType == XmlNodeType.Element && reader2.Name == ConfigConst.Parameter.ParameterConfig)
		//                {
		//                    DbParameter param = dataCommand.CreateParameter();
		//                    dataCommand.CreateParameterReader(param, reader2);
		//                    Parameters.Add(param);
		//                }
		//                else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name == ConfigConst.Parameter.ParametersConfig)
		//                    break;
		//            }
		//        }
		//        else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlElementName)
		//        {
		//            break;
		//        }
		//    }
		//}
		#endregion
	}
}
