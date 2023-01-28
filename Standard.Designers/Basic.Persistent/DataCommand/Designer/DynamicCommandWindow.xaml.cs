using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Basic.Configuration;

namespace Basic.Designer
{
	/// <summary>
	/// DynamicCommandWindow.xaml 的交互逻辑
	/// </summary>
	public partial class DynamicCommandWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly DynamicCommandElement _DynamicCommand;
		internal DynamicCommandWindow(DynamicCommandElement dynamicCommand)
		{
			InitializeComponent();
			this.DataContext = _DynamicCommand = dynamicCommand;
		}

		/// <summary>
		/// 设置界面文本框值。
		/// </summary>
		/// <param name="value"></param>
		public object GetDynamicCommand(PropertyDescriptor propertyDescriptor, object value)
		{
			if (propertyDescriptor.Name == "SelectText")
				return _DynamicCommand.SelectText;
			else if (propertyDescriptor.Name == "FromText")
				return _DynamicCommand.FromText;
			else if (propertyDescriptor.Name == "WhereText")
				return _DynamicCommand.WhereText;
			else if (propertyDescriptor.Name == "GroupText")
				return _DynamicCommand.GroupText;
			else if (propertyDescriptor.Name == "HavingText")
				return _DynamicCommand.HavingText;
			else if (propertyDescriptor.Name == "OrderText")
				return _DynamicCommand.OrderText;
			return value;
		}


		private void btnCopy_Click(object sender, EventArgs e)
		{
			Clipboard.Clear();
			StringBuilder text = new StringBuilder();
			if (_DynamicCommand.WithClauses.Count > 0)
			{
				text.Append("WITH "); List<string> clauses = new List<string>(_DynamicCommand.WithClauses.Count + 2);
				foreach (WithClause clause in _DynamicCommand.WithClauses)
				{
					clauses.Add(string.Concat(clause.TableName, "(", clause.TableDefinition, ") AS (", clause.TableQuery, ")"));
				}
				text.AppendLine(string.Join(",", clauses.ToArray()));
			}
			text.Append("SELECT ").AppendLine(_DynamicCommand.SelectText);
			text.Append("FROM ").AppendLine(_DynamicCommand.FromText);
			if (_DynamicCommand.HasWhere == true)
				text.Append("WHERE ").AppendLine(_DynamicCommand.WhereText);
			if (_DynamicCommand.HasGroup == true)
				text.Append("GROUP BY ").AppendLine(_DynamicCommand.GroupText);
			if (_DynamicCommand.HasHaving == true)
				text.Append("HAVING ").AppendLine(_DynamicCommand.HavingText);
			if (_DynamicCommand.HasOrder == true)
				text.Append("ORDER BY ").AppendLine(_DynamicCommand.OrderText);
			Clipboard.SetText(text.ToString());
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true; this.Close();
		}
	}
}
