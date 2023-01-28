using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 根据Table数据源选择符合条件的节点的委托。
	/// </summary>
	/// <typeparam name="T">IPagination类型数据源</typeparam>
	/// <typeparam name="R">模型类型</typeparam>
	/// <param name="source">Table类型数据源</param>
	/// <param name="row">Table中行类型</param>
	/// <returns>返回符合条件的行数组</returns>
	public delegate IEnumerable<R> EntitySelectedEventHandler<T, R>(T source, R row);

	/// <summary>
	/// 返回easyui.tree所需Json格式数据
	/// </summary>
	public class EntityTreeResult<T> : IActionResult where T : class
	{
		/// <summary>
		/// 选择子节点事件，如果选择的顶级节点，则第二个参数为null。
		/// </summary>
		public event TreeNodeSelectedEventHandler<IPagination<T>, T> SelectNodes;

		/// <summary>
		/// 创建节点信息事件
		/// </summary>
		public event TreeNodeEventHandler<T> CreateTreeNode;

		private readonly IPagination<T> source;
		private readonly bool AsyncTree = false;
		/// <summary>
		/// 初始化 TreeResult 类实例
		/// </summary>
		/// <param name="dataSource">树形结构数据源</param>
		public EntityTreeResult(IPagination<T> dataSource) : this(dataSource, false) { }

		/// <summary>
		/// 初始化 TreeResult 类实例
		/// </summary>
		/// <param name="dataSource">树形结构数据源</param>
		/// <param name="asyncTree">是否需要异步加载树形结构数据，默认值是 false。</param>
		public EntityTreeResult(IPagination<T> dataSource, bool asyncTree) { source = dataSource; AsyncTree = asyncTree; }

		/// <summary>
		/// 加载异步树型结构
		/// </summary>
		/// <param name="response"></param>
		private async Task LoadAsyncTreeAsync(HttpResponse response)
		{
			await response.WriteAsync("[");
			int rowIndex = 0;
			foreach (T row in source)
			{
				if (rowIndex > 0)
				{
					await response.WriteAsync(",");
				}
				rowIndex++;
				await response.WriteAsync("{");
				if (CreateTreeNode != null)
				{
					EasyTreeNode node = new EasyTreeNode(null, null);
					CreateTreeNode(row, node);
					node.WriteNodeJson(response);
				}
				await response.WriteAsync("}");
			}
			await response.WriteAsync("]");
		}
		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">用于执行结果的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		public async Task ExecuteResultAsync(ActionContext context)
		{
			HttpResponse response = context.HttpContext.Response;
			response.Clear();
			response.ContentType = "application/json";
			if (source == null || (source != null && source.Count == 0))
			{
				await response.WriteAsync("[]"); return;
			}
			if (AsyncTree) { await LoadAsyncTreeAsync(response); return; }
			if (SelectNodes != null)
			{
				IEnumerable<T> rowArray = SelectNodes(source, null);
				if (rowArray != null)
				{
					await response.WriteAsync("[");
					int rowIndex = 0;
					foreach (T row in rowArray)
					{
						if (rowIndex > 0) { await response.WriteAsync(","); }
						rowIndex++;
						await response.WriteAsync("{");
						if (CreateTreeNode != null)
						{
							EasyTreeNode node = new EasyTreeNode(null, null);
							CreateTreeNode(row, node);
							node.WriteNodeJson(response);
						}
						if (SelectNodes != null)
						{
							IEnumerable<T> rowArray1 = SelectNodes(source, row);
							if (rowArray1 != null && rowArray1.Any())
							{
								await response.WriteAsync(",\"children\":[");
								await WriteChildTreeNodeAsync(response, rowArray1);
								await response.WriteAsync("]");
							}
						}
						await response.WriteAsync("}");
					}
					await response.WriteAsync("]");
				}
			}
		}

		private async Task WriteChildTreeNodeAsync(HttpResponse response, IEnumerable<T> children)
		{
			int rowIndex = 0;
			foreach (T row in children)
			{
				if (rowIndex > 0)
				{
					await response.WriteAsync(",");
				}
				rowIndex++;
				await response.WriteAsync("{");
				if (CreateTreeNode != null)
				{
					EasyTreeNode node = new EasyTreeNode(null, null);
					CreateTreeNode(row, node);
					node.WriteNodeJson(response);
				}
				IEnumerable<T> rowArray1 = SelectNodes(source, row);
				if (rowArray1 != null && rowArray1.Any())
				{
					await response.WriteAsync(",\"children\":[");
					await WriteChildTreeNodeAsync(response, rowArray1);
					await response.WriteAsync("]");
				}
				await response.WriteAsync("}");
			}
		}
	}
}
