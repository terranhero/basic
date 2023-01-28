namespace Basic.Interfaces
{
	/// <summary>表示表结构定义</summary>
	public interface ITableDefinition
	{
		/// <summary>表所有者</summary>
		string Owner { get; }

		/// <summary>表名称</summary>
		string TableName { get; }
	}
}
