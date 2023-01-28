using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Basic.Configuration;
using Microsoft;

namespace Basic.Designer
{
    /// <summary>
    /// 属性类型编辑器
    /// </summary>
    public sealed class ProjectSelectorEditor : UITypeEditor
    {
        private ProjectListBox listBox;
        /// <summary>
        /// 获取由 EditValue 方法使用的编辑器样式。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            //指定为模式窗体属性编辑器类型
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// 使用 System.Drawing.Design.UITypeEditor.GetEditStyle() 方法所指示的编辑器样式编辑指定对象的值。
        /// </summary>
        /// <param name="context">可用于获取附加上下文信息的 System.ComponentModel.ITypeDescriptorContext。</param>
        /// <param name="provider">System.IServiceProvider，此编辑器可用其来获取服务。</param>
        /// <param name="value">要编辑的对象。</param>
        /// <returns>新的对象值。如果对象的值尚未更改，则它返回的对象应与传递给它的对象相同。</returns>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService == null) { return value; }
                ProjectInfo projectInfo = value as ProjectInfo;
                if (projectInfo == null) { return value; }
                if (this.listBox == null)
                {
                    EnvDTE.DTE dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
                    this.listBox = new ProjectListBox(dteClass);
                }
                PersistentDescriptor objectDescriptor = context.Instance as PersistentDescriptor;
                PersistentConfiguration persistet = objectDescriptor.DefinitionInfo;
                this.listBox.BeginEdit(editorService, provider, persistet, projectInfo.ProjectGuid);
                editorService.DropDownControl(this.listBox);
                ProjectInfo info = (ProjectInfo)listBox.SelectedItem;
                if (info == null) { return value; }
                projectInfo.ProjectGuid = info.ProjectGuid;
                projectInfo.UniqueName = info.UniqueName;
                projectInfo.ProjectName = info.ProjectName;
                return projectInfo;
            }
            return base.EditValue(context, provider, value);
        }

        private class ProjectListBox : ListBox
        {
            private IWindowsFormsEditorService _editorService;
            private PersistentConfiguration persistentConfiguration;
            private readonly EnvDTE.DTE dteClass;
            public ProjectListBox(EnvDTE.DTE dte)
            {
                dteClass = dte;
            }
            internal void BeginEdit(IWindowsFormsEditorService editorService, IServiceProvider provider, PersistentConfiguration persistent, Guid value)
            {
                persistentConfiguration = persistent;
                _editorService = editorService;
                this.Items.Clear();
                this.Items.Add(new ProjectInfo(persistent, Guid.Empty, null, null));
                IVsSolution vsSolution = (IVsSolution)provider.GetService(typeof(IVsSolution));
                Assumes.Present(vsSolution);
                EnvDTE.ProjectItem projectItem = dteClass.Solution.FindProjectItem(dteClass.ActiveDocument.FullName);
                EnvDTE.Project itemProject = null;
                if (projectItem != null) { itemProject = projectItem.ContainingProject; }
                this.DisplayMember = "ProjectName";
                //this.ValueMember = "ProjectGuid";
                foreach (EnvDTE.Project project in dteClass.Solution.Projects)
                {
                    if (itemProject == project) { continue; }
                    IVsHierarchy hierarchy;
                    Guid projectGuid;
                    switch (project.Kind)
                    {
                        case VSLangProj.PrjKind.prjKindCSharpProject:	//项目{66A26720-8FB5-11D2-AA7E-00C04F688DDE}
                        case VSLangProj.PrjKind.prjKindVBProject:	//项目
                        case VSLangProj.PrjKind.prjKindVSAProject:	//项目
                        case VSLangProj2.PrjKind2.prjKindSDECSharpProject:	//项目
                        case VSLangProj2.PrjKind2.prjKindSDEVBProject:	//项目
                        case VSLangProj2.PrjKind2.prjKindVJSharpProject:	//项目
                            vsSolution.GetProjectOfUniqueName(project.UniqueName, out hierarchy);
                            vsSolution.GetGuidOfProject(hierarchy, out projectGuid);
                            if (projectGuid != Guid.Empty)
                            {
                                this.Items.Add(new ProjectInfo(persistent, projectGuid, project.Name, project.UniqueName));
                                if (projectGuid == value) { this.SelectedIndex = this.Items.Count - 1; }
                            }
                            break;
                        case EnvDTE.Constants.vsProjectKindSolutionItems:	//文件夹
                            foreach (EnvDTE.ProjectItem pitem in project.ProjectItems)
                            {
                                if (pitem.SubProject == null) { continue; }
                                switch (pitem.SubProject.Kind)
                                {
                                    case VSLangProj.PrjKind.prjKindCSharpProject:	//项目{66A26720-8FB5-11D2-AA7E-00C04F688DDE}
                                    case VSLangProj.PrjKind.prjKindVBProject:	//项目
                                    case VSLangProj.PrjKind.prjKindVSAProject:	//项目
                                    case VSLangProj2.PrjKind2.prjKindSDECSharpProject:	//项目
                                    case VSLangProj2.PrjKind2.prjKindSDEVBProject:	//项目
                                    case VSLangProj2.PrjKind2.prjKindVJSharpProject:	//项目
                                        vsSolution.GetProjectOfUniqueName(pitem.SubProject.UniqueName, out hierarchy);
                                        vsSolution.GetGuidOfProject(hierarchy, out projectGuid);
                                        if (projectGuid != Guid.Empty)
                                        {
                                            this.Items.Add(new ProjectInfo(persistent, projectGuid, pitem.SubProject.Name, pitem.SubProject.UniqueName));
                                            if (projectGuid == value) { this.SelectedIndex = this.Items.Count - 1; }
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }
                this.Height = this.ItemHeight * 10;
                if (this.PreferredHeight <= this.Height)
                    this.Height = this.PreferredHeight;
                //if (value != null) { SelectedValue = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnSelectedIndexChanged(EventArgs e)
            {
                base.OnSelectedIndexChanged(e);
                _editorService.CloseDropDown();
            }
        }
    }
}
