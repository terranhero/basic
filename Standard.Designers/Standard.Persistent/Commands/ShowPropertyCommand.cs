using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace Basic.Commands
{
	/// <summary>
	/// 表示显示属性窗口的命令
	/// </summary>
	public class ShowPropertyCommand : ICommand
	{
		public ShowPropertyCommand() { }

		/// <summary>
		/// 当出现影响是否应执行该命令的更改时发生。
		/// </summary>
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnCanExecuteChanged(EventArgs e)
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, e);
		}
		/// <summary>
		/// 定义用于确定此命令是否可以在其当前状态下执行的方法。
		/// </summary>
		/// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
		/// <returns>如果可以执行此命令，则为 true；否则为 false。</returns>
		bool ICommand.CanExecute(object parameter)
		{
			return true;
		}

		/// <summary>
		/// 定义在调用此命令时调用的方法。
		/// </summary>
		/// <param name="parameter">此命令使用的数据。如果此命令不需要传递数据，则该对象可以设置为 null。</param>
		void ICommand.Execute(object parameter)
		{
		}
	}
}
