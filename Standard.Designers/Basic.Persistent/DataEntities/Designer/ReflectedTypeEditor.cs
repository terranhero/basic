using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Basic.Designer
{
    /// <summary>
    /// 属性类型编辑器
    /// </summary>
    public sealed class ReflectedTypeEditor : UITypeEditor
    {
        private PropertyTypeListBox listBox;
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
                if (editorService == null)
                {
                    return value;
                }
                if (this.listBox == null)
                {
                    this.listBox = new PropertyTypeListBox();
                }
                this.listBox.BeginEdit(editorService, provider, value);
                editorService.DropDownControl(this.listBox);
                return listBox.SelectedItem;
            }
            return value;
            //return base.EditValue(context, provider, value);
        }

        private class PropertyTypeListBox : ListBox
        {
            private IWindowsFormsEditorService _editorService;
            private readonly string[] _PrimitiveArray = new string[] { "System.Boolean", "System.Byte", "System.Byte[]", "System.Char", "System.DateTime", 
"System.DateTimeOffset", "System.Decimal","System.Double","System.Guid", 
"System.Int16","System.Int16[]","System.Int32",	"System.Int32[]","System.Int64", "System.Int64[]",
"System.Object","System.Single","System.String","System.String[]","System.TimeSpan" };
            public PropertyTypeListBox()
            {
                this.IntegralHeight = true;
            }

            internal void BeginEdit(IWindowsFormsEditorService editorService, System.IServiceProvider provider, object value)
            {
                _editorService = editorService;
                base.Items.Clear();
                //base.Items.AddRange(_PrimitiveArray);
                EnvDTE.DTE dteClass = (EnvDTE.DTE)provider.GetService(typeof(EnvDTE.DTE));
                Assumes.Present(dteClass);
                EnvDTE.ProjectItem projectItem = dteClass.Solution.FindProjectItem(dteClass.ActiveDocument.FullName);
                if (projectItem != null)
                {
                    EnvDTE.Project project = projectItem.ContainingProject;
                    IVsSolution2 vsSolution = (IVsSolution2)provider.GetService(typeof(SVsSolution));
                    Assumes.Present(vsSolution);
                    vsSolution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy);
                    vsSolution.GetGuidOfProject(hierarchy, out Guid projectGuid);
                    IVsHierarchy ivsh = VsShellUtilities.GetHierarchy(provider, projectGuid);
                    DynamicTypeService typeService = (DynamicTypeService)provider.GetService(typeof(DynamicTypeService));
                    Assumes.Present(typeService);
                    ITypeDiscoveryService discovery = typeService.GetTypeDiscoveryService(ivsh);
                    ICollection types = discovery.GetTypes(typeof(Enum), true);
                    SortedSet<string> list = new SortedSet<string>();
                    foreach (Type type in types)
                    {
                        if (type.IsPublic && type.IsEnum)
                        {
                            if (type.Namespace.Contains("System.")) { continue; }
                            else if (type.Namespace.Contains("Basic.")) { continue; }
                            else if (type.Namespace.EndsWith(".Enums")) { list.Add(type.Name); }
                            else if (type.Name.EndsWith("Enum")) { list.Add(type.Name); }
                        }
                    }
                    base.Items.AddRange(list.ToArray());
                }
                if (value != null && Items.Contains(value)) { SelectedItem = value; }
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
