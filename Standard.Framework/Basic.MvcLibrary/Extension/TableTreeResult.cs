using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


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
    public class TableTreeResult<TT, TR> : ActionResult
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
        public override void ExecuteResult(ControllerContext context)
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
                    HttpResponseBase response = context.HttpContext.Response;
                    response.Clear();
                    response.ContentType = "application/json";
                    response.Write("[");
                    int rowIndex = 0;
                    foreach (TR row in rowArray)
                    {
                        if (rowIndex > 0)
                        {
                            response.Write(",");
                        }
                        rowIndex++;
                        response.Write("{");
                        if (CreateTreeNode != null)
                        {
                            EasyTreeNode node = new EasyTreeNode(row[ValueField], Convert.ToString(row[TextField]));
                            CreateTreeNode(row, node);
							node.WriteNodeJson(response);
                        }
                        else
                        {
                            response.Write(string.Format("\"id\":{0}", row[ValueField]));
                            response.Write(string.Format(",\"text\":\"{0}\"", row[TextField]));
                            response.Write(string.Format(",\"label\":\"{0}\"", row[TextField]));
                        }
                        if (SelectNodes != null)
                        {
                            IEnumerable<TR> rowArray1 = SelectNodes(source, null);
                            if (rowArray1 != null && rowArray1.Any())
                            {
                                response.Write(",\"children\":[");
                                WriteChildTreeNode(response, rowArray1);
                                response.Write("]");
                            }
                        }
                        response.Write("}");
                    }
                    response.Write("]");
                }
            }
        }

        private void WriteChildTreeNode(HttpResponseBase response, IEnumerable<TR> children)
        {
            int rowIndex = 0;
            foreach (TR row in children)
            {
                if (rowIndex > 0)
                {
                    response.Write(",");
                }
                rowIndex++;
                response.Write("{");
                if (CreateTreeNode != null)
                {
                    EasyTreeNode node = new EasyTreeNode(row[ValueField], Convert.ToString(row[TextField]));
                    CreateTreeNode(row, node);
					node.WriteNodeJson(response);
                }
                else
                {
                    response.Write(string.Format("\"id\":{0}", row[ValueField]));
                    response.Write(string.Format(",\"text\":\"{0}\"", row[TextField]));
                    response.Write(string.Format(",\"label\":\"{0}\"", row[TextField]));
                }
                IEnumerable<TR> rowArray1 = SelectNodes(source, null);
                if (rowArray1 != null && rowArray1.Any())
                {
                    response.Write(",\"children\":[");
                    WriteChildTreeNode(response, rowArray1);
                    response.Write("]");
                }
                response.Write("}");
            }
        }
    }
}
