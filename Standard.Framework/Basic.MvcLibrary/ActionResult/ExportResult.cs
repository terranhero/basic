using System;
using System.Text;
using System.Web.Mvc;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 表示一个类，该类用于使用由 IViewEngine 对象返回的 IView 实例来呈现视图。
	/// </summary>
	public class ExportResult : ViewResultBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public override void ExecuteResult(ControllerContext context)
		{
			context.Controller.ViewData["Controller_RenderMode"] = 0;
			base.ExecuteResult(context);
		}

		/// <summary>
		/// 初始化DataGridResult类实例
		/// </summary>
		/// <param name="controller"></param>
		public ExportResult(Controller controller)
		{
			ViewData = controller.ViewData;
			TempData = controller.TempData;
		}

		/// <summary>
		/// 搜索已注册的视图引擎并返回用于呈现视图的对象。
		/// </summary>
		/// <param name="context">控制器上下文。</param>
		/// <exception cref="InvalidOperationException"></exception>
		/// <returns>用于呈现视图的对象。</returns>
		protected override ViewEngineResult FindView(ControllerContext context)
		{
			ViewEngineResult result = base.ViewEngineCollection.FindView(context, base.ViewName, this.MasterName);
			if (result.View != null)
			{
				return result;
			}
			StringBuilder builder = new StringBuilder();
			foreach (string str in result.SearchedLocations)
			{
				builder.AppendLine();
				builder.Append(str);
			}
			throw new InvalidOperationException(string.Format("视图 '{0}' 或者母版页不存在，或者视图引擎提供的搜索路径有问题，搜索路径如下:\r\n{1}",
				new object[] { base.ViewName, builder }));
		}

		/// <summary>
		/// 获取在呈现视图时要使用的母版视图（如母版页或模板）的名称。
		/// </summary>
		/// <value>母版视图的名称。</value>
		public string MasterName { get; set; }

	}
}
