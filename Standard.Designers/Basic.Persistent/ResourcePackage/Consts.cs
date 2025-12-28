using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basic.Localizations
{
    /// <summary>
    /// VsPackage Guid 常量值。
    /// </summary>
    internal static class Consts
    {
        /// <summary>
        /// 文件类型格式
        /// </summary>
        public const uint FileFormat = 0;

        /// <summary>
        /// 文件格式名称
        /// </summary>
        public const string FileFormatName = "Localization Resource";

        /// <summary>
        /// 产品版本号
        /// </summary>
        public const string ProductVersion = "4.8.5812";

        /// <summary>
        /// 表示 Package 类 Guid 常量值。
        /// </summary>
        public const string guidPackageString = "F9316CC5-5C97-4E30-8E2E-2436DCD1B14E";

        /// <summary>
        /// 表示 Factory 类 Guid 常量值。
        /// </summary>
        public const string guidFactoryString = "B76814ED-D201-46A4-99EA-3A31588FAD03";

		  /// <summary>
		  /// 表示 Generator 类 Guid 常量值。
		  /// </summary>
		  public const string guidGeneratorString = "0C94620C-5636-4162-8B32-1FF7B5006946";

        /// <summary>
        /// 表示 Factory  类 Guid 常量值。
        /// </summary>
        public static readonly System.Guid guidFactory = new System.Guid(guidFactoryString);

        /// <summary>
        /// 表示 WindowPane 类 Guid 常量值。
        /// </summary>
        public const string guidPaneString = "D09C2641-2F4C-4C53-A336-3CE7CC513C90";

        /// <summary>
        /// 表示 EditorLogicalView 类 Guid 常量值。
        /// </summary>
        public const string guidLogicalViewString = "A1DCBEBC-9B0C-45C0-9DDC-089B61F4430B";
    }
}
