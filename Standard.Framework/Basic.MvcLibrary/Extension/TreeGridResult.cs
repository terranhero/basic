using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Basic.Interfaces;
using Basic.MvcLibrary;
using Basic.EntityLayer;

namespace Basic.EasyLibrary
{
    /// <summary>
    /// 返回树形Grid需要的数据格式。
    /// </summary>
    public sealed class TreeGridResult<T> : ActionResult where T : class
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
        public TreeGridResult(IPagination<T> dataSource) : this(dataSource, false) { }

        /// <summary>
        /// 初始化 TreeResult 类实例
        /// </summary>
        /// <param name="dataSource">树形结构数据源</param>
        /// <param name="asyncTree">是否需要异步加载树形结构数据，默认值是 false。</param>
        public TreeGridResult(IPagination<T> dataSource, bool asyncTree) { source = dataSource; AsyncTree = asyncTree; }

        /// <summary>
        /// 加载异步树型结构
        /// </summary>
        /// <param name="response"></param>
        private void LoadAsyncTree(HttpResponseBase response)
        {
            response.Write("{\"Success\":true,\"total\":0,\"rows\":[");
            int rowIndex = 0;
            foreach (T row in source)
            {
                if (rowIndex > 0)
                {
                    response.Write(",");
                }
                rowIndex++;
                response.Write("{");
                if (CreateTreeNode != null)
                {
                    EasyTreeNode node = new EasyTreeNode(null, null);
                    CreateTreeNode(row, node);
                    response.Write(JsonSerializer.SerializeObject(row, false));
                    if (!string.IsNullOrWhiteSpace(node.IconCls)) { response.Write(string.Format(",\"iconCls\":\"{0}\"", node.IconCls)); }
                }
                else
                {
                    response.Write(JsonSerializer.SerializeObject(row, false));
                }
                response.Write("}");
            }
            response.Write("]}");
        }

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
                response.Write("{\"Success\":true,\"total\":0,\"rows\":[]}");
                return;
            }
            if (AsyncTree)
            {
                LoadAsyncTree(response);
                return;
            }
            response.Write(string.Format("{{\"Success\":true,\"total\":{0},\"rows\":[", source.Capacity));
            if (SelectNodes != null)
            {
                EasyTreeNode node = new EasyTreeNode(null, null);
                IEnumerable<T> rowArray = SelectNodes(source, null);
                if (rowArray != null)
                {
                    int rowIndex = 0;
                    foreach (T entity in rowArray)
                    {
                        if (rowIndex > 0) { response.Write(","); }
                        rowIndex++;
                        response.Write("{");
                        if (CreateTreeNode != null)
                        {
                            CreateTreeNode(entity, node);
                            response.Write(JsonSerializer.SerializeObject(entity, false));
                            if (!string.IsNullOrWhiteSpace(node.IconCls)) { response.Write(string.Format(",\"iconCls\":\"{0}\"", node.IconCls)); }
                            if (node.State) { response.Write(",\"state\":\"open\""); } else { response.Write(",\"state\":\"closed\""); }
                        }
                        else
                        {
                            response.Write(JsonSerializer.SerializeObject(entity, false));
                        }
                        if (SelectNodes != null)
                        {
                            IEnumerable<T> rowArray1 = SelectNodes(source, entity);
                            if (rowArray1 != null && rowArray1.Any())
                            {
                                response.Write(",\"children\":[");
                                WriteChildTreeNode(response, rowArray1);
                                response.Write("]");
                            }
                        }
                        response.Write("}");
                    }
                }
            }
            response.Write("]}");
        }

        private void WriteChildTreeNode(HttpResponseBase response, IEnumerable<T> children)
        {
            int rowIndex = 0;
            EasyTreeNode node = new EasyTreeNode(null, null);
            foreach (T row in children)
            {
                if (rowIndex > 0)
                {
                    response.Write(",");
                }
                rowIndex++;
                response.Write("{");
                if (CreateTreeNode != null)
                {
                    if (CreateTreeNode != null)
                    {
                        CreateTreeNode(row, node);
                        response.Write(JsonSerializer.SerializeObject(row, false));
                        if (!string.IsNullOrWhiteSpace(node.IconCls)) { response.Write(string.Format(",\"iconCls\":\"{0}\"", node.IconCls)); }
                        if (node.State) { response.Write(",\"state\":\"open\""); } else { response.Write(",\"state\":\"closed\""); }
                    }
                    else
                    {
                        response.Write(JsonSerializer.SerializeObject(row, false));
                    }
                }
                IEnumerable<T> rowArray1 = SelectNodes(source, row);
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
