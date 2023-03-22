using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Basic.Collections;
using Basic.Enums;
using Basic.Imports;
using Basic.Interfaces;
using Basic.Validations;

namespace Basic.EntityLayer
{
	/// <summary>
	/// 抽象实体，提供UI和数据表连接的数据载体
	/// </summary>
	[System.Serializable, System.ComponentModel.ToolboxItem(false)]
	public abstract partial class AbstractEntity : System.Dynamic.DynamicObject,
		INotifyPropertyChanging, INotifyPropertyChanged, IEditableObject, IXmlSerializable, IEntityInfo
	{
		private const string ValidationAttribute = "Validation";
		private const string PropertyElement = "PropertyInfo";
		private const string NameAttribute = "Name";
		private const string ValueAttribute = "Value";
		internal const string NullPropertyName = "9CC8BBE5022F4CB49F6ECA0F48362127";

		private EntityState m_EntityState;

		/// <summary>
		/// 初始化 AbstractEntity 类实例，此构造函数仅供子类调用
		/// </summary>
		protected AbstractEntity() : this(string.Empty, null) { }

		/// <summary>
		/// 初始化 AbstractEntity 类实例，此构造函数仅供子类调用
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		protected AbstractEntity(string tableName) : this(tableName, null) { }

		/// <summary>
		/// 初始化 AbstractEntity 类实例，此构造函数仅供子类调用
		/// </summary>
		/// <param name="enabledValidation">是否启用实体类中 IDataErrorInfo 的验证特性</param>
		protected AbstractEntity(bool enabledValidation) : this(null, enabledValidation) { }

		/// <summary>
		/// 初始化 AbstractEntity 类实例，此构造函数为了使早期版本兼容。
		/// 之后会取消此构造函数，请不要使用。
		/// </summary>
		/// <param name="tableName">数据库表名称</param>
		/// <param name="enabledValidation">是否启用实体类中 IDataErrorInfo 的验证特性,
		/// 默认为禁止使用。</param>
		private AbstractEntity(string tableName, bool? enabledValidation)
			: base()
		{
			m_EnabledValidation = enabledValidation;
#if (!NET35)
			fieldEntityValidation = new EntityValidationContext(this);
			_ValidationResult = new ValidationEntityResult(this);
#endif
			m_EntityState = EntityState.Default;
			Type type = GetType();
			EntityPropertyProvidor.TryGetProperties(type, out _EntityProperties, out _PrimaryProperties);
		}
		private bool? m_EnabledValidation = null;

		/// <summary>
		/// 当前实体类的属性定义集合
		/// </summary>
		[System.Runtime.Serialization.OptionalField(), System.Xml.Serialization.XmlIgnore()]
		private readonly EntityPropertyCollection _EntityProperties;

		/// <summary>
		/// 当前实体类的关键字属性集合
		/// </summary>
		[System.Runtime.Serialization.OptionalField(), System.Xml.Serialization.XmlIgnore()]
		private readonly EntityPropertyCollection _PrimaryProperties;

		/// <summary>
		/// 获取当前实体模型的主键属性集合
		/// </summary>
		/// <returns>返回一个 EntityPropertyDescriptor 数组，该数组表示当前实体类的主键定义信息。</returns>
		public EntityPropertyMeta[] GetPrimaryKey() { return _PrimaryProperties.ToArray(); }

		/// <summary>
		/// 获取当前实体模型的属性信息
		/// </summary>
		/// <returns>返回一个 EntityPropertyDescriptor 数组，该数组表示当前实体类的属性信息。</returns>
		public EntityPropertyMeta[] GetProperties() { return _EntityProperties.ToArray(); }

		#region Data Command Events
		//private Action<AbstractEntity> _CreateAction;
		///// <summary>
		///// Created 方法的回调
		///// </summary>
		///// <param name="action">Created 事件的回调函数。</param>
		//public void SetCreate(Action<AbstractEntity> action) { _CreateAction = action; }

		///// <summary>
		///// 引发 Created 事件的方法
		///// </summary>
		//[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		//internal void RaiseCreate() { if (_CreateAction != null) { _CreateAction(this); } }

		//private Action<AbstractEntity> _UpdateAction;
		///// <summary>
		///// Update 方法的回调
		///// </summary>
		///// <param name="action">Update 事件的回调函数。</param>
		//public void SetUpdate(Action<AbstractEntity> action) { _UpdateAction = action; }

		///// <summary>
		///// 引发 Update 事件的方法
		///// </summary>
		//[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		//internal void RaiseUpdate() { if (_UpdateAction != null) { _UpdateAction(this); } }

		//private Action<AbstractEntity> _DeleteAction;
		///// <summary>
		///// Delete 方法的回调
		///// </summary>
		///// <param name="action">Delete 事件的回调函数。</param>
		//public void SetDelete(Action<AbstractEntity> action) { _DeleteAction = action; }

		///// <summary>
		///// 引发 Delete 事件的方法
		///// </summary>
		//[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		//internal void RaiseDelete() { if (_DeleteAction != null) { _DeleteAction(this); } }

		private Func<AbstractEntity, string, string, string> _CheckedAction;
		/// <summary>
		/// Checked 方法的回调
		/// </summary>
		/// <param name="action">Delete 事件的回调函数。</param>
		public void SetChecked(Func<AbstractEntity, string, string, string> action) { _CheckedAction = action; }

		/// <summary>
		/// 引发 Checked 事件的方法
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="errorCode"></param>
		[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		internal string RaiseChecked(string converter, string errorCode)
		{
			if (_CheckedAction != null) { return _CheckedAction(this, converter, errorCode); }
			return errorCode;
		}

		private Action<AbstractEntity> _CreatedValueAction;
		/// <summary>
		/// CreatedValue 方法的回调
		/// </summary>
		/// <param name="action">CreatedValue 事件的回调函数。</param>
		public void SetCreatedValue(Action<AbstractEntity> action) { _CreatedValueAction = action; }

		/// <summary>
		/// 引发 CreatedValue 事件的方法
		/// </summary>
		[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
		internal void RaiseCreatedValue() { if (_CreatedValueAction != null) { _CreatedValueAction(this); } }
		#endregion

		/// <summary>
		/// 获取制定实体类类型的导入上下文信息，基于此上下文信息实现数据导入。
		/// </summary>
		/// <typeparam name="TE"></typeparam>
		/// <returns>实体类类型的导入上下文信息。</returns>
		public static IImportContext<TE> CreateImportContext<TE>() where TE : AbstractEntity, new()
		{
			EntityPropertyProvidor.TryGetProperties(typeof(TE), out EntityPropertyCollection importInfos);
			ImportContext<TE> context = new ImportContext<TE>();
			ImportPropertyCollection<TE> properties = context.Properties as ImportPropertyCollection<TE>;
			foreach (EntityPropertyMeta propertyInfo in importInfos)
			{
				if (propertyInfo.Import != null)
					properties.Add(new ImportProperty<TE>(context, propertyInfo));
			}
			return context;
		}

		#region  接口 INotifyPropertyChanging, INotifyPropertyChanged
		/// <summary>更改属性值时发生的事件</summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>引发 PropertyChanged 事件时的方法</summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal protected virtual void RaisePropertyChanged(string propertyName) { }

		/// <summary>判断属性值是否更改，并引发 PropertyChanged 事件</summary>
		/// <param name="propertyName">已更改的属性名。</param>
		/// <param name="newValue">属性的新值</param>
		internal protected T OnPropertyChanged<T>(string propertyName, T newValue)
		{
			OnPropertyChanged(propertyName);
			return newValue;
		}

		/// <summary>判断属性值是否更改，并引发 PropertyChanged 事件</summary>
		/// <param name="propertyName">已更改的属性名。</param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		internal protected T OnPropertyChanged<T>(string propertyName, out T oldValue, T newValue)
		{
			OnPropertyChanging(propertyName);
			oldValue = newValue;
			OnPropertyChanged(propertyName);

			return newValue;
		}

		/// <summary>引发 PropertyChanged 事件</summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal protected void OnPropertyChanged(string propertyName)
		{
			this.RaisePropertyChanged(propertyName);
			if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
		}

		/// <summary>引发 PropertyChanged 事件时的方法</summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal protected virtual void RaisePropertyChanging(string propertyName) { }

		/// <summary>属性值正在更改时发生的事件</summary>
		public event PropertyChangingEventHandler PropertyChanging;

		/// <summary>引发 PropertyChanging 事件</summary>
		/// <param name="propertyName">正在更改的属性名。</param>
		internal protected virtual void OnPropertyChanging(string propertyName)
		{
			this.RaisePropertyChanging(propertyName);
			if (PropertyChanging != null)
				PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
		}
		#endregion

		/// <summary>
		/// 根据属性格式化错误信息。
		/// </summary>
		/// <param name="propertyName">属性名称</param>
		/// <param name="errorMessage">需要格式化的错误信息。</param>
		/// <returns>返回格式化的才错误信息。</returns>
		internal string FormatMessage(string propertyName, string errorMessage)
		{
			if (string.IsNullOrEmpty(propertyName)) { return errorMessage; }
			if (string.IsNullOrEmpty(errorMessage)) { return errorMessage; }
			if (TryGetProperty(propertyName, out EntityPropertyMeta propertyInfo))
			{
				object value = propertyInfo.GetValue(this);
				return string.Format(errorMessage, value);
			}
			return errorMessage;
		}

		#region 实体类状态变更
		/// <summary>
		/// 获取当前实体的状态
		/// </summary>
		/// <returns>一个 EntityState 枚举的值。表示当前实体的状态。</returns>
		public EntityState GetEntityState() { return m_EntityState; }
		/// <summary>
		/// 设置当前实体的状态
		/// </summary>
		public void SetDefault() { m_EntityState = EntityState.Default; }
		/// <summary>
		/// 设置当前实体的状态
		/// </summary>
		public void SetAdded() { m_EntityState = EntityState.Added; }
		/// <summary>
		/// 设置当前实体的状态
		/// </summary>
		public void SetModified() { m_EntityState = EntityState.Modified; }
		/// <summary>
		/// 设置当前实体的状态
		/// </summary>
		public void SetDeleted() { m_EntityState = EntityState.Deleted; }
		#endregion

		#region 实体类属性操作方法
		/// <summary>
		/// 判断当前实体是否存在数据库字段信息
		/// </summary>
		/// <param name="columnName">字段名称</param>
		/// <returns>如果实体包含具有指定键的元素，则为 true；否则为 false。</returns>
		internal bool ContainColumn(string columnName)
		{
			return _EntityProperties.ContainColumn(columnName);
		}

		/// <summary>
		/// 获取指定字段名称的属性反射信息。
		/// </summary>
		/// <param name="columnName">要获取其值的字段名称。</param>
		/// <param name="propertyInfo">当此方法返回时，如果找到指定属性，则返回PropertyInfo 实例；
		/// 否则，将返回 propertyInfo 参数的类型的默认值。该参数未经初始化即被传递。</param>
		/// <returns>如果实体包含具有指定键的元素，则为 true；否则为 false。</returns>
		internal bool TryGetDbProperty(string columnName, out EntityPropertyMeta propertyInfo)
		{
			return _EntityProperties.TryGetDbProperty(columnName, out propertyInfo);
		}

		/// <summary>
		/// 获取指定属性名称的属性反射信息
		/// </summary>
		/// <param name="propertyName">要获取其值的属性名称。</param>
		/// <param name="propertyInfo">当此方法返回时，如果找到指定属性，则返回PropertyInfo 实例；
		/// 否则，将返回 propertyInfo 参数的类型的默认值。该参数未经初始化即被传递。</param>
		/// <returns>如果实体包含具有指定键的元素，则为 true；否则为 false。</returns>
		internal bool TryGetProperty(string propertyName, out EntityPropertyMeta propertyInfo)
		{
			return _EntityProperties.TryGetProperty(propertyName, out propertyInfo);
		}

		/// <summary>
		/// 获取所有属性的值
		/// </summary>
		internal static EntityPropertyMeta[] GetProperties<T>() where T : AbstractEntity
		{
			EntityPropertyProvidor.TryGetProperties(typeof(T), out EntityPropertyCollection propertyInfos);
			return propertyInfos.ToArray();
		}

		/// <summary>
		/// 表示当前对象的字符串表示形式。
		/// </summary>
		/// <returns>当前对象的字符串表示形式</returns>
		public override string ToString() { return GetType().Name; }

		/// <summary>
		/// 表示当前对象的字符串表示形式。
		/// </summary>
		/// <param name="isPrimaryKey">表示已何种方式序列化对象信息，仅如果为true则仅序列化主键信息，否则序列化对象所有属性信息。</param>
		/// <returns>当前对象的字符串表示形式</returns>
		public virtual string ToString(bool isPrimaryKey)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder(1000);
			if (isPrimaryKey)
			{
				foreach (EntityPropertyMeta property in _PrimaryProperties)
				{
					object obj = property.GetValue(this);
					if (obj == null || obj == DBNull.Value) { continue; }
					if (obj != null && obj != DBNull.Value && obj is string && Convert.ToString(obj) == "") { continue; }
					string propertyName = property.CultureDisplayName;
					if (builder.Length == 0)
						builder.Append(propertyName).Append("=").Append(obj);
					else
						builder.Append(",").Append(propertyName).Append("=").Append(obj);
				}
			}
			else
			{
				foreach (EntityPropertyMeta property in _EntityProperties)
				{
					if (!property.PrimaryKey && property.Ignore) { continue; }
					if (property.IsReadOnly) { continue; }
					object obj = property.GetValue(this);
					if (obj == null || obj == DBNull.Value) { continue; }
					if (obj != null && obj != DBNull.Value && obj is string && Convert.ToString(obj) == "") { continue; }
					string propertyName = property.CultureDisplayName;
					if (builder.Length == 0)
						builder.Append(propertyName).Append("=").Append(obj);
					else
						builder.Append(",").Append(propertyName).Append("=").Append(obj);
				}
			}
			return builder.ToString();
		}
		#endregion

		#region 接口 IEditableObject 的实现

		/// <summary>复制对象</summary>
		/// <typeparam name="T">复制的目标对象类型</typeparam>
		/// <param name="entity">复制的目标对象实例</param>
		/// <returns>复制成功则返回true，否则返回false，可能原因是类型不相同。</returns>
		public bool CopyTo<T>(T entity) where T : AbstractEntity
		{
			if (GetType() != typeof(T)) { return false; }
			foreach (EntityPropertyMeta property in _EntityProperties)
			{
				if (property.CanWrite == false) { continue; }
				property.SetValue(entity, property.GetValue(this));
			}
			entity.m_EntityState = m_EntityState;
			entity.m_EnabledValidation = m_EnabledValidation;
			entity._ValidationResult.Clear();
			foreach (ValidationPropertyResult vpr in _ValidationResult)
			{
				entity._ValidationResult.Add(vpr);
			}
			//entity._ValidationResult.
			return true;
		}

		/// <summary>
		/// 开始编辑对象。
		/// </summary>
		public void BeginEdit() { _EntityProperties.ForEach(m => m.BeginEdit(this)); }

		/// <summary>
		/// 放弃上一次 BeginEdit() 调用之后的更改。
		/// </summary>
		public void CancelEdit() { _EntityProperties.ForEach(m => m.CancelEdit(this)); }

		/// <summary>
		/// 将上一次 BeginEdit() 调用之后所进行的更改推到基础对象中。
		/// </summary>
		public void EndEdit() { _EntityProperties.ForEach(m => m.EndEdit()); }
		#endregion

		#region 接口 IXmlSerializable 的实现
		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected virtual bool ReadAttribute(string name, string value)
		{
			if (name == ValidationAttribute) { m_EnabledValidation = Convert.ToBoolean(value); return true; }
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected virtual void WriteAttribute(System.Xml.XmlWriter writer)
		{
			if (m_EnabledValidation.HasValue)
				writer.WriteAttributeString(ValidationAttribute, Convert.ToString(m_EnabledValidation.Value).ToLower());
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		protected virtual void ReadXml(System.Xml.XmlReader reader)
		{
			Type type = GetType();
			string rootName = type.Name;
			if (Attribute.IsDefined(type, typeof(XmlRootAttribute), true))
			{
				XmlRootAttribute root = (XmlRootAttribute)Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute), true);
				rootName = root.ElementName;
			}
			reader.MoveToContent();
			if (reader.HasAttributes)
			{
				for (int index = 0; index < reader.AttributeCount; index++)
				{
					reader.MoveToAttribute(index);
					ReadAttribute(reader.LocalName, reader.GetAttribute(index));
				}
			}
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == XmlNodeType.Element && reader.LocalName == PropertyElement)
				{
					XmlReader subReader = reader.ReadSubtree();
					subReader.MoveToContent();
					if (subReader.HasAttributes)
					{
						string propertyName = null, propertyValue = null;
						for (int index = 0; index < subReader.AttributeCount; index++)
						{
							subReader.MoveToAttribute(index);
							if (subReader.LocalName == NameAttribute) { propertyName = subReader.GetAttribute(index); }
							else if (subReader.LocalName == ValueAttribute) { propertyValue = subReader.GetAttribute(index); }
						}
						if (TryGetProperty(propertyName, out EntityPropertyMeta info))
						{
							info.DeserializeValue(this, propertyValue);
#if (!NET35)
							ReadProperty(subReader, info);
#endif

						}
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == rootName) { break; }
			}
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="info">需要写入 XmlWriter 流的属性定义信息。</param>
		protected virtual void WriteProperty(System.Xml.XmlWriter writer, EntityPropertyMeta info)
		{
			writer.WriteStartElement(PropertyElement);
			writer.WriteAttributeString(NameAttribute, info.Name);
			writer.WriteStartAttribute(ValueAttribute);
			writer.WriteValue(info.SerializedValue(this));
			writer.WriteEndAttribute();
#if (!NET35)
			if (_ValidationResult.ContainsKey(info.Name))
			{
				ValidationPropertyResult vpr = _ValidationResult[info.Name];
				vpr.WriteXml(writer);
			}
#endif

			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		private void WriteXmlDocument(System.Xml.XmlWriter writer)
		{
			Type type = GetType();
			if (Attribute.IsDefined(type, typeof(XmlRootAttribute), true))
			{
				XmlRootAttribute root = (XmlRootAttribute)Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute), true);
				writer.WriteStartElement(root.ElementName, root.Namespace);
			}
			else
			{
				writer.WriteStartElement(type.Name);
			}
			WriteXml(writer);
			writer.WriteEndElement();

		}
		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected virtual void WriteXml(System.Xml.XmlWriter writer)
		{
			WriteAttribute(writer);
			foreach (EntityPropertyMeta info in _EntityProperties)
			{
				WriteProperty(writer, info);
			}
		}
		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 System.Xml.Serialization.XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>System.Xml.Schema.XmlSchema，描述由 System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter) 
		/// 方法产生并由 System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader) 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { WriteXml(writer); }
		#endregion

		#region 包含byte[]组装和拆分的方法
		/// <summary>将整型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的字节数组</param>
		/// <returns>返回整型转换的字节数组</returns>
		protected void BytesToBytes(List<byte> bytes, byte[] value)
		{
			if (value == null || value.Length == 0) { IntToBytes(bytes, 0); return; }
			IntToBytes(bytes, value.Length);
			bytes.AddRange(value);
		}

		/// <summary>将整型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的字节数组</param>
		/// <returns>返回整型转换的字节数组</returns>
		protected void DecimalToBytes(List<byte> bytes, decimal value)
		{
			int[] bits = decimal.GetBits(value);
			IntToBytes(bytes, bits[0]);
			IntToBytes(bytes, bits[1]);
			IntToBytes(bytes, bits[2]);
			IntToBytes(bytes, bits[3]);
		}

		/// <summary>将整型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的字节数组</param>
		/// <returns>返回整型转换的字节数组</returns>
		protected void GuidToBytes(List<byte> bytes, System.Guid value)
		{
			bytes.AddRange(value.ToByteArray());
		}

		/// <summary>将整型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的字节数组</param>
		/// <returns>返回整型转换的字节数组</returns>
		protected void StringToBytes(List<byte> bytes, string value)
		{
			if (string.IsNullOrWhiteSpace(value)) { IntToBytes(bytes, 0); return; }
			byte[] strBytes = System.Text.Encoding.Unicode.GetBytes(value);
			IntToBytes(bytes, strBytes.Length);
			if (strBytes.Length > 0) { bytes.AddRange(strBytes); }
		}

		/// <summary>将布尔类型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的布尔值</param>
		protected void BooleanToBytes(List<byte> bytes, bool value)
		{
			bytes.Add((byte)(value ? 1 : 0));
		}

		/// <summary>将日期类型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的布尔值</param>
		protected void DateTimeToBytes(List<byte> bytes, DateTime value)
		{
			LongToBytes(bytes, value.Ticks);
		}

		/// <summary>将长整型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的字节数组</param>
		protected void LongToBytes(List<byte> bytes, long value)
		{
			bytes.AddRange(BitConverter.GetBytes(value));//8bit
		}

		/// <summary>将 byte 写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的字节数组</param>
		protected void ByteToBytes(List<byte> bytes, byte value) { bytes.Add(value); }

		/// <summary>将整型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="value">需要转换的字节数组</param>
		protected void IntToBytes(List<byte> bytes, int value)
		{
			bytes.Add((byte)((value >> 24) & 0xFF));
			bytes.Add((byte)((value >> 16) & 0xFF));
			bytes.Add((byte)((value >> 8) & 0xFF));
			bytes.Add((byte)(value & 0xFF));
		}

		/// <summary>将整型写入字节数组</summary>
		/// <param name="bytes"><![CDATA[需要写入的List<Byte> 类型数组]]></param>
		/// <param name="values">需要转换的字节数组</param>
		/// <returns>返回整型转换的字节数组</returns>
		protected void IntToBytes(List<byte> bytes, params int[] values)
		{
			if (values == null || values.Length == 0) { return; }
			foreach (int value in values) { IntToBytes(bytes, value); }
		}

		/// <summary>从 Byte[] 中提取布尔类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte数组当前位置</param>
		/// <returns>返回提取的布尔类型的值，如果不存在则返回 false。</returns>
		protected bool BytesToBoolean(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return false; }
			bool result = bytes[offset] > 0; offset++;
			return result;
		}

		/// <summary>从 Byte[] 中提取 DateTime 类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte[] 当前位置</param>
		/// <returns>返回提取的 DateTime 类型的值，如果不存在则返回 DateTime.MinValue。</returns>
		protected DateTime BytesToDateTime(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return DateTime.MinValue; }
			long ticks = BitConverter.ToInt64(bytes, offset); offset += 8;
			return new DateTime(ticks);
		}

		/// <summary>从 Byte[] 中提取 long 类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte[] 当前位置</param>
		/// <returns>返回提取的 long 类型的值，如果不存在则返回 0。</returns>
		protected long BytesToLong(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return 0; }
			offset += 8;
			return BitConverter.ToInt64(bytes, offset - 8);
		}

		/// <summary>从 Byte[] 中提取 byte[] 类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte[] 当前位置</param>
		/// <returns>返回提取的 byte[] 类型的值，如果不存在则返回 null。</returns>
		protected byte[] BytesToBytes(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return null; }
			//字符串长度
			int length = BytesToInt(bytes, ref offset);
			if (length == 0) { return null; }
			byte[] result = new byte[length];
			Array.Copy(bytes, offset, result, 0, length);
			offset += length;
			return result;
		}

		/// <summary>从 Byte[] 中提取 byte 类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte[] 当前位置</param>
		/// <returns>返回提取的 byte 类型的值，如果不存在则返回 0。</returns>
		protected byte BytesToByte(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return 0; }
			byte result = bytes[offset]; offset++;
			return result;
		}

		/// <summary>从 Byte[] 中提取 string 类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte[] 当前位置</param>
		/// <returns>返回提取的 string 类型的值，如果不存在则返回 null。</returns>
		protected string BytesToString(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return null; }
			//字符串长度
			int length = BytesToInt(bytes, ref offset);
			if (length == 0) { return string.Empty; }
			string result = System.Text.Encoding.Unicode.GetString(bytes, offset, length);
			offset += length;
			return result;
		}

		/// <summary>从 Byte[] 中提取 Guid 类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte[] 当前位置</param>
		/// <returns>返回提取的 Guid 类型的值，如果不存在则返回 Guid.Empty。</returns>
		protected System.Guid BytesToGuid(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return System.Guid.Empty; }
			byte[] bits = new byte[16];
			System.Array.Copy(bytes, offset, bits, 0, 16);
			offset += 16;
			return new System.Guid(bits);
		}

		/// <summary>从 Byte[] 中提取 int 类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte[] 当前位置</param>
		/// <returns>返回提取的 int 类型的值，如果不存在则返回 0。</returns>
		protected int BytesToInt(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return 0; }
			byte b1 = bytes[offset]; byte b2 = bytes[offset + 1];
			byte b3 = bytes[offset + 2]; byte b4 = bytes[offset + 3]; offset += 4;
			return b1 * 16777216 + b2 * 65536 + b3 * 256 + b4;
		}

		/// <summary>从 Byte[] 中提取 decimal 类型的值</summary>
		/// <param name="bytes">Byte[] 类型数组，其中包含要提取的值。</param>
		/// <param name="offset">Byte[] 当前位置</param>
		/// <returns>返回提取的 decimal 类型的值，如果不存在则返回 0。</returns>
		protected decimal BytesToDecimal(byte[] bytes, ref int offset)
		{
			if (offset >= bytes.Length) { return 0M; }
			int b1 = BytesToInt(bytes, ref offset);
			int b2 = BytesToInt(bytes, ref offset);
			int b3 = BytesToInt(bytes, ref offset);
			int b4 = BytesToInt(bytes, ref offset);
			return new decimal(new int[] { b1, b2, b3, b4 });
		}
		#endregion
	}
}
