using System.Threading.Tasks;
using Basic.EntityLayer;
using Basic.Interfaces;
using Basic.MvcLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
	public class EntityComboResult<T> : IActionResult where T : AbstractEntity
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
		public Task ExecuteResultAsync(ActionContext context)
		{
			HttpResponse response = context.HttpContext.Response;
			response.Clear();
			response.ContentType = "application/json";
			if (source == null || (source != null && source.Count == 0))
			{
				return response.WriteAsync("[]");
			}
			response.WriteAsync("[");
			int rowIndex = 0;
			if (CreateSelectItem != null)
			{
				foreach (T entity in source)
				{
					if (rowIndex > 0)
						response.WriteAsync(",");
					else
						rowIndex++;
					response.WriteAsync("{");
					EasyComboItem item = new EasyComboItem();
					CreateSelectItem(entity, item);
					item.WriteJson(response);
					response.WriteAsync("}");
				}
			}
			return response.WriteAsync("]");
		}
	}

	/// <summary>
	/// 表示 easyui 类的实例中的选定项。
	/// </summary>
	public sealed class EasyComboItem
	{
		/// <summary>
		/// 初始化 EasyComboItem 类的新实例。
		/// </summary>
		public EasyComboItem() { }

		/// <summary>
		/// 初始化 EasyComboItem 类的新实例。
		/// </summary>
		/// <param name="itemValue">选定项的值</param>
		/// <param name="itemText">定项的文本</param>
		public EasyComboItem(object itemValue, string itemText) : this(itemValue, itemText, false) { }

		/// <summary>
		/// 初始化 EasyComboItem 类的新实例。
		/// </summary>
		/// <param name="itemValue">选定项的值</param>
		/// <param name="itemText">定项的文本</param>
		/// <param name="isSelected">是否选择此项。</param>
		public EasyComboItem(object itemValue, string itemText, bool isSelected) { Value = itemValue; Text = itemText; Selected = isSelected; }

		/// <summary>
		/// 获取或设置一个值，该值指示是否选择此 EasyComboItem。
		/// </summary>
		/// <value>如果选定此项，则为 true；否则为 false。</value>
		public bool Selected { get; set; }

		/// <summary>
		/// 获取或设置选定项的文本。
		/// </summary>
		/// <value>文本。</value>
		public string Attrs { get; set; }

		/// <summary>
		/// 获取或设置选定项的文本。
		/// </summary>
		/// <value>文本。</value>
		public string Text { get; set; }

		/// <summary>
		/// 获取或设置选定项的值。
		/// </summary>
		/// <value>值。</value>
		public object Value { get; set; }

		/// <summary>
		/// 获取节点的Json数据格式表示形式
		/// </summary>
		/// <returns></returns>
		public void WriteJson(HttpResponse response)
		{
			response.WriteAsync("\"value\":"); response.WriteAsync(JsonSerializer.SerializeObject(Value));
			response.WriteAsync(",\"text\":"); response.WriteAsync(JsonSerializer.SerializeObject(Text));
			//response.Write(string.Format("\"value\":{0}", ));
			//response.Write(string.Format(",\"text\":{0}", JsonSerializer.SerializeObject(Text)));
			if (Selected) { response.WriteAsync(",\"selected\":\"true\""); }
			//if (!string.IsNullOrEmpty(Attrs))
			//    response.Write(string.Format(",\"attrs\":\"{0}\"", Attrs));
		}
	}
}
