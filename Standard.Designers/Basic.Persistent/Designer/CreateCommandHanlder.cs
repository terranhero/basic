using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Basic.DataEntities;
using System.Data;
using Basic.Windows;
using Basic.Configuration;

namespace Basic.Designer
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void EditCommandHanlder(object sender, EditCommandEventArgs e);

	/// <summary>
	/// 
	/// </summary>
	public class EditCommandEventArgs : RoutedEventArgs
	{
		// Fields
		private readonly DataEntityElement _DataEntity;
		private readonly DataCommandElement _DataCommand;

		public EditCommandEventArgs(RoutedEvent routedEvent, DataEntityElement newValue, DataCommandElement dataCommand)
			: base(routedEvent, newValue)
		{
			this._DataEntity = newValue;
			this._DataCommand = dataCommand;
			//base.RoutedEvent = routedEvent;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			EditCommandHanlder handler = (EditCommandHanlder)genericHandler;
			handler(genericTarget, (EditCommandEventArgs)this);
		}

		/// <summary>
		/// 表示创建何种命令
		/// </summary>
		public DataCommandElement DataCommand { get { return _DataCommand; } }

		/// <summary>
		/// 表示当前发起命令创建事件的实体对象。
		/// </summary>
		public DataEntityElement DataEntity { get { return this._DataEntity; } }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void CreateCommandHanlder(object sender, CreateCommandEventArgs e);

	/// <summary>
	/// 
	/// </summary>
	public class CreateCommandEventArgs : RoutedEventArgs
	{
		// Fields
		private readonly DataEntityElement _DataEntity;
		private readonly CommandType _CommandType;

		public CreateCommandEventArgs(RoutedEvent routedEvent, DataEntityElement newValue, CommandType type)
			: base(routedEvent, newValue)
		{
			this._DataEntity = newValue;
			this._CommandType = type;
			//base.RoutedEvent = routedEvent;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			CreateCommandHanlder handler = (CreateCommandHanlder)genericHandler;
			handler(genericTarget, (CreateCommandEventArgs)this);
		}

		/// <summary>
		/// 表示创建何种命令
		/// </summary>
		public CommandType CommandType { get { return _CommandType; } }

		/// <summary>
		/// 表示当前发起命令创建事件的实体对象。
		/// </summary>
		public DataEntityElement DataEntity { get { return this._DataEntity; } }
	}
}
