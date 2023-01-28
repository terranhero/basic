
using System;
using System.Globalization;
using System.Web.Mvc;
using Basic.Messages;

namespace Basic.MvcLibrary
{
    /// <summary>
    /// 多语言编码
    /// </summary>
    public static class HtmlHelpString
    {
        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="controller">此方法扩展的 Controller 控制器实例。</param>
        /// <param name="sourceName">要获取的资源名。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static void AddModelError(this System.Web.Mvc.Controller controller, string sourceName, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(sourceName))
            {
                System.Globalization.CultureInfo culture = controller.GetCultureInfo();
                string value = MessageContext.GetString(sourceName, culture, args);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    controller.ModelState.AddModelError(string.Empty, value);
                }
            }
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="controller">此方法扩展的 Controller 控制器实例。</param>
        /// <param name="name">模型属性名称。</param>
        /// <param name="sourceName">要获取的资源名。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static void AddModelError(this System.Web.Mvc.Controller controller, string name, string sourceName, params object[] args)
        {
            if (!string.IsNullOrWhiteSpace(sourceName))
            {
                System.Globalization.CultureInfo culture = controller.GetCultureInfo();
                string value = MessageContext.GetString(sourceName, culture, args);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    controller.ModelState.AddModelError(name, value);
                }
            }
        }

        /// <summary>
        /// 获取客户端语言
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        public static System.Globalization.CultureInfo GetCultureInfo(this System.Web.Mvc.HtmlHelper html)
        {
            return MessageCulture.GetCultureInfo(html.ViewContext.HttpContext.Request);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="controller">此方法扩展的 Controller 控制器实例。</param>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static string GetString(this System.Web.Mvc.ControllerBase controller, string name, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = controller.GetCultureInfo();
            return MessageContext.GetString(name, culture, args);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="controller">此方法扩展的 Controller 控制器实例。</param>
        /// <param name="name">要获取的资源名。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static string GetString(this System.Web.Mvc.ControllerBase controller, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = controller.GetCultureInfo();
            return MessageContext.GetString(name, culture);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="controller">此方法扩展的 Controller 控制器实例。</param>
        /// <param name="converterName">消息转换器名称</param>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static string GetString(this System.Web.Mvc.ControllerBase controller, string converterName, string name, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = controller.GetCultureInfo();
            return Basic.Messages.MessageContext.GetString(converterName, name, culture, args);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="controller">此方法扩展的 Controller 控制器实例。</param>
        /// <param name="converterName">消息转换器名称</param>
        /// <param name="name">要获取的资源名。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static string GetString(this System.Web.Mvc.ControllerBase controller, string converterName, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = controller.GetCultureInfo();
            return Basic.Messages.MessageContext.GetString(converterName, name, culture);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="converterName">消息转换器名称</param>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static string GetString(this System.Web.Mvc.HtmlHelper html, string converterName, string name, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = html.GetCultureInfo();
            return Basic.Messages.MessageContext.GetString(converterName, name, culture, args);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="converterName">消息转换器名称</param>
        /// <param name="name">要获取的资源名。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static string GetString(this System.Web.Mvc.HtmlHelper html, string converterName, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = html.GetCultureInfo();
            return Basic.Messages.MessageContext.GetString(converterName, name, culture);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static string GetString(this System.Web.Mvc.HtmlHelper html, string name, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = html.GetCultureInfo();
            return Basic.Messages.MessageContext.GetString(name, culture, args);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="name">要获取的资源名。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static string GetString(this System.Web.Mvc.HtmlHelper html, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = html.GetCultureInfo();
            return Basic.Messages.MessageContext.GetString(name, culture);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="name">要获取的资源名。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static MvcHtmlString GetMvcString(this System.Web.Mvc.HtmlHelper html, string name, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = html.GetCultureInfo();
            string text = Basic.Messages.MessageContext.GetString(name, culture, args);
            if (text != null)
                return MvcHtmlString.Create(text);
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="name">要获取的资源名。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static MvcHtmlString GetMvcString(this System.Web.Mvc.HtmlHelper html, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = html.GetCultureInfo();
            string text = Basic.Messages.MessageContext.GetString(name, culture);
            if (text != null)
                return MvcHtmlString.Create(text);
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="format"></param>
        /// <param name="name">要获取的资源名。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static MvcHtmlString GetMvcString(this System.Web.Mvc.HtmlHelper html, string format, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            System.Globalization.CultureInfo culture = html.GetCultureInfo();
            string text = Basic.Messages.MessageContext.GetString(name, culture);
            if (text != null)
                return MvcHtmlString.Create(string.Format(format, text));
            return MvcHtmlString.Empty;
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="format">要获取的资源名。</param>
        /// <param name="action">操作的名称。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static MvcHtmlString GetActionString(this System.Web.Mvc.HtmlHelper html, string format, string action)
        {
            return html.GetActionString(format, action, null);
        }

        /// <summary>
        /// 返回指定的 System.String 资源的值。
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="format">要获取的资源名。</param>
        /// <param name="action">操作的名称。</param>
        /// <param name="controller">控制器的名称。</param>
        /// <returns>针对调用方的当前区域性设置而本地化的资源的值。如果不可能有匹配项，则返回 null。</returns>
        /// <exception cref="System.ArgumentNullException">name 参数为 null。</exception>
        /// <exception cref="System.InvalidOperationException">指定资源的值不是字符串。</exception>
        /// <exception cref="System.Resources.MissingManifestResourceException">未找到可用的资源集，并且没有非特定区域性的资源。</exception>
        public static MvcHtmlString GetActionString(this System.Web.Mvc.HtmlHelper html, string format, string action, string controller)
        {
            string NavigateUrl = UrlHelper.GenerateUrl(null, action, controller, null, html.RouteCollection, html.ViewContext.RequestContext, true);
            return MvcHtmlString.Create(string.Format(format, NavigateUrl));
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="html">此方法扩展的 HTML 帮助程序实例。</param>
        /// <param name="value">值</param>
        /// <param name="format">格式化字符串</param>
        /// <returns>返回格式化成功的字符串</returns>
        internal static string FormatValue(this HtmlHelper html, object value, string format)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(format))
            {
                return Convert.ToString(value, CultureInfo.CurrentCulture);
            }
            return string.Format(CultureInfo.CurrentCulture, format, value);

        }
    }
}
