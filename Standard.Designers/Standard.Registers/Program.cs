using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace Standard.Registers
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				GetVisualStudioPath(out string vsInstallPath);
				RegisterFileExtensions(vsInstallPath, ".dpdl", "ASP.NET MVC 数据持久定义文件", "-218");
				RegisterFileExtensions(vsInstallPath, ".localresx", "ASP.NET 本地化资源文件", "-210");
				RegisterXmlFileExtensions(vsInstallPath, ".oraf", "ORACLE Config File", "-100");
				RegisterXmlFileExtensions(vsInstallPath, ".sqlf", "SQL SERVER Config File", "-100");
				RegisterXmlFileExtensions(vsInstallPath, ".myf", "MYSQL Config File", "-100");
				RegisterXmlFileExtensions(vsInstallPath, ".dbf", "IBM DB2 Config File", "-100");
				RegisterXmlFileExtensions(vsInstallPath, ".pgf", "PostgreSQL Config File", "-100");
			}
			else if (args != null && args.Length > 0)
			{
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

		private static void UnregisterFileExtensions(string extension, string description, string icon)
		{
			RegistryKey HKCR = Registry.ClassesRoot;
			GetVisualStudioPath(out string vsInstallPath);
			HKCR.DeleteSubKey(extension);
			RegistryKey key = HKCR.OpenSubKey(extension, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
			if (key == null) { key = HKCR.CreateSubKey(extension, RegistryKeyPermissionCheck.ReadWriteSubTree); }
			key.SetValue("", $"VisualStudio{extension}.16.0", RegistryValueKind.String);

			RegistryKey keyInfo = HKCR.OpenSubKey($"VisualStudio{extension}.16.0", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
			if (keyInfo == null) { keyInfo = HKCR.CreateSubKey($"VisualStudio{extension}.16.0", RegistryKeyPermissionCheck.ReadWriteSubTree); }
			keyInfo.SetValue("", description, RegistryValueKind.String);

			RegistryKey defaultIconKey = keyInfo.OpenSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
			if (defaultIconKey == null) { defaultIconKey = keyInfo.CreateSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree); }
			defaultIconKey.SetValue("", $"\"{vsInstallPath}\\msenvico.dll\", {icon}", RegistryValueKind.String);
		}
	}
}
