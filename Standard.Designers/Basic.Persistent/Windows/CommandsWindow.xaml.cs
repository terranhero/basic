using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Basic.Collections;
using Basic.Configuration;
using Basic.Database;
using Basic.DataContexts;
using Basic.DataEntities;
using Basic.Enums;

namespace Basic.Windows
{
	/// <summary>
	/// CommandWindow.xaml 的交互逻辑
	/// </summary>
	public partial class CommandsWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
	{
		private readonly EnvDTE80.DTE2 dteClass;
		private readonly TableDesignerCollection designerTables;
		private readonly TableDesignerInfo tableDesignerInfo;
		private readonly DataEntityElement dataEntityElement;
		private StaticCommandElement staticCommand;
		public CommandsWindow(EnvDTE80.DTE2 dte, DataEntityElement entity, StaticCommandElement command)
			: base("Microsoft.VisualStudio.PlatformUI.DialogWindow")
		{
			designerTables = new TableDesignerCollection("Database_Tables");
			dteClass = dte;
			dataEntityElement = entity;
			staticCommand = command;
			tableDesignerInfo = designerTables.CreateTable("T1", true);
			InitializeComponent();
			tableDesignerInfo.CopyFrom(entity.Persistent.TableInfo);
			DataContext = designerTables;
			designerTables.Add(tableDesignerInfo);
			designerTables.CheckedChanged += new ColumnCheckedHandler(tableDesignerInfo_CheckedChanged);
			designerTables.IsWhereChanged += new ColumnIsWhereChangedHandler(tableDesignerInfo_IsWhereChanged);
			designerTables.UseDefaultChanged += new ColumnUseDefaultHandler(tableDesignerInfo_UseDefaultChanged);
			designerTables.ColumnChanged += new ColumnChangedHandler(tableDesignerInfo_ColumnChanged);
			if (staticCommand != null && staticCommand.Kind == ConfigurationTypeEnum.AddNew)
			{
				this.Title = string.Format("Edit \"{0}\" Command", staticCommand.Name);
				tabInsert.IsSelected = true;
				tabUpdate.Visibility = System.Windows.Visibility.Collapsed;
				tabDelete.Visibility = System.Windows.Visibility.Collapsed;
				tabProcedure.Visibility = System.Windows.Visibility.Collapsed;
				tabSelect.Visibility = System.Windows.Visibility.Collapsed;
			}
			else if (staticCommand != null && staticCommand.Kind == ConfigurationTypeEnum.Modify)
			{
				this.Title = string.Format("Edit \"{0}\" Command", staticCommand.Name);
				tabInsert.Visibility = System.Windows.Visibility.Collapsed;
				tabUpdate.IsSelected = true;
				tabDelete.Visibility = System.Windows.Visibility.Collapsed;
				tabProcedure.Visibility = System.Windows.Visibility.Collapsed;
				tabSelect.Visibility = System.Windows.Visibility.Collapsed;
			}
			else if (staticCommand != null && staticCommand.Kind == ConfigurationTypeEnum.Remove)
			{
				this.Title = string.Format("Edit \"{0}\" Command", staticCommand.Name);
				tabInsert.Visibility = System.Windows.Visibility.Collapsed;
				tabUpdate.Visibility = System.Windows.Visibility.Collapsed;
				tabDelete.IsSelected = true;
				tabProcedure.Visibility = System.Windows.Visibility.Collapsed;
				tabSelect.Visibility = System.Windows.Visibility.Collapsed;
			}
			else if (staticCommand != null && staticCommand.Kind == ConfigurationTypeEnum.Other)
			{
				this.Title = string.Format("Edit \"{0}\" Command", staticCommand.Name);
				tabInsert.Visibility = System.Windows.Visibility.Collapsed;
				tabUpdate.Visibility = System.Windows.Visibility.Collapsed;
				tabDelete.Visibility = System.Windows.Visibility.Collapsed;
				tabProcedure.Visibility = System.Windows.Visibility.Collapsed;
				tabSelect.IsSelected = true;
			}
		}

		private void tableDesignerInfo_UseDefaultChanged(object sender, ColumnUseDefaultEventArgs e)
		{
			if (tabInsert.IsSelected)
				txtInsertText.Text = tableDesignerInfo.CreateInsertCommand();
			else if (tabUpdate.IsSelected)
				txtUpdateText.Text = tableDesignerInfo.CreateUpdateCommand();
			else if (tabSelect.IsSelected)
				txtSelectText.Text = tableDesignerInfo.CreateSelectCommand();
		}

		private void tableDesignerInfo_IsWhereChanged(object sender, ColumnIsWhereEventArgs e)
		{
			if (tabUpdate.IsSelected)
				txtUpdateText.Text = tableDesignerInfo.CreateUpdateCommand();
			else if (tabSelect.IsSelected)
				txtSelectText.Text = tableDesignerInfo.CreateSelectCommand();
		}

		private void tableDesignerInfo_IsFromChanged(object sender, ColumnCheckedEventArgs e)
		{
			if (tabSelect.IsSelected)
				txtSelectText.Text = tableDesignerInfo.CreateSelectCommand();
		}

		private void tableDesignerInfo_CheckedChanged(object sender, ColumnCheckedEventArgs e)
		{
			if (tabInsert.IsSelected)
				txtInsertText.Text = tableDesignerInfo.CreateInsertCommand();
			else if (tabUpdate.IsSelected)
				txtUpdateText.Text = tableDesignerInfo.CreateUpdateCommand();
			else if (tabDelete.IsSelected)
				txtDeleteText.Text = tableDesignerInfo.CreateDeleteCommand();
			else if (tabSelect.IsSelected)
				txtSelectText.Text = tableDesignerInfo.CreateSelectCommand();
		}

		private void tableDesignerInfo_ColumnChanged(object sender, ColumnChangedEventArgs e)
		{
			if (tabInsert.IsSelected)
				txtInsertText.Text = tableDesignerInfo.CreateInsertCommand();
			else if (tabUpdate.IsSelected)
				txtUpdateText.Text = tableDesignerInfo.CreateUpdateCommand();
			else if (tabDelete.IsSelected)
				txtDeleteText.Text = tableDesignerInfo.CreateDeleteCommand();
			else if (tabSelect.IsSelected)
				txtSelectText.Text = tableDesignerInfo.CreateSelectCommand();
		}

		private void OnCancelClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void OnCreateInsertCommand(object sender, RoutedEventArgs e)
		{
			if (staticCommand == null)
			{
				int count = dataEntityElement.Persistent.DataCommands.Count;
				StaticCommandElement staticCommand1 = new StaticCommandElement(dataEntityElement);
				if (tableDesignerInfo.CreateInsertParameter(dataEntityElement, staticCommand1))
				{
					staticCommand1.Name = string.Concat("InsertCommand", count + 1);
					staticCommand1.ExecutableMethod = Enums.StaticMethodEnum.ExecuteNonQuery;
					staticCommand1.CommandText = txtInsertText.Text;
					dataEntityElement.DataCommands.Add(staticCommand1);
					this.DialogResult = true;
					this.Close();
				}
			}
			else if (staticCommand is StaticCommandElement)
			{
				StaticCommandElement staticCommand1 = staticCommand as StaticCommandElement;
				staticCommand1.CommandText = txtInsertText.Text;
				staticCommand1.Parameters.Clear();
				if (tableDesignerInfo.CreateInsertParameter(dataEntityElement, staticCommand1))
				{
					this.DialogResult = true;
					this.Close();
				}
			}
		}

		private void OnCreateUpdateCommand(object sender, RoutedEventArgs e)
		{
			if (staticCommand == null)
			{
				int count = dataEntityElement.Persistent.DataCommands.Count;
				StaticCommandElement staticCommand1 = new StaticCommandElement(dataEntityElement);
				staticCommand1.Name = string.Concat("UpdateCommand", count + 1);
				staticCommand1.ExecutableMethod = Enums.StaticMethodEnum.ExecuteNonQuery;
				staticCommand1.CommandText = txtUpdateText.Text;
				if (tableDesignerInfo.CreateUpdateParameter(dataEntityElement, staticCommand1))
				{
					dataEntityElement.DataCommands.Add(staticCommand1);
					this.DialogResult = true;
					this.Close();
				}
			}
			else if (staticCommand is StaticCommandElement)
			{
				staticCommand.CommandText = txtUpdateText.Text;
				staticCommand.Parameters.Clear();
				if (tableDesignerInfo.CreateUpdateParameter(dataEntityElement, staticCommand))
				{
					this.DialogResult = true;
					this.Close();
				}
			}
		}

		private void OnCreateSelectCommand(object sender, RoutedEventArgs e)
		{
			if (staticCommand == null)
			{
				int count = dataEntityElement.Persistent.DataCommands.Count;
				StaticCommandElement staticCommand1 = new StaticCommandElement(dataEntityElement);
				staticCommand1.Name = string.Concat("SelectCommand", count + 1);
				staticCommand1.ExecutableMethod = StaticMethodEnum.ExecuteNonQuery;
				staticCommand1.CommandText = txtSelectText.Text;
				if (tableDesignerInfo.CreateSelectParameter(dataEntityElement, staticCommand1))
				{
					dataEntityElement.DataCommands.Add(staticCommand1);
					this.DialogResult = true;
					this.Close();
				}
			}
			else if (staticCommand is StaticCommandElement)
			{
				staticCommand.CommandText = txtSelectText.Text;
				staticCommand.Parameters.Clear();
				if (tableDesignerInfo.CreateSelectParameter(dataEntityElement, staticCommand))
				{
					this.DialogResult = true;
					this.Close();
				}
			}
		}

		private void OnCreateDeleteCommand(object sender, RoutedEventArgs e)
		{
			if (staticCommand == null)
			{
				int count = dataEntityElement.Persistent.DataCommands.Count;
				StaticCommandElement staticCommand1 = new StaticCommandElement(dataEntityElement);
				staticCommand1.Name = string.Concat("DeleteCommand", count + 1);
				staticCommand1.ExecutableMethod = Enums.StaticMethodEnum.ExecuteNonQuery;
				staticCommand1.CommandText = txtDeleteText.Text;
				if (tableDesignerInfo.CreateDeleteParameter(dataEntityElement, staticCommand1))
				{
					dataEntityElement.DataCommands.Add(staticCommand1);
					this.DialogResult = true;
					this.Close();
				}
			}
			else if (staticCommand is StaticCommandElement)
			{
				staticCommand.CommandText = txtDeleteText.Text;
				staticCommand.Parameters.Clear();
				if (tableDesignerInfo.CreateDeleteParameter(dataEntityElement, staticCommand))
				{
					this.DialogResult = true;
					this.Close();
				}
			}
		}

		private void OnCreateProcedure(object sender, RoutedEventArgs e)
		{
			Dispatcher.VerifyAccess();
			if (cmbTables.SelectedIndex < 0)
				return;
			try
			{
				this.DialogResult = false;
				StoreProcedure procedure = cmbTables.SelectedItem as StoreProcedure;
				using (IDataContext context = DataContextFactory.CreateDbAccess())
				{
					context.GetParameters(procedure);
					if (staticCommand != null)
					{
						this.DialogResult = true;
						if (staticCommand is StaticCommandElement)
						{
							procedure.CreateCommand(staticCommand);
						}
						else
						{
							StaticCommandElement staticCommand1 = new StaticCommandElement(dataEntityElement);
							procedure.CreateCommand(staticCommand1);
						}
					}
					else
					{
						StaticCommandElement staticCommand1 = new StaticCommandElement(dataEntityElement);
						procedure.CreateCommand(staticCommand1);
						dataEntityElement.DataCommands.Add(staticCommand1);
					}
					if (context.GetColumns(procedure))
					{
						this.DialogResult = this.DialogResult.Value || staticCommand == null;
						procedure.CreateEntity(dataEntityElement);
					}
				}
				this.Close();
			}
			catch (Exception)
			{

			}
		}

		[SuppressMessage("Usage", "VSTHRD001:避免旧线程切换 API", Justification = "<挂起>")]
		private void OnTabSelectedChanged(object sender, SelectionChangedEventArgs e)
		{
			if (tabProcedure.IsSelected)
			{
				if (!cmbTables.HasItems)
				{
					_ = Dispatcher.BeginInvoke(new Action(() =>
					  {
						  Dispatcher.VerifyAccess();
						  try
						  {
							  using (IDataContext context = DataContextFactory.CreateDbAccess())
							  {
								  cmbTables.ItemsSource = context.GetProcedures();
							  }
						  }
						  catch (Exception)
						  {
						  }
					  }));
				}
			}
			else if (tabInsert.IsSelected)
				txtInsertText.Text = tableDesignerInfo.CreateInsertCommand();
			else if (tabUpdate.IsSelected)
				txtUpdateText.Text = tableDesignerInfo.CreateUpdateCommand();
			else if (tabDelete.IsSelected)
				txtDeleteText.Text = tableDesignerInfo.CreateDeleteCommand();
			else if (tabSelect.IsSelected)
				txtSelectText.Text = tableDesignerInfo.CreateSelectCommand();
		}
	}
}
