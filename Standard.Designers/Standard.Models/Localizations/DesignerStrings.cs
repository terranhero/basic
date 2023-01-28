using System.Resources;
using System.Globalization;

namespace Basic.Properties
{
    /// <summary>
    /// 
    /// </summary>
    internal static class DesignerStrings
    {
        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture = CultureInfo.CurrentUICulture;

        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new ResourceManager("Basic.Localizations.Strings", typeof(DesignerStrings).Assembly);
                }
                return resourceMan;
            }
        }
        /// <summary>
        /// 获取为指定区域性本地化的 System.String 资源的值。
        /// </summary>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="cultureInfo">System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。
        /// 请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的 System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。
        /// 如果此值为 null，则使用当前线程的System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        public static string GetString(string name, CultureInfo cultureInfo)
        {
            if (cultureInfo != null)
            {
                if (cultureInfo.LCID == 2052)
                    return ResourceManager.GetString(string.Format("{0}-zh-CN", name));
                else
                    return ResourceManager.GetString(string.Format("{0}-en-US", name));
            }
            string returnValue;
            if (resourceCulture.LCID == 2052)
                returnValue = ResourceManager.GetString(string.Format("{0}-zh-CN", name));
            else
                returnValue = ResourceManager.GetString(string.Format("{0}-en-US", name));
            if (!string.IsNullOrWhiteSpace(returnValue))
                return returnValue;
            return name;
        }


        /// <summary>
        /// 获取为指定区域性本地化的 System.String 资源的值。
        /// </summary>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        public static string GetString(string name, params object[] args)
        {
            string returnValue;
            if (resourceCulture.LCID == 2052)
                returnValue = ResourceManager.GetString(string.Format("{0}-zh-CN", name));
            else
                returnValue = ResourceManager.GetString(string.Format("{0}-en-US", name));
            if (!string.IsNullOrWhiteSpace(returnValue) && args != null && args.Length > 0)
                return string.Format(returnValue, args);
            else if (!string.IsNullOrWhiteSpace(returnValue) && (args == null || args.Length == 0))
                return returnValue;
            return name;
        }

        /// <summary>
        /// 从指定的资源返回 System.IO.UnmanagedMemoryStream 对象。
        /// </summary>
        /// <param name="name">资源的名称。</param>
        /// <returns> 一个 System.IO.UnmanagedMemoryStream 对象。</returns>
        public static System.IO.Stream GetStream(string name)
        {
            return ResourceManager.GetStream(name);
        }

        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }
    }
}
