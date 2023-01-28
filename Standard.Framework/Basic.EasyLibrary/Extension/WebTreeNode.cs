using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Basic.EasyLibrary
{
	/// <summary>
	/// 
	/// </summary>
	public class NodeCollection : Collection<WebTreeNode>
	{
	}
	/// <summary>
	/// 树控件的节点
	/// </summary>
	public class WebTreeNode
	{
		/// <summary>
		/// 
		/// </summary>
		public WebTreeNode() { Nodes = new NodeCollection(); }
		/// <summary>
		/// 获取 TreeNode 对象的集合，它表示 TreeView 控件中根节点。
		/// </summary>
		public NodeCollection Nodes { get; private set; }

		/// <summary>
		/// 节点图片属性Css样式类
		/// </summary>
		public string IconCls { get; set; }
		/// <summary>
		/// : node state, 'open' or 'closed', default is 'open'. When set to 'closed', the node have children nodes and will load them from remote site 
		/// </summary>
		public bool State { get; set; }
		/// <summary>
		/// : Indicate whether the node is checked selected. 
		/// </summary>
		public bool Checked { get; set; }
		/// <summary>
		/// : custom attributes can be added to a node 
		/// </summary>
		public string Attributes { get; set; }
		/// <summary>
		/// 获取或设置为 TreeView 控件中的节点显示的文本。
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// 获取或设置用于存储有关节点的任何其他数据的非显示值。
		/// </summary>
		public string Value { get; set; }
	}
}
