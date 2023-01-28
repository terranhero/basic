using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Basic.Collections;
using Basic.Messages;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 实体属性的定义信息。
	/// </summary>
	public sealed class EntityPropertyMeta : PropertyDescriptor
	{
		private readonly PropertyInfo _PropertyInfo;
		private readonly ColumnMappingAttribute fieldColumnMapping;
		private readonly JoinFieldAttribute fieldJoinField;
		private readonly ColumnAttribute fieldColumn;
		private readonly DisplayFormatAttribute fieldDisplayFormat;
		private readonly WebDisplayAttribute fieldWebDisplay;
		private readonly ImportAttribute fieldImport;
		private readonly ValidationCollection fieldValidations;
		private readonly GroupNameAttribute _GroupName;
		private readonly static long _Int32Max = Convert.ToInt64(int.MaxValue);
		private readonly bool _PrimaryKey = false;
		private readonly bool _IgnoreProperty = false;
		internal EntityPropertyMeta(PropertyInfo info)
			: base(info.Name, Attribute.GetCustomAttributes(info))
		{
			_PropertyInfo = info;
			fieldValidations = new ValidationCollection();
			object[] attributes = info.GetCustomAttributes(true);
			if (attributes == null || attributes.Length == 0) { return; }
			foreach (System.Attribute attribute in attributes)
			{
				if (attribute is ColumnMappingAttribute) { fieldColumnMapping = attribute as ColumnMappingAttribute; }
				else if (attribute is ColumnAttribute)
				{
					fieldColumn = attribute as ColumnAttribute;
					_PrimaryKey = fieldColumn.PrimaryKey;
				}
				else if (attribute is DisplayFormatAttribute) { fieldDisplayFormat = attribute as DisplayFormatAttribute; }
				else if (attribute is ImportAttribute) { fieldImport = attribute as ImportAttribute; }
				else if (attribute is PrimaryKeyAttribute) { _PrimaryKey = true; }
				else if (attribute is IgnorePropertyAttribute) { _IgnoreProperty = true; }
				else if (attribute is WebDisplayAttribute) { fieldWebDisplay = attribute as WebDisplayAttribute; }
				else if (attribute is ValidationAttribute) { fieldValidations.Add(attribute as ValidationAttribute); }
				else if (attribute is GroupNameAttribute) { _GroupName = attribute as GroupNameAttribute; }
				else if (attribute is JoinFieldAttribute) { fieldJoinField = attribute as JoinFieldAttribute; }
			}
		}

		/// <summary>当前属性的所属类型</summary>
		/// <value>当前属性的所属类型</value>
		public System.Type ContainerType { get { return _PropertyInfo.DeclaringType; } }

		///// <summary>Gets the class object that was used to obtain this instance of MemberInfo</summary>
		///// <value>The Type object through which this MemberInfo object was obtained</value>
		//public System.Type ReflectedType { get { return _PropertyInfo.ReflectedType; } }

		/// <summary>获取一个值，该值指示该属性是否可读。</summary>
		/// <value>如果此属性可读，则为 true；否则，为 false。</value>
		public bool CanRead { get { return _PropertyInfo.CanRead; } }

		/// <summary>获取一个值，该值指示此属性是否可写。</summary>
		/// <value>如果此属性可写，则为 true；否则，为 false。</value>
		public bool CanWrite { get { return _PropertyInfo.CanWrite; } }

		/// <summary>获取当前属性导入特性信息。</summary>
		public ImportAttribute Import { get { return fieldImport; } }

		/// <summary>获取当前属性的验证特性集合。</summary>
		internal ValidationCollection Validations { get { return fieldValidations; } }

		/// <summary>获取当前属性的数据库字段信息</summary>
		public ColumnMappingAttribute Mapping { get { return fieldColumnMapping; } }

		/// <summary>获取当前属性的数据库字段信息</summary>
		public JoinFieldAttribute JoinField { get { return fieldJoinField; } }

		/// <summary>获取当前属性的数据库字段信息</summary>
		public ColumnAttribute Column { get { return fieldColumn; } }

		/// <summary>获取当前属性的格式化显示信息</summary>
		public DisplayFormatAttribute DisplayFormat { get { return fieldDisplayFormat; } }

		/// <summary>获取或设置当前属性显示的格式字符串。</summary>
		public string DisplayFormatString { get { if (fieldDisplayFormat != null) { return fieldDisplayFormat.DataFormatString; } return null; } }

		/// <summary>获取当前属性的显示的文本信息。</summary>
		public WebDisplayAttribute Display { get { return fieldWebDisplay; } }

		/// <summary>属性显示文本资源分组信息</summary>
		public GroupNameAttribute GroupName { get { return _GroupName; } }

		/// <summary>获取当前属性的本地化 DisplayName 属性信息。</summary>
		public string GetCultureDisplayName(CultureInfo cultureInfo)
		{
			if (fieldWebDisplay != null && fieldWebDisplay.HasDisplayName)
			{
				string displayName = fieldWebDisplay.DisplayName;
				string converterName = fieldWebDisplay.ConverterName;
				return MessageContext.GetString(converterName, displayName, cultureInfo);
			}
			return base.DisplayName;
		}

		/// <summary>
		/// 获取当前属性的本地化 DisplayName 属性信息。
		/// </summary>
		internal string CultureDisplayName
		{
			get
			{
				if (fieldWebDisplay != null && fieldWebDisplay.HasDisplayName)
				{
					CultureInfo cultureInfo = CultureInfo.CurrentUICulture;
					string displayName = fieldWebDisplay.DisplayName;
					string converterName = fieldWebDisplay.ConverterName;
					return MessageContext.GetString(converterName, displayName, cultureInfo);
				}
				return base.DisplayName;
			}
		}

		/// <summary>
		/// 获取可以显示在窗口（如“属性”窗口）中的名称。
		/// </summary>
		/// <value>为该成员显示的名称。</value>
		public override string DisplayName { get { return CultureDisplayName; } }

		/// <summary>
		/// 获取当前属性是否需要序列化
		/// 在SelectByKey查询时是否需要。
		/// </summary>
		public bool IgnoreProperty { get { return _IgnoreProperty; } }

		/// <summary>当前属性是否有 IgnorePropertyAttribute 特性标记</summary>
		public bool Ignore { get { return _IgnoreProperty; } }

		/// <summary>
		/// 获取当前属性是否为主键。
		/// </summary>
		public bool PrimaryKey { get { return _PrimaryKey; } }

		private TypeConverter m_TypeConverter = null;
		/// <summary>
		/// 获取该属性的类型转换器。
		/// </summary>
		/// <value> 一个 System.ComponentModel.TypeConverter，用于转换该属性的 System.Type。</value>
		public override TypeConverter Converter
		{
			get
			{
				if (m_TypeConverter != null) { return m_TypeConverter; }
				else if (_PropertyInfo.PropertyType == typeof(System.DateTime))
				{
					if (m_TypeConverter == null) { m_TypeConverter = new Basic.EntityLayer.DateTimeConverter(); }
					return m_TypeConverter;
				}
				else if (_PropertyInfo.PropertyType == typeof(System.Nullable<System.DateTime>))
				{
					if (m_TypeConverter == null) { m_TypeConverter = new Basic.EntityLayer.DateTimeConverter(); }
					return m_TypeConverter;
				}
				else if (_PropertyInfo.PropertyType == typeof(bool) || _PropertyInfo.PropertyType == typeof(System.Nullable<bool>))
				{
					if (m_TypeConverter == null) { m_TypeConverter = new Basic.EntityLayer.BooleanConverter(); }
					return m_TypeConverter;
				}
				else if (_PropertyInfo.PropertyType == typeof(System.Guid) || _PropertyInfo.PropertyType == typeof(System.Nullable<System.Guid>))
				{
					if (m_TypeConverter == null) { m_TypeConverter = new Basic.EntityLayer.GuidConverter(); }
					return m_TypeConverter;
				}
				return base.Converter;
			}
		}
		/// <summary>返回重置对象时是否更改其值。</summary>
		/// <param name="component">要测试重置功能的组件。</param>
		/// <returns>如果重置组件更改其值，则为 true；否则为 false。</returns>
		public override bool CanResetValue(object component) { return _PropertyInfo.CanWrite; }

		/// <summary>获取该属性绑定到的组件的类型。</summary>
		public override System.Type ComponentType { get { return _PropertyInfo.DeclaringType; } }

		/// <summary>获取指示该属性是否为只读的值。</summary>
		public override bool IsReadOnly { get { return _PropertyInfo.CanWrite == false; } }

		/// <summary>获取该属性的类型。</summary>
		public override System.Type PropertyType { get { return _PropertyInfo.PropertyType; } }

		/// <summary>
		/// 获取组件上的属性的当前值。
		/// </summary>
		/// <param name="component">具有为其检索值的属性的组件。</param>
		/// <returns>给定组件的属性的值。</returns>
		public override object GetValue(object component)
		{
			MethodInfo getMethod = _PropertyInfo.GetGetMethod(true);
			if (getMethod == null) { throw new System.ArgumentException("属性 get 访问器无效。", _PropertyInfo.Name); }
			return getMethod.Invoke(component, null);
		}

		/// <summary>
		/// 将组件的此属性的值重置为默认值。
		/// </summary>
		/// <param name="component">具有要重置为默认值的属性值的组件。</param>
		public override void ResetValue(object component)
		{
			MethodInfo setMethod = _PropertyInfo.GetSetMethod(true);
			if (setMethod == null) { return; }
			System.Type pType = _PropertyInfo.PropertyType;

			if (pType.IsGenericType && pType.GUID == typeof(System.Nullable<>).GUID) { setMethod.Invoke(component, new object[] { null }); }
			else if (pType.IsClass) { setMethod.Invoke(component, new object[] { null }); }
			else if (pType.IsValueType) { setMethod.Invoke(component, new object[] { System.Activator.CreateInstance(pType) }); }
		}

		/// <summary>发序列化属性的值(从XmlReader中读取属性的值)。</summary>
		/// <param name="component">具有要进行设置的属性值的组件。</param>
		/// <param name="value">新值。</param>
		public void DeserializeValue(object component, string value)
		{
			if (string.IsNullOrEmpty(value)) { return; }
			if (Converter != null) { this.SetValue(component, Converter.ConvertFromString(value)); }
			else { this.SetValue(component, value); }
		}

		/// <summary>
		/// 获取组件上的属性的当前值。
		/// </summary>
		/// <param name="component">具有要进行设置的属性值的组件。</param>
		/// <returns>给定组件的属性的值。</returns>
		public string SerializedValue(object component)
		{
			object result = GetValue(component);
			if (Converter == null) { return System.Convert.ToString(result); }
			return Converter.ConvertToString(result);
		}
		/// <summary>
		/// 将组件的值设置为一个不同的值。
		/// </summary>
		/// <param name="component">具有要进行设置的属性值的组件。</param>
		/// <param name="value">新值。</param>
		public override void SetValue(object component, object value)
		{
			MethodInfo setMethod = _PropertyInfo.GetSetMethod(true);
			if (setMethod == null) { return; }
			System.Guid nullableGuid = typeof(System.Nullable<>).GUID;
			System.Type pType = _PropertyInfo.PropertyType;
			bool valueIsNull = value == System.DBNull.Value || value == null;
			if (valueIsNull)
			{
				if (pType.IsGenericType && pType.GUID == nullableGuid) { setMethod.Invoke(component, new object[] { null }); }
				else if (pType.IsClass) { setMethod.Invoke(component, new object[] { null }); }
			}
			else
			{
				if (pType == value.GetType()) { setMethod.Invoke(component, new object[] { value }); }
				else if (pType.IsEnum) { setMethod.Invoke(component, new object[] { Enum.ToObject(pType, Convert.ToInt32(value)) }); }
				else if (pType == typeof(int))
				{
					if (value is long @long && @long <= _Int32Max) { setMethod.Invoke(component, new object[] { Convert.ToInt32(@long) }); }
					else { setMethod.Invoke(component, new object[] { value }); }
				}
				else if (pType == typeof(bool))
				{
					if (value is short @short) { setMethod.Invoke(component, new object[] { @short >= 1 }); }
					else if (value is int @int) { setMethod.Invoke(component, new object[] { @int >= 1 }); }
					else if (value is string @string) { setMethod.Invoke(component, new object[] { @string == "Y" }); }
				}
				else if (pType.IsGenericType && pType.GUID == nullableGuid)
				{
					Type[] typeArray = pType.GetGenericArguments();
					if (typeArray.Length > 0 && typeArray[0].IsEnum)
					{
						setMethod.Invoke(component, new object[] { Enum.ToObject(typeArray[0], Convert.ToInt32(value)) });
					}
					else { setMethod.Invoke(component, new object[] { value }); }
				}
				else
				{
					setMethod.Invoke(component, new object[] { value });
				}
			}
		}

		/// <summary>
		/// 确定一个值，该值指示是否需要永久保存此属性的值。
		/// </summary>
		/// <param name="component">具有要检查其持久性的属性的组件。</param>
		/// <returns>如果属性应该被永久保存，则为 true；否则为 false。</returns>
		public override bool ShouldSerializeValue(object component) { return false; }

		#region 提供接口 IEditableObject 的实现
		private object _oldValue = null;
		/// <summary>
		/// 开始编辑对象。
		/// </summary>
		internal void BeginEdit(object component) { _oldValue = this.GetValue(component); }

		/// <summary>
		/// 放弃上一次 BeginEdit() 调用之后的更改。
		/// </summary>
		internal void CancelEdit(object component)
		{
			if (_oldValue == null) { return; }
			MethodInfo setMethod = _PropertyInfo.GetSetMethod(true);
			if (setMethod == null) { return; }
			setMethod.Invoke(component, new object[] { _oldValue });
			SetValue(component, _oldValue); _oldValue = null;
		}

		/// <summary>
		/// 将上一次 BeginEdit() 调用之后所进行的更改推到基础对象中。
		/// </summary>
		internal void EndEdit() { _oldValue = null; }
		#endregion
	}
}
