// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Basic.Caches;
using Basic.Collections;
using Basic.DataAccess;
using Basic.EntityLayer;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace MyApp // Note: actual namespace depends on the project name.
{
	internal class Program
	{


		const string sql1 = @"(EMPKEY,EMPLOYEECODE,CHINESENAME,CORPKEY,CORPNAME,ORGKEY,DEPARTCODE,DEPARTNAME,NODECODE,NODENAME) ";

		const string sql3 = @"WITH TEMP_EMPLOYEE(EMPKEY,  CORPKEY, CORPNAME, ORGKEY, DEPARTCODE, DEPARTNAME, NODECODE, NODENAME,NODECODE1,NODENAME1) AS (
SELECT TE.EMPKEY, TE.COMPANYKEY AS CORPKEY, SO.NODENAME AS CORPNAME, TE.ORGKEY, 
T3.DEPARTCODE, T3.DEPARTNAME, T3.NODECODE, T3.NODENAME,T3.NODECODE1,T3.NODENAME1
FROM PDM_EMPLOYEE AS TE INNER JOIN   SYS_ORGANIZATION AS SO
     ON TE.COMPANYKEY = SO.ORGKEY AND SO.NODETYPE IN (1, 2, 3) INNER JOIN
     SYS_DEPARTMENT AS T3     ON TE.ORGKEY = T3.ORGKEY 
WHERE EXISTS (SELECT 1 FROM SYS_ORGANIZATIONPERMISSION AS OP WHERE OP.ORGKEY = TE.ORGKEY AND OP.POSKEY = TE.POSKEY AND OP.GROUPKEY = @WEBGROUPKEY)
)
SELECT T1.GROUPKEY, TE.CORPKEY, TE.CORPNAME, TE.ORGKEY, TE.DEPARTCODE, TE.DEPARTNAME, TE.NODECODE, TE.NODENAME, TE.NODECODE1,TE.NODENAME1,
SG.GROUPDATE, SG.PAYDATE
FROM PSM_SALARYCALCRESULT AS T1 INNER JOIN PSM_SALARYGROUP AS SG ON T1.GROUPKEY = SG.GROUPKEY
 INNER JOIN TEMP_EMPLOYEE AS TE ON T1.EMPKEY = TE.EMPKEY
 GROUP BY T1.GROUPKEY, TE.CORPKEY, TE.CORPNAME, TE.ORGKEY, TE.DEPARTCODE, TE.DEPARTNAME, TE.NODECODE, TE.NODENAME, TE.NODECODE1,TE.NODENAME1,
SG.GROUPDATE, SG.PAYDATE";
		const string sql2 = @"WITH TEMP_MENU(MENUKEY,MENUNAME,ENUSTEXT,MENUTEXT) AS (
SELECT T1.MENUKEY,T1.MENUTEXT,T1.ENUSTEXT,T1.MENUTEXT FROM SYS_MENU T1 WHERE T1.SUPERKEY IS NULL UNION
SELECT T1.MENUKEY,T1.MENUTEXT,T1.ENUSTEXT,T1.MENUTEXT FROM SYS_MENU T1 WHERE T1.SUPERKEY IS NULL UNION ALL
SELECT T1.MENUKEY,T1.MENUTEXT,T1.ENUSTEXT,CAST(T2.MENUTEXT+N'/'+ T1.MENUTEXT AS NVARCHAR(100))
FROM SYS_MENU T1 JOIN TEMP_MENU T2 ON T1.SUPERKEY=T2.MENUKEY
WHERE 1=1)


SELECT T1.REPORTKEY, T1.SUMMARYTYPE, T1.REPORTNAME, T1.REPORTFILE, T1.REPORTALIAS, T1.DESIGNCONTENT, T1.REPORTCONTENT, T1.MENUKEY, T2.MENUTEXT, T1.ENABLED, T1.USERNAME, T1.CREATEDTIME, T1.MODIFIEDTIME, T1.DESCRIPTION
FROM PSM_SUMMARYREPORT AS T1 LEFT JOIN TEMP_MENU AS T2 ON T1.MENUKEY = T2.MENUKEY
ORDER BY T1.SUMMARYTYPE, T1.REPORTNAME OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";
		const string sql = @"DECLARE @WEBGROUPKEY uniqueidentifier
SET @WEBGROUPKEY='13902C38-9CC5-4956-A48D-9B698D8170F5';

WITH TEMP_EMPLOYEE AS (
SELECT TE.EMPKEY,TE.EMPLOYEECODE,TE.CHINESENAME,TE.COMPANYKEY,SO.NODENAME,TE.ORGKEY,T3.DEPARTCODE,T3.DEPARTNAME,T3.NODECODE,T3.NODENAME
FROM PDM_EMPLOYEE TE JOIN SYS_ORGANIZATION SO ON TE.COMPANYKEY=SO.ORGKEY AND SO.NODETYPE in(1,2,3)
JOIN SYS_DEPARTMENT T3 ON TE.ORGKEY=T3.ORGKEY
WHERE  EXISTS(SELECT 1 FROM SYS_ORGANIZATIONPERMISSION OP WHERE OP.ORGKEY=TE.ORGKEY AND OP.POSKEY=TE.POSKEY AND OP.GROUPKEY=@WEBGROUPKEY)
GROUP BY TE.EMPKEY,TE.EMPLOYEECODE
HAVING (COUNT(TE.EMPKEY)>0))

SELECT NEWID() GUID,dbo.SYSF_ARRAYJOIN(T1.EMPKEY, '/') TEXT,T1.GROUPKEY,T1.EMPKEY,EMPLOYEECODE,TE.CHINESENAME,TE.CORPKEY,TE.CORPNAME,TE.ORGKEY,TE.DEPARTCODE,TE.DEPARTNAME,TE.NODECODE,TE.NODENAME,SG.GROUPDATE,SG.PAYDATE
,T1.NOPAYING,T1.LOCKING,T1.PAIDDATE,T1.FORMNOTE,T1.*
FROM PSM_SALARYCALCRESULT T1 JOIN PSM_SALARYGROUP SG ON T1.GROUPKEY=SG.GROUPKEY
JOIN TEMP_EMPLOYEE TE ON T1.EMPKEY=TE.EMPKEY
ORDER BY T1.EMPKEY,T1.EMPLOYEECODE
";
		/// <summary>缓存内连接命令</summary>
		private static readonly ConcurrentDictionary<string, DynamicJoinCommand> _innerJoins = new ConcurrentDictionary<string, DynamicJoinCommand>();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0090:使用 \"new(...)\"", Justification = "<挂起>")]
		static async Task Main(string[] args)
		{
			CacheClientFactory.SetClientFactory(new MemoryClientFactory());
			Console.WriteLine(OrderedGuidGenerator.NewGuid("SYS_EVENTLOGGER", DateTime.Now));
			Console.WriteLine(OrderedGuidGenerator.NewGuid("SYS_EVENTLOGGER", DateTime.Today));
			Console.WriteLine(OrderedGuidGenerator.NewGuid("SYS_EVENTLOGGER"));
			Console.WriteLine(OrderedGuidGenerator.NewGuid("SYS_EVENTLOGGER"));

			Console.WriteLine(OrderedGuidGenerator.NewGuid("SYS_EVENTLOGGES"));
			Console.WriteLine(OrderedGuidGenerator.NewGuid("SYS_EVENTLOGGES"));

			Console.WriteLine(OrderedGuidGenerator.NewGuid("SYS_EVENTLOG"));
			Console.WriteLine(OrderedGuidGenerator.NewGuid("SYS_EVENTLOG"));

			Console.WriteLine(OrderedGuidGenerator.NewGuid("EAM_LEAVERECORD"));
			Console.WriteLine(OrderedGuidGenerator.NewGuid("EAM_LEAVERECORD"));

			Console.WriteLine(OrderedGuidGenerator.NewGuid("EAM_OVERTIMEFORM"));
			Console.WriteLine(OrderedGuidGenerator.NewGuid("EAM_OVERTIMEFORM"));

			ICacheClient client = CacheClientFactory.GetClient("");
			List<int> list = new List<int>() { 2, 4, 6, 7 };
			await client.SetListAsync("test", list);
			IEnumerable<KeyInfo> keys = client.GetKeyInfosAsync();
			//Type type = typeof(ConsecutiveWorkEntity);
			//if (_innerJoins.TryGetValue(type.FullName, out DynamicJoinCommand cmd))
			//{
			// return ;
			//}
			//List<InnerJoinAttribute> joins = new List<InnerJoinAttribute>(10);
			//List<JoinParameterAttribute> parameters = new List<JoinParameterAttribute>(10);
			//List<JoinOrderAttribute> orders = new List<JoinOrderAttribute>(10);
			//for (Type et = type; et != null; et = et.BaseType)
			//{
			//	foreach (var attribute in et.GetCustomAttributes(true))
			//	{
			//		if (attribute is InnerJoinAttribute) { joins.Add((InnerJoinAttribute)attribute); }
			//		else if (attribute is JoinParameterAttribute) { parameters.Add((JoinParameterAttribute)attribute); }
			//		else if (attribute is JoinOrderAttribute) { orders.Add((JoinOrderAttribute)attribute); }
			//	}
			//}
			//EntityPropertyProvidor.TryGetProperties(type, out EntityPropertyCollection properties);
			//List<string> fields = new List<string>(50);
			//foreach (EntityPropertyMeta meta in properties)
			//{
			//	if (meta.JoinField == null) { continue; }
			//	fields.Add(meta.JoinField.Script);
			//}
			//List<string> whereClauses = new List<string>(10);
			//List<DbParameter> dbParameters = new List<DbParameter>(10);
			//foreach (JoinParameterAttribute param in parameters)
			//{
			//	DbParameter parameter = CreateParameter(param);
			//	dbParameters.Add(parameter);
			//	whereClauses.Add(param.WhereClause.Replace("{%" + param.FieldName + "%}", parameter.ParameterName));
			//}
			//if (fields.Count == 0 || joins.Count == 0) { return false; }
			//_dynamicJoinCommand = new DynamicJoinCommand(string.Join(", ", fields),
			//	string.Join("\r\n", joins.Select(m => m.JoinScript)),
			//	string.Join(", ", whereClauses),
			//	 string.Join(", ", orders.SelectMany(m => m.OrderClauses)),
			//	dbParameters.ToArray()
			//	);
			////attributes.
			//TSqlParser parser = new TSql120Parser(false);
			//using (StringReader reader = new StringReader(sql3))
			//{
			//	StatementList statementList = parser.ParseStatementList(reader, out IList<ParseError> errors);
			//	if (errors != null && errors.Count > 0)
			//	{
			//		foreach (ParseError error in errors)
			//		{
			//			Console.WriteLine("Line:{0}, Column:{1}, Message:{2}", error.Line, error.Column, error.Message);
			//		}
			//		Console.ReadKey();
			//		return;
			//	}
			//	var tokens = statementList.ScriptTokenStream.Where(m => m.TokenType == TSqlTokenType.Variable);
			//	foreach (var token in tokens)
			//	{
			//		Console.WriteLine("Text :{0}, TokenType:{1}", token.Text, token.TokenType);
			//	}

			//	//Console.WriteLine(GenerateScript(statementList));
			//	//Console.WriteLine("===========================================================");
			//	foreach (TSqlStatement statement in statementList.Statements)
			//	{
			//		if (statement is not SelectStatement) { continue; }
			//		SelectStatement selectStatement = (SelectStatement)statement;
			//		//Console.WriteLine(GenerateScript(selectStatement));
			//		//Console.WriteLine("===========================================================");

			//		IList<CommonTableExpression> tableExpressions = selectStatement.WithCtesAndXmlNamespaces.CommonTableExpressions;
			//		foreach (CommonTableExpression withClause in tableExpressions)
			//		{
			//			//Console.Write("WITH ");
			//			//Console.Write(withClause.ExpressionName.Value);
			//			//Console.Write("(");
			//			//Console.WriteLine(GenerateScript(withClause.QueryExpression));
			//			//Console.WriteLine(")");
			//			if (withClause.QueryExpression is QuerySpecification withQuery)
			//			{
			//				Console.WriteLine(GenerateScript(withQuery));
			//				if (withQuery.WhereClause != null) { Console.WriteLine(GenerateScript(withQuery.WhereClause)); }
			//				if (withQuery.OrderByClause != null) { Console.WriteLine(GenerateScript(withQuery.OrderByClause)); }
			//				if (withQuery.GroupByClause != null) { Console.WriteLine(GenerateScript(withQuery.GroupByClause)); }
			//				if (withQuery.HavingClause != null) { Console.WriteLine(GenerateScript(withQuery.HavingClause)); }
			//			}
			//			else if (withClause.QueryExpression is BinaryQueryExpression query2)
			//			{
			//				StringBuilder stringBuilder = new StringBuilder(1000);
			//				//Console.WriteLine(GenerateScript(withClause));
			//				GenerateScript(query2, stringBuilder);
			//				Console.WriteLine(stringBuilder.ToString());
			//				Console.WriteLine("============================================================");
			//				//Console.WriteLine(GenerateScript(query2));
			//				//Console.WriteLine("===========================================================");

			//				//Console.WriteLine(GenerateScript(query2.FirstQueryExpression));
			//				//Console.WriteLine("===========================2================================");
			//				//Console.WriteLine(GenerateScript(query2.SecondQueryExpression));

			//				if (query2.OrderByClause != null) { Console.WriteLine(GenerateScript(query2.OrderByClause)); }
			//				if (query2.OffsetClause != null) { Console.WriteLine(GenerateScript(query2.OffsetClause)); }
			//			}
			//		}
			//		if (selectStatement.QueryExpression is QuerySpecification select)
			//		{
			//			//Console.WriteLine(GenerateScript(select));
			//			if (select.SelectElements != null)
			//			{
			//				foreach (SelectElement item in select.SelectElements)
			//				{
			//					Console.Write(GenerateScript(item)); Console.Write(", ");
			//				}
			//				Console.WriteLine();
			//			}
			//			if (select.FromClause != null)
			//			{
			//				foreach (TableReference item in select.FromClause.TableReferences)
			//				{
			//					Console.Write(GenerateScript(item)); Console.Write(", ");
			//				}
			//				Console.WriteLine();
			//				//Console.WriteLine(GenerateScript(select.FromClause));
			//			}
			//			if (select.WhereClause != null) { Console.WriteLine(GenerateScript(select.WhereClause)); }
			//			if (select.GroupByClause != null) { Console.WriteLine(GenerateScript(select.GroupByClause)); }
			//			if (select.HavingClause != null) { Console.WriteLine(GenerateScript(select.HavingClause)); }
			//			if (select.OrderByClause != null) { Console.WriteLine(GenerateScript(select.OrderByClause)); }
			//			if (select.OffsetClause != null) { Console.WriteLine(GenerateScript(select.OffsetClause)); }
			//		}
			//	}
			//}

			//Console.WriteLine("end");


			//Console.ReadKey();
		}
		static void GenerateScript(BinaryQueryExpression query2, StringBuilder builder)
		{

			SqlScriptGeneratorOptions options = new SqlScriptGeneratorOptions();
			options.MultilineSelectElementsList = false;
			options.MultilineWherePredicatesList = false;
			options.NewLineBeforeFromClause = true;
			options.NewLineBeforeWhereClause = true;
			options.AlignClauseBodies = true;
			options.AsKeywordOnOwnLine = true;
			options.NewLineBeforeJoinClause = false;
			options.NewLineBeforeOutputClause = false;
			options.NewLineBeforeOpenParenthesisInMultilineList = true;
			options.NewLineBeforeCloseParenthesisInMultilineList = true;
			options.KeywordCasing = KeywordCasing.Uppercase;
			options.SqlVersion = SqlVersion.Sql120;
			SqlScriptGenerator script = new Sql120ScriptGenerator(options);
			if (query2.FirstQueryExpression is BinaryQueryExpression expression)
			{
				GenerateScript(expression, builder);
			}
			else
			{
				script.GenerateScript(query2.FirstQueryExpression, out string scriptString1);
				builder.AppendLine(scriptString1);
			}
			if (query2.BinaryQueryExpressionType == BinaryQueryExpressionType.Union)
			{
				builder.AppendLine(query2.All ? "UNION ALL" : "UNION");
			}
			if (query2.SecondQueryExpression is BinaryQueryExpression expression2)
			{
				GenerateScript(expression2, builder);
			}
			else
			{
				script.GenerateScript(query2.SecondQueryExpression, out string scriptString2);
				builder.AppendLine(scriptString2);
			}

		}

		static string GenerateScript(TSqlFragment query)
		{
			SqlScriptGeneratorOptions options = new SqlScriptGeneratorOptions();
			options.MultilineSelectElementsList = false;
			options.MultilineWherePredicatesList = false;
			options.NewLineBeforeFromClause = true;
			options.NewLineBeforeWhereClause = true;
			options.AlignClauseBodies = true;
			options.AsKeywordOnOwnLine = true;
			options.NewLineBeforeJoinClause = false;
			options.NewLineBeforeOutputClause = false;
			options.NewLineBeforeOpenParenthesisInMultilineList = true;
			options.NewLineBeforeCloseParenthesisInMultilineList = true;
			options.KeywordCasing = KeywordCasing.Uppercase;
			options.SqlVersion = SqlVersion.Sql120;
			SqlScriptGenerator script = new Sql120ScriptGenerator(options);
			script.GenerateScript(query, out string scriptString);
			return scriptString;
		}
		static string ParseQuery(QuerySpecification query)
		{
			List<string> selects = new List<string>(100);
			StringBuilder temp = new StringBuilder(100);
			foreach (SelectElement item in query.SelectElements)
			{
				if (item != null && item is SelectScalarExpression scalar)
				{
					temp.Clear();
					temp.AppendSelect(scalar);
					selects.Add(temp.ToString());
				}
				else if (item != null && item is SelectStarExpression star)
				{
					selects.Add(string.Join(".", star.Qualifier.Identifiers.Select(m => m.Value)) + ".*");
				}
				else if (item != null && item is SelectSetVariable variable)
				{
					//selects.Add(variable.ToString());
				}
			}
			if (query.FromClause != null)
			{
				foreach (TableReference clause in query.FromClause.TableReferences)
				{

				}
			}

			return string.Join(", ", selects);
		}
	}

	public static class StringBuilderExtension
	{

		/// <summary></summary>
		/// <param name="builder"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static StringBuilder AppendSelect(this StringBuilder builder, SelectScalarExpression scalar)
		{
			if (scalar.Expression is FunctionCall call)
			{
				if (call.CallTarget is ExpressionCallTarget target1)
				{
				}
				else if (call.CallTarget is MultiPartIdentifierCallTarget target2)
				{
					builder.Identifiers(target2.MultiPartIdentifier).Append(".");
				}
				else if (call.CallTarget is UserDefinedTypeCallTarget target3)
				{
					//call.CallTarget.
				}
				builder.Append(call.FunctionName.Value).Append("(");
				if (call.Parameters.Count > 0)
				{
					builder.Append(string.Join(", ", call.Parameters.Select(se =>
					{
						if (se is ColumnReferenceExpression column)
						{
							return column.MultiPartIdentifier.Identifiers();
						}
						else if (se is StringLiteral literal1)
						{
							return "'" + literal1.Value + "'";
						}
						else if (se is Literal literal2)
						{
							return literal2.Value;
						}
						return "";
					})));
				}
				builder.Append(")");

			}
			else if (scalar.Expression is ColumnReferenceExpression column)
			{
				builder.Identifiers(column.MultiPartIdentifier);
			}
			else if (scalar.Expression is CastCall cast)
			{

			}

			if (scalar.ColumnName != null)
			{
				builder.Append(" AS ").Append(scalar.ColumnName.Value);
			}
			return builder;
		}
		/// <summary></summary>
		/// <param name="builder"></param>
		/// <param name="scalar"></param>
		/// <returns></returns>
		public static StringBuilder AppendFrom(this StringBuilder builder, SelectScalarExpression scalar)
		{
			if (scalar.Expression is FunctionCall call)
			{
				if (call.CallTarget is ExpressionCallTarget target1)
				{
				}
				else if (call.CallTarget is MultiPartIdentifierCallTarget target2)
				{
					builder.Identifiers(target2.MultiPartIdentifier).Append(".");
				}
				else if (call.CallTarget is UserDefinedTypeCallTarget target3)
				{
					//call.CallTarget.
				}
				builder.Append(call.FunctionName.Value).Append("(");
				if (call.Parameters.Count > 0)
				{
					builder.Append(string.Join(", ", call.Parameters.Select(se =>
					{
						if (se is ColumnReferenceExpression column)
						{
							return column.MultiPartIdentifier.Identifiers();
						}
						else if (se is StringLiteral literal1)
						{
							return "'" + literal1.Value + "'";
						}
						else if (se is Literal literal2)
						{
							return literal2.Value;
						}
						return "";
					})));
				}
				builder.Append(")");

			}
			else if (scalar.Expression is ColumnReferenceExpression column)
			{
				builder.Identifiers(column.MultiPartIdentifier);
			}
			else if (scalar.Expression is CastCall cast)
			{

			}

			if (scalar.ColumnName != null)
			{
				builder.Append(" AS ").Append(scalar.ColumnName.Value);
			}
			return builder;
		}

		/// <summary></summary>
		/// <param name="builder"></param>
		/// <param name="identifiers"></param>
		/// <returns></returns>
		public static StringBuilder Identifiers(this StringBuilder builder, MultiPartIdentifier identifiers)
		{
			return builder.Append(string.Join(".", identifiers.Identifiers.Select(m => m.Value)));
		}

		/// <summary></summary>
		/// <param name="builder"></param>
		/// <param name="identifiers"></param>
		/// <returns></returns>
		public static string Identifiers(this MultiPartIdentifier identifiers)
		{
			return string.Join(".", identifiers.Identifiers.Select(m => m.Value));
		}
	}
}