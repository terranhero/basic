using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using MDC = Microsoft.Data.ConnectionUI;

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
    /// <summary>
    /// 数据持久命令服务类
    /// 提供在 Visual Studio 解决方案资源管理器中对特定文件类型（如 .dpdl、.localresx 等）进行注册和上下文菜单控制的功能。
    /// </summary>
    public sealed partial class PersistentService
    {
        /// <summary>
        /// 为注册文件扩展名的命令计算是否可用。
        /// 根据当前在 __Solution Explorer__ 中选择的项决定菜单命令的可见性与启用状态。
        /// 当所选项包含物理文件且扩展名为 .dpdl 或 .localresx 时，命令可见并启用。
        /// </summary>
        /// <param name="sender">触发该事件的菜单命令（应为 <see cref="OleMenuCommand"/>）。</param>
        /// <param name="e">事件参数（未使用）。</param>
        private void OnCanRegisterFileExtension(object sender, EventArgs e)
        {
            OleMenuCommand menu = sender as OleMenuCommand;
            EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
            Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
            menu.Enabled = menu.Visible = false;
            if (array == null || array.Length == 0) { return; }
            foreach (EnvDTE.UIHierarchyItem item in array)
            {
                if (item.Object is EnvDTE.ProjectItem)
                {
                    EnvDTE.ProjectItem pItem = item.Object as EnvDTE.ProjectItem;
                    if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".dpdl"))
                    {
                        menu.Enabled = menu.Visible = true; return;
                    }
                    else if (pItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile && pItem.Name.EndsWith(".localresx"))
                    {
                        menu.Enabled = menu.Visible = true; return;
                    }
                }
            }
        }

        /// <summary>
        /// 执行注册文件扩展名的命令处理逻辑。
        /// 遍历当前选中的项目，根据文件扩展名调用相应的注册方法，将扩展名与 Visual Studio 相关联并设置图标与显示名称。
        /// </summary>
        /// <param name="sender">事件发送者（通常为菜单命令）。</param>
        /// <param name="e">事件参数（未使用）。</param>
        /// <remarks>
        /// - 方法在 UI 线程上运行（调用了 <see cref="ThreadHelper.ThrowIfNotOnUIThread"/>）。
        /// - 注册操作会修改注册表（HKEY_CLASSES_ROOT），可能需要管理员权限或在受限系统上失败。
        /// - 具体注册包括针对 .dpdl 文件批量注册若干数据库相关扩展，以及对 .localresx 的注册。
        /// - 若发生异常，调用 <see cref="ShowMessage(string)"/> 显示错误信息。
        /// </remarks>
        private void OnRegisterFileExtension(object sender, EventArgs e)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
                Array array = dteClass.ToolWindows.SolutionExplorer.SelectedItems as Array;
                if (array == null || array.Length == 0) { return; }
                string kindPhysicalFile = EnvDTE.Constants.vsProjectItemKindPhysicalFile;
                string vsInstallPath = Path.GetDirectoryName(dteClass.FullName);
                foreach (EnvDTE.UIHierarchyItem uihItem in array)
                {
                    if (uihItem.Object is EnvDTE.ProjectItem)
                    {
                        EnvDTE.ProjectItem projectItem = uihItem.Object as EnvDTE.ProjectItem;
                        if (projectItem.Kind == kindPhysicalFile && projectItem.Name.EndsWith(".dpdl", StringComparison.CurrentCultureIgnoreCase))
                        {
                            RegisterFileExtensions(vsInstallPath, ".dpdl", "ASP.NET MVC 数据持久定义文件", "-218");
                            RegisterXmlFileExtensions(vsInstallPath, ".oraf", "ORACLE Config File", "-100");
                            RegisterXmlFileExtensions(vsInstallPath, ".sqlf", "SQL SERVER Config File", "-100");
                            RegisterXmlFileExtensions(vsInstallPath, ".myf", "MYSQL Config File", "-100");
                            RegisterXmlFileExtensions(vsInstallPath, ".dbf", "IBM DB2 Config File", "-100");
                            RegisterXmlFileExtensions(vsInstallPath, ".pgf", "PostgreSQL Config File", "-100");
                        }
                        else if (projectItem.Kind == kindPhysicalFile && projectItem.Name.EndsWith(".localresx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            RegisterFileExtensions(vsInstallPath, ".localresx", "ASP.NET 本地化资源文件", "-210");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        /// <summary>
        /// 使用 __vswhere.exe__ 查找当前 Visual Studio 的安装路径（productPath）。
        /// </summary>
        /// <param name="vsInstallPath">输出参数，返回 Visual Studio 可执行文件所在目录。</param>
        /// <remarks>
        /// - 依赖于安装在默认位置的 __vswhere.exe__：%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe。
        /// - 调用外部进程并读取其标准输出，请确保进程能够启动且有权限读取输出。
        /// - 返回值为 productPath 的目录（即 msenv 可执行所在目录）。
        /// </remarks>
        private static void GetVisualStudioPath(out string vsInstallPath)
        {
            string pfx86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string vswherePath = Path.Combine(pfx86Path, @"Microsoft Visual Studio\Installer\vswhere.exe");
            //ProcessStartInfo vswhereInfo = new ProcessStartInfo(vswherePath, "-nologo -all");
            //ProcessStartInfo vswhereInfo = new ProcessStartInfo(vswherePath, "-nologo -property installationPath");
            ProcessStartInfo vswhereInfo = new ProcessStartInfo(vswherePath, "-nologo -property productPath");
            vswhereInfo.CreateNoWindow = true;
            vswhereInfo.UseShellExecute = false;
            vswhereInfo.RedirectStandardOutput = true;
            vswhereInfo.RedirectStandardError = true;
            Process vswhereProceee = Process.Start(vswhereInfo);
            vsInstallPath = vswhereProceee.StandardOutput.ReadToEnd();
            vsInstallPath = vsInstallPath.Trim(Environment.NewLine.ToCharArray());
            vsInstallPath = Path.GetDirectoryName(vsInstallPath);
        }

        /// <summary>
        /// 在注册表（HKEY_CLASSES_ROOT）中为指定扩展名创建或更新类信息并设置默认图标。
        /// </summary>
        /// <param name="vsInstallPath">Visual Studio 安装目录，用于定位内置图标资源（msenvico.dll）。</param>
        /// <param name="extension">要注册的文件扩展名（含点），例如 ".dpdl"。</param>
        /// <param name="description">文件类型的显示名称（注册表默认值）。</param>
        /// <param name="icon">图标索引（字符串），将与 msenvico.dll 配合使用，例如 "-218"。</param>
        /// <remarks>
        /// - 方法会在 HKCR 下创建或更新两个键：扩展名本身（例如 ".dpdl"）和值为 "VisualStudio{extension}.16.0" 的类信息键。
        /// - 默认图标键为 "DefaultIcon"，其值指向 Visual Studio 的 msenvico.dll。
        /// - 可能需要管理员权限才能写入注册表。
        /// </remarks>
        private static void RegisterFileExtensions(string vsInstallPath, string extension, string description, string icon)
        {
            RegistryKey HKCR = Registry.ClassesRoot;
            RegistryKey key = HKCR.OpenSubKey(extension, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            if (key == null) { key = HKCR.CreateSubKey(extension, RegistryKeyPermissionCheck.ReadWriteSubTree); }
            key.SetValue("", $"VisualStudio{extension}.16.0", RegistryValueKind.String);

            RegistryKey keyInfo = HKCR.OpenSubKey($"VisualStudio{extension}.16.0", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            if (keyInfo == null) { keyInfo = HKCR.CreateSubKey($"VisualStudio{extension}.16.0", RegistryKeyPermissionCheck.ReadWriteSubTree); }
            keyInfo.SetValue("", description, RegistryValueKind.String);

            RegistryKey defaultIconKey = keyInfo.OpenSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            if (defaultIconKey == null) { defaultIconKey = keyInfo.CreateSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree); }
            defaultIconKey.SetValue("", $"\"{vsInstallPath}\\msenvico.dll\",{icon}", RegistryValueKind.String);
        }

        /// <summary>
        /// 为基于 XML 的配置文件注册扩展名并设置 XML 编辑器的默认图标。
        /// </summary>
        /// <param name="vsInstallPath">Visual Studio 安装目录，用于定位 XML 编辑器相关的 UI DLL。</param>
        /// <param name="extension">要注册的文件扩展名（含点），如 ".oraf"、".sqlf" 等。</param>
        /// <param name="description">文件类型的显示名称。</param>
        /// <param name="icon">图标索引（字符串），与 XML 编辑器的资源 DLL 一起使用。</param>
        /// <remarks>
        /// - 与 <see cref="RegisterFileExtensions(string,string,string,string)"/> 类似，但默认图标指向
        ///   "{vsInstallPath}\Xml\Microsoft.XmlEditorNeutralUI.dll"。
        /// - 适用于自定义的 XML 配置文件类型，便于在资源管理器中显示对应图标并与 Visual Studio 的 XML 编辑器关联。
        /// </remarks>
        private static void RegisterXmlFileExtensions(string vsInstallPath, string extension, string description, string icon)
        {
            RegistryKey HKCR = Registry.ClassesRoot;
            RegistryKey key = HKCR.OpenSubKey(extension, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            if (key == null) { key = HKCR.CreateSubKey(extension, RegistryKeyPermissionCheck.ReadWriteSubTree); }
            key.SetValue("", $"VisualStudio{extension}.16.0", RegistryValueKind.String);

            RegistryKey keyInfo = HKCR.OpenSubKey($"VisualStudio{extension}.16.0", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            if (keyInfo == null) { keyInfo = HKCR.CreateSubKey($"VisualStudio{extension}.16.0", RegistryKeyPermissionCheck.ReadWriteSubTree); }
            keyInfo.SetValue("", description, RegistryValueKind.String);

            RegistryKey defaultIconKey = keyInfo.OpenSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            if (defaultIconKey == null) { defaultIconKey = keyInfo.CreateSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree); }
            defaultIconKey.SetValue("", $"\"{vsInstallPath}\\Xml\\Microsoft.XmlEditorNeutralUI.dll\",{icon}", RegistryValueKind.String);
        }

    }
}
