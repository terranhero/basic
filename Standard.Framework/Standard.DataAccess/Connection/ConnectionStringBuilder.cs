using System.Collections.Generic;
using System.Data.Common;
using Basic.Enums;

namespace Basic.Configuration
{
	/// <summary>
	/// 数据库连接字符串构建器
	/// </summary>
	internal static class ConnectionStringBuilder
	{
		/// <summary>获取与此 ConnectionConfig 关联的显示字符串。</summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <returns>返回与此 ConnectionConfig 关联的连接字符串。</returns>
		public static string CreateDisplayString(ConnectionElement element)
		{
			if (element.ConnectionType == ConnectionType.SqlConnection)
				return CreateSqlConnectionString(element, true);
			else if (element.ConnectionType == ConnectionType.OracleConnection)
				return CreateOracleConnectionString(element, true);
			else if (element.ConnectionType == ConnectionType.MySqlConnection)
				return CreateMySqlConnectionString(element, true);
			else if (element.ConnectionType == ConnectionType.OleDbConnection)
				return CreateOracleConnectionString(element, true);
			else if (element.ConnectionType == ConnectionType.Db2Connection)
				return CreateDb2ConnectionString(element, true);
			else if (element.ConnectionType == ConnectionType.SQLiteConnection)
				return CreateDbConnectionString(element, true);
			return CreateDbConnectionString(element, true);
		}

		/// <summary>
		/// 获取与此 ConnectionConfig 关联的连接字符串。
		/// </summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <returns>返回与此 ConnectionConfig 关联的连接字符串。</returns>
		public static string CreateConnectionString(ConnectionElement element)
		{
			if (element.ConnectionType == ConnectionType.SqlConnection)
				return CreateSqlConnectionString(element, false);
			else if (element.ConnectionType == ConnectionType.OracleConnection)
				return CreateOracleConnectionString(element, false);
			else if (element.ConnectionType == ConnectionType.MySqlConnection)
				return CreateMySqlConnectionString(element, false);
			else if (element.ConnectionType == ConnectionType.OleDbConnection)
				return CreateOracleConnectionString(element, false);
			else if (element.ConnectionType == ConnectionType.Db2Connection)
				return CreateDb2ConnectionString(element, false);
			else if (element.ConnectionType == ConnectionType.SQLiteConnection)
				return CreateDbConnectionString(element, false);
			return CreateDbConnectionString(element, false);
		}

		private static readonly List<string> dataSourceKeys = new List<string>(new string[] {
			"DATASOURCE", "DATA SOURCE", "SERVER", "ADDRESS", "ADDR", "NETWORK ADDRESS" });
		private static readonly List<string> userKeys = new List<string>(new string[] { "USERID", "USER ID", "USER", "UID" });
		private static readonly List<string> pwdKeys = new List<string>(new string[] { "PASSWORD", "PWD" });

		/// <summary>
		/// 获取与此 ConnectionConfig 关联的连接字符串。
		/// </summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <param name="isDisplay">是否创建连接字符串显示文本</param>
		/// <returns>返回与此 ConnectionConfig 关联的连接字符串。</returns>
		private static string CreateSqlConnectionString(ConnectionElement element, bool isDisplay = false)
		{
			DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
			//builder.IntegratedSecurity = false;
			foreach (ConnectionItem item in element.Values)
			{
				if (string.IsNullOrEmpty(item.Name)) { continue; }
				else if (string.IsNullOrEmpty(item.Value)) { continue; }
				string upperName = item.Name.ToUpper();

				if (dataSourceKeys.Contains(upperName)) { builder["Data Source"] = item.Value; }
				else if (userKeys.Contains(upperName)) { if (isDisplay == false) { builder["User ID"] = item.Value; } }
				else if (pwdKeys.Contains(upperName)) { if (isDisplay == false) { builder["Password"] = ConfigurationAlgorithm.Decryption(item.Value); } }
				else { builder[item.Name] = item.Value; }
			}
			return builder.ConnectionString;
		}

		/// <summary>
		/// 获取与此 ConnectionConfig 关联的连接字符串。
		/// </summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <param name="isDisplay">是否创建连接字符串显示文本</param>
		/// <returns>返回与此 ConnectionConfig 关联的连接字符串。</returns>
		private static string CreateDbConnectionString(ConnectionElement element, bool isDisplay = false)
		{
			DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
			foreach (ConnectionItem item in element.Values)
			{
				if (string.IsNullOrEmpty(item.Name)) { continue; }
				else if (string.IsNullOrEmpty(item.Value)) { continue; }
				string upperName = item.Name.ToUpper();

				if (dataSourceKeys.Contains(upperName)) { builder["Data Source"] = item.Value; }
				else if (userKeys.Contains(upperName)) { if (isDisplay == false) { builder["User ID"] = item.Value; } }
				else if (pwdKeys.Contains(upperName)) { if (isDisplay == false) { builder["Password"] = ConfigurationAlgorithm.Decryption(item.Value); } }
				else { builder[item.Name] = item.Value; }
			}
			return builder.ConnectionString;
		}

		/// <summary>
		/// 获取与此 ConnectionConfig 关联的连接字符串。
		/// </summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <param name="isDisplay">是否创建连接字符串显示文本</param>
		/// <returns>返回与此 ConnectionConfig 关联的连接字符串。</returns>
		private static string CreateDb2ConnectionString(ConnectionElement element, bool isDisplay = false)
		{
			DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
			builder.Clear();
			foreach (ConnectionItem item in element.Values)
			{
				if (string.IsNullOrEmpty(item.Name)) { continue; }
				else if (string.IsNullOrEmpty(item.Value)) { continue; }
				string upperName = item.Name.ToUpper();

				if (userKeys.Contains(upperName)) { if (isDisplay == false) { builder["User ID"] = item.Value; } }
				else if (pwdKeys.Contains(upperName)) { if (isDisplay == false) { builder["Password"] = ConfigurationAlgorithm.Decryption(item.Value); } }
				else { builder[item.Name] = item.Value; }
			}
			return builder.ConnectionString;
		}

		/// <summary>
		/// 获取与此 ConnectionConfig 关联的连接字符串。
		/// </summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <param name="isDisplay">是否创建连接字符串显示文本</param>
		/// <returns>返回与此 ConnectionConfig 关联的连接字符串。</returns>
		private static string CreateOracleConnectionString(ConnectionElement element, bool isDisplay = false)
		{
			DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
			foreach (ConnectionItem item in element.Values)
			{
				if (string.IsNullOrEmpty(item.Name)) { continue; }
				else if (string.IsNullOrEmpty(item.Value)) { continue; }
				string upperName = item.Name.ToUpper();

				if (dataSourceKeys.Contains(upperName)) { builder["Data Source"] = item.Value; }
				else if (userKeys.Contains(upperName)) { if (isDisplay == false) { builder["User ID"] = item.Value; } }
				else if (pwdKeys.Contains(upperName)) { if (isDisplay == false) { builder["Password"] = ConfigurationAlgorithm.Decryption(item.Value); } }
				else { builder[item.Name] = item.Value; }
			}
			return builder.ConnectionString;
		}

		/// <summary>
		/// 获取与此 ConnectionConfig 关联的连接字符串。
		/// </summary>
		/// <param name="element">数据库连接配置信息</param>
		/// <param name="isDisplay">是否创建连接字符串显示文本</param>
		/// <returns>返回与此 ConnectionConfig 关联的连接字符串。</returns>
		private static string CreateMySqlConnectionString(ConnectionElement element, bool isDisplay = false)
		{
			DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
			foreach (ConnectionItem item in element.Values)
			{
				if (string.IsNullOrEmpty(item.Name)) { continue; }
				else if (string.IsNullOrEmpty(item.Value)) { continue; }
				string upperName = item.Name.ToUpper();

				if (dataSourceKeys.Contains(upperName)) { builder["Data Source"] = item.Value; }
				else if (userKeys.Contains(upperName)) { if (isDisplay == false) { builder["User ID"] = item.Value; } }
				else if (pwdKeys.Contains(upperName)) { if (isDisplay == false) { builder["Password"] = ConfigurationAlgorithm.Decryption(item.Value); } } else { builder[item.Name] = item.Value; }
			}
			return builder.ConnectionString;
		}
	}
}
