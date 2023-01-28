using System;
using System.Xml;
using Basic.Collections;
using Basic.Designer;
using Basic.Enums;

namespace Basic.Database
{
	/// <summary>
	/// 数据库表列信息
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(TableInfoConverter))]
	public sealed class DesignTableInfo : AbstractCustomTypeDescriptor
	{
		#region 实体定义字段
		internal const string CreatedTimeColumn = "CREATEDTIME";
		internal const string CreateTimeColumn = "CREATETIME";
		internal const string ModifiedTimeColumn = "MODIFIEDTIME";
		internal const string ModifyTimeColumn = "MODIFYTIME";
		private readonly AbstractCustomTypeDescriptor notifyObject;
		private readonly DesignColumnCollection _TableColumns;
		private readonly UniqueConstraintCollection _UniqueConstraints;
		private readonly UniqueConstraint _PrimaryKeyConstraint;
		internal readonly string XmlNamespace;
		internal readonly string XmlPrefix;

		internal const string XmlElementName = "TableInfo";
		internal const string TableNameAttribute = "TableName";
		internal const string ViewNameAttribute = "ViewName";
		internal const string EntityNameAttribute = "EntityName";
		internal const string DescriptionAttribute = "Description";
		internal const string OwnerAttribute = "Owner";
		internal const string ObjectTypeAttribute = "ObjectType";
		#endregion

		/// <summary>
		/// 初始化 DesignTableInfo 类实例
		/// </summary>
		/// <param name="persistent">包含此类实例的 PersistentConfiguration 类实例。</param>
		internal DesignTableInfo(AbstractCustomTypeDescriptor persistent)
			: this(persistent, null, null) { }

		/// <summary>
		/// 初始化 DesignTableInfo 类实例
		/// </summary>
		/// <param name="persistent">包含此类实例的 PersistentConfiguration 类实例。</param>
		/// <param name="prefix">Xml文档元素前缀。</param>
		/// <param name="elementns">Xml文档元素命名空间。</param>
		internal DesignTableInfo(AbstractCustomTypeDescriptor persistent, string prefix, string elementns)
			: base(persistent)
		{
			notifyObject = persistent;
			_TableColumns = new DesignColumnCollection(this);
			_UniqueConstraints = new UniqueConstraintCollection(this);
			_PrimaryKeyConstraint = new UniqueConstraint(this, _TableColumns, true);
			XmlNamespace = elementns;
			XmlPrefix = prefix;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableInfo"></param>
		internal void CopyFrom(DesignTableInfo tableInfo)
		{
			this._TableName = tableInfo._TableName;
			this._ViewName = tableInfo._ViewName;
			this._Description = tableInfo._Description;
			this._EntityName = tableInfo._EntityName;
			this._OldEntityName = tableInfo._OldEntityName;
			this._ObjectType = tableInfo._ObjectType;
			this._Owner = tableInfo._Owner;
		}

		/// <summary>
		/// 当前表信息是否为空
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public bool IsEmpty
		{
			get { return string.IsNullOrWhiteSpace(_TableName); }
		}

		private string _Owner = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[System.ComponentModel.Browsable(false)]
		public string Owner
		{
			get { return _Owner; }
			internal set
			{
				if (_Owner != value)
				{
					_Owner = value;
					base.RaisePropertyChanged("Owner");
				}
			}
		}

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[System.ComponentModel.Browsable(false)]
		public string Type
		{
			get
			{
				if (_ObjectType == ObjectTypeEnum.UserTable)
					return "Table";
				else if (_ObjectType == ObjectTypeEnum.ClrTableFunction)
					return "Table";
				else if (_ObjectType == ObjectTypeEnum.InlineTableFunction)
					return "Table";
				else if (_ObjectType == ObjectTypeEnum.SqlTableFunction)
					return "Table";
				else if (_ObjectType == ObjectTypeEnum.UserView)
					return "View";
				else if (_ObjectType == ObjectTypeEnum.StoredProcedure)
					return "Procedure";
				return "Table";
			}
		}

		private ObjectTypeEnum _ObjectType = ObjectTypeEnum.UserTable;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[System.ComponentModel.Browsable(false)]
		public ObjectTypeEnum ObjectType
		{
			get { return _ObjectType; }
			internal set
			{
				if (_ObjectType != value)
				{
					_ObjectType = value;
					base.RaisePropertyChanged("Type");
					base.RaisePropertyChanged("ObjectType");
				}
			}
		}

		private string _TableName = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_TableName")]
		public string TableName
		{
			get { return _TableName; }
			internal set
			{
				if (_TableName != value)
				{
					_TableName = value;
					base.RaisePropertyChanged("TableName");
				}
			}
		}

		private string _ViewName = null;
		/// <summary>
		/// 当前配置文件关联的数据库视图名称
		/// </summary>
		/// <value>数据库视图名称。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_ViewName")]
		public string ViewName
		{
			get { return _ViewName; }
			set
			{
				if (_ViewName != value)
				{
					_ViewName = value;
					base.RaisePropertyChanged("ViewName");
				}
			}
		}

		private string _EntityName = null;
		private string _OldEntityName = null;
		internal string OldEntityName { get { return _OldEntityName; } }
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_EntityName")]
		public string EntityName
		{
			get { return _EntityName; }
			set
			{
				if (_EntityName != value)
				{
					if (string.IsNullOrWhiteSpace(_EntityName)) { _OldEntityName = value; }
					else { _OldEntityName = _EntityName; }
					_EntityName = value;
					base.RaisePropertyChanged("EntityName");
				}
			}
		}

		private string _Description = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		[Basic.Designer.PackageDescription("PersistentDescription_Description")]
		public string Description
		{
			get { return _Description; }
			set
			{
				if (_Description != value)
				{
					_Description = value;
					base.RaisePropertyChanged("Description");
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Basic.Designer.PackageDescription("数据库列表列集合。")]
		public DesignColumnCollection Columns { get { return _TableColumns; } }

		/// <summary>
		/// 
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public UniqueConstraintCollection UniqueConstraints { get { return _UniqueConstraints; } }

		/// <summary>
		/// 
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public UniqueConstraint PrimaryKey { get { return _PrimaryKeyConstraint; } }

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(_Owner) && !string.IsNullOrWhiteSpace(_TableName))
				return string.Concat(_Owner, ".", _TableName);
			return typeof(DesignTableInfo).Name.Replace("Element", "");
		}
		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		protected internal override bool ReadAttribute(string name, string value)
		{
			if (name == TableNameAttribute) { _TableName = value; return true; }
			else if (name == ViewNameAttribute) { _ViewName = value; return true; }
			else if (name == OwnerAttribute) { _Owner = value; return true; }
			else if (name == ObjectTypeAttribute) { return Enum.TryParse<ObjectTypeEnum>(value, out _ObjectType); }
			else if (name == EntityNameAttribute) { _EntityName = value; return true; }
			else if (name == DescriptionAttribute) { _Description = value; return true; }
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>如果当前节点已经读到结尾则返回 true，否则返回 false。</returns>
		protected internal override bool ReadContent(System.Xml.XmlReader reader)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Whitespace) { continue; }
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == DesignColumnInfo.XmlElementName)
				{
					DesignColumnInfo column = _TableColumns.CreateColumn();
					column.ReadXml(reader.ReadSubtree());
					_TableColumns.Add(column);
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == UniqueConstraintCollection.XmlElementName)
				{
					_UniqueConstraints.ReadXml(reader.ReadSubtree());
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == UniqueConstraint.XmlElementName)
				{
					_PrimaryKeyConstraint.ReadXml(reader.ReadSubtree());
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.LocalName == UniqueConstraint.PrimaryKeyElementName)
				{
					_PrimaryKeyConstraint.ReadXml(reader.ReadSubtree());
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement && reader.LocalName == XmlElementName)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式中属性部分。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString(TableNameAttribute, _TableName);
			writer.WriteAttributeString(ViewNameAttribute, _ViewName);
			writer.WriteAttributeString(OwnerAttribute, _Owner);
			writer.WriteAttributeString(ObjectTypeAttribute, _ObjectType.ToString());
			writer.WriteAttributeString(EntityNameAttribute, _EntityName);
			if (!string.IsNullOrWhiteSpace(_Description))
				writer.WriteAttributeString(DescriptionAttribute, _Description);
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		protected internal override void WriteContent(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(DesignColumnCollection.XmlElementName);
			foreach (DesignColumnInfo tableColumn in _TableColumns)
				tableColumn.WriteXml(writer);
			writer.WriteEndElement();
			_PrimaryKeyConstraint.WriteXml(writer);
			writer.WriteStartElement(UniqueConstraintCollection.XmlElementName);
			foreach (UniqueConstraint uniqueConstraint in _UniqueConstraints)
				uniqueConstraint.WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 将对象转换为其 XML 表示形式,共SQL SERVER/ORACLE使用
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		/// <param name="connectionType">表示数据库连接类型</param>
		protected internal override void GenerateConfiguration(XmlWriter writer, ConnectionTypeEnum connectionType)
		{
			writer.WriteStartElement(XmlPrefix, XmlElementName, null);
			writer.WriteAttributeString(TableNameAttribute, _TableName);
			writer.WriteAttributeString(ViewNameAttribute, _ViewName);
			if (!string.IsNullOrWhiteSpace(_Description))
				writer.WriteAttributeString(DescriptionAttribute, _Description);
			foreach (DesignColumnInfo tableColumn in _TableColumns)
				tableColumn.GenerateConfiguration(writer, connectionType);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 返回此组件实例的类名。
		/// </summary>
		/// <returns>该对象的类名；如果此类没有名称，则为 null。</returns>
		public override string GetClassName() { return XmlElementName; }

		/// <summary>
		/// 返回此组件实例的名称。
		/// </summary>
		/// <returns>该对象的名称；如果该对象没有名称，则为 null。</returns>
		public override string GetComponentName() { return _TableName ?? XmlElementName; }

		/// <summary>
		/// 获取当前节点元素名称
		/// </summary>
		protected internal override string ElementName { get { return XmlElementName; } }

		/// <summary>
		/// 获取当前节点元素命名空间
		/// </summary>
		protected internal override string ElementNamespace { get { return XmlNamespace; } }

		/// <summary>
		/// 获取当前节点元素前缀
		/// </summary>
		protected internal override string ElementPrefix { get { return XmlPrefix; } }
		#endregion

		///// <summary>
		///// 根据数据库列信息，创建新增数据库命令及其参数
		///// </summary>
		///// <param name="staticCommand">从配置文件总读取的命令结构信息</param>
		///// <param name="fileClass">配置文件信息</param>
		///// <param name="conType">需要生成命令结构的类型</param>
		//internal StaticCommandElement CreateInsertSqlStruct(DataEntityElement dataEntity, StaticCommandElement staticCommand)
		//{
		//   if (Columns == null || Columns.Count == 0) { return null; }
		//   if (staticCommand == null) { staticCommand = new StaticCommandElement(dataEntity); }
		//   staticCommand.Parameters.Clear();
		//   staticCommand.CheckCommands.Clear();
		//   staticCommand.NewCommands.Clear();
		//   staticCommand.Kind = ConfigurationKindEnum.AddNew;
		//   staticCommand.Name = staticCommand.Kind.ToString();
		//   dataEntity.Comment = this.Description;
		//   dataEntity.Name = string.Concat(EntityName, "C");
		//   dataEntity.TableName = this.TableName;
		//   DataEntityPropertyCollection dataProperties = dataEntity.Properties;
		//   StringBuilder sqlBuilder = new StringBuilder("INSERT INTO ", 1000);
		//   sqlBuilder.AppendFormat("{0}(", TableName);
		//   int insertLength = sqlBuilder.Length;
		//   StringBuilder valueBuilder = new StringBuilder("VALUES(", 500);
		//   int valueLength = valueBuilder.Length;

		//   #region 构建 INSERT,实体类
		//   foreach (TableColumnElement column in tableColumns)
		//   {
		//      string pn = StringHelper.GetCsName(column.Name);
		//      string columnName = column.Name.ToUpper();

		//      if (insertLength == sqlBuilder.Length)
		//         sqlBuilder.AppendFormat("{0}", column.Name);
		//      else
		//         sqlBuilder.AppendFormat(",{0}", column.Name);

		//      if (columnName == CreatedTimeColumn && column.DbType == DbTypeEnum.DateTime)
		//      {
		//         if (dataProperties.ContainsKey(pn))
		//            dataProperties.Remove(pn);
		//         if (valueLength == valueBuilder.Length)
		//            valueBuilder.Append("{%NOW%}");
		//         else
		//            valueBuilder.Append(",{%NOW%}");
		//         continue;
		//      }
		//      else if (columnName == ModifiedTimeColumn && (column.DbType == DbTypeEnum.DateTime ||
		//         column.DbType == DbTypeEnum.DateTime2 || column.DbType == DbTypeEnum.Timestamp))
		//      {
		//         if (dataProperties.ContainsKey(pn))
		//            dataProperties.Remove(pn);
		//         if (valueLength == valueBuilder.Length)
		//            valueBuilder.Append("{%NOW%}");
		//         else
		//            valueBuilder.Append(",{%NOW%}");
		//         continue;
		//      }
		//      DataEntityPropertyElement property;
		//      if (dataProperties.TryGetValue(column.Name, out property))
		//         DbBuilderHelper.CreateAbstractProperty(property, column, true);
		//      else
		//      {
		//         property = new DataEntityPropertyElement(dataEntity);
		//         DbBuilderHelper.CreateAbstractProperty(property, column, false);
		//         dataProperties.Add(property);
		//      }

		//      CommandParameter parameter = new CommandParameter(staticCommand);
		//      parameter.Name = column.Name;
		//      parameter.SourceColumn = column.Name;
		//      parameter.ParameterType = column.DbType;
		//      if (column.DbType == DbTypeEnum.Decimal)
		//      {
		//         parameter.Precision = column.Precision;
		//         parameter.Scale = (byte)column.Scale;
		//      }
		//      else
		//      {
		//         parameter.Size = column.Size;
		//      }
		//      parameter.Direction = ParameterDirection.Input;
		//      parameter.Nullable = column.Nullable;
		//      staticCommand.Parameters.Add(parameter);
		//      if (valueLength == valueBuilder.Length)
		//         valueBuilder.Append(staticCommand.CreateParameterName(column.Name));
		//      else
		//         valueBuilder.AppendFormat(",{0}", staticCommand.CreateParameterName(column.Name));
		//   }
		//   sqlBuilder.AppendLine(")");
		//   sqlBuilder.Append(valueBuilder.ToString());
		//   sqlBuilder.Append(")");
		//   staticCommand.CommandText = sqlBuilder.ToString();
		//   #endregion

		//   #region 创建主键新值命令
		//   if (primaryKeyConstraint.Columns.Count > 0)
		//   {
		//      NewCommandElement newCommand = new NewCommandElement(staticCommand);
		//      newCommand.Name = primaryKeyConstraint.Name;
		//      foreach (TableColumnElement column in primaryKeyConstraint.Columns)
		//      {
		//      }
		//      staticCommand.NewCommands.Add(newCommand);
		//   }
		//   #endregion

		//   #region 创建约束检查
		//   if (uniqueConstraints.Count > 0)
		//   {
		//      foreach (TableUniqueConstraint unique in uniqueConstraints)
		//      {
		//         sqlBuilder.Clear();
		//         CheckCommandElement checkCommand = new CheckCommandElement(staticCommand);
		//         checkCommand.Name = unique.Name;
		//         sqlBuilder.AppendFormat("SELECT 1 FROM {0} WHERE ", TableName); int length = sqlBuilder.Length;
		//         foreach (TableColumnElement column in unique.Columns)
		//         {
		//            string parameterName = checkCommand.CreateParameterName(column.Name);
		//            if (length == sqlBuilder.Length)
		//               sqlBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
		//            else
		//               sqlBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
		//            CommandParameter parameter = new CommandParameter(checkCommand);
		//            parameter.Name = column.Name;
		//            parameter.SourceColumn = column.Name;
		//            parameter.ParameterType = column.DbType;
		//            if (column.DbType == DbTypeEnum.Decimal)
		//            {
		//               parameter.Precision = column.Precision;
		//               parameter.Scale = (byte)column.Scale;
		//            }
		//            else
		//            {
		//               parameter.Size = column.Size;
		//            }
		//            parameter.Direction = ParameterDirection.Input;
		//            parameter.Nullable = column.Nullable;
		//            checkCommand.Parameters.Add(parameter);
		//         }
		//         checkCommand.CommandText = sqlBuilder.ToString();
		//         staticCommand.CheckCommands.Add(checkCommand);
		//      }
		//   }
		//   #endregion
		//   return staticCommand;
		//}

		///// <summary>
		///// 根据数据库列信息，创建新增数据库命令及其参数
		///// </summary>
		///// <param name="staticCommand">从配置文件总读取的命令结构信息</param>
		///// <param name="fileClass">配置文件信息</param>
		///// <param name="conType">需要生成命令结构的类型</param>
		//internal StaticCommandElement CreateUpdateSqlStruct(DataEntityElement dataEntity, StaticCommandElement staticCommand)
		//{
		//   if (Columns == null || Columns.Count == 0) { return null; }
		//   if (staticCommand == null) { staticCommand = new StaticCommandElement(dataEntity); }
		//   staticCommand.Kind = ConfigurationKindEnum.Update;
		//   staticCommand.Name = staticCommand.Kind.ToString();
		//   staticCommand.Parameters.Clear();
		//   staticCommand.CheckCommands.Clear();
		//   staticCommand.NewCommands.Clear();
		//   dataEntity.Comment = this.Description;
		//   dataEntity.Name = string.Concat(EntityName, "E");
		//   dataEntity.TableName = this.TableName;
		//   DataEntityPropertyCollection dataProperties = dataEntity.Properties;

		//   #region 构建 UPDATE SQL
		//   StringBuilder updateBuilder = new StringBuilder("UPDATE ", 1000);
		//   updateBuilder.AppendFormat("{0} SET ", TableName);
		//   int updateLength = updateBuilder.Length;
		//   StringBuilder whereBuilder = new StringBuilder(" WHERE ", 500);
		//   int whereLength = whereBuilder.Length;
		//   TableColumnElement timeStampColumn = null;
		//   foreach (TableColumnElement column in tableColumns)
		//   {
		//      string pn = StringHelper.GetCsName(column.Name);
		//      DataEntityPropertyElement property;
		//      if (dataProperties.TryGetValue(column.Name, out property))
		//         DbBuilderHelper.CreateAbstractProperty(property, column, true);
		//      else
		//      {
		//         property = new DataEntityPropertyElement(dataEntity);
		//         DbBuilderHelper.CreateAbstractProperty(property, column, false);
		//         dataProperties.Add(property);
		//      }

		//      if (column.Name == CreatedTimeColumn)
		//         continue;
		//      if ((column.DbType == DbTypeEnum.DateTime || column.DbType == DbTypeEnum.DateTime2)
		//         && column.Name == ModifiedTimeColumn)
		//      {
		//         timeStampColumn = column;
		//      }
		//      else if (column.DbType == DbTypeEnum.Timestamp)
		//      {
		//         timeStampColumn = column;
		//      }
		//      CommandParameter parameter = new CommandParameter(staticCommand);
		//      string parameterName = staticCommand.CreateParameterName(column.Name);
		//      parameter.Name = column.Name;
		//      parameter.SourceColumn = column.Name;

		//      parameter.ParameterType = column.DbType;
		//      if (column.DbType == DbTypeEnum.Decimal)
		//      {
		//         parameter.Precision = column.Precision;
		//         parameter.Scale = (byte)column.Scale;
		//      }
		//      else { parameter.Size = column.Size; }
		//      parameter.Direction = ParameterDirection.Input;
		//      parameter.Nullable = column.Nullable;
		//      staticCommand.Parameters.Add(parameter);
		//      if (!column.PrimaryKey)
		//      {
		//         if (updateLength == updateBuilder.Length)
		//            updateBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
		//         else
		//            updateBuilder.AppendFormat(", {0}={1}", column.Name, parameterName);
		//      }
		//      if (column.PrimaryKey || timeStampColumn == column)
		//      {
		//         if (whereLength == whereBuilder.Length)
		//            whereBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
		//         else
		//            whereBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
		//      }
		//   }
		//   updateBuilder.AppendLine();
		//   updateBuilder.Append(whereBuilder.ToString());
		//   staticCommand.CommandText = updateBuilder.ToString();
		//   #endregion

		//   if (timeStampColumn != null)
		//   {
		//      #region 检测更新时间戳
		//      CheckCommandElement checkCommand = new CheckCommandElement(staticCommand);
		//      checkCommand.Name = "Check_TimeStamp";
		//      StringBuilder builder = new StringBuilder();
		//      builder.AppendFormat("SELECT 1 FROM {0} WHERE ", TableName);
		//      int length = builder.Length;
		//      if (PrimaryKey != null && PrimaryKey.Columns != null && PrimaryKey.Columns.Count > 0)
		//      {
		//         foreach (TableColumnElement column in PrimaryKey.Columns)
		//         {
		//            CommandParameter parameter = new CommandParameter(checkCommand);
		//            string parameterName = staticCommand.CreateParameterName(column.Name);
		//            parameter.Name = column.Name;
		//            parameter.SourceColumn = column.Name;
		//            parameter.ParameterType = column.DbType;
		//            if (column.DbType == DbTypeEnum.Decimal)
		//            {
		//               parameter.Precision = column.Precision;
		//               parameter.Scale = (byte)column.Scale;
		//            }
		//            else { parameter.Size = column.Size; }
		//            parameter.Direction = ParameterDirection.Input;
		//            parameter.Nullable = column.Nullable;
		//            checkCommand.Parameters.Add(parameter);

		//            if (length == builder.Length)
		//               builder.AppendFormat("{0}={1}", column.Name, parameterName);
		//            else
		//               builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
		//         }
		//         builder.AppendFormat(" AND {0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
		//      }
		//      else
		//      {
		//         builder.AppendFormat("{0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
		//      }
		//      checkCommand.ErrorCode = "Global_TimeStamp_Error";
		//      checkCommand.CommandText = builder.ToString();
		//      staticCommand.CheckCommands.Add(checkCommand);
		//      #endregion
		//   }

		//   if (uniqueConstraints.Count > 0)
		//   {
		//      #region 约束检测
		//      foreach (TableUniqueConstraint unique in uniqueConstraints)
		//      {
		//         StringBuilder builder = new StringBuilder();
		//         builder.AppendFormat("SELECT 1 FROM {0} WHERE ", TableName);
		//         int length = builder.Length;
		//         CheckCommandElement checkCommand = new CheckCommandElement(staticCommand);
		//         checkCommand.Name = unique.Name;
		//         if (PrimaryKey != null && PrimaryKey.Columns != null && PrimaryKey.Columns.Count > 0)
		//         {
		//            foreach (TableColumnElement column in PrimaryKey.Columns)
		//            {
		//               CommandParameter parameter = new CommandParameter(checkCommand);
		//               string parameterName = staticCommand.CreateParameterName(column.Name);
		//               parameter.Name = column.Name;
		//               parameter.SourceColumn = column.Name;
		//               parameter.ParameterType = column.DbType;
		//               if (column.DbType == DbTypeEnum.Decimal)
		//               {
		//                  parameter.Precision = column.Precision;
		//                  parameter.Scale = (byte)column.Scale;
		//               }
		//               else { parameter.Size = column.Size; }
		//               parameter.Direction = ParameterDirection.Input;
		//               parameter.Nullable = column.Nullable;
		//               checkCommand.Parameters.Add(parameter);
		//               if (length == builder.Length)
		//                  builder.AppendFormat("{0}<>{1}", column.Name, parameterName);
		//               else
		//                  builder.AppendFormat(" AND {0}<>{1}", column.Name, parameterName);
		//            }
		//         }

		//         foreach (TableColumnElement column in unique.Columns)
		//         {
		//            CommandParameter parameter = new CommandParameter(checkCommand);
		//            string parameterName = staticCommand.CreateParameterName(column.Name);
		//            parameter.Name = column.Name;
		//            parameter.SourceColumn = column.Name;
		//            parameter.ParameterType = column.DbType;
		//            if (column.DbType == DbTypeEnum.Decimal)
		//            {
		//               parameter.Precision = column.Precision;
		//               parameter.Scale = (byte)column.Scale;
		//            }
		//            else { parameter.Size = column.Size; }
		//            parameter.Direction = ParameterDirection.Input;
		//            parameter.Nullable = column.Nullable;
		//            checkCommand.Parameters.Add(parameter);
		//            if (length == builder.Length)
		//               builder.AppendFormat("{0}={1}", column.Name, parameterName);
		//            else
		//               builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
		//         }
		//         checkCommand.CommandText = builder.ToString();
		//         staticCommand.CheckCommands.Add(checkCommand);
		//      }
		//      #endregion
		//   }
		//   return staticCommand;
		//}

		///// <summary>
		///// 根据数据库列信息，创建删除数据库命令及其参数
		///// </summary>
		///// <param name="sqlStruct">从配置文件总读取的命令结构信息</param>
		///// <param name="fileClass">配置文件信息</param>
		///// <param name="conType">需要生成命令结构的类型</param>
		//internal StaticCommandElement CreateDeleteSqlStruct(DataEntityElement dataEntity, StaticCommandElement staticCommand)
		//{
		//   if (Columns == null || Columns.Count == 0) { return null; }
		//   if (staticCommand == null) { staticCommand = new StaticCommandElement(dataEntity); }
		//   staticCommand.Kind = ConfigurationKindEnum.Delete;
		//   staticCommand.Name = staticCommand.Kind.ToString();
		//   staticCommand.Parameters.Clear();
		//   staticCommand.CheckCommands.Clear();
		//   staticCommand.NewCommands.Clear();
		//   dataEntity.Comment = this.Description;
		//   dataEntity.Name = string.Concat(EntityName, "D");
		//   dataEntity.TableName = this.TableName;
		//   DataEntityPropertyCollection dataProperties = dataEntity.Properties;

		//   #region 构建 DELETE SQL
		//   StringBuilder deleteBuilder = new StringBuilder(500);
		//   deleteBuilder.Append("DELETE FROM ").Append(TableName).Append(" WHERE ");
		//   int deleteLength = deleteBuilder.Length;
		//   TableColumnElement timeStampColumn = null;
		//   foreach (TableColumnElement column in Columns)
		//   {
		//      if ((column.DbType == DbTypeEnum.DateTime || column.DbType == DbTypeEnum.DateTime2)
		//         && column.Name == ModifiedTimeColumn) { timeStampColumn = column; }
		//      else if (column.DbType == DbTypeEnum.Timestamp) { timeStampColumn = column; }
		//      if (column.PrimaryKey || timeStampColumn == column)
		//      {
		//         string pn = StringHelper.GetCsName(column.Name);
		//         DataEntityPropertyElement property;
		//         if (dataProperties.TryGetValue(column.Name, out property))
		//            DbBuilderHelper.CreateAbstractProperty(property, column, true);
		//         else
		//         {
		//            property = new DataEntityPropertyElement(dataEntity);
		//            DbBuilderHelper.CreateAbstractProperty(property, column, false);
		//            dataProperties.Add(property);
		//         }

		//         CommandParameter parameter = new CommandParameter(staticCommand);
		//         string parameterName = staticCommand.CreateParameterName(column.Name);
		//         parameter.Name = column.Name;
		//         parameter.SourceColumn = column.Name;

		//         parameter.ParameterType = column.DbType;
		//         if (column.DbType == DbTypeEnum.Decimal)
		//         {
		//            parameter.Precision = column.Precision;
		//            parameter.Scale = (byte)column.Scale;
		//         }
		//         else { parameter.Size = column.Size; }
		//         parameter.Direction = ParameterDirection.Input;
		//         parameter.Nullable = column.Nullable;
		//         staticCommand.Parameters.Add(parameter);
		//         if (deleteLength == deleteBuilder.Length)
		//            deleteBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
		//         else
		//            deleteBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
		//      }
		//   }
		//   staticCommand.CommandText = deleteBuilder.ToString();
		//   #endregion

		//   if (timeStampColumn != null)
		//   {
		//      #region 检测更新时间戳
		//      CheckCommandElement checkCommand = new CheckCommandElement(staticCommand);
		//      checkCommand.Name = "Check_TimeStamp";
		//      StringBuilder builder = new StringBuilder();
		//      builder.AppendFormat("SELECT 1 FROM {0} WHERE ", TableName);
		//      int length = builder.Length;
		//      if (PrimaryKey != null && PrimaryKey.Columns != null && PrimaryKey.Columns.Count > 0)
		//      {
		//         foreach (TableColumnElement column in PrimaryKey.Columns)
		//         {
		//            CommandParameter parameter = new CommandParameter(checkCommand);
		//            string parameterName = staticCommand.CreateParameterName(column.Name);
		//            parameter.Name = column.Name;
		//            parameter.SourceColumn = column.Name;
		//            parameter.ParameterType = column.DbType;
		//            if (column.DbType == DbTypeEnum.Decimal)
		//            {
		//               parameter.Precision = column.Precision;
		//               parameter.Scale = (byte)column.Scale;
		//            }
		//            else { parameter.Size = column.Size; }
		//            parameter.Direction = ParameterDirection.Input;
		//            parameter.Nullable = column.Nullable;
		//            checkCommand.Parameters.Add(parameter);

		//            if (length == builder.Length)
		//               builder.AppendFormat("{0}={1}", column.Name, parameterName);
		//            else
		//               builder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
		//         }
		//         builder.AppendFormat(" AND {0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
		//      }
		//      else
		//      {
		//         builder.AppendFormat("{0}<>{1}", timeStampColumn.Name, checkCommand.CreateParameterName(timeStampColumn.Name));
		//      }
		//      checkCommand.ErrorCode = "Global_TimeStamp_Error";
		//      checkCommand.CommandText = builder.ToString();
		//      staticCommand.CheckCommands.Add(checkCommand);
		//      #endregion
		//   }
		//   return staticCommand;
		//}

		///// <summary>
		///// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		///// </summary>
		///// <param name="sqlStruct">从配置文件总读取的命令结构信息</param>
		///// <param name="tableColumns">数据库列信息</param>
		///// <param name="viewName">表对应的视图名称</param>
		///// <param name="conType">需要生成命令结构的类型</param>
		//internal DynamicCommandElement CreateSelectAllSqlStruct(DataEntityElement dataEntity, DynamicCommandElement dynamicCommand)
		//{
		//   if (dynamicCommand == null) { dynamicCommand = new DynamicCommandElement(dataEntity); }
		//   dynamicCommand.Kind = ConfigurationKindEnum.SearchTable;
		//   dynamicCommand.Name = dynamicCommand.Kind.ToString();
		//   dynamicCommand.Parameters.Clear();

		//   dataEntity.Comment = this.Description;
		//   dataEntity.Name = EntityName;
		//   dataEntity.TableName = this.TableName;
		//   DataEntityPropertyCollection dataProperties = dataEntity.Properties;

		//   DataConditionElement dataCondition = dynamicCommand.DataCondition;
		//   dataCondition.Comment = this.Description;
		//   dataCondition.Name = EntityName;
		//   dataCondition.TableName = this.TableName;
		//   DataConditionPropertyCollection conditionProperties = dataCondition.Arguments;
		//   bool conditionPropertyIsExists = conditionProperties.Count == 0;
		//   StringBuilder selectBuilder = new StringBuilder(600);
		//   int indexColumn = 0; DataConditionPropertyElement conditionProperty;
		//   foreach (TableColumnElement column in tableColumns)
		//   {
		//      string pn = StringHelper.GetCsName(column.Name);
		//      DataEntityPropertyElement property;
		//      if (dataProperties.TryGetValue(column.Name, out property))
		//         DbBuilderHelper.CreateAbstractProperty(property, column, true);
		//      else
		//      {
		//         property = new DataEntityPropertyElement(dataEntity);
		//         DbBuilderHelper.CreateAbstractProperty(property, column, false);
		//         dataProperties.Add(property);
		//      }

		//      if (conditionPropertyIsExists)
		//      {
		//         if (conditionProperties.TryGetValue(column.Name, out conditionProperty))
		//            DbBuilderHelper.CreateAbstractProperty(conditionProperty, column, true);
		//         else
		//         {
		//            conditionProperty = new DataConditionPropertyElement(dataCondition);
		//            DbBuilderHelper.CreateAbstractProperty(conditionProperty, column);
		//            conditionProperties.Add(conditionProperty);
		//         }
		//      }
		//      if (column.PrimaryKey)
		//         dynamicCommand.OrderText = column.Name;
		//      if (indexColumn == 0)
		//         selectBuilder.AppendFormat("{0}", column.Name);
		//      else
		//         selectBuilder.AppendFormat(",{0}", column.Name);
		//      indexColumn++;
		//   }
		//   dynamicCommand.SelectText = selectBuilder.ToString();
		//   dynamicCommand.FromText = TableName;
		//   return dynamicCommand;
		//}

		///// <summary>
		///// 根据数据库列信息，创建查询表中所有数据的命令及其参数
		///// </summary>
		///// <param name="sqlStruct">从配置文件总读取的命令结构信息</param>
		///// <param name="tableColumns">数据库列信息</param>
		///// <param name="viewName">表对应的视图名称</param>
		///// <param name="conType">需要生成命令结构的类型</param>
		//internal StaticCommandElement CreateSelectByPKeySqlStruct(DataEntityElement dataEntity, StaticCommandElement staticCommand)
		//{
		//   if (staticCommand == null) { staticCommand = new StaticCommandElement(dataEntity); }
		//   staticCommand.Kind = ConfigurationKindEnum.SelectByKey;
		//   staticCommand.Name = staticCommand.Kind.ToString();
		//   staticCommand.Parameters.Clear();
		//   staticCommand.CheckCommands.Clear();
		//   staticCommand.NewCommands.Clear();
		//   StringBuilder selectBuilder = new StringBuilder("SELECT ", 1000);
		//   int indexColumn = 0;
		//   StringBuilder whereBuilder = new StringBuilder("WHERE ", 500);
		//   int whereLength = whereBuilder.Length;
		//   foreach (TableColumnElement column in Columns)
		//   {
		//      if (indexColumn == 0)
		//         selectBuilder.AppendFormat("{0}", column.Name);
		//      else
		//         selectBuilder.AppendFormat(",{0}", column.Name);
		//      if (column.PrimaryKey)
		//      {
		//         CommandParameter parameter = new CommandParameter(staticCommand);
		//         string parameterName = staticCommand.CreateParameterName(column.Name);
		//         parameter.Name = column.Name;
		//         parameter.SourceColumn = column.Name;
		//         parameter.ParameterType = column.DbType;
		//         if (column.DbType == DbTypeEnum.Decimal)
		//         {
		//            parameter.Precision = column.Precision;
		//            parameter.Scale = (byte)column.Scale;
		//         }
		//         else { parameter.Size = column.Size; }
		//         parameter.Direction = ParameterDirection.Input;
		//         parameter.Nullable = column.Nullable;
		//         staticCommand.Parameters.Add(parameter);
		//         if (whereLength == whereBuilder.Length)
		//            whereBuilder.AppendFormat("{0}={1}", column.Name, parameterName);
		//         else
		//            whereBuilder.AppendFormat(" AND {0}={1}", column.Name, parameterName);
		//      }
		//      indexColumn++;
		//   }
		//   selectBuilder.AppendLine();
		//   selectBuilder.AppendFormat("FROM {0}", TableName);
		//   selectBuilder.AppendLine();
		//   selectBuilder.Append(whereBuilder.ToString());
		//   staticCommand.CommandText = selectBuilder.ToString();
		//   return staticCommand;
		//}
	}
}
