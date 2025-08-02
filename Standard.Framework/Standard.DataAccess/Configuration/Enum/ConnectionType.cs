
using System.ComponentModel;
namespace Basic.Enums
{
	/// <summary>
	/// 数据库连接类型
	/// </summary>
	public enum ConnectionType : byte
	{
		/// <summary>
		/// SqlConnection类型数据库连接
		/// </summary>
		SqlConnection = 1,

		/// <summary>
		/// OracleConnection类型数据库连接
		/// </summary>
		OracleConnection = 2,

		/// <summary>
		/// OleDbConnection类型数据库连接
		/// </summary>
		OleDbConnection = 3,

		/// <summary>
		/// OdbcConnection类型数据库连接
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		OdbcConnection = 4,

		/// <summary>
		/// SQLiteConnection类型数据库连接
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		SQLiteConnection = 5,

		/// <summary>
		/// DB2Connection类型数据库连接
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		Db2Connection = 6,

		/// <summary>
		/// MySqlConnection类型数据库连接
		/// </summary>
		MySqlConnection = 7,

		/// <summary>
		/// NpgSqlConnection 类型数据库连接
		/// </summary>
		NpgSqlConnection = 8,

		/// <summary>
		/// SqlConnection 类型数据库连接(指链接SQL SERVER 2012及以后的版本，主要分页查询有差异)
		/// </summary>
		NewSqlConnection = 9,
	}
}
