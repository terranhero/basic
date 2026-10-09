using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
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
            if (menu == null) { return; }
            menu.Enabled = menu.Visible = false;
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
                if (dteClass == null) { return; }
                Array array = dteClass.ToolWindows?.SolutionExplorer?.SelectedItems as Array;
                if (array == null || array.Length == 0) { return; }
                string kindPhysicalFile = EnvDTE.Constants.vsProjectItemKindPhysicalFile;
                foreach (EnvDTE.UIHierarchyItem item in array)
                {
                    if (item != null && item.Object is EnvDTE.ProjectItem pItem && pItem.Kind == kindPhysicalFile)
                    {
                        string name = pItem.Name ?? string.Empty;
                        if (name.EndsWith(".dpdl", StringComparison.CurrentCultureIgnoreCase) ||
                            name.EndsWith(".localresx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            menu.Enabled = menu.Visible = true;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 仅查询菜单可见性，失败时保持隐藏即可，不打断用户
                Debug.WriteLine(ex);
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

                // 1) 取 DTE 与选中项
                EnvDTE80.DTE2 dteClass = GetDTE() as EnvDTE80.DTE2;
                if (dteClass == null)
                {
                    ShowMessage("无法获取 Visual Studio 的 DTE 对象，请重新打开解决方案后重试。");
                    return;
                }
                Array array = dteClass.ToolWindows?.SolutionExplorer?.SelectedItems as Array;
                if (array == null || array.Length == 0) { return; }

                // 2) 归类：按选中文件的扩展名合并需要注册的扩展类型（避免多选重复执行）
                string kindPhysicalFile = EnvDTE.Constants.vsProjectItemKindPhysicalFile;
                bool registerDpdl = false, registerLocalResx = false;
                foreach (EnvDTE.UIHierarchyItem uihItem in array)
                {
                    if (uihItem != null && uihItem.Object is EnvDTE.ProjectItem projectItem &&
                        projectItem.Kind == kindPhysicalFile)
                    {
                        string name = projectItem.Name ?? string.Empty;
                        if (name.EndsWith(".dpdl", StringComparison.CurrentCultureIgnoreCase))
                        {
                            registerDpdl = true;
                        }
                        else if (name.EndsWith(".localresx", StringComparison.CurrentCultureIgnoreCase))
                        {
                            registerLocalResx = true;
                        }
                    }
                }
                if (!registerDpdl && !registerLocalResx) { return; }

                // 3) 解析 VS 安装目录：DTE FullName 优先，其次 vswhere
                if (!TryGetVisualStudioInstallPath(dteClass, out string vsInstallPath))
                {
                    ShowMessage("无法定位 Visual Studio 安装目录，请确认 vswhere.exe 存在后再试。");
                    return;
                }

                // 4) 权限自检：HKCR 写入需要管理员
                if (!IsProcessElevated())
                {
                    ShowMessage("注册文件扩展名需要管理员权限。\n请先关闭 Visual Studio，然后右键以管理员身份重新启动后再执行此命令。");
                    return;
                }

                // 5) 真正执行注册
                if (registerDpdl)
                {
                    RegisterFileExtensions(vsInstallPath, ".dpdl", "ASP.NET MVC 数据持久定义文件", "-218");
                    RegisterXmlFileExtensions(vsInstallPath, ".oraf", "ORACLE Config File", "-100");
                    RegisterXmlFileExtensions(vsInstallPath, ".sqlf", "SQL SERVER Config File", "-100");
                    RegisterXmlFileExtensions(vsInstallPath, ".myf", "MYSQL Config File", "-100");
                    RegisterXmlFileExtensions(vsInstallPath, ".dbf", "IBM DB2 Config File", "-100");
                    RegisterXmlFileExtensions(vsInstallPath, ".pgf", "PostgreSQL Config File", "-100");
                }
                if (registerLocalResx)
                {
                    RegisterFileExtensions(vsInstallPath, ".localresx", "ASP.NET 本地化资源文件", "-210");
                }

                ShowMessage("文件扩展名注册成功。\nVS 安装目录：" + vsInstallPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowMessage("没有写入 HKEY_CLASSES_ROOT 的权限，请以管理员身份重新启动 Visual Studio 后再执行此命令。\n" + ex.Message);
            }
            catch (System.Security.SecurityException ex)
            {
                ShowMessage("注册表访问被拒绝（HKEY_CLASSES_ROOT 可能由 SYSTEM/TrustedInstaller 持有）。\n请以管理员身份重新启动 Visual Studio。\n" + ex.Message);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        /// <summary>
        /// 解析当前运行的 Visual Studio 安装目录。
        /// 优先尝试从 DTE 的 <c>FullName</c>（devenv.exe 路径）推断；失败时退化到 <c>vswhere.exe</c>。
        /// </summary>
        /// <param name="dteClass">DTE2 实例，用于读取 <c>FullName</c>。</param>
        /// <param name="vsInstallPath">输出：VS 可执行文件所在目录。</param>
        /// <returns>解析成功返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        private static bool TryGetVisualStudioInstallPath(EnvDTE80.DTE2 dteClass, out string vsInstallPath)
        {
            vsInstallPath = null;

            // 策略 1：通过 DTE.FullName 推断（最贴近当前进程）
            try
            {
                string fullName = dteClass != null ? dteClass.FullName : null;
                if (!string.IsNullOrEmpty(fullName))
                {
                    string dir = Path.GetDirectoryName(fullName);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    {
                        vsInstallPath = dir;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("通过 DTE.FullName 推断 VS 安装目录失败：" + ex);
            }

            // 策略 2：通过 vswhere.exe 查找
            try
            {
                string pfx86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                string vswherePath = Path.Combine(pfx86Path, @"Microsoft Visual Studio\Installer\vswhere.exe");
                if (File.Exists(vswherePath))
                {
                    ProcessStartInfo psi = new ProcessStartInfo(vswherePath, "-nologo -property productPath")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    using (Process p = Process.Start(psi))
                    {
                        if (p != null)
                        {
                            string output = p.StandardOutput.ReadToEnd();
                            p.WaitForExit(3000);
                            output = (output ?? string.Empty).Trim(Environment.NewLine.ToCharArray());
                            string dir = Path.GetDirectoryName(output);
                            if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                            {
                                vsInstallPath = dir;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("通过 vswhere 解析 VS 安装目录失败：" + ex);
            }

            return false;
        }

        /// <summary>
        /// 检查当前进程是否已获得管理员权限（UAC 提权后）。
        /// </summary>
        private static bool IsProcessElevated()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    if (identity == null) { return false; }
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("管理员检测失败：" + ex);
                return false;
            }
        }

        /// <summary>
        /// 兼容保留的旧入口。内部委托到 <see cref="TryGetVisualStudioInstallPath"/>。
        /// </summary>
        private static void GetVisualStudioPath(out string vsInstallPath)
        {
            if (!TryGetVisualStudioInstallPath(null, out vsInstallPath))
            {
                vsInstallPath = null;
            }
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
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(extension, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (key == null) { throw new IOException($"无法创建或打开注册表项：HKEY_CLASSES_ROOT\\{extension}"); }
                key.SetValue("", $"VisualStudio{extension}.16.0", RegistryValueKind.String);
            }

            using (RegistryKey keyInfo = Registry.ClassesRoot.CreateSubKey($"VisualStudio{extension}.16.0", RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (keyInfo == null) { throw new IOException($"无法创建或打开注册表项：HKEY_CLASSES_ROOT\\VisualStudio{extension}.16.0"); }
                keyInfo.SetValue("", description, RegistryValueKind.String);

                using (RegistryKey defaultIconKey = keyInfo.CreateSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (defaultIconKey == null) { throw new IOException("无法创建或打开注册表项：DefaultIcon"); }
                    defaultIconKey.SetValue("", $"\"{vsInstallPath}\\msenvico.dll\",{icon}", RegistryValueKind.String);
                }
            }
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
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(extension, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (key == null) { throw new IOException($"无法创建或打开注册表项：HKEY_CLASSES_ROOT\\{extension}"); }
                key.SetValue("", $"VisualStudio{extension}.16.0", RegistryValueKind.String);
            }

            using (RegistryKey keyInfo = Registry.ClassesRoot.CreateSubKey($"VisualStudio{extension}.16.0", RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (keyInfo == null) { throw new IOException($"无法创建或打开注册表项：HKEY_CLASSES_ROOT\\VisualStudio{extension}.16.0"); }
                keyInfo.SetValue("", description, RegistryValueKind.String);

                using (RegistryKey defaultIconKey = keyInfo.CreateSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (defaultIconKey == null) { throw new IOException("无法创建或打开注册表项：DefaultIcon"); }
                    defaultIconKey.SetValue("", $"\"{vsInstallPath}\\Xml\\Microsoft.XmlEditorNeutralUI.dll\",{icon}", RegistryValueKind.String);
                }
            }
        }

    }
}
