using System;

namespace Basic.Loggers
{
    /// <summary>
    /// 表示一个特性，该特性用于无限制调用方对操作方法的访问。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class NoneLoggingAttribute : Attribute { }
}
