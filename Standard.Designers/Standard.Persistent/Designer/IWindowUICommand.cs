using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Basic.Designer
{
	/// <summary>
	/// 表示界面命令接口
	/// </summary>
	public interface IWindowUICommands
	{
		/// <summary>
		/// 设置属性窗口选择对象
		/// </summary>
		/// <param name="listObjects">已选择对象</param>
		void SetSelectedObjects(ICollection listObjects);

		/// <summary>
		/// 显示属性窗口
		/// </summary>
		void ShowProperty();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		void ShowMessage(string message);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		void ShowMessage(string message, string title);

		/// <summary>
		/// 显示对话框消息
		/// </summary>
		/// <param name="message"></param>
		int Confirm(string message);

		/// <summary>
		/// 显示对话框消息
		/// </summary>
		/// <param name="message"></param>
		/// <param name="title"></param>
		int Confirm(string message, string title);
	}
}
