
using System;
using System.Reflection;
using System.Security;

namespace Basic.Messages
{
    /// <summary>
    /// 默认资源转换器管理类
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class MessageConverter : System.Resources.ResourceManager, IMessageConverter
    {
        #region 构造函数
        /// <summary>
        /// 使用默认值初始化 System.Resources.ResourceManager 类的新实例。
        /// </summary>
        [SecuritySafeCritical]
        public MessageConverter()
            : base("Basic.Messages.Message", typeof(MessageConverter).Assembly)
        {
        }

        /// <summary>
        /// 创建一个 System.Resources.ResourceManager，它根据指定的 System.Type 中的信息在附属程序集内查找资源。
        /// </summary>
        /// <param name="resourceSource">一个 System.Type，System.Resources.ResourceManager 从其中派生所有用于查找 .resources 文件的信息。</param>
        /// <exception cref="System.ArgumentNullException">baseName 或 assembly 参数为 null。</exception>
        [SecuritySafeCritical]
        public MessageConverter(Type resourceSource) : base(resourceSource) { }

        /// <summary>
        /// 初始化 System.Resources.ResourceManager 类的新实例，该实例使用给定的 System.Reflection.Assembly
        /// 查找从指定根名称导出的文件中包含的资源。
        /// </summary>
        /// <param name="baseName">资源的根名称。例如，名为“MyResource.en-US.resources”的资源文件的根名称为“MyResource”。</param>
        /// <param name="assembly">资源的主 System.Reflection.Assembly。</param>
        /// <exception cref="System.ArgumentNullException">baseName 或 assembly 参数为 null。</exception>
        [SecuritySafeCritical]
        public MessageConverter(string baseName, Assembly assembly) : base(baseName, assembly) { }

        /// <summary>
        /// 初始化 System.Resources.ResourceManager 类的新实例，
        /// 该实例使用给定的 System.Reflection.Assembly查找从指定根名称导出的文件中包含的资源。
        /// </summary>
        /// <param name="baseName">资源的根名称。例如，名为“MyResource.en-US.resources”的资源文件的根名称为“MyResource”。</param>
        /// <param name="assembly">资源的主 System.Reflection.Assembly。</param>
        /// <param name="usingResourceSet">要使用的自定义 System.Resources.ResourceSet 的 System.Type。如果为 null，则使用默认的运行时 System.Resources.ResourceSet。</param>
        /// <exception cref="System.ArgumentException">usingResourceset 不是 System.Resources.ResourceSet 的派生类。</exception>
        /// <exception cref="System.ArgumentNullException">baseName 或 assembly 参数为 null。</exception>
        [SecuritySafeCritical]
        public MessageConverter(string baseName, Assembly assembly, Type usingResourceSet) : base(baseName, assembly, usingResourceSet) { }
        #endregion

        /// <summary>
        /// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="culture">
        /// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
        /// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
        /// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
        /// </param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        public string GetString(string name, System.Globalization.CultureInfo culture, params object[] args)
        {
            string source = base.GetString(name, culture);
            if (args == null || args.Length == 0)
                return source;
            return string.Format(source, args);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值。
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
        public string GetString(string name, params object[] args)
        {
            string source = base.GetString(name);
            if (args == null || args.Length == 0)
                return source;
            return string.Format(source, args);
        }

        /// <summary>
        /// 表示转换器名称
        /// </summary>
        public string Name { get { return "GlobalMessagerConverter"; } }
    }
}
