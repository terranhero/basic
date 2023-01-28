
namespace Basic.Messages
{
    /// <summary>
    /// 消息转换器接口
    /// </summary>
    public interface IMessageConverter
    {
        /// <summary>
        /// 表示转换器名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取为指定区域性本地化的 System.String 资源的值。
        /// </summary>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="culture">
        /// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。请注意，如果尚未为此区域性本地化该资源，则查找将使用当前线程的
        /// System.Globalization.CultureInfo.Parent 属性回退，并在非特定语言区域性中查找后停止。如果此值为 null，则使用当前线程的
        /// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
        /// </param>
        /// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
        [System.Security.SecuritySafeCritical]
        string GetString(string name, System.Globalization.CultureInfo culture);

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="name">要获取的资源名。</param>
        /// <returns> 针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        [System.Security.SecuritySafeCritical]
        string GetString(string name);

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
        [System.Security.SecuritySafeCritical]
        string GetString(string name, System.Globalization.CultureInfo culture, params object[] args);

        /// <summary>
        /// 返回指定的 System.String 资源的值，使用指定的参数替换资源中空缺的值
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>为指定区域性本地化的资源的值。如果不可能有最佳匹配，则返回 null。</returns>
        [System.Security.SecuritySafeCritical]
        string GetString(string name, params object[] args);

        /// <summary>
        /// 从指定的资源返回 System.IO.UnmanagedMemoryStream 对象。
        /// </summary>
        /// <param name="name">资源的名称。</param>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是 System.IO.MemoryStream 对象。</exception>
        /// <exception cref="System.ArgumentNullException">name 为 null。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        /// <returns>一个 System.IO.Stream 对象。</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        System.IO.UnmanagedMemoryStream GetStream(string name);

        /// <summary>
        /// 使用指定的区域性从指定的资源返回 System.IO.UnmanagedMemoryStream 对象。
        /// </summary>
        /// <param name="name">资源的名称。</param>
        /// <param name="culture"> System.Globalization.CultureInfo 对象，它指定用于资源查找的区域性。如果 culture 为 null，则使用当前线程的区域性。</param>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是 System.IO.MemoryStream 对象。</exception>
        /// <exception cref="System.ArgumentNullException">name 为 null。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        /// <returns> 一个 System.IO.Stream 对象。</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        System.IO.UnmanagedMemoryStream GetStream(string name, System.Globalization.CultureInfo culture);

        /// <summary>
        /// 返回指定的 System.Object 资源的值。
        /// </summary>
        /// <param name="name">要获取的资源名。</param>
        /// <exception cref="System.ArgumentNullException">name 为 null。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。资源值可以为 null。</returns>
        object GetObject(string name);

        /// <summary>
        /// 返回指定的 System.Object 资源的值。
        /// </summary>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="culture">
        /// System.Globalization.CultureInfo 对象，它表示资源被本地化为的区域性。注意，如果尚未为该区域性本地化此资源，则查找将使用区域性的
        /// System.Globalization.CultureInfo.Parent 属性回退，并在签入非特定语言区域性后停止。如果该值为 null，则使用区域性的
        /// System.Globalization.CultureInfo.CurrentUICulture 属性获取 System.Globalization.CultureInfo。
        /// </param>
        /// <exception cref="System.ArgumentNullException">name 为 null。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。资源值可以为 null。</returns>
        object GetObject(string name, System.Globalization.CultureInfo culture);
    }
}
