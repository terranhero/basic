using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Basic.Collections;
using Basic.Configuration;
using Basic.Database;
using Basic.DataEntities;
using Basic.Designer;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Microsoft.VisualStudio.Debugger.Interop;
using MSTS = Microsoft.SqlServer.TransactSql.ScriptDom;

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
		private readonly static SqlScriptGeneratorOptions options = new SqlScriptGeneratorOptions();
		private readonly static SqlScriptGenerator generator = new Sql120ScriptGenerator(options);

		static TransactSqlResolver()
		{
			options.MultilineSelectElementsList = false;
			options.MultilineWherePredicatesList = false;
			options.AlignClauseBodies = false;
			options.IncludeSemicolons = false;
			options.AsKeywordOnOwnLine = false;
			options.NewLineBeforeFromClause = true;
			options.NewLineBeforeWhereClause = true;
			options.NewLineBeforeJoinClause = false;
			options.NewLineBeforeOffsetClause = false;
			options.NewLineBeforeOutputClause = false;
			options.NewLineBeforeOpenParenthesisInMultilineList = false;
			options.NewLineBeforeCloseParenthesisInMultilineList = false;
			options.KeywordCasing = KeywordCasing.Uppercase;
			options.SqlVersion = SqlVersion.Sql120;
		}

		/// <summary>
		/// 测试当前传入的 Transact-SQL 是否可以分解(是否符合预期的格式)。
		/// </summary>
		/// <param name="sql">需要测试分解的 Transact-SQL。</param>
		/// <returns>如果测试成功则返回True，否则返回False。</returns>
		public static bool CanPaste(string sql)
		{
			TSqlParser parser = new TSql120Parser(true); IList<ParseError> errors = null;
			IList<TSqlParserToken> tokens = parser.GetTokenStream(new StringReader(sql), out errors);
			if (errors == null || errors.Count == 0) { return tokens.Any(m => m.TokenType == TSqlTokenType.Select); }
			return false;
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
			TransactTableCollection result = new TransactTableCollection(false);
			TSqlParser parser = new TSql120Parser(true);
			using (StringReader reader = new StringReader(sql))
			{
				StatementList statementList = parser.ParseStatementList(reader, out IList<ParseError> errors);
				foreach (TSqlStatement statement in statementList.Statements)
				{
					if (!(statement is SelectStatement)) { continue; }
					SelectStatement selectStatement = (SelectStatement)statement;

					if (selectStatement.WithCtesAndXmlNamespaces != null)
					{
						IList<CommonTableExpression> tableExpressions = selectStatement.WithCtesAndXmlNamespaces.CommonTableExpressions;
						foreach (CommonTableExpression withClause in tableExpressions)
						{
							if (withClause.QueryExpression is QuerySpecification withQuery)
							{
								if (withQuery.FromClause != null)
								{
									foreach (TableReference tableReference in withQuery.FromClause.TableReferences)
									{
										PasteFromClause(result, tableReference);
									}
								}
							}
						}
					}

					if (selectStatement.QueryExpression is QuerySpecification select)
					{
						if (select.FromClause != null)
						{
							foreach (TableReference tableReference in select.FromClause.TableReferences)
							{
								PasteFromClause(result, tableReference);
							}
						}
					}
				}
			}
			if (result.Count > 0) { result.TableName = result.First().Name; }
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
					clauses.Add(clause.ToSql());
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

		private static void GenerateScript(BinaryQueryExpression query, StringBuilder builder)
		{
			if (query.FirstQueryExpression is BinaryQueryExpression expression)
			{
				GenerateScript(expression, builder);
			}
			else
			{
				GenerateScript(query.FirstQueryExpression, builder);
			}
			if (query.BinaryQueryExpressionType == BinaryQueryExpressionType.Union)
			{
				builder.AppendLine(query.All ? " UNION ALL" : " UNION");
			}
			else if (query.BinaryQueryExpressionType == BinaryQueryExpressionType.Except)
			{
				builder.AppendLine(" EXCEPT");
			}
			else if (query.BinaryQueryExpressionType == BinaryQueryExpressionType.Intersect)
			{
				builder.AppendLine(" INTERSECT");
			}
			if (query.SecondQueryExpression is BinaryQueryExpression expression2)
			{
				GenerateScript(expression2, builder);
			}
			else
			{
				GenerateScript(query.SecondQueryExpression, builder);
			}
		}

		private static void GenerateScript(QueryExpression query, StringBuilder builder)
		{
			if (query is QuerySpecification query1)
			{
				generator.GenerateScript(query1, out string script); builder.Append(script.Trim());
				if (query1.WhereClause != null)
				{
					options.NewLineBeforeWhereClause = true;
					GenerateScript(query1.WhereClause, builder, true);
				}
				if (query1.GroupByClause != null)
				{
					options.NewLineBeforeGroupByClause = true;
					GenerateScript(query1.GroupByClause, builder, true);
				}
				if (query1.HavingClause != null)
				{
					options.NewLineBeforeHavingClause = true;
					GenerateScript(query1.HavingClause, builder, true);
				}
				if (query1.OrderByClause != null)
				{
					options.NewLineBeforeOrderByClause = true;
					GenerateScript(query1.OrderByClause, builder, true);
					if (query1.OffsetClause != null)
					{
						options.NewLineBeforeOffsetClause = false; builder.Append(" ");
						GenerateScript(query1.OffsetClause, builder);
					}
				}
			}
			else if (query is BinaryQueryExpression query2)
			{
				GenerateScript(query2, builder);
			}
		}

		private static void GenerateScript(TSqlFragment query, StringBuilder builder, bool beforeNewLine = false)
		{
			if (beforeNewLine) { builder.AppendLine(); }
			generator.GenerateScript(query, out string script); builder.Append(script);
		}

		private static void GenerateScript(TSqlFragment query, out string script)
		{
			generator.GenerateScript(query, out script);
		}

		private static void GenerateScript(FromClause fromClause, out string script)
		{
			StringBuilder builder = new StringBuilder();
			foreach (TableReference tableReference in fromClause.TableReferences)
			{
				GenerateScript(tableReference, builder);
			}
			script = builder.ToString().Trim();
		}

		private static void GenerateScript(TableReference tableReference, StringBuilder builder)
		{
			if (tableReference is NamedTableReference named)
			{
				GenerateScript(named, out string scropt); builder.Append(" ").Append(scropt);
			}
			else if (tableReference is QualifiedJoin join)
			{
				options.AlignClauseBodies = false;
				GenerateScript(join, out string scropt); builder.Append(" ").Append(scropt);
			}
		}
		/// <summary>生成 SELECT 子句代码</summary>
		/// <param name="identifiers"></param>
		private static string GenerateScript(IList<Identifier> identifiers)
		{
			List<string> columns = new List<string>(identifiers.Count + 3);
			foreach (Identifier column in identifiers) { generator.GenerateScript(column, out string script); columns.Add(script); }
			return string.Join(", ", columns);
		}
		/// <summary>生成 SELECT 子句代码</summary>
		/// <param name="selectElements"></param>
		/// <returns></returns>
		private static string GenerateScript(IList<SelectElement> selectElements)
		{
			List<string> columns = new List<string>(selectElements.Count + 3);
			foreach (SelectElement column in selectElements) { generator.GenerateScript(column, out string script); columns.Add(script); }
			return string.Join(", ", columns);
		}

		private static void PasteFromClause(TransactTableCollection result, TableReference tableReference)
		{
			if (tableReference is NamedTableReference named)
			{
				result.AddTable(named.SchemaObject.BaseIdentifier.Value, named.Alias.Value);
			}
			else if (tableReference is QualifiedJoin join)
			{
				//join.QualifiedJoinType== QualifiedJoinType.Inner
				PasteFromClause(result, join.FirstTableReference);
				PasteFromClause(result, join.SecondTableReference);
			}
		}

		/// <summary>
		/// 将当前选中的 DynamicCommand 类型的命令，属性 CommandText 替换为传入的 sql 值。
		/// </summary>
		/// <param name="dynamicCommand">需要修改的 DynamicCommand 类型的命令。</param>
		/// <param name="reader">需要替换的 Transact-SQL 查询语句。</param>
		/// <returns>如果替换成功则返回 True，否则返回 False。</returns>
		public static bool PasteCommand(TransactTableCollection result, DynamicCommandElement dynamicCommand, StringReader reader)
		{
			if (dynamicCommand == null) { return false; }

			StringBuilder stringBuilder = new StringBuilder(1000);
			MSTS.TSqlParser parser = new MSTS.TSql120Parser(true);
			MSTS.StatementList statementList = parser.ParseStatementList(reader, out IList<MSTS.ParseError> errors);
			foreach (TSqlStatement statement in statementList.Statements)
			{
				if (!(statement is SelectStatement)) { continue; }
				SelectStatement selectStatement = (SelectStatement)statement;
				if (selectStatement.WithCtesAndXmlNamespaces != null)
				{
					IList<CommonTableExpression> tableExpressions = selectStatement.WithCtesAndXmlNamespaces.CommonTableExpressions;
					dynamicCommand.WithClauses.Clear();
					foreach (CommonTableExpression withClause in tableExpressions)
					{
						Basic.Designer.WithClause clause = new Designer.WithClause(dynamicCommand);
						clause.TableName = withClause.ExpressionName.Value;
						clause.TableDefinition = GenerateScript(withClause.Columns);
						stringBuilder.Clear();
						GenerateScript(withClause.QueryExpression, stringBuilder);
						clause.TableQuery = stringBuilder.ToString();
						dynamicCommand.WithClauses.Add(clause);
					}
				}
				if (selectStatement.QueryExpression is QuerySpecification select)
				{
					dynamicCommand.SelectText = GenerateScript(select.SelectElements);
					if (select.FromClause != null)
					{
						options.NewLineBeforeWhereClause = false;
						options.NewLineBeforeFromClause = false;
						GenerateScript(select.FromClause, out string fromClause);
						options.NewLineBeforeWhereClause = true;
						options.NewLineBeforeFromClause = true;

						dynamicCommand.FromText = fromClause.Replace("FROM ", "").Trim();
						result.FromBuilder.Append(dynamicCommand.FromText);
						foreach (TableReference tableReference in select.FromClause.TableReferences)
						{
							PasteFromClause(result, tableReference);
						}
						if (result.Count > 0) { result.TableName = result.First().Name; }
					}
					dynamicCommand.WhereText = null;
					dynamicCommand.GroupText = null;
					dynamicCommand.HavingText = null;
					dynamicCommand.OrderText = null;
					if (select.WhereClause != null)
					{
						GenerateScript(select.WhereClause, out string whereClause);
						dynamicCommand.WhereText = whereClause.Replace("WHERE ", "").Trim();
						result.WhereBuilder.Append(dynamicCommand.WhereText);
					}
					if (select.GroupByClause != null)
					{
						GenerateScript(select.GroupByClause, out string fromClause);
						dynamicCommand.GroupText = fromClause.Replace("GROUP BY ", "").Trim();
						result.GroupBuilder.Append(dynamicCommand.GroupText);
					}
					if (select.HavingClause != null)
					{
						GenerateScript(select.HavingClause, out string fromClause);
						dynamicCommand.HavingText = fromClause.Replace("HAVING ", "").Trim();
						result.HavingBuilder.Append(dynamicCommand.HavingText);
					}
					if (select.OrderByClause != null)
					{
						GenerateScript(select.OrderByClause, out string fromClause);
						dynamicCommand.OrderText = fromClause.Replace("ORDER BY ", "").Trim();
						result.OrderBuilder.Append(dynamicCommand.OrderText);
					}
				}
				break;
			}
			return true;
		}

		/// <summary>
		/// 将当前选中的 DynamicCommand 类型的命令，属性 CommandText 替换为传入的 sql 值。
		/// </summary>
		/// <param name="dynamicCommand">需要修改的 DynamicCommand 类型的命令。</param>
		/// <param name="sql">需要替换的 Transact-SQL 查询语句。</param>
		/// <returns>如果替换成功则返回 True，否则返回 False。</returns>
		public static bool PasteDynamicCommand(DynamicCommandElement dynamicCommand, StringReader reader)
		{
			if (dynamicCommand == null) { return false; }
			//TransactTableCollection result = ResolverTransactSql(sql);
			StringBuilder stringBuilder = new StringBuilder(1000);
			MSTS.TSqlParser parser = new MSTS.TSql120Parser(true);
			MSTS.StatementList statementList = parser.ParseStatementList(reader, out IList<MSTS.ParseError> errors);
			foreach (TSqlStatement statement in statementList.Statements)
			{
				if (!(statement is SelectStatement)) { continue; }
				SelectStatement selectStatement = (SelectStatement)statement;

				if (selectStatement.WithCtesAndXmlNamespaces != null)
				{
					dynamicCommand.WithClauses.Clear();
					IList<CommonTableExpression> tableExpressions = selectStatement.WithCtesAndXmlNamespaces.CommonTableExpressions;
					foreach (CommonTableExpression withClause in tableExpressions)
					{
						Basic.Designer.WithClause clause = new Designer.WithClause(dynamicCommand);
						clause.TableName = withClause.ExpressionName.Value;
						clause.TableDefinition = GenerateScript(withClause.Columns);
						stringBuilder.Clear();
						GenerateScript(withClause.QueryExpression, stringBuilder);
						clause.TableQuery = stringBuilder.ToString();
						dynamicCommand.WithClauses.Add(clause);
					}
				}

				if (selectStatement.QueryExpression is QuerySpecification select)
				{
					dynamicCommand.SelectText = GenerateScript(select.SelectElements);
					if (select.FromClause != null)
					{
						options.NewLineBeforeWhereClause = false;
						options.NewLineBeforeFromClause = false;
						options.NewLineBeforeJoinClause = false;
						options.NewLineBeforeOutputClause = false;
						GenerateScript(select.FromClause, out string fromClause);
						options.NewLineBeforeWhereClause = true;
						options.NewLineBeforeFromClause = true;
						dynamicCommand.FromText = fromClause.Replace("FROM ", "").Trim();
					}
					dynamicCommand.WhereText = null;
					dynamicCommand.GroupText = null;
					dynamicCommand.HavingText = null;
					dynamicCommand.OrderText = null;
					if (select.WhereClause != null)
					{
						GenerateScript(select.WhereClause, out string whereClause);
						dynamicCommand.WhereText = whereClause.Replace("WHERE ", "").Trim();
					}
					if (select.GroupByClause != null)
					{
						GenerateScript(select.GroupByClause, out string fromClause);
						dynamicCommand.GroupText = fromClause.Replace("GROUP BY ", "").Trim();
					}
					if (select.HavingClause != null)
					{
						GenerateScript(select.HavingClause, out string fromClause);
						dynamicCommand.HavingText = fromClause.Replace("HAVING ", "").Trim();
					}
					if (select.OrderByClause != null)
					{
						GenerateScript(select.OrderByClause, out string fromClause);
						dynamicCommand.OrderText = fromClause.Replace("ORDER BY ", "").Trim();
					}
				}
				break;
			}

			//CreateDynamicCommand(dynamicCommand, result);
			return true;
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
			TransactTableCollection result = new TransactTableCollection(true);


			persistent.UpdatePropertyMapping(result.PropertyMapping);
			DataEntityElement dataEntityElement = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
			DynamicCommandElement dynamicCommand = new DynamicCommandElement(dataEntityElement);
			using (StringReader reader = new StringReader(sql))
			{
				PasteCommand(result, dynamicCommand, reader);
			}
			dynamicCommand.Name = string.Concat("DynamicCommand_", persistent.DataCommands.Count);
			using (IDataContext context = DataContextFactory.CreateDbAccess())
			{
				context.GetParameters(result);
				context.GetTransactSql(result, sql);
			}
			//CreateDynamicCommand(dynamicCommand, result);
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
