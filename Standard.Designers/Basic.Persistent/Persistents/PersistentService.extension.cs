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
    /// <summary>数据持久命令服务类</summary>
    public sealed partial class PersistentService
    {
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
