using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Basic.EasyLibrary
{
	internal static class HtmlTextWriterExtension
	{
		/// <summary>
		/// 向其开始标记中添加指定的标记特性和值。
		/// </summary>
		/// <param name="writer">接收EasyUI.DataGrid内容的 HtmlTextWriter 对象。</param>
		/// <param name="options"></param>
		public static void AddAttribute(this HtmlTextWriter writer, IDictionary<string, object> options)
		{
			foreach (var item in options)
			{
				writer.AddAttribute(item.Key, Convert.ToString(item.Value));
			}
		}
		/// <summary>
		/// 向其开始标记中添加指定的标记特性和值。
		/// </summary>
		/// <param name="writer">接收EasyUI.DataGrid内容的 HtmlTextWriter 对象。</param>
		/// <param name="options"></param>
		public static void AddStyleAttribute(this HtmlTextWriter writer, IDictionary<string, string> options)
		{
			foreach (var item in options)
			{
				writer.AddStyleAttribute(item.Key, item.Value);
			}
		}
	}
}
