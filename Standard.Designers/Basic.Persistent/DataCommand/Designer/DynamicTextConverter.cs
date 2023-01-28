using System;
using System.Windows;
using System.Windows.Data;

namespace Basic.Designer
{
	/// <summary>
	/// 表示动态命令的转换器
	/// </summary>
	public sealed class DynamicTextConverter : IValueConverter
	{
		/// <summary>
		/// 转换值(绑定源到目标)。
		/// </summary>
		/// <param name="value">绑定源生成的值</param>
		/// <param name="targetType">绑定目标属性的类型</param>
		/// <param name="parameter">要使用的转换器参数</param>
		/// <param name="culture">要用在转换器中的区域性</param>
		/// <returns>转换后的值。 如果该方法返回 null，则使用有效的 null 值。</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null) { return DependencyProperty.UnsetValue; }
			string strValue = (string)value;

			if (strValue.IndexOf(Environment.NewLine) >= 0) { return value; }
			return strValue.Replace("\n", Environment.NewLine);
		}

		/// <summary>
		/// 转换值(目标到绑定源)。
		/// </summary>
		/// <param name="value">绑定目标生成的值</param>
		/// <param name="targetType">要转换到的类型</param>
		/// <param name="parameter">要使用的转换器参数</param>
		/// <param name="culture">要用在转换器中的区域性</param>
		/// <returns>转换后的值。 如果该方法返回 null，则使用有效的 null 值。</returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}
