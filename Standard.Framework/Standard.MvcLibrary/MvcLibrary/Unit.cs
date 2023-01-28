using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using Basic.Properties;

namespace Basic.MvcLibrary
{
	/// <summary>
	/// 
	/// </summary>
	public enum UnitType
	{
		/// <summary>
		/// 
		/// </summary>
		Cm = 6,
		/// <summary>
		/// 
		/// </summary>
		Em = 8,
		/// <summary>
		/// 
		/// </summary>
		Ex = 9,
		/// <summary>
		/// 
		/// </summary>
		Inch = 4,
		/// <summary>
		/// 
		/// </summary>
		Mm = 5,
		/// <summary>
		/// 
		/// </summary>
		Percentage = 7,
		/// <summary>
		/// 
		/// </summary>
		Pica = 3,
		/// <summary>
		/// 
		/// </summary>
		Pixel = 1,
		/// <summary>
		/// 
		/// </summary>
		Point = 2
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(UnitConverter))]
	public struct Unit
	{
		/// <summary>
		/// 
		/// </summary>
		public static readonly Unit Empty;
		internal const int MaxValue = 0x7fff;
		internal const int MinValue = -32768;
		private readonly UnitType type;
		private readonly double value;
		/// <summary>
		/// 
		/// </summary>
		public Unit(int value)
		{
			if ((value < -32768) || (value > 0x7fff))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			this.value = value;
			this.type = UnitType.Pixel;
		}

		/// <summary>
		/// 
		/// </summary>
		public Unit(double value)
		{
			if ((value < -32768.0) || (value > 32767.0))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			this.value = (int)value;
			this.type = UnitType.Pixel;
		}

		/// <summary>
		/// 
		/// </summary>
		public Unit(double value, UnitType type)
		{
			if ((value < -32768.0) || (value > 32767.0))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (type == UnitType.Pixel)
			{
				this.value = (int)value;
			}
			else
			{
				this.value = value;
			}
			this.type = type;
		}

		/// <summary>
		/// 
		/// </summary>
		public Unit(string value) : this(value, CultureInfo.CurrentCulture, UnitType.Pixel)
		{
		}
		/// <summary>
		/// 
		/// </summary>
		public Unit(string value, CultureInfo culture) : this(value, culture, UnitType.Pixel)
		{
		}

		internal Unit(string value, CultureInfo culture, UnitType defaultType)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.value = 0.0;
				this.type = (UnitType)0;
			}
			else
			{
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				string str = value.Trim().ToLower(CultureInfo.InvariantCulture);
				int length = str.Length;
				int num2 = -1;
				for (int i = 0; i < length; i++)
				{
					char ch = str[i];
					if (((ch < '0') || (ch > '9')) && (((ch != '-') && (ch != '.')) && (ch != ',')))
					{
						break;
					}
					num2 = i;
				}
				if (num2 == -1)
				{
					object[] args = new object[] { value };
					throw new FormatException(string.Format(JsonStrings.UnitParseNoDigits, args));
				}
				if (num2 < (length - 1))
				{
					this.type = GetTypeFromString(str.Substring(num2 + 1).Trim());
				}
				else
				{
					this.type = defaultType;
				}
				string text = str.Substring(0, num2 + 1);
				try
				{
					TypeConverter converter = new SingleConverter();
					this.value = (float)converter.ConvertFromString(null, culture, text);
					if (this.type == UnitType.Pixel)
					{
						this.value = (int)this.value;
					}
				}
				catch
				{
					object[] objArray2 = new object[] { value, text, this.type.ToString("G") };
					throw new FormatException(string.Format(JsonStrings.UnitParseNumericPart, objArray2));
				}
				if ((this.value < -32768.0) || (this.value > 32767.0))
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return (this.type == ((UnitType)0));
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public UnitType Type
		{
			get
			{
				if (!this.IsEmpty)
				{
					return this.type;
				}
				return UnitType.Pixel;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public double Value
		{
			get
			{
				return this.value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override int GetHashCode()
		{
			return this.type.GetHashCode() ^ this.value.GetHashCode();
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool Equals(object obj)
		{
			if ((obj == null) || !(obj is Unit))
			{
				return false;
			}
			Unit unit = (Unit)obj;
			return ((unit.type == this.type) && (unit.value == this.value));
		}

		/// <summary>
		/// 
		/// </summary>
		public static bool operator ==(Unit left, Unit right)
		{
			return ((left.type == right.type) && (left.value == right.value));
		}

		/// <summary>
		/// 
		/// </summary>
		public static bool operator !=(Unit left, Unit right)
		{
			if (left.type == right.type)
			{
				return !(left.value == right.value);
			}
			return true;
		}

		private static string GetStringFromType(UnitType type)
		{
			switch (type)
			{
				case UnitType.Pixel:
					return "px";

				case UnitType.Point:
					return "pt";

				case UnitType.Pica:
					return "pc";

				case UnitType.Inch:
					return "in";

				case UnitType.Mm:
					return "mm";

				case UnitType.Cm:
					return "cm";

				case UnitType.Percentage:
					return "%";

				case UnitType.Em:
					return "em";

				case UnitType.Ex:
					return "ex";
			}
			return string.Empty;
		}

		private static UnitType GetTypeFromString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return UnitType.Pixel;
			}
			if (value.Equals("px"))
			{
				return UnitType.Pixel;
			}
			if (value.Equals("pt"))
			{
				return UnitType.Point;
			}
			if (value.Equals("%"))
			{
				return UnitType.Percentage;
			}
			if (value.Equals("pc"))
			{
				return UnitType.Pica;
			}
			if (value.Equals("in"))
			{
				return UnitType.Inch;
			}
			if (value.Equals("mm"))
			{
				return UnitType.Mm;
			}
			if (value.Equals("cm"))
			{
				return UnitType.Cm;
			}
			if (value.Equals("em"))
			{
				return UnitType.Em;
			}
			if (!value.Equals("ex"))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			return UnitType.Ex;
		}

		/// <summary>
		/// 
		/// </summary>
		public static Unit Parse(string s)
		{
			return new Unit(s, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Unit Parse(string s, CultureInfo culture)
		{
			return new Unit(s, culture);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Unit Percentage(double n)
		{
			return new Unit(n, UnitType.Percentage);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Unit Pixel(int n)
		{
			return new Unit(n);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Unit Point(int n)
		{
			return new Unit((double)n, UnitType.Point);
		}

		/// <summary>
		/// 
		/// </summary>
		public override string ToString()
		{
			return this.ToString((IFormatProvider)CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// 
		/// </summary>
		public string ToString(CultureInfo culture)
		{
			return this.ToString((IFormatProvider)culture);
		}

		/// <summary>
		/// 
		/// </summary>
		public string ToString(IFormatProvider formatProvider)
		{
			string str;
			if (this.IsEmpty)
			{
				return string.Empty;
			}
			if (this.type == UnitType.Pixel)
			{
				str = ((int)this.value).ToString(formatProvider);
			}
			else
			{
				str = ((float)this.value).ToString(formatProvider);
			}
			return (str + GetStringFromType(this.type));
		}

		/// <summary>
		/// 
		/// </summary>
		public static implicit operator Unit(int n)
		{
			return Pixel(n);
		}
	}

}
