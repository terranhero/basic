using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 提供将 System.DateTime 对象与其他各种表示形式相互转换的类型转换器。
	/// </summary>
	public sealed class DateTimeConverter : System.ComponentModel.DateTimeConverter
	{
		/// <summary>
		/// 初始化 Basic.EntityLayer.DateTimeConveter 类的实例。
		/// </summary>
		public DateTimeConverter() : base() { }

		/// <summary>
		/// 获取一个值，该值指示此转换器是否可使用指定上下文将给定源类型的对象转换为 System.DateTime。
		/// </summary>
		/// <param name="context">System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
		/// <param name="sourceType">System.Type，表示要从中进行转换的类型。</param>
		/// <returns>如果此对象可以执行转换，则为 true；否则为 false。</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// 获取一个值，该值指示此转换器能否使用上下文将对象转换为给定的目标类型。
		/// </summary>
		/// <param name="context">System.ComponentModel.ITypeDescriptorContext，提供格式上下文。</param>
		/// <param name="destinationType">表示要转换到的类型的 System.Type。</param>
		/// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// 将给定的值对象转换为 System.DateTime。
		/// </summary>
		/// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">一个可选的 System.Globalization.CultureInfo。如果未提供区域性设置，则使用当前区域性。</param>
		/// <param name="value">要转换的 System.Object。</param>
		/// <exception cref="System.FormatException">value 不是目标类型的有效值。</exception>
		/// <exception cref="System.NotSupportedException">不能执行转换。</exception>
		/// <returns>表示转换的 value 的 System.Object。</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string @string && !string.IsNullOrEmpty(@string))
			{
				if (Regex.IsMatch(@string, "^\\d+$"))
				{
					if (Convert.ToInt64(value) == 0) { return DateTime.MinValue; }
					return new DateTime(Convert.ToInt64(value));
				}
			}
			else if (value is long @long)
			{
				if (@long == 0) { return DateTime.MinValue; }
				return new DateTime(@long);
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Converts the given value object to a System.DateTime using the arguments.
		/// </summary>
		/// <param name="context">An System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">一个可选的 System.Globalization.CultureInfo。如果未提供区域性设置，则使用当前区域性。</param>
		/// <param name="value">要转换的 System.Object。</param>
		/// <param name="destinationType">要将值转换成的 System.Type。</param>
		/// <exception cref="System.NotSupportedException">不能执行转换。</exception>
		/// <returns>表示转换的 value 的 System.Object。</returns>
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is DateTime? && value != null && ((DateTime?)value) != DateTime.MinValue)
			{
				if (((DateTime?)value).HasValue)
					return Convert.ToString(((DateTime?)value).Value.Ticks);
				return string.Empty;
			}
			else if (destinationType == typeof(long) && value is DateTime? && value != null && ((DateTime?)value) != DateTime.MinValue)
			{
				if (((DateTime?)value).HasValue)
					return ((DateTime?)value).Value.Ticks;
				return string.Empty;
			}
			else if (destinationType == typeof(string) && value is DateTime time && time != DateTime.MinValue)
			{
				return Convert.ToString(time.Ticks);
			}
			else if (destinationType == typeof(long) && value is DateTime time1 && time1 != DateTime.MinValue)
			{
				return time1.Ticks;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// 获取当前日期时间
		/// </summary>
		public static DateTime Now
		{
			get { return DateTime.Now; }
		}

		/// <summary>
		/// 获取当前日期
		/// </summary>
		public static DateTime Today
		{
			get { return DateTime.Today; }
		}

		/// <summary>
		/// 获取前一个月的今天
		/// </summary>
		public static DateTime PreMonthToday
		{
			get { return Today.AddMonths(-1); }
		}

		/// <summary>
		/// 获取下一个月的今天
		/// </summary>
		public static DateTime NextMonthToday
		{
			get { return Today.AddMonths(1); }
		}

		/// <summary>
		/// 获取当前星期的第一天,星期日为第一天
		/// </summary>
		public static DateTime WeekFirstDay
		{
			get
			{
				switch (Today.DayOfWeek)
				{
					case DayOfWeek.Sunday:
						return Today;
					case DayOfWeek.Monday:
						return Today.AddDays(-1);
					case DayOfWeek.Tuesday:
						return Today.AddDays(-2);
					case DayOfWeek.Wednesday:
						return Today.AddDays(-3);
					case DayOfWeek.Thursday:
						return Today.AddDays(-4);
					case DayOfWeek.Friday:
						return Today.AddDays(-5);
					case DayOfWeek.Saturday:
						return Today.AddDays(-6);
				}
				return Today;
			}
		}

		/// <summary>
		/// 获取当前星期的最后一天,星期日为第一天
		/// </summary>
		public static DateTime WeekLastDay
		{
			get
			{
				switch (Today.DayOfWeek)
				{
					case DayOfWeek.Sunday:
						return Today.AddDays(6);
					case DayOfWeek.Monday:
						return Today.AddDays(5);
					case DayOfWeek.Tuesday:
						return Today.AddDays(4);
					case DayOfWeek.Wednesday:
						return Today.AddDays(3);
					case DayOfWeek.Thursday:
						return Today.AddDays(2);
					case DayOfWeek.Friday:
						return Today.AddDays(1);
					case DayOfWeek.Saturday:
						return Today;
				}
				return Today;
			}
		}

		/// <summary>
		/// 获取当前月份的第一天
		/// </summary>
		public static DateTime MonthFirstDay
		{
			get { return new DateTime(Today.Year, Today.Month, 1); }
		}

		/// <summary>
		/// 获取当前月份的最后一天
		/// </summary>
		public static DateTime MonthLastDay
		{
			get
			{
				DateTime dt = MonthFirstDay;
				dt = dt.AddMonths(1);
				return dt.AddDays(-1);
			}
		}

		/// <summary>
		/// 获取日历的第一天(1月1日)
		/// </summary>
		public static DateTime GetFirstDay(int year) { return new DateTime(year, 1, 1); }

		/// <summary>
		/// 获取日历的第一天(12月31日)
		/// </summary>
		public static DateTime GetLastDay(int year) { return new DateTime(year, 12, 31); }

		/// <summary>
		/// 获取今年日历的第一天(1月1日)
		/// </summary>
		public static DateTime FirstDay
		{
			get { return new DateTime(Today.Year, 1, 1); }
		}

		/// <summary>
		/// 获取今年日历的最后一天(12月31日)
		/// </summary>
		public static DateTime LastDay
		{
			get { return new DateTime(Today.Year, 12, 31); }
		}

		/// <summary>
		/// 获取相对于今天的指定日期
		/// </summary>
		public static DateTime GetRelativeDate(int year, int month, int day, int hour, int minute, int second)
		{
			DateTime dt = Today.AddYears(year);
			dt = dt.AddMonths(month);
			dt = dt.AddDays(day);
			dt = dt.AddHours(hour);
			dt = dt.AddMinutes(minute);
			dt = dt.AddSeconds(second);
			return dt;
		}

		/// <summary>获取系统最小日期</summary>
		public static DateTime MinValue { get; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

		/// <summary>获取系统最大日期</summary>
		public static DateTime MaxValue { get; } = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Unspecified);
	}
}
