
using System.Text;
using System.Web;
namespace Basic.EasyLibrary
{
	/// <summary>
	/// 
	/// </summary>
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
		public void WriteNodeJson(HttpResponseBase response)
		{
			if (Value is string || Value is System.Guid)
				response.Write(string.Format("\"id\":\"{0}\"", Value));
			else
				response.Write(string.Format("\"id\":{0}", Value));
			response.Write(string.Format(",\"text\":\"{0}\"", Text));
			response.Write(string.Format(",\"label\":\"{0}\"", Text));
			if (!string.IsNullOrWhiteSpace(IconCls))
				response.Write(string.Format(",\"iconCls\":\"{0}\"", IconCls));
			if (!string.IsNullOrWhiteSpace(nodeCls))
				response.Write(string.Format(",\"nodeCls\":\"{0}\"", nodeCls));
			if (!string.IsNullOrWhiteSpace(Url))
				response.Write(string.Format(",\"url\":\"{0}\"", Url));
			else
				response.Write(",\"url\":null");
			if (!State)
				response.Write(",\"state\":\"closed\"");
			if (Checked)
				response.Write(",\"checked\":true");

			if (AllowSelected)
				response.Write(",\"allowSelected\":true");
			else
				response.Write(",\"allowSelected\":false");

			if (!string.IsNullOrWhiteSpace(Attributes))
				response.Write(string.Format(",\"attributes\":{{{0}}}", Attributes));
			if (Nodes != null && Nodes.Count > 0)
			{
				response.Write(",\"children\":[");
				WriteChildTreeNode(response, Nodes);
				response.Write("]");
			}
		}

		private void WriteChildTreeNode(HttpResponseBase response, EasyTreeNodeCollection nodes)
		{
			int rowIndex = 0;
			foreach (EasyTreeNode node in nodes)
			{
				if (rowIndex > 0)
					response.Write(",");
				else
					rowIndex++;
				response.Write("{");
				node.WriteNodeJson(response);
				if (node.Nodes != null && node.Nodes.Count > 0)
				{
					response.Write(",\"children\":[");
					WriteChildTreeNode(response, node.Nodes);
					response.Write("]");
				}
				response.Write("}");
			}
		}
	}
}
