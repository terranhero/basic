using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;

namespace Basic.Designer
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void CommandChengedHandler(object sender, CommandChangedEventArgs e);

	public class CommandChangedEventArgs : RoutedEventArgs
	{
		// Fields
		private ICollection _newValues;

		public CommandChangedEventArgs(RoutedEvent routedEvent, ICollection newValue)
		{
			this._newValues = newValue;
			base.RoutedEvent = routedEvent;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			CommandChengedHandler handler = (CommandChengedHandler)genericHandler;
			handler(genericTarget, (CommandChangedEventArgs)this);
		}

		// Properties
		public ICollection SelectedValues
		{
			get
			{
				return this._newValues;
			}
		}
	}
}
