using System;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 表示可为空的属性基元类型
	/// </summary>
	/// <typeparam name="T">表示DocNet 基元类型</typeparam>
	public struct PropertyMetaNullable<T> : IPropertyMeta<T> where T : struct, IComparable<T>, IEquatable<T>
	{
		private Nullable<T> m_Value;
		private readonly string m_Name;
		/// <summary>
		/// 根据属性名称初始化 PropertyMetaNullable 类型实例。
		/// </summary>
		/// <param name="name">当前 PropertyMetaNullable 类型实例的属性名称。</param>
		public PropertyMetaNullable(string name) : this(name, new Nullable<T>()) { }

		/// <summary>
		/// 根据属性名称和初始值初始化 PropertyMetaNullable 类型实例。
		/// </summary>
		/// <param name="name">当前 PropertyMetaNullable 类型实例的属性名称。</param>
		/// <param name="value">当前 PropertyMetaNullable 对象的值</param>
		public PropertyMetaNullable(string name, T value) : this(name, new Nullable<T>(value)) { }

		/// <summary>
		/// 根据属性名称和初始值初始化 PropertyMetaNullable 类型实例。
		/// </summary>
		/// <param name="name">当前 PropertyMetaNullable 类型实例的属性名称。</param>
		/// <param name="value">当前 PropertyMetaNullable 对象的值</param>
		private PropertyMetaNullable(string name, T? value)
		{
			m_Value = value; m_Name = name;
		}

		/// <summary>
		/// 表示当前属性的名称
		/// </summary>
		public string Key { get { return m_Name; } }

		/// <summary>
		/// 表示当前属性的值
		/// </summary>
		public T Value
		{
			get { return m_Value.Value; }
			set { m_Value = value; }
		}

		/// <summary>
		/// <![CDATA[检索当前 PropertyMetaNullable<T> 对象的值，或该对象的默认值。]]>
		/// </summary>
		public T GetValueOrDefault()
		{
			return m_Value.GetValueOrDefault();
		}

		/// <summary>
		/// <![CDATA[获取一个值，指示当前的 PropertyMetaNullable<T> 对象是否有值。]]>
		/// </summary>
		/// <value><![CDATA[如果当前的 PropertyMetaNullable<T> 对象具有值，则为 true；如果当前的 PropertyMetaNullable<T> 对象没有值，则为 false。]]></value>
		public bool HasValue { get { return m_Value.HasValue; } }

		/// <summary>
		/// 获取 IPropertyMeta 属性的值
		/// </summary>
		/// <returns>返回值</returns>
		object IPropertyMeta.GetValue() { return m_Value; }

		#region 重载操作符
		/// <summary>
		/// 指示此实例与指定对象是否相等。
		/// </summary>
		/// <param name="obj">要比较的另一个对象。</param>
		/// <returns>如果 obj 和该实例具有相同的类型并表示相同的值，则为 true；否则为 false。</returns>
		public override bool Equals(object obj)
		{
			PropertyMetaNullable<T> tt = (PropertyMetaNullable<T>)obj;
			if (m_Value.HasValue && tt.HasValue) { return (m_Value as IEquatable<T>).Equals(tt.Value); }
			if (m_Value.HasValue || tt.HasValue) { return false; }
			return true;
		}

		/// <summary>
		/// 返回此实例的哈希代码。
		/// </summary>
		/// <returns>一个 32 位有符号整数，它是该实例的哈希代码。</returns>
		public override int GetHashCode() { return m_Value.GetHashCode(); }

		/// <summary>
		/// 判断当前两个值是否相等
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 等于 b 则为 true；否则为 false。</returns>
		public static bool operator ==(PropertyMetaNullable<T> a, PropertyMetaNullable<T> b) { return a.Equals(b); }

		/// <summary>
		/// 判断当前两个值是否不相等
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 不等于 b 则为 true；否则为 false。</returns>
		public static bool operator !=(PropertyMetaNullable<T> a, PropertyMetaNullable<T> b) { return !a.Equals(b); }

		/// <summary>
		/// 判断当前对象是否大于等于另一个对象
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 大于等于 b 则为 true；否则为 false。</returns>
		public static bool operator >=(PropertyMetaNullable<T> a, PropertyMetaNullable<T> b) { return a.CompareTo(b) <= 0; }

		/// <summary>
		/// 判断当前对象是否小于等于另一个对象
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 小于等于 b 则为 true；否则为 false。</returns>
		public static bool operator <=(PropertyMetaNullable<T> a, PropertyMetaNullable<T> b) { return a.CompareTo(b) >= 0; }

		/// <summary>
		/// 判断当前对象是否大于另一个对象
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 大于 b 则为 true；否则为 false。</returns>
		public static bool operator >(PropertyMetaNullable<T> a, PropertyMetaNullable<T> b) { return a.CompareTo(b) < 0; }

		/// <summary>
		/// 判断当前对象是否小于另一个对象。
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 小于 b 则为 true；否则为 false。</returns>
		public static bool operator <(PropertyMetaNullable<T> a, PropertyMetaNullable<T> b) { return a.CompareTo(b) > 0; }
		#endregion

		#region 实现接口 IComparable<T>, IEquatable<T>
		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。 
		/// 返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 零 此对象等于 other。 
		/// 大于零此对象大于 other。</returns>
		private int CompareTo(IPropertyMeta<T> other)
		{
			if (m_Value.HasValue && other.HasValue) { return (m_Value as IComparable<T>).CompareTo(other.Value); }
			if (m_Value.HasValue && !other.HasValue) { return 1; }
			if (!m_Value.HasValue && other.HasValue) { return -1; }
			return 0;
		}

		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。 
		/// 返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 零 此对象等于 other。 
		/// 大于零此对象大于 other。</returns>
		int IComparable<IPropertyMeta<T>>.CompareTo(IPropertyMeta<T> other) { return CompareTo(other); }

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 other 参数，则为 true；否则为 false。</returns>
		private bool Equals(IPropertyMeta<T> other)
		{
			if (m_Value.HasValue && other.HasValue) { return (m_Value as IEquatable<T>).Equals(other.Value); }
			if (m_Value.HasValue || other.HasValue) { return false; }
			return true;
		}

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 other 参数，则为 true；否则为 false。</returns>
		bool IEquatable<IPropertyMeta<T>>.Equals(IPropertyMeta<T> other) { return Equals(other); }
		#endregion
	}

	/// <summary>
	/// 表示属性基元类型
	/// </summary>
	/// <typeparam name="T">表示DocNet 基元类型</typeparam>
	public struct PropertyMeta<T> : IPropertyMeta<T> where T : struct, IComparable<T>, IEquatable<T>
	{
		private T m_Value;
		private readonly string m_Title;
		private readonly string m_Key;
		/// <summary>
		/// 根据属性名称初始化 PropertyMeta 类型实例。
		/// </summary>
		/// <param name="key">当前 PropertyMeta 类型实例的属性名称。</param>
		public PropertyMeta(string key) : this(key, key, default(T)) { }

		/// <summary>
		/// 根据属性名称和初始值初始化 PropertyMeta 类型实例。
		/// </summary>
		/// <param name="key">当前 PropertyMeta 类型实例的属性名称。</param>
		/// <param name="value">当前 PropertyMeta 对象的值</param>
		public PropertyMeta(string key, T value) : this(key, key, value) { }

		/// <summary>
		/// 根据属性名称和初始值初始化 PropertyMeta 类型实例。
		/// </summary>
		/// <param name="key">当前 PropertyMeta 类型实例的属性名称。</param>
		/// <param name="title">当前 PropertyMeta 类型实例的标题名称。</param>
		/// <param name="value">当前 PropertyMeta 对象的值</param>
		public PropertyMeta(string key, string title, T value)
		{
			m_Value = value; m_Key = key; m_Title = title;
		}

		/// <summary>
		/// 表示当前属性的名称
		/// </summary>
		public string Key { get { return m_Key; } }

		/// <summary>
		/// 表示当前属性的标题
		/// </summary>
		public string Title { get { return m_Title; } }

		/// <summary>
		/// 表示当前属性的值
		/// </summary>
		public T Value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}

		/// <summary>
		/// <![CDATA[获取一个值，指示当前的 PropertyMeta<T> 对象是否有值。]]>
		/// </summary>
		/// <value><![CDATA[始终返回 true。]]></value>
		public bool HasValue { get { return true; } }


		/// <summary>
		/// 获取 IPropertyMeta 属性的值
		/// </summary>
		/// <returns>返回值</returns>
		object IPropertyMeta.GetValue() { return m_Value; }

		#region 重载操作符
		/// <summary>
		/// 指示此实例与指定对象是否相等。
		/// </summary>
		/// <param name="obj">要比较的另一个对象。</param>
		/// <returns>如果 obj 和该实例具有相同的类型并表示相同的值，则为 true；否则为 false。</returns>
		public override bool Equals(object obj)
		{
			PropertyMeta<T> tt = (PropertyMeta<T>)obj;
			return (m_Value as IEquatable<T>).Equals(tt.Value);
		}

		/// <summary>
		/// 返回此实例的哈希代码。
		/// </summary>
		/// <returns>一个 32 位有符号整数，它是该实例的哈希代码。</returns>
		public override int GetHashCode() { return m_Value.GetHashCode(); }

		/// <summary>
		/// 判断当前两个值是否相等
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 等于 b 则为 true；否则为 false。</returns>
		public static bool operator ==(PropertyMeta<T> a, PropertyMeta<T> b) { return a.Equals(b); }

		/// <summary>
		/// 判断当前两个值是否不相等
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 不等于 b 则为 true；否则为 false。</returns>
		public static bool operator !=(PropertyMeta<T> a, PropertyMeta<T> b) { return !a.Equals(b); }

		/// <summary>
		/// 判断当前对象是否大于等于另一个对象
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 大于等于 b 则为 true；否则为 false。</returns>
		public static bool operator >=(PropertyMeta<T> a, PropertyMeta<T> b) { return a.CompareTo(b) <= 0; }

		/// <summary>
		/// 判断当前对象是否小于等于另一个对象
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 小于等于 b 则为 true；否则为 false。</returns>
		public static bool operator <=(PropertyMeta<T> a, PropertyMeta<T> b) { return a.CompareTo(b) >= 0; }

		/// <summary>
		/// 判断当前对象是否大于另一个对象
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 大于 b 则为 true；否则为 false。</returns>
		public static bool operator >(PropertyMeta<T> a, PropertyMeta<T> b) { return a.CompareTo(b) < 0; }

		/// <summary>
		/// 判断当前对象是否小于另一个对象。
		/// </summary>
		/// <param name="a">要比较的第一个对象。</param>
		/// <param name="b">要比较的第二个对象。</param>
		/// <returns>如果a 小于 b 则为 true；否则为 false。</returns>
		public static bool operator <(PropertyMeta<T> a, PropertyMeta<T> b) { return a.CompareTo(b) > 0; }
		#endregion

		#region 实现接口 IComparable<T>, IEquatable<T>
		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。 
		/// 返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 零 此对象等于 other。 
		/// 大于零此对象大于 other。</returns>
		private int CompareTo(IPropertyMeta<T> other) { return (m_Value as IComparable<T>).CompareTo(other.Value); }

		/// <summary>
		/// 比较当前对象和同一类型的另一对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>一个值，指示要比较的对象的相对顺序。 
		/// 返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 零 此对象等于 other。 
		/// 大于零此对象大于 other。</returns>
		int IComparable<IPropertyMeta<T>>.CompareTo(IPropertyMeta<T> other) { return this.CompareTo(other); }

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 other 参数，则为 true；否则为 false。</returns>
		private bool Equals(IPropertyMeta<T> other) { return (m_Value as IEquatable<T>).Equals(other.Value); }

		/// <summary>
		/// 指示当前对象是否等于同一类型的另一个对象。
		/// </summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 other 参数，则为 true；否则为 false。</returns>
		bool IEquatable<IPropertyMeta<T>>.Equals(IPropertyMeta<T> other) { return Equals(other); }
		#endregion
	}

	/// <summary>
	/// 表示属性基元类型接口
	/// </summary>
	internal interface IPropertyMeta
	{
		/// <summary>
		/// 表示当前属性的名称
		/// </summary>
		string Key { get; }

		/// <summary>
		/// <![CDATA[获取一个值，指示当前的 IPropertyMeta<T> 对象是否有值。]]>
		/// </summary>
		bool HasValue { get; }

		/// <summary>
		/// 获取 IPropertyMeta 属性的值
		/// </summary>
		/// <returns>返回值</returns>
		object GetValue();
	}

	/// <summary>
	/// 表示属性基元类型接口
	/// </summary>
	/// <typeparam name="T">表示DocNet 基元类型</typeparam>
	internal interface IPropertyMeta<T> : IPropertyMeta, IComparable<IPropertyMeta<T>>, IEquatable<IPropertyMeta<T>> where T : struct, IComparable<T>, IEquatable<T>
	{
		/// <summary>
		/// 表示当前属性的值
		/// </summary>
		T Value { get; set; }
	}
}
