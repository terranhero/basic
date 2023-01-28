using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 根据Table数据源选择符合条件的节点的委托。
	/// </summary>
	/// <typeparam name="T">Table类型数据源</typeparam>
	/// <typeparam name="R">Table中行类型</typeparam>
	/// <param name="source">Table类型数据源</param>
	/// <param name="row">Table中行类型</param>
	/// <returns>返回符合条件的行数组</returns>
	public delegate IEnumerable<R> TreeNodeSelectedEventHandler<T, R>(T source, R row);

	/// <summary>
	/// 创建节点委托
	/// </summary>
	/// <typeparam name="R">行类型信息</typeparam>
	/// <param name="row">节点数据源信息</param>
	/// <param name="node">节点信息</param>
	public delegate void TreeNodeEventHandler<R>(R row, EasyTreeNode node);

	/// <summary>
	/// 返回easyui.tree所需Json格式数据
	/// </summary>
	public class TableTreeResult<TT, TR> : IActionResult
		where TR : System.Data.DataRow
		where TT : System.Data.DataTable
	{
		/// <summary>
		/// 选择子节点事件，如果选择的顶级节点，则第二个参数为null。
		/// </summary>
		public event TreeNodeSelectedEventHandler<TT, TR> SelectNodes;

		/// <summary>
		/// 创建节点信息事件
		/// </summary>
		public event TreeNodeEventHandler<TR> CreateTreeNode;
		private readonly TT source;
		private readonly string ValueField;
		private readonly string TextField;
		/// <summary>
		/// 初始化 TreeResult 类实例
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="textField"></param>
		/// <param name="valueField"></param>
		public TableTreeResult(TT dataSource, string valueField, string textField)
		{
			source = dataSource;
			ValueField = valueField;
			TextField = textField;
		}

		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">用于执行结果的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		public async Task ExecuteResultAsync(ActionContext context)
		{
			if (source == null)
			{
				return;
			}
			if (SelectNodes != null)
			{
				IEnumerable<TR> rowArray = SelectNodes(source, null);
				if (rowArray != null && rowArray.First() != null)
				{
					HttpResponse response = context.HttpContext.Response;
					response.Clear();
					response.ContentType = "application/json";
					await response.WriteAsync("[");
					int rowIndex = 0;
					foreach (TR row in rowArray)
					{
						if (rowIndex > 0) { await response.WriteAsync(","); }
						rowIndex++;
						await response.WriteAsync("{");
						if (CreateTreeNode != null)
						{
							EasyTreeNode node = new EasyTreeNode(row[ValueField], Convert.ToString(row[TextField]));
							CreateTreeNode(row, node);
							node.WriteNodeJson(response);
						}
						else
						{
							await response.WriteAsync(string.Format("\"id\":{0}", row[ValueField]));
							await response.WriteAsync(string.Format(",\"text\":\"{0}\"", row[TextField]));
							await response.WriteAsync(string.Format(",\"label\":\"{0}\"", row[TextField]));
						}
						if (SelectNodes != null)
						{
							IEnumerable<TR> rowArray1 = SelectNodes(source, null);
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

		private async Task WriteChildTreeNodeAsync(HttpResponse response, IEnumerable<TR> children)
		{
			int rowIndex = 0;
			foreach (TR row in children)
			{
				if (rowIndex > 0)
				{
					await response.WriteAsync(",");
				}
				rowIndex++;
				await response.WriteAsync("{");
				if (CreateTreeNode != null)
				{
					EasyTreeNode node = new EasyTreeNode(row[ValueField], Convert.ToString(row[TextField]));
					CreateTreeNode(row, node);
					node.WriteNodeJson(response);
				}
				else
				{
					await response.WriteAsync(string.Format("\"id\":{0}", row[ValueField]));
					await response.WriteAsync(string.Format(",\"text\":\"{0}\"", row[TextField]));
					await response.WriteAsync(string.Format(",\"label\":\"{0}\"", row[TextField]));
				}
				IEnumerable<TR> rowArray1 = SelectNodes(source, null);
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
