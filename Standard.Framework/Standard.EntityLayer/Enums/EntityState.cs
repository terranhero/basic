
namespace Basic.Enums
{
	/// <summary>
	/// 表示 AbstractEntity 对象的状态。
	/// </summary>
	public enum EntityState
	{
		/// <summary>
		/// 此实体类自创建以来尚未更改。  
		/// </summary>
		Default,

		/// <summary>
		/// 此实体类已经创建成功，已经调用 SetAdded 方法。 
		/// </summary>
		Added,

		/// <summary>
		/// 此实体类已经修改数据，已经调用 SetModified 方法。 
		/// </summary>
		Modified,

		/// <summary>
		/// 此实体类为删除状态，已经调用 SetDeleted 方法。 
		/// </summary>
		Deleted
	}
}
