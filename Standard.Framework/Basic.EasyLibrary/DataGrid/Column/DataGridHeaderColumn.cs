using Basic.MvcLibrary;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 表示DataGrid列头信息
	/// </summary>
	public sealed class DataGridHeaderColumn<T> : DataGridColumn<T>
	{
		/// <summary>
		/// 初始化 DataGridHeaderColumn 列信息
		/// </summary>
		/// <param name="context">表示当前HTTP客户端的请求信息。</param>
		internal DataGridHeaderColumn(IBasicContext context) : base(context) { }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override string GetString(T model) { return ""; }

		/// <summary>
		/// 获取当前列实体类的值
		/// </summary>
		/// <param name="model">实体模型</param>
		/// <returns>返回模型的值</returns>
		public override object GetValue(T model)
		{
			return null;
		}
	}
}
