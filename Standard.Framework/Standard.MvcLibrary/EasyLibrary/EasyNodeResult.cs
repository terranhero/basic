using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 根据树节点返回
	/// </summary>
	public sealed class EasyNodeResult : IActionResult
	{
		private readonly IEnumerable<EasyTreeNode> source;
		/// <summary>
		/// 初始化 TreeResult 类实例
		/// </summary>
		/// <param name="dataSource">树形结构数据源</param>
		public EasyNodeResult(IEnumerable<EasyTreeNode> dataSource) { source = dataSource; }

		/// <summary>
		/// 通过从 System.Web.Mvc.ActionResult 类继承的自定义类型，启用对操作方法结果的处理。
		/// </summary>
		/// <param name="context">用于执行结果的上下文。 上下文信息包括控制器、HTTP 内容、请求上下文和路由数据。</param>
		public Task ExecuteResultAsync(ActionContext context)
		{
			HttpResponse response = context.HttpContext.Response;
			response.Clear();
			response.ContentType = "application/json";
			if (source == null || (source != null && source.Count() == 0))
			{
				return response.WriteAsync("[]");
			}
			response.WriteAsync("["); int rowIndex = 0;
			foreach (EasyTreeNode node in source)
			{
				if (rowIndex > 0)
					response.WriteAsync(",");
				else
					rowIndex++;
				response.WriteAsync("{");
				node.WriteNodeJson(response);
				response.WriteAsync("}");
			}
			return response.WriteAsync("]");
		}
	}

	/// <summary>表示 EasyTreeNode 类集合</summary>
	public class EasyTreeNodeCollection : System.Collections.ObjectModel.Collection<EasyTreeNode>
	{
	}

	/// <summary>
	/// EasyUI。Tree控件节点信息
	/// </summary>
	public sealed class EasyTreeNode
	{
		/// <summary>
		/// 
		/// </summary>
		public EasyTreeNode() : this(null, null) { }
		/// <summary>
		/// 获取 TreeNode 对象的集合，它表示 TreeView 控件中根节点。
		/// </summary>
		public EasyTreeNodeCollection Nodes { get; private set; }
		/// <summary>
		/// 初始化EasyTreeNode 类实例
		/// </summary>
		public EasyTreeNode(object value, string text)
		{
			State = true; Checked = false; Value = value; Text = text;
			AllowSelected = true;
			Nodes = new EasyTreeNodeCollection();
		}
		/// <summary>
		///  节点ID
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// : node text to show 
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// 树节点Url属性。
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// 节点样式
		/// </summary>
		public string nodeCls { get; set; }

		/// <summary>
		/// 节点图片属性Css样式类
		/// </summary>
		public string IconCls { get; set; }

		/// <summary>
		/// 节点状态，当前节点是否需要展开，如果需要展开则为True，否则为False。
		/// </summary>
		public bool State { get; set; }

		/// <summary>
		/// 节点内容左侧是否需要显示复选框。
		/// </summary>
		public bool Checked { get; set; }

		/// <summary>
		/// 当前节点是否允许触发SelectedNode事件状态。
		/// </summary>
		public bool AllowSelected { get; set; }

		/// <summary>
		///树节点自定义属性。
		/// </summary>
		public string Attributes { get; set; }

		/// <summary>
		/// 获取节点的Json数据格式表示形式
		/// </summary>
		/// <returns></returns>
		public string GetNodeJson()
		{
			StringBuilder builder = new StringBuilder(100);
			if (Value is string || Value is System.Guid)
				builder.AppendFormat("\"id\":\"{0}\"", Value);
			else
				builder.AppendFormat("\"id\":{0}", Value);
			builder.AppendFormat(",\"text\":\"{0}\"", Text);
			builder.AppendFormat(",\"label\":\"{0}\"", Text);
			if (!string.IsNullOrWhiteSpace(IconCls))
				builder.AppendFormat(",\"iconCls\":\"{0}\"", IconCls);
			if (!string.IsNullOrWhiteSpace(nodeCls))
				builder.AppendFormat(",\"nodeCls\":\"{0}\"", nodeCls);
			if (!string.IsNullOrWhiteSpace(Url))
				builder.AppendFormat(",\"url\":\"{0}\"", Url);
			if (!State)
				builder.Append(",\"state\":\"closed\"");
			if (Checked)
				builder.Append(",\"checked\":true");
			if (AllowSelected)
				builder.Append(",\"allowSelected\":true");
			else
				builder.Append(",\"allowSelected\":false");
			if (!string.IsNullOrWhiteSpace(Attributes))
				builder.AppendFormat(",\"attributes\":{{{0}}}", Attributes);
			return builder.ToString();
		}

		/// <summary>
		/// 获取节点的Json数据格式表示形式
		/// </summary>
		/// <returns></returns>
		public void WriteNodeJson(HttpResponse response)
		{
			if (Value is string || Value is System.Guid)
				response.WriteAsync(string.Format("\"id\":\"{0}\"", Value));
			else
				response.WriteAsync(string.Format("\"id\":{0}", Value));
			response.WriteAsync(string.Format(",\"text\":\"{0}\"", Text));
			response.WriteAsync(string.Format(",\"label\":\"{0}\"", Text));
			if (!string.IsNullOrWhiteSpace(IconCls))
				response.WriteAsync(string.Format(",\"iconCls\":\"{0}\"", IconCls));
			if (!string.IsNullOrWhiteSpace(nodeCls))
				response.WriteAsync(string.Format(",\"nodeCls\":\"{0}\"", nodeCls));
			if (!string.IsNullOrWhiteSpace(Url))
				response.WriteAsync(string.Format(",\"url\":\"{0}\"", Url));
			else
				response.WriteAsync(",\"url\":null");
			if (!State)
				response.WriteAsync(",\"state\":\"closed\"");
			if (Checked)
				response.WriteAsync(",\"checked\":true");

			if (AllowSelected)
				response.WriteAsync(",\"allowSelected\":true");
			else
				response.WriteAsync(",\"allowSelected\":false");

			if (!string.IsNullOrWhiteSpace(Attributes))
				response.WriteAsync(string.Format(",\"attributes\":{{{0}}}", Attributes));
			if (Nodes != null && Nodes.Count > 0)
			{
				response.WriteAsync(",\"children\":[");
				WriteChildTreeNode(response, Nodes);
				response.WriteAsync("]");
			}
		}

		private void WriteChildTreeNode(HttpResponse response, EasyTreeNodeCollection nodes)
		{
			int rowIndex = 0;
			foreach (EasyTreeNode node in nodes)
			{
				if (rowIndex > 0)
					response.WriteAsync(",");
				else
					rowIndex++;
				response.WriteAsync("{");
				node.WriteNodeJson(response);
				if (node.Nodes != null && node.Nodes.Count > 0)
				{
					response.WriteAsync(",\"children\":[");
					WriteChildTreeNode(response, node.Nodes);
					response.WriteAsync("]");
				}
				response.WriteAsync("}");
			}
		}
	}
}
