using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
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
using Microsoft.VisualStudio.Shell;

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

        /// <summary>数据持久类快捷菜单复制代码</summary>
        public static readonly CommandID CopyCodeID = new CommandID(VsMenus.guidStandardCommandSet97, 0x2062);

        /// <summary>数据持久类快捷菜单复制数据库命令</summary>
        public static readonly CommandID CopySqlID = new CommandID(VsMenus.guidStandardCommandSet97, 0x2064);

        /// <summary>数据持久类快捷菜单查看数据库表信息</summary>
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
            menu.Enabled = menu.Visible = canvas.SelectedItem.SelectedObject is DesignerDataConditionProperty ||
                 canvas.SelectedItem.SelectedObject is DesignerDataEntityProperty;
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
            menu.Enabled = menu.Visible = canvas.SelectedItem.SelectedObject is DesignerDataConditionProperty ||
                 canvas.SelectedItem.SelectedObject is DesignerDataEntityProperty ||
                 canvas.SelectedItem.SelectedObject is DesignerDataCondition ||
                 canvas.SelectedItem.SelectedObject is DesignerDataEntity;
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
                if (pane.Content is DesignerEntitiesCanvas canvas && config.ReadJson(pane.ProjectItem))
                {
                    if (canvas.SelectedItem == null)
                    {
                        PersistentDesigner persistent = pane.GetPersistent();
                        DesignerDataEntity entityEntityElement = new DesignerDataEntity(persistent) { Guid = Guid.NewGuid() };
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
                        DesignerDataEntity entityEntityElement = designerItem.DataContext as DesignerDataEntity;
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
                if (config.ReadJson(pane.ProjectItem))
                {
                    if (canvas.SelectedItem != null)
                    {
                        DesignerDataEntity entityEntityElement = canvas.SelectedItem.DataContext as DesignerDataEntity;
                        DesignerStaticCommand dataCommand = canvas.SelectedItem.SelectedObject as DesignerStaticCommand;
                        EnvDTE80.DTE2 dteClass = this.GetDTE();
                        CommandsWindow window1 = new CommandsWindow(dteClass, entityEntityElement, dataCommand);
                        if (window1.ShowModal() == true) { }
                    }
                }
                //else if (config.ReadConfig(pane.ProjectItem))
                //{
                //    if (canvas.SelectedItem != null)
                //    {
                //        DataEntityElement entityEntityElement = canvas.SelectedItem.DataContext as DataEntityElement;
                //        StaticCommandElement dataCommand = canvas.SelectedItem.SelectedObject as StaticCommandElement;
                //        EnvDTE80.DTE2 dteClass = this.GetDTE();
                //        CommandsWindow window1 = new CommandsWindow(dteClass, entityEntityElement, dataCommand);
                //        if (window1.ShowModal() == true) { }
                //    }
                //}
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
            menu.Enabled = menu.Visible = canvas.SelectedItem.SelectedObject is DesignerStaticCommand;
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
                if (config.ReadJson(pane.ProjectItem)) { return; }
                //else if (config.ReadConfig(pane.ProjectItem) == false) { return; }
                if (canvas.SelectedItem != null)
                {
                    DesignerDataEntity entityEntityElement = canvas.SelectedItem.DataContext as DesignerDataEntity;
                    DesignerDataCommand dataCommand = canvas.SelectedItem.SelectedObject as DesignerDataCommand;
                    if (dataCommand is DesignerStaticCommand)
                    {
                        DesignerStaticCommand staticCommand = dataCommand as DesignerStaticCommand;
                        if (TransactSqlResolver.PasteStaticCommand(staticCommand, text)) { Clipboard.Clear(); }
                    }
                    else
                    {
                        DesignerStaticCommand staticCommand = new DesignerStaticCommand(entityEntityElement);
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
                    PersistentDesigner persistent = this.GetPersistentConfiguration();
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
                if (config.ReadJson(pane.ProjectItem) == false) { return; }
                if (canvas.SelectedItem != null)
                {
                    DesignerDataEntity entityEntityElement = canvas.SelectedItem.DataContext as DesignerDataEntity;
                    if (canvas.SelectedItem.SelectedObject is DesignerDynamicCommand)
                    {
                        DesignerDynamicCommand dynamicCommand = canvas.SelectedItem.SelectedObject as DesignerDynamicCommand;
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
                        DesignerDynamicCommand dynamicCommand = new DesignerDynamicCommand(entityEntityElement)
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
                    PersistentDesigner persistent = this.GetPersistentConfiguration();
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
            PersistentDesigner persistent = this.GetPersistentConfiguration();
            if (persistent.TableInfo.IsEmpty) { menu.Enabled = menu.Visible = false; return; }
            menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem != null &&
                 canvas.SelectedItem.SelectedObject is DesignerDataCommand &&
                 (!(canvas.SelectedItem.SelectedObject is DesignerDynamicCommand));
        }

        private void OnUpdateCommand(object sender, EventArgs e)
        {
            try
            {
                DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
                if (canvas == null) { return; }
                PersistentPane pane = this.GetPersistentPane();
                PersistentDesigner persistent = this.GetPersistentConfiguration();
                ConnectionConfiguration config = new ConnectionConfiguration(this);
                if (config.ReadJson(pane.ProjectItem) == false) { return; }
                SetWaitCursor();
                if (canvas.SelectedItem != null)
                {
                    DesignerEntity selectedItem = canvas.SelectedItem;
                    if (selectedItem.SelectedObject is DesignerStaticCommand)
                    {
                        DesignerStaticCommand command = selectedItem.SelectedObject as DesignerStaticCommand;
                        if (command.Kind == ConfigurationTypeEnum.AddNew)
                            persistent.TableInfo.CreateInsertSqlStruct(command);
                        else if (command.Kind == ConfigurationTypeEnum.Modify)
                            persistent.TableInfo.CreateUpdateSqlStruct(command);
                        else if (command.Kind == ConfigurationTypeEnum.Remove)
                            persistent.TableInfo.CreateDeleteSqlStruct(command);
                        else if (command.Kind == ConfigurationTypeEnum.SelectByKey)
                            persistent.TableInfo.CreateSelectByPKeySqlStruct(command);
                    }
                    else if (selectedItem.SelectedObject is DesignerDynamicCommand)
                    {
                        DesignerDynamicCommand dataCommand = selectedItem.SelectedObject as DesignerDynamicCommand;
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
            PersistentDesigner persistent = this.GetPersistentConfiguration();
            menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem == null && !persistent.TableInfo.IsEmpty;
        }

        private void OnUpdateTable(object sender, EventArgs e)
        {
            try
            {
                ConnectionConfiguration configInfo = new ConnectionConfiguration(this);
                PersistentPane pane = this.GetPersistentPane();
                PersistentDesigner persistent = pane.GetPersistent();
                if (configInfo.ReadJson(pane.ProjectItem) == false) { return; }
                if (persistent.TableInfo.IsEmpty)
                {
                    new InitializationWindow(persistent).ShowModal();
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
                 canvas.SelectedItem.SelectedObject is DesignerDynamicCommand;
        }

        private void OnUpdateCondition(object sender, EventArgs e)
        {
            try
            {
                SetWaitCursor();
                DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
                PersistentPane pane = this.GetPersistentPane();
                PersistentDesigner persistent = this.GetPersistentConfiguration();
                DesignTableInfo _TableInfo = persistent.TableInfo;
                DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
                if (canvas.SelectedItem != null && canvas.SelectedItem.SelectedObject is DesignerDynamicCommand)
                {
                    DesignerDataEntity entity = canvas.SelectedItem.DataContext as DesignerDataEntity;
                    ConnectionConfiguration config = new ConnectionConfiguration(this);
                    if (config.ReadJson(pane.ProjectItem))
                    {
                        DesignerDynamicCommand dynamicCommand = canvas.SelectedItem.SelectedObject as DesignerDynamicCommand;
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
            menu.Enabled = menu.Visible = canvas != null && canvas.SelectedItem == null;
        }
        /// <summary>显示数据库字典编辑</summary>
        private void OnShowColumns(object sender, EventArgs e)
        {
            try
            {
                PersistentPane pane = this.GetPersistentPane();
                //DesignerEntitiesCanvas canvas = pane.Content as DesignerEntitiesCanvas;
                PersistentDesigner persistent = pane.GetPersistent();
                DesignTableInfo _TableInfo = persistent.TableInfo;

                //asyncPackage.GetDialogPage()

                //UITypeEditor editor = (UITypeEditor)TypeDescriptor.GetEditor(typeof(CollectionEditor), typeof(UITypeEditor));
                //var context = new TypeDescriptorContext(asyncPackage, typeof(CollectionEditor));
                //var editValue = editor.EditValue(context, asyncPackage, null);
                //DTE dte = (DTE)asyncPackage.GetGlobalService(typeof(SDTE));
                //hwnd = dte?.MainWindow?.HWnd ?? IntPtr.Zero;
                //_DteClass.Windows;
                ColumnsWindow window = new ColumnsWindow(this, _TableInfo.Columns);
                //window.Owner = new Window();
                if (window.ShowModal() == true)
                {

                }
                //CollectionEditor editor = new CollectionEditor(typeof(DesignColumnCollection));
                //UITypeEditor editor = (UITypeEditor)TypeDescriptor.GetEditor(typeof(CollectionEditor), typeof(UITypeEditor));
                //object editValue = editor.EditValue(asyncPackage, _TableInfo.Columns);
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
                PersistentDesigner persistent = pane.GetPersistent();
                DesignTableInfo _TableInfo = persistent.TableInfo;
                DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
                DesignerDataEntity entity = designerEntity.DataContext as DesignerDataEntity;
                if (designerEntity.SelectedObject is DesignerDataConditionProperty || designerEntity.SelectedObject is DesignerDataCondition)
                {
                    _TableInfo.CreateDataConditionElement(entity.Condition);
                }
                else if (designerEntity.SelectedObject is DesignerStaticCommand)
                {
                    DesignerStaticCommand command = designerEntity.SelectedObject as DesignerStaticCommand;
                    if (command.Kind == ConfigurationTypeEnum.AddNew)
                        _TableInfo.CreateInsertSqlStruct(command);
                    else if (command.Kind == ConfigurationTypeEnum.Modify)
                        _TableInfo.CreateUpdateSqlStruct(command);
                    else if (command.Kind == ConfigurationTypeEnum.Remove)
                        _TableInfo.CreateDeleteSqlStruct(command);
                    else if (command.Kind == ConfigurationTypeEnum.SelectByKey)
                        _TableInfo.CreateSelectByPKeySqlStruct(command);
                }
                else if (designerEntity.SelectedObject is DesignerDynamicCommand)
                {
                    ConnectionConfiguration config = new ConnectionConfiguration(this);
                    if (config.ReadJson(pane.ProjectItem))
                    {
                        DesignerDynamicCommand dynamicCommand = designerEntity.SelectedObject as DesignerDynamicCommand;
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
            PersistentDesigner persistent = canvas.GetPersistent();
            menu.Enabled = menu.Visible = canvas != null && persistent.TableInfo.Columns.Count > 0;
        }

        private void OnUpdateEntities(object sender, EventArgs e)
        {
            try
            {
                DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
                PersistentDesigner persistent = canvas.GetPersistent();
                if (persistent.TableInfo.Columns.Count == 0)
                {
                    WriteToOutput("缺少数据表字段信息，请右击弹出菜单执行\"更新数据表\"命令");
                    return;
                }

                DesignerDataEntityCollection entities = persistent.DataEntities;
                DesignerDataEntity newEntity = entities.FirstOrDefault(m => m.DataCommands.Any(p => p.Kind == ConfigurationTypeEnum.AddNew));
                DesignerDataEntity selectEntity = entities.FirstOrDefault(m => m.DataCommands.Any(p => p.Kind == ConfigurationTypeEnum.SearchTable));
                DesignerDataEntity updateEntity = entities.FirstOrDefault(m => m.DataCommands.Any(p => p.Kind == ConfigurationTypeEnum.Modify));
                DesignerDataEntity deleteEntity = entities.FirstOrDefault(m => m.DataCommands.Any(p => p.Kind == ConfigurationTypeEnum.Remove));
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
                        selectEntity = new DesignerDataEntity(persistent) { Guid = Guid.NewGuid() };
                        selectEntity.DesignerInfo.Left = 12;
                        selectEntity.DesignerInfo.Top = 22.0;
                        selectEntity.DesignerInfo.Expander = true;
                        persistent.DataEntities.Add(selectEntity);
                        DesignerDynamicCommand dynamicCommand = new DesignerDynamicCommand(selectEntity);
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
                        selectEntity = new DesignerDataEntity(persistent) { Guid = Guid.NewGuid() };
                        selectEntity.DesignerInfo.Left = 12;
                        selectEntity.DesignerInfo.Top = 22.0;
                        selectEntity.DesignerInfo.Expander = true;
                        persistent.DataEntities.Add(selectEntity);
                    }
                    if (notExistSelectTable)
                    {
                        DesignerDynamicCommand dynamicCommand = new DesignerDynamicCommand(selectEntity);
                        persistent.TableInfo.CreateSearchSqlStruct(selectEntity, dynamicCommand);
                        selectEntity.DataCommands.Add(dynamicCommand);
                    }
                    if (newEntity == null)
                    {
                        DesignerStaticCommand staticCommand = new DesignerStaticCommand(selectEntity);
                        persistent.TableInfo.CreateInsertSqlStruct(selectEntity, staticCommand);
                        selectEntity.DataCommands.Add(staticCommand);
                    }
                    if (updateEntity == null)
                    {
                        DesignerStaticCommand staticCommand = new DesignerStaticCommand(selectEntity);
                        persistent.TableInfo.CreateUpdateSqlStruct(selectEntity, staticCommand);
                        selectEntity.DataCommands.Add(staticCommand);
                    }
                    if (deleteEntity == null)
                    {
                        DesignerStaticCommand staticCommand = new DesignerStaticCommand(selectEntity);
                        persistent.TableInfo.CreateDeleteSqlStruct(selectEntity, staticCommand);
                        selectEntity.DataCommands.Add(staticCommand);
                    }
                }
                else if (persistent.GenerateMode == GenerateActionEnum.Multiple)
                {
                    if (selectEntity == null)
                    {
                        selectEntity = new DesignerDataEntity(persistent) { Guid = Guid.NewGuid() };
                        selectEntity.DesignerInfo.Left = 470.0;
                        selectEntity.DesignerInfo.Top = 22.0;
                        selectEntity.DesignerInfo.Expander = true;
                        DesignerDynamicCommand dynamicCommand = new DesignerDynamicCommand(selectEntity);
                        persistent.TableInfo.CreateSearchSqlStruct(selectEntity, dynamicCommand);
                        selectEntity.DataCommands.Add(dynamicCommand);
                        persistent.DataEntities.Add(selectEntity);
                    }
                    if (newEntity == null)
                    {
                        newEntity = new DesignerDataEntity(persistent) { Guid = Guid.NewGuid() };
                        newEntity.DesignerInfo.Left = 12;
                        newEntity.DesignerInfo.Top = 22.0;
                        newEntity.DesignerInfo.Expander = true;
                        DesignerStaticCommand staticCommand = new DesignerStaticCommand(newEntity);
                        persistent.TableInfo.CreateInsertSqlStruct(newEntity, staticCommand);
                        newEntity.DataCommands.Add(staticCommand);
                        persistent.DataEntities.Add(newEntity);
                    }
                    if (updateEntity == null)
                    {
                        updateEntity = new DesignerDataEntity(persistent) { Guid = Guid.NewGuid() };
                        updateEntity.DesignerInfo.Left = 240;
                        updateEntity.DesignerInfo.Top = 22.0;
                        updateEntity.DesignerInfo.Expander = true;
                        DesignerStaticCommand staticCommand = new DesignerStaticCommand(updateEntity);
                        persistent.TableInfo.CreateUpdateSqlStruct(updateEntity, staticCommand);
                        updateEntity.DataCommands.Add(staticCommand);
                        persistent.DataEntities.Add(updateEntity);
                    }
                    if (deleteEntity == null)
                    {
                        deleteEntity = new DesignerDataEntity(persistent) { Guid = Guid.NewGuid() };
                        deleteEntity.DesignerInfo.Left = 12;
                        deleteEntity.DesignerInfo.Top = 292.0;
                        deleteEntity.DesignerInfo.Expander = true;
                        DesignerStaticCommand staticCommand = new DesignerStaticCommand(deleteEntity);
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
            PersistentDesigner persistent = GetPersistentConfiguration();
            menu.Enabled = menu.Visible = false;
            if (canvas != null && canvas.SelectedItem != null)
            {
                DesignerEntity item = canvas.SelectedItem;
                menu.Enabled = menu.Visible = true;
                if (item.SelectedObject is DesignerDataCommand)
                    menu.Enabled = menu.Visible = (persistent != null && persistent.GenerateContext);
            }
        }

        private void OnEditCode(object sender, EventArgs e)
        {
            DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
            PersistentPane _PersistentPane = this.GetPersistentPane();
            if (canvas.SelectedItem == null) { return; }
            DesignerEntity item = canvas.SelectedItem;
            if (item.SelectedObject is DesignerDataEntityProperty)
            {
                DesignerDataEntityProperty property = item.SelectedObject as DesignerDataEntityProperty;
                _PersistentPane.EditDataEntityCode(item.DataContext as DesignerDataEntity, property);
            }
            else if (item.SelectedObject is DesignerDataConditionProperty)
            {
                DesignerDataConditionProperty property = item.SelectedObject as DesignerDataConditionProperty;
                _PersistentPane.EditConditionCode(item.DataContext as DesignerDataEntity, property);
            }
            else if (item.SelectedObject is DesignerDataCommand)
            {
                _PersistentPane.EditCommandCode(item.SelectedObject as DesignerDataCommand, item.DataContext as DesignerDataEntity);
            }
            else if (item.SelectedObject is DesignerDataEntity)
            {
                _PersistentPane.EditDataEntityCode(item.DataContext as DesignerDataEntity, null);
            }
            else if (item.SelectedObject is DesignerDataCondition)
            {
                _PersistentPane.EditConditionCode(item.DataContext as DesignerDataEntity, null);
            }
        }

        private void OnCanEditAsyncCode(object sender, EventArgs e)
        {
            OleMenuCommand menu = sender as OleMenuCommand;
            DesignerEntitiesCanvas canvas = GetItemsCanvas();
            PersistentDesigner persistent = GetPersistentConfiguration();
            menu.Enabled = menu.Visible = false;
            if (canvas != null && canvas.SelectedItem != null)
            {
                DesignerEntity item = canvas.SelectedItem;
                menu.Enabled = menu.Visible = true;
                if (item.SelectedObject is DesignerDataCommand)
                    menu.Enabled = menu.Visible = (persistent != null && persistent.GenerateContext);
            }
        }

        private void OnEditAsyncCode(object sender, EventArgs e)
        {
            DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
            PersistentPane _PersistentPane = this.GetPersistentPane();
            if (canvas.SelectedItem == null) { return; }
            DesignerEntity item = canvas.SelectedItem;
            if (item.SelectedObject is DesignerDataEntityProperty)
            {
                DesignerDataEntityProperty property = item.SelectedObject as DesignerDataEntityProperty;
                _PersistentPane.EditDataEntityCode(item.DataContext as DesignerDataEntity, property);
            }
            else if (item.SelectedObject is DesignerDataConditionProperty)
            {
                DesignerDataConditionProperty property = item.SelectedObject as DesignerDataConditionProperty;
                _PersistentPane.EditConditionCode(item.DataContext as DesignerDataEntity, property);
            }
            else if (item.SelectedObject is DesignerDataCommand)
            {
                _PersistentPane.EditCommandCode(item.SelectedObject as DesignerDataCommand, item.DataContext as DesignerDataEntity);
            }
            else if (item.SelectedObject is DesignerDataEntity)
            {
                _PersistentPane.EditDataEntityCode(item.DataContext as DesignerDataEntity, null);
            }
            else if (item.SelectedObject is DesignerDataCondition)
            {
                _PersistentPane.EditConditionCode(item.DataContext as DesignerDataEntity, null);
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
                menu.Enabled = menu.Visible = item.SelectedObject is DesignerDataCommand;
            }
        }

        private void OnCopySql(object sender, EventArgs e)
        {
            DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
            if (canvas.SelectedItem == null) { return; }
            DesignerEntity item = canvas.SelectedItem as DesignerEntity;
            if (item == null) { return; }
            if (item.SelectedObject is DesignerDynamicCommand)
            {
                DesignerDynamicCommand dynamicCommand = item.SelectedObject as DesignerDynamicCommand;
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
            else if (item.SelectedObject is DesignerStaticCommand)
            {
                DesignerStaticCommand staticCommand = item.SelectedObject as DesignerStaticCommand;
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
            PersistentDesigner persistent = GetPersistentConfiguration();
            menu.Enabled = menu.Visible = false;
            if (canvas != null && canvas.SelectedItem != null)
            {
                DesignerEntity item = canvas.SelectedItem;
                if (item.SelectedObject is DesignerDataCommand)
                    menu.Enabled = menu.Visible = (persistent != null && !persistent.GenerateContext);
            }

        }

        private void OnCopyCode(object sender, EventArgs e)
        {
            DesignerEntitiesCanvas canvas = this.GetItemsCanvas();
            PersistentPane pane = this.GetPersistentPane();
            PersistentDesigner persistent = GetPersistentConfiguration();
            if (canvas.SelectedItem == null) { return; }
            DesignerEntity item = canvas.SelectedItem as DesignerEntity;
            if (item == null) { return; }
            if (item.SelectedObject is DesignerDataCommand)
            {
                DesignerDataCommand command = item.SelectedObject as DesignerDataCommand;
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
                DesignerDataEntity entity = canvas.SelectedItem.DataContext as DesignerDataEntity;
                string confirmMessage = string.Format("确定要删除\"{0}\"实体对象,删除对象将同时删除对象关联的命令？", entity.ClassName);
                if (Confirm(confirmMessage))//确定
                {
                    PersistentDesigner persistent = this.GetPersistentConfiguration();
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
                    PersistentDesigner persistent = this.GetPersistentConfiguration();
                    DesignerDataEntity entityElement = new DesignerDataEntity(persistent) { Guid = Guid.NewGuid() };
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
                    DesignerDataEntity entityElement = designerEntity.DataContext as DesignerDataEntity;
                    using (StringReader strReader = new StringReader(text))
                    {
                        using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
                        {
                            DesignerDataEntityProperty property = new DesignerDataEntityProperty(entityElement);
                            property.ReadXml(reader);
                            if (designerEntity.SelectedObject is DesignerDataEntityProperty)
                            {
                                int index = entityElement.Properties.IndexOf(designerEntity.SelectedObject as DesignerDataEntityProperty);
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
                    DesignerDataEntity entityElement = designerEntity.DataContext as DesignerDataEntity;
                    using (StringReader strReader = new StringReader(text))
                    {
                        using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
                        {
                            DesignerDataConditionProperty property = new DesignerDataConditionProperty(entityElement.Condition);
                            property.ReadXml(reader);
                            if (designerEntity.SelectedObject is DesignerDataConditionProperty)
                            {
                                int index = entityElement.Condition.Arguments.IndexOf(designerEntity.SelectedObject as DesignerDataConditionProperty);
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
                    DesignerDataEntity entityElement = designerEntity.DataContext as DesignerDataEntity;
                    using (StringReader strReader = new StringReader(text))
                    {
                        using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
                        {
                            DesignerStaticCommand command = new DesignerStaticCommand(entityElement);
                            command.ReadXml(reader);
                            entityElement.DataCommands.Add(command);
                            Clipboard.Clear();
                        }
                    }
                }
                else if (canvas.SelectedItem != null && expressCode == ClipboardFormat.PersistentDynamicCommandFormat)
                {
                    DesignerEntity designerEntity = canvas.SelectedItem as DesignerEntity;
                    DesignerDataEntity entityElement = designerEntity.DataContext as DesignerDataEntity;
                    using (StringReader strReader = new StringReader(text))
                    {
                        using (XmlReader reader = XmlReader.Create(strReader, new XmlReaderSettings() { IgnoreComments = true }))
                        {
                            DesignerDynamicCommand command = new DesignerDynamicCommand(entityElement);
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
                if (config.ReadJson(pane.ProjectItem) == false) { return; }
                if (canvas.SelectedItem != null)
                {
                    DesignerDataEntity entityEntityElement = canvas.SelectedItem.DataContext as DesignerDataEntity;
                    if (canvas.SelectedItem.SelectedObject is DesignerDynamicCommand)
                    {
                        DesignerDynamicCommand dynamicCommand = canvas.SelectedItem.SelectedObject as DesignerDynamicCommand;
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
                        DesignerDynamicCommand dynamicCommand = new DesignerDynamicCommand(entityEntityElement)
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
                    PersistentDesigner persistent = this.GetPersistentConfiguration();
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
            DesignerDataEntity entityElement = item.DataContext as DesignerDataEntity;
            if (item.SelectedObject == null || item.SelectedObject is DesignerDataEntity || item.SelectedObject is DesignerDataCondition)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentDataEntityFormat);
                    entityElement.WriteXml(writer);
                }
                Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
            }
            else if (item.SelectedObject is DesignerDataConditionProperty)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentConditionPropertyFormat);
                    DesignerDataConditionProperty property = item.SelectedObject as DesignerDataConditionProperty;
                    property.WriteXml(writer);
                }
                Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
            }
            else if (item.SelectedObject is DesignerDataEntityProperty)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentEntityPropertyFormat);
                    DesignerDataEntityProperty property = item.SelectedObject as DesignerDataEntityProperty;
                    property.WriteXml(writer);
                }
                Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
            }
            else if (item.SelectedObject is DesignerStaticCommand)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentStaticCommandFormat);
                    DesignerStaticCommand command = item.SelectedObject as DesignerStaticCommand;
                    command.WriteXml(writer);
                }
                Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
            }
            else if (item.SelectedObject is DesignerDynamicCommand)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentDynamicCommandFormat);
                    DesignerDynamicCommand command = item.SelectedObject as DesignerDynamicCommand;
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
            DesignerDataEntity entityElement = item.DataContext as DesignerDataEntity;
            if (item.SelectedObject == null || item.SelectedObject is DesignerDataEntity || item.SelectedObject is DesignerDataCondition)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentDataEntityFormat);
                    entityElement.WriteXml(writer);
                }
                PersistentDesigner persistent = this.GetPersistentConfiguration();
                persistent.DataEntities.Remove(entityElement);
                Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
            }
            else if (item.SelectedObject is DesignerDataConditionProperty)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentConditionPropertyFormat);
                    DesignerDataConditionProperty property = item.SelectedObject as DesignerDataConditionProperty;
                    property.WriteXml(writer);
                    entityElement.Condition.Arguments.Remove(property);
                }
                Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
            }
            else if (item.SelectedObject is DesignerDataEntityProperty)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentEntityPropertyFormat);
                    DesignerDataEntityProperty property = item.SelectedObject as DesignerDataEntityProperty;
                    property.WriteXml(writer);
                    entityElement.Properties.Remove(property);
                }
                Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
            }
            else if (item.SelectedObject is DesignerStaticCommand)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentStaticCommandFormat);
                    DesignerStaticCommand command = item.SelectedObject as DesignerStaticCommand;
                    command.WriteXml(writer);
                    entityElement.DataCommands.Remove(command);
                }
                Clipboard.SetText(builder.ToString(), TextDataFormat.Xaml);
            }
            else if (item.SelectedObject is DesignerDynamicCommand)
            {
                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    builder.Append(ClipboardFormat.PersistentDynamicCommandFormat);
                    DesignerDynamicCommand command = item.SelectedObject as DesignerDynamicCommand;
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
