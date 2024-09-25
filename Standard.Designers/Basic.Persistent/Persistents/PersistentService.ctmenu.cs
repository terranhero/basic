using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using Basic.Collections;
using Basic.DataAccess;
using Basic.Database;
using Basic.DataContexts;
using Basic.DataEntities;
using Basic.Designer;
using Basic.Enums;
using Basic.Windows;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

//VSLangProj.prjKindVBProject for VB.NET projects.
//VSLangProj.prjKindCSharpProject for C# projects.
//VSLangProj.prjKindVSAProject for VSA projects (macros projects).
//VSLangProj2.prjKindVJSharpProject for Visual J# projects.
//VSLangProj2.prjKindSDEVBProject for VB.NET Smart Device projects (Visual Studio .NET 2002/2003 only, see below for Visual Studio 2005).
//VSLangProj2.prjKindSDECSharpProject for C# projects  (Visual Studio .NET 2002/2003 only, see below for Visual Studio 2005).
//{7D353B21-6E36-11D2-B35A-0000F81F0C06} for "Enterprise Projects" (Visual Studio .NET 2002/2003 only).
//{54435603-DBB4-11D2-8724-00A0C9A8B90C} for Setup projects.
//EnvDTE.Constants.vsProjectKindSolutionItems for the Solution Items folder of the Solution Explorer.
//EnvDTE.Constants.vsProjectKindMisc for the Miscellaneous Files folder of the Solution Explorer.

namespace Basic.Configuration
{
	/// <summary>数据持久命令服务类</summary>
	public sealed partial class PersistentService
	{
		#region 数据持久类快捷菜单命令
		/// <summary>快捷菜单Guid字符串</summary>
		public const string ContextGuidString = "{739E35C9-1A1A-4507-936F-C7433642AC55}";

		/// <summary>表示数据持久类菜单标识符。</summary>
		public static readonly Guid ContextMenuGuid = new Guid(ContextGuidString);

		/// <summary>数据持久类快捷菜单命令ID</summary>
		public static readonly CommandID ContextMenuID = new CommandID(ContextMenuGuid, 0x2000);

		/// <summary>数据持久类快捷菜单更新数据表命令ID</summary>
		public static readonly CommandID UpdateTableID = new CommandID(ContextMenuGuid, 0x2012);
		/// <summary>
		/// 数据持久类快捷菜单更新实体模型命令ID
		/// </summary>
		public static readonly CommandID UpdateEntitiesID = new CommandID(ContextMenuGuid, 0x2013);

		/// <summary>创建本地化资源ID</summary>
		public static readonly CommandID CreateResourceID = new CommandID(ContextMenuGuid, 0x2021);
		/// <summary>设置属性资源键ID</summary>
		public static readonly CommandID ResetResourceID = new CommandID(ContextMenuGuid, 0x2022);

		/// <summary>
		/// 创建属性资源ID
		/// </summary>
		public static readonly CommandID CreatePropertyResourceID = new CommandID(ContextMenuGuid, 0x2023);
		/// <summary>
		/// 创建命令资源ID
		/// </summary>
		public static readonly CommandID CreateCommandResourceID = new CommandID(ContextMenuGuid, 0x2024);

		/// <summary>
		/// 数据持久类快捷菜单更新实体模型命令ID
		/// </summary>
		public static readonly CommandID UpdateEntityID = new CommandID(ContextMenuGuid, 0x2042);
		/// <summary>
		/// 数据持久类快捷菜单更新条件实体模型命令ID
		/// </summary>
		public static readonly CommandID UpdateConditionID = new CommandID(ContextMenuGuid, 0x2043);
		/// <summary>
		/// 数据持久类快捷菜单更新静态/动态命令ID
		/// </summary>
		public static readonly CommandID UpdateCommandID = new CommandID(ContextMenuGuid, 0x2044);

		/// <summary>数据持久类代码编辑命令ID</summary>
		public static readonly CommandID EditCodeID = new CommandID(VsMenus.guidStandardCommandSet97, 0x2061);

		/// <summary>数据持久类异步代码编辑命令ID</summary>
		public static readonly CommandID EditCodeAsyncID = new CommandID(VsMenus.guidStandardCommandSet97, 0x2063);

		/// <summary>数据持久类快捷菜单更新静态/动态命令ID</summary>
		public static readonly CommandID CopyCodeID = new CommandID(VsMenus.guidStandardCommandSet97, 0x2062);

		/// <summary>数据持久类快捷菜单更新静态/动态命令ID</summary>
		public static readonly CommandID CopySqlID = new CommandID(VsMenus.guidStandardCommandSet97, 0x2064);

		/// <summary>数据持久类快捷菜单更新静态/动态命令ID</summary>
		public static readonly CommandID tableColumnsID = new CommandID(VsMenus.guidStandardCommandSet97, 0x2067);

		/// <summary>数据持久类快捷菜单更新静态/动态命令ID</summary>
		public static readonly CommandID DataCommandID = new CommandID(ContextMenuGuid, 0x2200);
		public static readonly CommandID CreateCommandID = new CommandID(ContextMenuGuid, 0x2211);
		public static readonly CommandID EditCommandID = new CommandID(ContextMenuGuid, 0x2212);
		public static readonly CommandID PasteStaticCommandID = new CommandID(ContextMenuGuid, 0x2213);
		public static readonly CommandID PasteDynamicCommandID = new CommandID(ContextMenuGuid, 0x2221);

		public static readonly CommandID InsertPropertyCommandID = new CommandID(ContextMenuGuid, 0x2031);
		public static readonly CommandID CreatePropertyCommandID = new CommandID(ContextMenuGuid, 0x2032);

		public static readonly CommandID Cut = new CommandID(ContextMenuGuid, 0x2081);
		public static readonly CommandID Copy = new CommandID(ContextMenuGuid, 0x2082);
		public static readonly CommandID Paste = new CommandID(ContextMenuGuid, 0x2083);
		public static readonly CommandID Delete = new CommandID(ContextMenuGuid, 0x2085);
		#endregion

		#region ShowCode命令，查看代码
		private void OnCanShowCode(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Visible = menu.Enabled = false;
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			EnvDTE.UIHierarchyItem item = array.GetValue(0) as EnvDTE.UIHierarchyItem;
			if (item.Object is EnvDTE.ProjectItem)
			{
				EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
				if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".dpdl"))
				{
					menu.Enabled = menu.Visible = true; return;
				}
			}
		}

		private void OnShowContextCode(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
				if (pItem == null) { continue; }
				foreach (EnvDTE.ProjectItem file in pItem.ProjectItems)
				{
					if (file.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && file.Name.EndsWith("Context.cs"))
					{
						if (file.IsOpen == true) { file.Document.Activate(); }
						else { file.Open().Activate(); }
						break;
					}
				}
			}
		}
		private void OnShowAccessCode(object sender, EventArgs e)
		{
			EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
			Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
			if (array.Length > 1) { return; }
			foreach (EnvDTE.UIHierarchyItem item in array)
			{
				if (item.Object is EnvDTE.ProjectItem)
				{
					EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
					if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".dpdl"))
					{
					}
				}
			}
		}
		#endregion

		#region InsertProperty 命令执行和状态查询
		private void OnInsertProperty(object sender, EventArgs e)
		{
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			if (canvas != null && canvas.SelectedItem != null)
				canvas.SelectedItem.InsertProperty(sender, e);
		}

		private void OnCanInsertProperty(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			menu.Enabled = menu.Visible = false;
			if (canvas == null || canvas.SelectedItem == null) { return; }
			menu.Enabled = menu.Visible = canvas.SelectedItem.SelectedObject is DataConditionPropertyElement ||
				 canvas.SelectedItem.SelectedObject is DataEntityPropertyElement;
		}
		#endregion

		#region NewProperty 命令执行和状态查询
		private void OnCreateProperty(object sender, EventArgs e)
		{
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			if (canvas != null && canvas.SelectedItem != null)
				canvas.SelectedItem.CreateProperty(sender, e);
		}

		private void OnCanCreateProperty(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			menu.Enabled = menu.Visible = false;
			if (canvas == null || canvas.SelectedItem == null) { return; }
			menu.Enabled = menu.Visible = canvas.SelectedItem.SelectedObject is DataConditionPropertyElement ||
				 canvas.SelectedItem.SelectedObject is DataEntityPropertyElement ||
				 canvas.SelectedItem.SelectedObject is DataConditionElement ||
				 canvas.SelectedItem.SelectedObject is DataEntityElement;
		}
		#endregion

		#region CreateCommand 命令执行和状态查询
		[SuppressMessage("Usage", "VSTHRD100:避免使用 Async Void 方法", Justification = "<挂起>")]
		private async void OnCreateCommand(object sender, EventArgs e)
		{
			try
			{
				ConnectionConfiguration config = new ConnectionConfiguration(this);
				PersistentPane pane = await GetPersistentPaneAsync();
				if (pane.Content is DesignerEntitiesCanvas canvas && config.ReadConfig(pane.ProjectItem))
				{
					if (canvas.SelectedItem == null)
					{
						PersistentConfiguration persistent = pane.GetPersistent();
						DataEntityElement entityEntityElement = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
						entityEntityElement.Name = string.Concat(persistent.TableInfo.EntityName, persistent.DataEntities.Count);
						EnvDTE80.DTE2 dteClass = this.GetDTE();
						CommandsWindow window = new CommandsWindow(dteClass, entityEntityElement, null);
						if (window.ShowModal() == true)
						{
							entityEntityElement.TableName = persistent.TableName;
							entityEntityElement.Comment = persistent.TableInfo.Description;
							persistent.DataEntities.Add(entityEntityElement);
						}
					}
					else
					{
						DesignerEntity designerItem = canvas.SelectedItem as DesignerEntity;
						DataEntityElement entityEntityElement = designerItem.DataContext as DataEntityElement;
						EnvDTE80.DTE2 dteClass = this.GetDTE();
						CommandsWindow window1 = new CommandsWindow(dteClass, entityEntityElement, null);
						if (window1.ShowModal() == true) { }
					}
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}

		private void OnCanCreateCommand(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			menu.Enabled = menu.Visible = canvas != null;
		}
		#endregion

		#region EditCommand 命令执行和状态查询
		private void OnEditCommand(object sender, EventArgs e)
		{
			try
			{
				ConnectionConfiguration config = new ConnectionConfiguration(this);
				DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
				PersistentPane pane = this.GetPersistentPane();
				if (canvas == null || canvas.SelectedItem == null) { return; }
				if (config.ReadConfig(pane.ProjectItem))
				{
					if (canvas.SelectedItem != null)
					{
						DataEntityElement entityEntityElement = canvas.SelectedItem.DataContext as DataEntityElement;
						StaticCommandElement dataCommand = canvas.SelectedItem.SelectedObject as StaticCommandElement;
						EnvDTE80.DTE2 dteClass = this.GetDTE();
						CommandsWindow window1 = new CommandsWindow(dteClass, entityEntityElement, dataCommand);
						if (window1.ShowModal() == true) { }
					}
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}

		private void OnCanEditCommand(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			menu.Enabled = menu.Visible = false;
			if (canvas == null || canvas.SelectedItem == null) { return; }
			menu.Enabled = menu.Visible = canvas.SelectedItem.SelectedObject is StaticCommandElement;
		}
		#endregion

		#region Paste Command 命令执行和状态查询
		private void OnCanPasteCommand(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			string text = Clipboard.GetText(TextDataFormat.UnicodeText);
			if (menu.CommandID == PasteStaticCommandID)
			{
				menu.Enabled = menu.Visible = TransactSqlResolver.CanPaste(text);
			}
			else if (menu.CommandID == PasteDynamicCommandID)
			{
				menu.Enabled = menu.Visible = TransactSqlResolver.CanPaste(text);
			}
		}

		private void OnPasteStaticCommand(object sender, EventArgs e)
		{
			try
			{
				PersistentPane pane = this.GetPersistentPane();
				DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
				if (canvas == null) { return; }
				string text = Clipboard.GetText(TextDataFormat.UnicodeText);
				ConnectionConfiguration config = new ConnectionConfiguration(this);
				if (config.ReadConfig(pane.ProjectItem) == false) { return; }
				if (canvas.SelectedItem != null)
				{
					DataEntityElement entityEntityElement = canvas.SelectedItem.DataContext as DataEntityElement;
					DataCommandElement dataCommand = canvas.SelectedItem.SelectedObject as DataCommandElement;
					if (dataCommand is StaticCommandElement)
					{
						StaticCommandElement staticCommand = dataCommand as StaticCommandElement;
						if (TransactSqlResolver.PasteStaticCommand(staticCommand, text)) { Clipboard.Clear(); }
					}
					else
					{
						StaticCommandElement staticCommand = new StaticCommandElement(entityEntityElement);
						if (TransactSqlResolver.PasteStaticCommand(staticCommand, text))
						{
							Clipboard.Clear();
							entityEntityElement.DataCommands.Add(staticCommand);
						}
					}
				}
				else
				{
					SetWaitCursor();
					PersistentConfiguration persistent = this.GetPersistentConfiguration();
					if (TransactSqlResolver.PasteStaticCommand(persistent, text)) { Clipboard.Clear(); }

				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}

		private void OnPasteDynamicCommand(object sender, EventArgs e)
		{
			try
			{
				PersistentPane pane = this.GetPersistentPane();
				DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
				if (canvas == null) { return; }
				string text = Clipboard.GetText(TextDataFormat.UnicodeText);
				ConnectionConfiguration config = new ConnectionConfiguration(this);
				if (config.ReadConfig(pane.ProjectItem) == false) { return; }
				if (canvas.SelectedItem != null)
				{
					DataEntityElement entityEntityElement = canvas.SelectedItem.DataContext as DataEntityElement;
					if (canvas.SelectedItem.SelectedObject is DynamicCommandElement)
					{
						DynamicCommandElement dynamicCommand = canvas.SelectedItem.SelectedObject as DynamicCommandElement;
						using (StringReader reader = new StringReader(text))
						{
							if (TransactSqlResolver.PasteDynamicCommand(dynamicCommand, reader))
							{
								Clipboard.Clear();
							}
						}
					}
					else
					{
						DynamicCommandElement dynamicCommand = new DynamicCommandElement(entityEntityElement)
						{
							Name = string.Concat("DynamicCommand_", entityEntityElement.DataCommands.Count)
						};
						using (StringReader reader = new StringReader(text))
						{
							if (TransactSqlResolver.PasteDynamicCommand(dynamicCommand, reader))
							{
								Clipboard.Clear(); entityEntityElement.DataCommands.Add(dynamicCommand);
							}
						}
					}
				}
				else
				{
					SetWaitCursor();
					PersistentConfiguration persistent = this.GetPersistentConfiguration();
					if (TransactSqlResolver.PasteDynamicCommand(persistent, text))
					{
						Clipboard.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}
		#endregion

		#region UpdateCommand 命令执行和状态查询
		private void OnCanUpdateCommand(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			PersistentConfiguration persistent = this.GetPersistentConfiguration();
			if (persistent.TableInfo.IsEmpty) { menu.Enabled = menu.Visible = false; return; }
			menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem != null &&
				 canvas.SelectedItem.SelectedObject is DataCommandElement &&
				 (!(canvas.SelectedItem.SelectedObject is DynamicCommandElement));
		}

		private void OnUpdateCommand(object sender, EventArgs e)
		{
			try
			{
				DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
				if (canvas == null) { return; }
				PersistentPane pane = this.GetPersistentPane();
				PersistentConfiguration persistent = this.GetPersistentConfiguration();
				ConnectionConfiguration config = new ConnectionConfiguration(this);
				if (config.ReadConfig(pane.ProjectItem) == false) { return; }
				SetWaitCursor();
				if (canvas.SelectedItem != null)
				{
					DesignerEntity selectedItem = canvas.SelectedItem;
					if (selectedItem.SelectedObject is StaticCommandElement)
					{
						StaticCommandElement command = selectedItem.SelectedObject as StaticCommandElement;
						if (command.Kind == ConfigurationTypeEnum.AddNew)
							persistent.TableInfo.CreateInsertSqlStruct(command);
						else if (command.Kind == ConfigurationTypeEnum.Modify)
							persistent.TableInfo.CreateUpdateSqlStruct(command);
						else if (command.Kind == ConfigurationTypeEnum.Remove)
							persistent.TableInfo.CreateDeleteSqlStruct(command);
						else if (command.Kind == ConfigurationTypeEnum.SelectByKey)
							persistent.TableInfo.CreateSelectByPKeySqlStruct(command);
					}
					else if (selectedItem.SelectedObject is DynamicCommandElement)
					{
						DynamicCommandElement dataCommand = selectedItem.SelectedObject as DynamicCommandElement;
						string text = Clipboard.GetText(TextDataFormat.UnicodeText);
						if (!string.IsNullOrWhiteSpace(text) && TransactSqlResolver.CanPaste(text))
						{
							using (StringReader reader = new StringReader(text))
							{
								if (TransactSqlResolver.PasteDynamicCommand(dataCommand, reader))
								{
									Clipboard.Clear();
								}
							}
						}
						else if (dataCommand.Kind == ConfigurationTypeEnum.SearchTable)
						{ persistent.TableInfo.CreateSelectAllSqlStruct(dataCommand); }
					}
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}
		#endregion

		#region UpdateTable 命令执行和状态查询
		private void OnCanUpdateTable(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			PersistentConfiguration persistent = this.GetPersistentConfiguration();
			menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem == null && !persistent.TableInfo.IsEmpty;
		}

		private void OnUpdateTable(object sender, EventArgs e)
		{
			try
			{
				ConnectionConfiguration configInfo = new ConnectionConfiguration(this);
				PersistentPane pane = this.GetPersistentPane();
				PersistentConfiguration persistent = pane.GetPersistent();
				if (configInfo.ReadConfig(pane.ProjectItem) == false) { return; }
				if (persistent.TableInfo.IsEmpty)
				{
					InitializationWindow window = new InitializationWindow(persistent);
					window.ShowModal();
				}
				else
				{
					SetWaitCursor();
					using (IDataContext context = DataContextFactory.CreateDbAccess())
					{
						context.GetColumns(persistent.TableInfo);
					}
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}
		#endregion

		#region UpdateCondition 命令执行和状态查询
		private void OnCanUpdateCondition(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem != null &&
				 canvas.SelectedItem.SelectedObject is DynamicCommandElement;
		}

		private void OnUpdateCondition(object sender, EventArgs e)
		{
			try
			{
				SetWaitCursor();
				DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
				PersistentPane pane = this.GetPersistentPane();
				PersistentConfiguration persistent = this.GetPersistentConfiguration();
				DesignTableInfo _TableInfo = persistent.TableInfo;
				DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
				if (canvas.SelectedItem != null && canvas.SelectedItem.SelectedObject is DynamicCommandElement)
				{
					DataEntityElement entity = canvas.SelectedItem.DataContext as DataEntityElement;
					ConnectionConfiguration config = new ConnectionConfiguration(this);
					if (config.ReadConfig(pane.ProjectItem))
					{
						DynamicCommandElement dynamicCommand = canvas.SelectedItem.SelectedObject as DynamicCommandElement;
						StringBuilder textBuilder = new StringBuilder(1000);
						if (dynamicCommand.WithClauses.Count > 0)
						{
							textBuilder.Append("WITH "); List<string> clauses = new List<string>(dynamicCommand.WithClauses.Count + 2);
							foreach (Basic.Designer.WithClause clause in dynamicCommand.WithClauses)
							{
								clauses.Add(clause.ToSql());
							}
							textBuilder.AppendLine(string.Join(",", clauses.ToArray()));
						}
						textBuilder.Append("SELECT ").AppendLine(dynamicCommand.SelectText);
						textBuilder.Append("FROM ").Append(dynamicCommand.FromText);
						if (dynamicCommand.HasGroup) { textBuilder.Append("GROUP BY ").Append(dynamicCommand.GroupText); }
						TransactSqlResolver.UpdateDataCondition(entity.Condition, textBuilder.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}
		#endregion

		#region 显示数据库字段
		private void OnCanShowColumns(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			menu.Enabled = menu.Visible = canvas != null;
		}
		/// <summary>显示数据库字典编辑</summary>
		private void OnShowColumns(object sender, EventArgs e)
		{
			try
			{
				PersistentPane pane = this.GetPersistentPane();
				DesignerEntitiesCanvas canvas = pane.Content as DesignerEntitiesCanvas;
				PersistentConfiguration persistent = pane.GetPersistent();
				DesignTableInfo _TableInfo = persistent.TableInfo;

				//asyncPackage.GetDialogPage()

				//UITypeEditor editor = (UITypeEditor)TypeDescriptor.GetEditor(typeof(CollectionEditor), typeof(UITypeEditor));
				//var context = new TypeDescriptorContext(asyncPackage, typeof(CollectionEditor));
				//var editValue = editor.EditValue(context, asyncPackage, null);

				CollectionEditor editor = new CollectionEditor(typeof(DesignColumnCollection));
				//UITypeEditor editor = (UITypeEditor)TypeDescriptor.GetEditor(typeof(CollectionEditor), typeof(UITypeEditor));
				object editValue = editor.EditValue(asyncPackage, _TableInfo.Columns);
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}
		#endregion

		#region 更新数据持久类实体模型(单个)
		private void OnCanUpdateEntity(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem != null;
		}

		private void OnUpdateEntity(object sender, EventArgs e)
		{
			try
			{
				PersistentPane pane = this.GetPersistentPane();
				DesignerEntitiesCanvas canvas = pane.Content as DesignerEntitiesCanvas;
				PersistentConfiguration persistent = pane.GetPersistent();
				DesignTableInfo _TableInfo = persistent.TableInfo;
				DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
				DataEntityElement entity = designerEntity.DataContext as DataEntityElement;
				if (designerEntity.SelectedObject is DataConditionPropertyElement || designerEntity.SelectedObject is DataConditionElement)
				{
					_TableInfo.CreateDataConditionElement(entity.Condition);
				}
				else if (designerEntity.SelectedObject is StaticCommandElement)
				{
					StaticCommandElement command = designerEntity.SelectedObject as StaticCommandElement;
					if (command.Kind == ConfigurationTypeEnum.AddNew)
						_TableInfo.CreateInsertSqlStruct(command);
					else if (command.Kind == ConfigurationTypeEnum.Modify)
						_TableInfo.CreateUpdateSqlStruct(command);
					else if (command.Kind == ConfigurationTypeEnum.Remove)
						_TableInfo.CreateDeleteSqlStruct(command);
					else if (command.Kind == ConfigurationTypeEnum.SelectByKey)
						_TableInfo.CreateSelectByPKeySqlStruct(command);
				}
				else if (designerEntity.SelectedObject is DynamicCommandElement)
				{
					ConnectionConfiguration config = new ConnectionConfiguration(this);
					if (config.ReadConfig(pane.ProjectItem))
					{
						DynamicCommandElement dynamicCommand = designerEntity.SelectedObject as DynamicCommandElement;
						TransactSqlResolver.UpdateDataEntity(entity, dynamicCommand);
					}
				}
				else
				{
					ConfigurationTypeEnum kind = ConfigurationTypeEnum.SearchTable;
					if (entity.DataCommands.Count > 0) { kind = entity.DataCommands[0].Kind; }
					_TableInfo.CreateDataEntityElement(entity, kind);
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}
		#endregion

		#region 更新数据持久类实体模型(全部)
		private void OnCanUpdateEntities(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			PersistentConfiguration persistent = canvas.GetPersistent();
			menu.Enabled = menu.Visible = canvas != null && persistent.TableInfo.Columns.Count > 0;
		}

		private void OnUpdateEntities(object sender, EventArgs e)
		{
			try
			{
				DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
				PersistentConfiguration persistent = canvas.GetPersistent();
				if (persistent.TableInfo.Columns.Count == 0)
				{
					WriteToOutput("缺少数据表字段信息，请右击弹出菜单执行\"更新数据表\"命令");
					return;
				}

				DataEntityElementCollection entities = persistent.DataEntities;
				DataEntityElement newEntity = entities.FirstOrDefault(m => m.DataCommands.Any(p => p.Kind == ConfigurationTypeEnum.AddNew));
				DataEntityElement selectEntity = entities.FirstOrDefault(m => m.DataCommands.Any(p => p.Kind == ConfigurationTypeEnum.SearchTable));
				DataEntityElement updateEntity = entities.FirstOrDefault(m => m.DataCommands.Any(p => p.Kind == ConfigurationTypeEnum.Modify));
				DataEntityElement deleteEntity = entities.FirstOrDefault(m => m.DataCommands.Any(p => p.Kind == ConfigurationTypeEnum.Remove));
				//foreach (DataEntityElement entity in persistent.DataEntities)
				//{
				//	foreach (DataCommandElement element in entity.DataCommands)
				//	{
				//		if (element.Kind == ConfigurationTypeEnum.AddNew) { newEntity = entity; }
				//		else if (element.Kind == ConfigurationTypeEnum.Modify) { updateEntity = entity; }
				//		else if (element.Kind == ConfigurationTypeEnum.Remove) { deleteEntity = entity; }
				//		else if (element.Kind == ConfigurationTypeEnum.SearchTable) { selectEntity = entity; }
				//	}
				//}

				if (persistent.BaseAccess == typeof(AbstractDbAccess).Name)
				{
					if (selectEntity == null)
					{
						selectEntity = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
						selectEntity.DesignerInfo.Left = 12;
						selectEntity.DesignerInfo.Top = 22.0;
						selectEntity.DesignerInfo.Expander = true;
						persistent.DataEntities.Add(selectEntity);
						DynamicCommandElement dynamicCommand = new DynamicCommandElement(selectEntity);
						persistent.TableInfo.CreateSearchSqlStruct(selectEntity, dynamicCommand);
						selectEntity.DataCommands.Add(dynamicCommand);
					}
					return;
				}

				if (persistent.GenerateMode == GenerateActionEnum.Single)
				{
					bool notExistSelectTable = selectEntity == null;
					if (selectEntity == null)
					{
						selectEntity = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
						selectEntity.DesignerInfo.Left = 12;
						selectEntity.DesignerInfo.Top = 22.0;
						selectEntity.DesignerInfo.Expander = true;
						persistent.DataEntities.Add(selectEntity);
					}
					if (notExistSelectTable)
					{
						DynamicCommandElement dynamicCommand = new DynamicCommandElement(selectEntity);
						persistent.TableInfo.CreateSearchSqlStruct(selectEntity, dynamicCommand);
						selectEntity.DataCommands.Add(dynamicCommand);
					}
					if (newEntity == null)
					{
						StaticCommandElement staticCommand = new StaticCommandElement(selectEntity);
						persistent.TableInfo.CreateInsertSqlStruct(selectEntity, staticCommand);
						selectEntity.DataCommands.Add(staticCommand);
					}
					if (updateEntity == null)
					{
						StaticCommandElement staticCommand = new StaticCommandElement(selectEntity);
						persistent.TableInfo.CreateUpdateSqlStruct(selectEntity, staticCommand);
						selectEntity.DataCommands.Add(staticCommand);
					}
					if (deleteEntity == null)
					{
						StaticCommandElement staticCommand = new StaticCommandElement(selectEntity);
						persistent.TableInfo.CreateDeleteSqlStruct(selectEntity, staticCommand);
						selectEntity.DataCommands.Add(staticCommand);
					}
				}
				else if (persistent.GenerateMode == GenerateActionEnum.Multiple)
				{
					if (selectEntity == null)
					{
						selectEntity = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
						selectEntity.DesignerInfo.Left = 470.0;
						selectEntity.DesignerInfo.Top = 22.0;
						selectEntity.DesignerInfo.Expander = true;
						DynamicCommandElement dynamicCommand = new DynamicCommandElement(selectEntity);
						persistent.TableInfo.CreateSearchSqlStruct(selectEntity, dynamicCommand);
						selectEntity.DataCommands.Add(dynamicCommand);
						persistent.DataEntities.Add(selectEntity);
					}
					if (newEntity == null)
					{
						newEntity = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
						newEntity.DesignerInfo.Left = 12;
						newEntity.DesignerInfo.Top = 22.0;
						newEntity.DesignerInfo.Expander = true;
						StaticCommandElement staticCommand = new StaticCommandElement(newEntity);
						persistent.TableInfo.CreateInsertSqlStruct(newEntity, staticCommand);
						newEntity.DataCommands.Add(staticCommand);
						persistent.DataEntities.Add(newEntity);
					}
					if (updateEntity == null)
					{
						updateEntity = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
						updateEntity.DesignerInfo.Left = 240;
						updateEntity.DesignerInfo.Top = 22.0;
						updateEntity.DesignerInfo.Expander = true;
						StaticCommandElement staticCommand = new StaticCommandElement(updateEntity);
						persistent.TableInfo.CreateUpdateSqlStruct(updateEntity, staticCommand);
						updateEntity.DataCommands.Add(staticCommand);
						persistent.DataEntities.Add(updateEntity);
					}
					if (deleteEntity == null)
					{
						deleteEntity = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
						deleteEntity.DesignerInfo.Left = 12;
						deleteEntity.DesignerInfo.Top = 292.0;
						deleteEntity.DesignerInfo.Expander = true;
						StaticCommandElement staticCommand = new StaticCommandElement(deleteEntity);
						persistent.TableInfo.CreateDeleteSqlStruct(deleteEntity, staticCommand);
						deleteEntity.DataCommands.Add(staticCommand);
						persistent.DataEntities.Add(deleteEntity);
					}
				}
			}
			catch (Exception ex)
			{
				ShowMessage(ex);
			}
		}
		#endregion

		#region 编辑代码命令 EditCode
		private void OnCanEditCode(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			PersistentConfiguration persistent = GetPersistentConfiguration();
			menu.Enabled = menu.Visible = false;
			if (canvas != null && canvas.SelectedItem != null)
			{
				DesignerEntity item = canvas.SelectedItem;
				menu.Enabled = menu.Visible = true;
				if (item.SelectedObject is DataCommandElement)
					menu.Enabled = menu.Visible = (persistent != null && persistent.GenerateContext);
			}
		}

		private void OnEditCode(object sender, EventArgs e)
		{
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			PersistentPane _PersistentPane = this.GetPersistentPane();
			if (canvas.SelectedItem == null) { return; }
			DesignerEntity item = canvas.SelectedItem;
			if (item.SelectedObject is DataEntityPropertyElement)
			{
				DataEntityPropertyElement property = item.SelectedObject as DataEntityPropertyElement;
				_PersistentPane.EditDataEntityCode(item.DataContext as DataEntityElement, property);
			}
			else if (item.SelectedObject is DataConditionPropertyElement)
			{
				DataConditionPropertyElement property = item.SelectedObject as DataConditionPropertyElement;
				_PersistentPane.EditConditionCode(item.DataContext as DataEntityElement, property);
			}
			else if (item.SelectedObject is DataCommandElement)
			{
				_PersistentPane.EditCommandCode(item.SelectedObject as DataCommandElement, item.DataContext as DataEntityElement);
			}
			else if (item.SelectedObject is DataEntityElement)
			{
				_PersistentPane.EditDataEntityCode(item.DataContext as DataEntityElement, null);
			}
			else if (item.SelectedObject is DataConditionElement)
			{
				_PersistentPane.EditConditionCode(item.DataContext as DataEntityElement, null);
			}
		}

		private void OnCanEditAsyncCode(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			PersistentConfiguration persistent = GetPersistentConfiguration();
			menu.Enabled = menu.Visible = false;
			if (canvas != null && canvas.SelectedItem != null)
			{
				DesignerEntity item = canvas.SelectedItem;
				menu.Enabled = menu.Visible = true;
				if (item.SelectedObject is DataCommandElement)
					menu.Enabled = menu.Visible = (persistent != null && persistent.GenerateContext);
			}
		}

		private void OnEditAsyncCode(object sender, EventArgs e)
		{
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			PersistentPane _PersistentPane = this.GetPersistentPane();
			if (canvas.SelectedItem == null) { return; }
			DesignerEntity item = canvas.SelectedItem;
			if (item.SelectedObject is DataEntityPropertyElement)
			{
				DataEntityPropertyElement property = item.SelectedObject as DataEntityPropertyElement;
				_PersistentPane.EditDataEntityCode(item.DataContext as DataEntityElement, property);
			}
			else if (item.SelectedObject is DataConditionPropertyElement)
			{
				DataConditionPropertyElement property = item.SelectedObject as DataConditionPropertyElement;
				_PersistentPane.EditConditionCode(item.DataContext as DataEntityElement, property);
			}
			else if (item.SelectedObject is DataCommandElement)
			{
				_PersistentPane.EditCommandCode(item.SelectedObject as DataCommandElement, item.DataContext as DataEntityElement);
			}
			else if (item.SelectedObject is DataEntityElement)
			{
				_PersistentPane.EditDataEntityCode(item.DataContext as DataEntityElement, null);
			}
			else if (item.SelectedObject is DataConditionElement)
			{
				_PersistentPane.EditConditionCode(item.DataContext as DataEntityElement, null);
			}
		}
		#endregion

		#region 复制命令 CopySql
		private void OnCanCopySql(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			menu.Enabled = menu.Visible = false;
			if (canvas != null && canvas.SelectedItem != null)
			{
				DesignerEntity item = canvas.SelectedItem;
				menu.Enabled = menu.Visible = item.SelectedObject is DataCommandElement;
			}
		}

		private void OnCopySql(object sender, EventArgs e)
		{
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			if (canvas.SelectedItem == null) { return; }
			DesignerEntity item = canvas.SelectedItem as DesignerEntity;
			if (item == null) { return; }
			if (item.SelectedObject is DynamicCommandElement)
			{
				DynamicCommandElement dynamicCommand = item.SelectedObject as DynamicCommandElement;
				Clipboard.Clear(); StringBuilder text = new StringBuilder(2000);
				string newLine = Environment.NewLine;
				if (dynamicCommand.WithClauses.Count > 0)
				{
					text.Append("WITH "); List<string> clauses = new List<string>(dynamicCommand.WithClauses.Count + 2);
					foreach (Designer.WithClause clause in dynamicCommand.WithClauses)
					{
						clauses.Add(clause.ToSql());
					}
					text.AppendLine(string.Join("," + newLine, clauses.ToArray()));
				}
				text.Append("SELECT ").AppendLine(dynamicCommand.SelectText);
				text.Append("FROM ").AppendLine(dynamicCommand.FromText);
				if (dynamicCommand.HasWhere == true)
					text.Append("WHERE ").AppendLine(dynamicCommand.WhereText);
				if (dynamicCommand.HasGroup == true)
					text.Append("GROUP BY ").AppendLine(dynamicCommand.GroupText);
				if (dynamicCommand.HasHaving == true)
					text.Append("HAVING ").AppendLine(dynamicCommand.HavingText);
				if (dynamicCommand.HasOrder == true)
					text.Append("ORDER BY ").AppendLine(dynamicCommand.OrderText);
				Clipboard.SetText(text.ToString());
			}
			else if (item.SelectedObject is StaticCommandElement)
			{
				StaticCommandElement staticCommand = item.SelectedObject as StaticCommandElement;
				Clipboard.Clear(); StringBuilder text = new StringBuilder(2000);
				Clipboard.SetText(staticCommand.CommandText);
			}
		}
		#endregion

		#region 复制代码命令 CopyCode
		private void OnCanCopyCode(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			PersistentConfiguration persistent = GetPersistentConfiguration();
			menu.Enabled = menu.Visible = false;
			if (canvas != null && canvas.SelectedItem != null)
			{
				DesignerEntity item = canvas.SelectedItem;
				if (item.SelectedObject is DataCommandElement)
					menu.Enabled = menu.Visible = (persistent != null && !persistent.GenerateContext);
			}

		}

		private void OnCopyCode(object sender, EventArgs e)
		{
			DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
			PersistentPane pane = this.GetPersistentPane();
			PersistentConfiguration persistent = GetPersistentConfiguration();
			if (canvas.SelectedItem == null) { return; }
			DesignerEntity item = canvas.SelectedItem as DesignerEntity;
			if (item == null) { return; }
			if (item.SelectedObject is DataCommandElement)
			{
				DataCommandElement command = item.SelectedObject as DataCommandElement;
				CodeTypeMemberCollection members = new CodeTypeMemberCollection();
				CodeDomProvider provider = pane.GetCodeDomProvider();
				CodeMemberMethod codeMethod = command.WriteContextDesignerCode(members, persistent, provider);
				CodeGeneratorOptions options = new CodeGeneratorOptions
				{
					BlankLinesBetweenMembers = true,
					BracingStyle = "C"// "C";
				};
				System.Text.StringBuilder textBuilder = new System.Text.StringBuilder(1500);
				using (StringWriter writer = new StringWriter(textBuilder))
				{
					provider.GenerateCodeFromMember(codeMethod, writer, options);
				}
				Clipboard.SetText(textBuilder.ToString());
			}
		}
		#endregion

		#region 标准/Cut/Copy/Paste/Delete命令
		private void OnDelete(object sender, EventArgs e)
		{
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			if (canvas == null || canvas.SelectedItem == null) { return; }
			if (canvas.SelectedItem.Remove() == false)
			{
				DataEntityElement entity = canvas.SelectedItem.DataContext as DataEntityElement;
				string confirmMessage = string.Format("确定要删除\"{0}\"实体对象,删除对象将同时删除对象关联的命令？", entity.ClassName);
				if (Confirm(confirmMessage))//确定
				{
					PersistentConfiguration persistent = this.GetPersistentConfiguration();
					persistent.DataEntities.Remove(entity);
				}
			}
		}

		private void OnCanPaste(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			PersistentPane pane = this.GetPersistentPane();
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			if (canvas == null) { return; }
			if (Clipboard.ContainsText(TextDataFormat.Xaml))
			{
				string text = Clipboard.GetText(TextDataFormat.Xaml);
				if (string.IsNullOrWhiteSpace(text)) { return; }
				if (text.StartsWith(ClipboardFormat.PersistentDataEntityFormat))
					menu.Enabled = menu.Visible = true;
				else if (canvas.SelectedItem != null && text.StartsWith(ClipboardFormat.PersistentStaticCommandFormat))
					menu.Enabled = menu.Visible = true;
				else if (canvas.SelectedItem != null && text.StartsWith(ClipboardFormat.PersistentDynamicCommandFormat))
					menu.Enabled = menu.Visible = true;
				else if (canvas.SelectedItem != null && text.StartsWith(ClipboardFormat.PersistentConditionPropertyFormat))
					menu.Enabled = menu.Visible = true;
				else if (canvas.SelectedItem != null && text.StartsWith(ClipboardFormat.PersistentEntityPropertyFormat))
					menu.Enabled = menu.Visible = true;
				return;
			}
			else if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
			{
				string text = Clipboard.GetText(TextDataFormat.UnicodeText);
				menu.Enabled = menu.Visible = TransactSqlResolver.CanPaste(text);
			}
		}

		private void OnPaste(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			menu.Enabled = menu.Visible = false;
			PersistentPane pane = this.GetPersistentPane();
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			if (canvas == null) { return; }
			if (Clipboard.ContainsText(TextDataFormat.Xaml))
			{
				string text = Clipboard.GetText(TextDataFormat.Xaml);
				#region 粘帖对象（实体模型/条件模型/模型属性/条件属性/静态命令/动态命令）
				if (string.IsNullOrWhiteSpace(text)) { return; }
				string expressCode = text.Substring(0, ClipboardFormat.ClipboardFormatLength);
				text = text.Remove(0, ClipboardFormat.ClipboardFormatLength);
				if (expressCode == ClipboardFormat.PersistentDataEntityFormat)
				{
					PersistentConfiguration persistent = this.GetPersistentConfiguration();
					DataEntityElement entityElement = new DataEntityElement(persistent) { Guid = Guid.NewGuid() };
					using (StringReader strReader = new StringReader(text))
					{
						using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
						{
							entityElement.ReadXml(reader);
						}
					}
					persistent.DataEntities.Add(entityElement);
					Clipboard.Clear();
				}
				else if (canvas.SelectedItem != null && expressCode == ClipboardFormat.PersistentEntityPropertyFormat)
				{
					DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
					DataEntityElement entityElement = designerEntity.DataContext as DataEntityElement;
					using (StringReader strReader = new StringReader(text))
					{
						using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
						{
							DataEntityPropertyElement property = new DataEntityPropertyElement(entityElement);
							property.ReadXml(reader);
							if (designerEntity.SelectedObject is DataEntityPropertyElement)
							{
								int index = entityElement.Properties.IndexOf(designerEntity.SelectedObject as DataEntityPropertyElement);
								entityElement.Properties.Insert(index, property);
							}
							else { entityElement.Properties.Add(property); }
							Clipboard.Clear();
						}
					}
				}
				else if (canvas.SelectedItem != null && expressCode == ClipboardFormat.PersistentConditionPropertyFormat)
				{
					DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
					DataEntityElement entityElement = designerEntity.DataContext as DataEntityElement;
					using (StringReader strReader = new StringReader(text))
					{
						using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
						{
							DataConditionPropertyElement property = new DataConditionPropertyElement(entityElement.Condition);
							property.ReadXml(reader);
							if (designerEntity.SelectedObject is DataConditionPropertyElement)
							{
								int index = entityElement.Condition.Arguments.IndexOf(designerEntity.SelectedObject as DataConditionPropertyElement);
								entityElement.Condition.Arguments.Insert(index, property);
							}
							else { entityElement.Condition.Arguments.Add(property); }


							Clipboard.Clear();
						}
					}
				}
				else if (canvas.SelectedItem != null && expressCode == ClipboardFormat.PersistentStaticCommandFormat)
				{
					DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
					DataEntityElement entityElement = designerEntity.DataContext as DataEntityElement;
					using (StringReader strReader = new StringReader(text))
					{
						using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
						{
							StaticCommandElement command = new StaticCommandElement(entityElement);
							command.ReadXml(reader);
							entityElement.DataCommands.Add(command);
							Clipboard.Clear();
						}
					}
				}
				else if (canvas.SelectedItem != null && expressCode == ClipboardFormat.PersistentDynamicCommandFormat)
				{
					DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
					DataEntityElement entityElement = designerEntity.DataContext as DataEntityElement;
					using (StringReader strReader = new StringReader(text))
					{
						using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
						{
							DynamicCommandElement command = new DynamicCommandElement(entityElement);
							command.ReadXml(reader);
							entityElement.DataCommands.Add(command);
							Clipboard.Clear();
						}
					}
				}
				#endregion
			}
			else if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
			{
				string text = Clipboard.GetText(TextDataFormat.UnicodeText);
				#region 粘帖为动态命令
				ConnectionConfiguration config = new ConnectionConfiguration(this);
				if (config.ReadConfig(pane.ProjectItem) == false) { return; }
				if (canvas.SelectedItem != null)
				{
					DataEntityElement entityEntityElement = canvas.SelectedItem.DataContext as DataEntityElement;
					if (canvas.SelectedItem.SelectedObject is DynamicCommandElement)
					{
						DynamicCommandElement dynamicCommand = canvas.SelectedItem.SelectedObject as DynamicCommandElement;
						using (StringReader reader = new StringReader(text))
						{
							if (TransactSqlResolver.PasteDynamicCommand(dynamicCommand, reader))
							{
								Clipboard.Clear();
							}
						}
					}
					else
					{
						DynamicCommandElement dynamicCommand = new DynamicCommandElement(entityEntityElement)
						{
							Name = string.Concat("DynamicCommand_", entityEntityElement.DataCommands.Count)
						};
						using (StringReader reader = new StringReader(text))
						{
							if (TransactSqlResolver.PasteDynamicCommand(dynamicCommand, reader))
							{
								Clipboard.Clear(); entityEntityElement.DataCommands.Add(dynamicCommand);
							}
						}
					}
				}
				else
				{
					PersistentConfiguration persistent = this.GetPersistentConfiguration();
					if (TransactSqlResolver.PasteDynamicCommand(persistent, text))
					{
						Clipboard.Clear();
					}
				}
				#endregion
			}
		}

		private void OnCopy(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			if (canvas == null || canvas.SelectedItem == null) { return; }
			StringBuilder builder = new StringBuilder(5000);
			DesignerEntity item = canvas.SelectedItem as DesignerEntity;
			DataEntityElement entityElement = item.DataContext as DataEntityElement;
			if (item.SelectedObject == null || item.SelectedObject is DataEntityElement || item.SelectedObject is DataConditionElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentDataEntityFormat);
					entityElement.WriteXml(writer);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
			else if (item.SelectedObject is DataConditionPropertyElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentConditionPropertyFormat);
					DataConditionPropertyElement property = item.SelectedObject as DataConditionPropertyElement;
					property.WriteXml(writer);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
			else if (item.SelectedObject is DataEntityPropertyElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentEntityPropertyFormat);
					DataEntityPropertyElement property = item.SelectedObject as DataEntityPropertyElement;
					property.WriteXml(writer);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
			else if (item.SelectedObject is StaticCommandElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentStaticCommandFormat);
					StaticCommandElement command = item.SelectedObject as StaticCommandElement;
					command.WriteXml(writer);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
			else if (item.SelectedObject is DynamicCommandElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentDynamicCommandFormat);
					DynamicCommandElement command = item.SelectedObject as DynamicCommandElement;
					command.WriteXml(writer);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
		}

		private void OnCut(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			if (canvas == null || canvas.SelectedItem == null) { return; }
			StringBuilder builder = new StringBuilder(5000);
			DesignerEntity item = canvas.SelectedItem as DesignerEntity;
			DataEntityElement entityElement = item.DataContext as DataEntityElement;
			if (item.SelectedObject == null || item.SelectedObject is DataEntityElement || item.SelectedObject is DataConditionElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentDataEntityFormat);
					entityElement.WriteXml(writer);
				}
				PersistentConfiguration persistent = this.GetPersistentConfiguration();
				persistent.DataEntities.Remove(entityElement);
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
			else if (item.SelectedObject is DataConditionPropertyElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentConditionPropertyFormat);
					DataConditionPropertyElement property = item.SelectedObject as DataConditionPropertyElement;
					property.WriteXml(writer);
					entityElement.Condition.Arguments.Remove(property);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
			else if (item.SelectedObject is DataEntityPropertyElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentEntityPropertyFormat);
					DataEntityPropertyElement property = item.SelectedObject as DataEntityPropertyElement;
					property.WriteXml(writer);
					entityElement.Properties.Remove(property);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
			else if (item.SelectedObject is StaticCommandElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentStaticCommandFormat);
					StaticCommandElement command = item.SelectedObject as StaticCommandElement;
					command.WriteXml(writer);
					entityElement.DataCommands.Remove(command);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
			else if (item.SelectedObject is DynamicCommandElement)
			{
				using (XmlWriter writer = XmlWriter.Create(builder))
				{
					builder.Append(ClipboardFormat.PersistentDynamicCommandFormat);
					DynamicCommandElement command = item.SelectedObject as DynamicCommandElement;
					command.WriteXml(writer);
					entityElement.DataCommands.Remove(command);
				}
				Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
			}
		}

		/// <summary>
		/// 查询执行菜单 是否选择实体 命令 
		/// </summary>
		private void OnSelectedEnabled(object sender, EventArgs e)
		{
			OleMenuCommand menu = sender as OleMenuCommand;
			DesignerEntitiesCanvas canvas = GetItemsCanvas();
			menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem != null;
		}
		#endregion
	}
}
