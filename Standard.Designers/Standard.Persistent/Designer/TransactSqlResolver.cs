﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Configuration;
using Basic.DataEntities;
using Basic.Database;
using Basic.Collections;
using Basic.Designer;
using MSTS = Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;

namespace Basic.DataContexts
{
	/// <summary>
	/// Transact-SQL行类型
	/// </summary>
	internal enum LineType { None, With, Select, From, Where, GroupBy, Having, OrderBy }

	/// <summary>
	/// From子句类型
	/// </summary>
	internal enum FromType { None, Keyword, Table, Alias, Condition, LogicalSymbol }

	/// <summary>
	/// Transact-SQL查询语句分解器
	/// </summary>
	internal static class TransactSqlResolver
	{
		/// <summary>
		/// 测试当前传入的 Transact-SQL 是否可以分解(是否符合预期的格式)。
		/// </summary>
		/// <param name="sql">需要测试分解的 Transact-SQL。</param>
		/// <returns>如果测试成功则返回True，否则返回False。</returns>
		public static bool Test(string sql)
		{
			if (string.IsNullOrWhiteSpace(sql)) { return false; }
			string[] sqlLines = sql.Split(new char[] { '\x0a' }, StringSplitOptions.RemoveEmptyEntries);    //换行分割
			bool startSelect = false, startFrom = false;
			foreach (string line in sqlLines)
			{
				string newLine = line.Trim();
				if (string.IsNullOrWhiteSpace(newLine)) { continue; }
				if (!startSelect) { startSelect = newLine.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase); }
				if (!startFrom) { startFrom = newLine.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase); }
			}
			return startSelect && startFrom;
		}

		/// <summary>
		/// 解析 FROM 子句中的一行，将当前行的数据表和表别名解析出来。
		/// </summary>
		/// <param name="fromText">一个 String 类型的值，该值表示 FROM 子句中的一行。</param>
		private static void PasteFromText(TransactTableCollection result, string fromText)
		{
			fromText = fromText.Replace('\x0a', ' ').ToUpperInvariant();
			List<string> keywords = new List<string>(new string[] { "FULL JOIN", "FULL OUTER JOIN", "LEFT OUTER JOIN", "RIGHT OUTER JOIN", "INNER JOIN" });
			keywords.AddRange(new string[] { "LEFT JOIN", "RIGHT JOIN", "JOIN" });
			string[] fromLines = fromText.Split(keywords.ToArray(), StringSplitOptions.RemoveEmptyEntries);
			#region 解析from子句
			foreach (string fromLine in fromLines)
			{
				string newLine = fromLine.Trim();
				if (string.IsNullOrWhiteSpace(newLine)) { continue; }
				else if (newLine.Contains(" ON "))
				{
					int spaceIndex = newLine.IndexOf(" "); int onIndex = newLine.IndexOf(" ON ");
					int asIndex = newLine.IndexOf(" AS ");
					string tableName = newLine.Substring(0, spaceIndex);
					if (tableName.Contains("."))
					{
						int docIndex = tableName.IndexOf(".");
						tableName = tableName.Substring(docIndex + 1, tableName.Length - docIndex - 1);
					}
					string aliasName = tableName;
					if (asIndex > 0 && asIndex < onIndex) { aliasName = newLine.Substring(spaceIndex + 4, onIndex - spaceIndex - 4); }
					else if (spaceIndex > 0 && spaceIndex < onIndex) { aliasName = newLine.Substring(spaceIndex + 1, onIndex - spaceIndex - 1); }
					result.AddTable(tableName, aliasName);
				}
				else
				{
					int spaceIndex = newLine.IndexOf(" "); int asIndex = newLine.IndexOf(" AS ");
					string tableName = newLine;
					if (spaceIndex > 0) { tableName = newLine.Substring(0, spaceIndex); }
					if (tableName.Contains("."))
					{
						int docIndex = tableName.IndexOf(".");
						tableName = tableName.Substring(docIndex + 1, tableName.Length - docIndex - 1);
					}
					string aliasName = tableName;
					if (asIndex > 0 && asIndex < newLine.Length) { aliasName = newLine.Substring(spaceIndex + 4, newLine.Length - spaceIndex - 4); }
					else if (spaceIndex > 0 && spaceIndex < newLine.Length) { aliasName = newLine.Substring(spaceIndex + 1, newLine.Length - spaceIndex - 1); }
					result.AddTable(tableName, aliasName);
				}
			}
			#endregion
			result.TableName = result.First().Name;
		}

		/// <summary>
		/// 将当前选中的 StaticCommand 类型的命令，属性 CommandText 替换为传入的 sql 值。
		/// </summary>
		/// <param name="staticCommand">需要修改的 StaticCommand 类型的命令。</param>
		/// <param name="sql">需要替换的 Transact-SQL 查询语句。</param>
		/// <returns>如果替换成功则返回 True，否则返回 False。</returns>
		public static bool PasteStaticCommand(StaticCommandElement staticCommand, string sql)
		{
			if (staticCommand == null) { return false; }
			staticCommand.CommandText = sql;
			TransactTableCollection result = ResolverTransactSql(sql);
			using (IDataContext context = DataContextFactory.CreateDbAccess())
			{
				context.GetParameters(result);
			}
			result.CreateParameters(staticCommand);
			return true;
		}

		/// <summary>
		/// 将当前选中的 StaticCommand 类型的命令，属性 CommandText 替换为传入的 sql 值。
		/// </summary>
		/// <param name="persistent">需要修改的 PersistentConfiguration 类实例。</param>
		/// <param name="sql">需要替换的 Transact-SQL 查询语句。</param>
		/// <returns>如果替换成功则返回 True，否则返回 False。</returns>
		public static bool PasteStaticCommand(PersistentConfiguration persistent, string sql)
		{
			if (persistent == null) { return false; }
			TransactTableCollection result = ResolverTransactSql(sql);
			persistent.UpdatePropertyMapping(result.PropertyMapping);
			DataEntityElement dataEntityElement = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
			StaticCommandElement staticCommand = new StaticCommandElement(dataEntityElement)
			{
				Name = "StaticCommand1"
			};
			dataEntityElement.DataCommands.Add(staticCommand);
			staticCommand.CommandText = sql;
			using (IDataContext context = DataContextFactory.CreateDbAccess())
			{
				context.GetParameters(result);
				context.GetTransactSql(result, sql);
			}
			result.CreateDataEntityElement(dataEntityElement);
			result.CreateDataConditionElement(dataEntityElement.Condition);
			result.CreateParameters(staticCommand);
			persistent.DataEntities.Add(dataEntityElement);
			return true;
		}

		/// <summary>
		/// 将当前传入的 Transact-SQL 语句，解析为一个 DynamicCommandElement 类型的实例。
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static TransactTableCollection ResolverTransactSql(string sql)
		{
			MSTS.TSql80Parser parser = new MSTS.TSql80Parser(true);
			MSTS.TSqlFragment tsql = parser.Parse(new StringReader(sql), out IList<MSTS.ParseError> errors);
			//foreach (MSTS.ParseError error in errors)
			//{
			//	error.Message
			//}
			foreach (MSTS.TSqlParserToken token in tsql.ScriptTokenStream)
			{
				_ = token.TokenType == MSTS.TSqlTokenType.Select;
			}

			TransactTableCollection result = new TransactTableCollection(false);
			if (string.IsNullOrWhiteSpace(sql)) { return result; }
			string[] sqlLines = sql.Split(new char[] { '\x0a', '\x0d' }, StringSplitOptions.RemoveEmptyEntries);    //换行分割
			LineType lineType = LineType.None;
			foreach (string line in sqlLines)
			{
				string newLine = line.Trim();
				if (string.IsNullOrWhiteSpace(newLine)) { continue; }
				else if (newLine.StartsWith("DECLARE ", StringComparison.OrdinalIgnoreCase) && lineType == LineType.None)
				{
					newLine = newLine.Remove(0, 7);
				}
				else if (newLine.StartsWith("WITH ", StringComparison.OrdinalIgnoreCase) && lineType == LineType.None)
				{
					lineType = LineType.With; newLine = newLine.Remove(0, 5);
				}
				else if (newLine.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase) && (lineType == LineType.None || lineType == LineType.With))
				{
					lineType = LineType.Select; newLine = newLine.Remove(0, 7);
				}
				else if (newLine.StartsWith("FROM ", StringComparison.OrdinalIgnoreCase) && lineType == LineType.Select)
				{
					lineType = LineType.From; newLine = newLine.Remove(0, 5);
				}
				else if (newLine.StartsWith("WHERE ", StringComparison.OrdinalIgnoreCase) && lineType == LineType.From)
				{
					lineType = LineType.Where; newLine = newLine.Remove(0, 6);
				}
				else if (newLine.StartsWith("GROUP BY ", StringComparison.OrdinalIgnoreCase) && (lineType == LineType.Where || lineType == LineType.From))
				{
					lineType = LineType.GroupBy; newLine = newLine.Remove(0, 9);
				}
				else if (newLine.StartsWith("HAVING ", StringComparison.OrdinalIgnoreCase) && lineType == LineType.GroupBy)
				{
					lineType = LineType.Having; newLine = newLine.Remove(0, 7);
				}
				else if (newLine.StartsWith("ORDER BY ", StringComparison.OrdinalIgnoreCase) &&
					(lineType == LineType.Where || lineType == LineType.From || lineType == LineType.GroupBy || lineType == LineType.Having))
				{
					lineType = LineType.OrderBy; newLine = newLine.Remove(0, 9);
				}

				switch (lineType)
				{
					case LineType.With:
						if (result.WithBuilder.Length == 0)
							result.WithBuilder.Append(newLine);
						else
							result.WithBuilder.Append(char.ToString('\x0a')).Append(newLine);
						break;
					case LineType.Select:
						if (result.SelectBuilder.Length == 0)
							result.SelectBuilder.Append(newLine);
						else
							result.SelectBuilder.Append(char.ToString('\x0a')).Append(newLine);
						break;
					case LineType.From:
						if (result.FromBuilder.Length == 0)
							result.FromBuilder.Append(newLine);
						else
							result.FromBuilder.Append(char.ToString('\x0a')).Append(newLine);
						break;
					case LineType.Where:
						if (result.WhereBuilder.Length == 0)
							result.WhereBuilder.Append(newLine);
						else
							result.WhereBuilder.Append(char.ToString('\x0a')).Append(newLine);
						break;
					case LineType.GroupBy:
						if (result.GroupBuilder.Length == 0)
							result.GroupBuilder.Append(newLine);
						else
							result.GroupBuilder.Append(char.ToString('\x0a')).Append(newLine);
						break;
					case LineType.Having:
						if (result.HavingBuilder.Length == 0)
							result.HavingBuilder.Append(newLine);
						else
							result.HavingBuilder.Append(char.ToString('\x0a')).Append(newLine);
						break;
					case LineType.OrderBy:
						if (result.OrderBuilder.Length == 0)
							result.OrderBuilder.Append(newLine);
						else
							result.OrderBuilder.Append(char.ToString('\x0a')).Append(newLine);
						break;
				}
			}
			PasteFromText(result, result.FromText);
			return result;
		}

		/// <summary>
		/// 根据SQL语句更新实体模型
		/// </summary>
		/// <param name="entity">需要更新的实体模型</param>
		/// <param name="result">SQL语句解析结果。</param>
		/// <param name="text">SQL解析源。</param>
		/// <returns>执行成功返回 true，否则返回 false。</returns>
		private static bool UpdateDataEntity(DataEntityElement entity, TransactTableCollection result, string text)
		{
			using (IDataContext context = DataContextFactory.CreateDbAccess())
			{
				entity.Persistent.UpdatePropertyMapping(result.PropertyMapping);
				context.GetParameters(result);
				context.GetTransactSql(result, text);
			}
			result.CreateDataEntityElement(entity);
			return true;
		}

		/// <summary>
		/// 根据SQL语句更新实体模型
		/// </summary>
		/// <param name="entity">需要更新的实体模型</param>
		/// <param name="text">数据源。</param>
		/// <returns>执行成功返回 true，否则返回 false。</returns>
		public static bool UpdateDataEntity(DataEntityElement entity, DynamicCommandElement dynamicCommand)
		{
			StringBuilder textBuilder = new StringBuilder(1000);
			if (dynamicCommand.WithClauses.Count > 0)
			{
				textBuilder.Append("WITH "); List<string> clauses = new List<string>(dynamicCommand.WithClauses.Count + 2);
				foreach (WithClause clause in dynamicCommand.WithClauses)
				{
					clauses.Add(string.Concat(clause.TableName, "(", clause.TableDefinition, ") AS (", clause.TableQuery, ")"));
				}
				textBuilder.AppendLine(string.Join(",", clauses.ToArray()));
			}

			textBuilder.Append(" SELECT ").AppendLine(dynamicCommand.SelectText);
			textBuilder.Append(" FROM ").AppendLine(dynamicCommand.FromText);
			if (dynamicCommand.HasGroup) { textBuilder.Append(" GROUP BY ").Append(dynamicCommand.GroupText); }

			TransactTableCollection result = ResolverTransactSql(textBuilder.ToString());
			result.GetParameters(dynamicCommand);
			return UpdateDataEntity(entity, result, textBuilder.ToString());
		}

		/// <summary>
		/// 根据SQL语句更新实体模型
		/// </summary>
		/// <param name="entity">需要更新的实体模型</param>
		/// <param name="result">SQL语句解析结果。</param>
		/// <param name="text">SQL解析源。</param>
		/// <returns>执行成功返回 true，否则返回 false。</returns>
		private static bool UpdateDataCondition(DataConditionElement entity, TransactTableCollection result, string text)
		{
			using (IDataContext context = DataContextFactory.CreateDbAccess())
			{
				entity.Persistent.UpdatePropertyMapping(result.PropertyMapping);
				context.GetParameters(result);
				context.GetTransactSql(result, text);
			}
			result.CreateDataConditionElement(entity);
			return true;
		}

		/// <summary>
		/// 根据SQL语句更新实体模型
		/// </summary>
		/// <param name="entity">需要更新的实体模型</param>
		/// <param name="text">数据源。</param>
		/// <returns>执行成功返回 true，否则返回 false。</returns>
		public static bool UpdateDataCondition(DataConditionElement entity, string text)
		{
			TransactTableCollection result = ResolverTransactSql(text);
			return UpdateDataCondition(entity, result, text);
		}

		/// <summary>
		/// 将当前选中的 DynamicCommand 类型的命令，属性 CommandText 替换为传入的 sql 值。
		/// </summary>
		/// <param name="dynamicCommand">需要修改的 DynamicCommand 类型的命令。</param>
		/// <param name="sql">需要替换的 Transact-SQL 查询语句。</param>
		/// <returns>如果替换成功则返回 True，否则返回 False。</returns>
		public static bool PasteDynamicCommand(DynamicCommandElement dynamicCommand, string sql)
		{
			if (dynamicCommand == null) { return false; }
			TransactTableCollection result = ResolverTransactSql(sql);
			using (IDataContext context = DataContextFactory.CreateDbAccess())
			{
				context.GetParameters(result);
			}
			CreateDynamicCommand(dynamicCommand, result);
			return true;
		}

		/// <summary>
		/// 将sql语句粘帖为动态命令
		/// </summary>
		/// <param name="persistent"></param>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static bool PasteDynamicCommand(PersistentConfiguration persistent, string sql)
		{
			if (persistent == null) { return false; }
			TransactTableCollection result = ResolverTransactSql(sql);
			StringBuilder textBuilder = new StringBuilder("SELECT ", 500);
			textBuilder.AppendLine(result.SelectText);
			textBuilder.Append("FROM ").Append(result.FromText);
			persistent.UpdatePropertyMapping(result.PropertyMapping);
			DataEntityElement dataEntityElement = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
			DynamicCommandElement dynamicCommand = new DynamicCommandElement(dataEntityElement);
			dynamicCommand.Name = string.Concat("DynamicCommand_", persistent.DataCommands.Count);
			using (IDataContext context = DataContextFactory.CreateDbAccess())
			{
				context.GetParameters(result);
				//context.GetTransactSql(result, textBuilder.ToString());
				context.GetTransactSql(result, sql);
			}
			CreateDynamicCommand(dynamicCommand, result);
			dataEntityElement.DataCommands.Add(dynamicCommand);
			result.CreateDataEntityElement(dataEntityElement);
			result.CreateDataConditionElement(dataEntityElement.Condition);
			persistent.DataEntities.Add(dataEntityElement);
			return true;
		}

		private static void CreateDynamicCommand(DynamicCommandElement dynamicCommand, TransactTableCollection result)
		{
			if (result.WithBuilder.Length > 0)
			{
				string[] withClauses = result.WithText.Split(new string[] { "),", ")\n," }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string withClause in withClauses)
				{
					string[] clauseArray = withClause.Split(new string[] { ") AS (", ")\n AS (", ")\n AS(", ") AS \n(", ") AS \n (" }, StringSplitOptions.RemoveEmptyEntries);
					if (clauseArray.Length == 2)
					{
						Basic.Designer.WithClause clause = new Designer.WithClause(dynamicCommand);
						clause.TableName = clauseArray[0].Substring(0, clauseArray[0].IndexOf("(", 0)).Trim();
						clause.TableDefinition = clauseArray[0].Replace(clause.TableName, "").Trim().TrimStart('(');
						clause.TableQuery = clauseArray[1].Trim().TrimEnd(')');
						dynamicCommand.WithClauses.Add(clause);
					}
				}
			}
			dynamicCommand.SelectText = result.SelectText.Trim();
			dynamicCommand.FromText = result.FromText.Trim();
			dynamicCommand.WhereText = result.WhereText.Trim();
			dynamicCommand.GroupText = result.GroupText.Trim();
			dynamicCommand.HavingText = result.HavingText.Trim();
			dynamicCommand.OrderText = result.OrderText.Trim();
			result.CreateParameters(dynamicCommand);
		}
	}
}
