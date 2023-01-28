using System.Web;
using System.Web.Mvc;
using Basic.EntityLayer;
using Basic.Interfaces;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 创建节点委托
	/// </summary>
	/// <typeparam name="T">行类型信息</typeparam>
	/// <param name="entity">节点数据源信息</param>
	/// <param name="item">节点信息</param>
	public delegate void EasyComboEventHandler<T>(T entity, EasyComboItem item);

	/// <summary>
	/// 获取easyui下拉框数据源结果
	/// </summary>
	/// <typeparam name="T">继承与DataEntity基类的实体</typeparam>
	public class EntityComboResult<T> : ActionResult where T : AbstractEntity
	{
		private readonly IPagination<T> source;
		/// <summary>
		/// 创建节点信息事件
		/// </summary>
		public event EasyComboEventHandler<T> CreateSelectItem;

		/// <summary>
		/// 初始化 EntityComboResult 类实例
		/// </summary>
		/// <param name="dataSource">树形结构数据源</param>
		public EntityComboResult(IPagination<T> dataSource) { source = dataSource; }

		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">用于执行结果的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		public override void ExecuteResult(ControllerContext context)
		{
			HttpResponseBase response = context.HttpContext.Response;
			response.Clear();
			response.ContentType = "application/json";
			if (source == null || (source != null && source.Count == 0))
			{
				response.Write("[]");
				return;
			}
			response.Write("[");
			int rowIndex = 0;
			if (CreateSelectItem != null)
			{
				foreach (T entity in source)
				{
					if (rowIndex > 0)
						response.Write(",");
					else
						rowIndex++;
					response.Write("{");
					EasyComboItem item = new EasyComboItem();
					CreateSelectItem(entity, item);
					item.WriteJson(response);
					response.Write("}");
				}
			}
			response.Write("]");
		}
	}
}
