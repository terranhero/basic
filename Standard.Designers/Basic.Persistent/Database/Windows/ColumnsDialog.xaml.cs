using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Basic.Collections;
using Basic.Configuration;
using Basic.Database;
using Basic.DataContexts;
using Basic.EntityLayer;
using Basic.Enums;
using Basic.Interfaces;

namespace Basic.Windows
{
	/// <summary>
	/// ColumnsDialog.xaml 的交互逻辑
	/// </summary>
	public class ColumnsDialog : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		static ColumnsDialog()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColumnsDialog),
				new FrameworkPropertyMetadata(typeof(ColumnsDialog)));
		}

		private readonly DesignColumnCollection _columns;
		private readonly PersistentService _CommandService;
		public ColumnsDialog(PersistentService commandService, DesignColumnCollection columns)
		{
			_CommandService = commandService; _columns = columns;
		}

		#region 重载 OnApplyTemplate 方法，绑定 PART_TREEVIEW 事件
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			// 查找模板中的控件
			if (GetTemplateChild("PART_DataGrid") is DataGrid dgColumns)
			{
				dgColumns.ItemsSource = _columns;
			}
		}
		#endregion
	}
}
