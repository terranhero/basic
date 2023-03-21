using Basic.EntityLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Basic.DataAccess
{
	/// <summary>
	/// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
	/// </summary>
	[System.ComponentModel.ToolboxItem(false), System.Serializable()]
	[System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class JoinCommand : Component, IXmlSerializable, INotifyPropertyChanged
	{
		#region Xml 节点名称常量
		/// <summary>
		/// 表示Xml元素名称
		/// </summary>
		internal const string XmlElementName = "JoinCommand";

		/// <summary>
		/// SelectText 配置节名称
		/// </summary>
		internal const string SelectTextElement = "SelectText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中From 数据库表部分。
		/// </summary>
		internal const string FromTextElement = "FromText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中Where 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		internal const string WhereTextElement = "WhereText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中Group 部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		internal const string GroupTextElement = "GroupText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中Hanving条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		internal const string HavingTextElement = "HavingText";

		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中Order By条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句或存储过程，默认值为空字符串。</value>
		internal const string OrderTextElement = "OrderText";
		#endregion

		/// <summary>
		/// 动态命令结构中静态参数列表
		/// </summary>
		private readonly DbParameter[] m_Parameters;
		private string _JoinOnField;
		/// <summary>
		/// 初始化 DynamicCommand 类的新实例。 
		/// </summary>
		/// <param name="dbCommand"></param>
		internal JoinCommand(DynamicCommand dbCommand)
			: base()
		{
			_SelectText = dbCommand.SelectText;
			_FromText = dbCommand.FromText;
			_WhereText = dbCommand.WhereText;
			_GroupText = dbCommand.GroupText;
			_HavingText = dbCommand.HavingText;
			_OrderText = dbCommand.OrderText;
			m_Parameters = dbCommand.DbParameters.ToArray();
		}

		private Expression RemoveUnary(Expression body)
		{
			if (body is UnaryExpression)
			{
				return (body as UnaryExpression).Operand;
			}
			else if (body is ConditionalExpression)
			{
				return (body as ConditionalExpression).Test;
			}
#if (NET35)
            //else if (body is System.Linq.Expressions.BinaryExpression)
            //{
            //    return (body as BinaryExpression).ReduceExtensions();
            //}
            //else if (body is MethodCallExpression)
            //{
            //    MethodCallExpression mce = body as MethodCallExpression;
            //    return mce.ReduceExtensions();
            //}
#else
			else if (body is System.Linq.Expressions.BinaryExpression)
			{
				return (body as BinaryExpression).ReduceExtensions();
			}
			else if (body is MethodCallExpression)
			{
				MethodCallExpression mce = body as MethodCallExpression;
				return mce.ReduceExtensions();
			}
#endif
			return body;
		}

		/// <summary>
		/// Specifies the columns to use. 
		/// </summary>
		/// <param name="expression">一个表达式，标识包含要呈现的属性的对象。</param>
		/// <returns></returns>
		public void JoinFor<T>(Expression<Func<T, bool>> expression)
		{
			BinaryExpression body = expression.Body as BinaryExpression;
			if (body != null && body.Left is MemberExpression && body.Right is MemberExpression)
			{
				MemberExpression leftMember = body.Left as MemberExpression;
				MemberExpression rightMember = body.Right as MemberExpression;
				string leftField = null, rightField = null;
				ColumnMappingAttribute cma = (ColumnMappingAttribute)Attribute.GetCustomAttribute(leftMember.Member, typeof(ColumnMappingAttribute));
				ColumnAttribute ca = (ColumnAttribute)Attribute.GetCustomAttribute(leftMember.Member, typeof(ColumnAttribute));
				if (cma != null) { leftField = string.IsNullOrEmpty(cma.TableAlias) ? cma.SourceColumn : string.Concat(cma.TableAlias, ".", cma.SourceColumn); }
				else if (ca != null) { leftField = string.IsNullOrEmpty(ca.TableName) ? ca.ColumnName : string.Concat(ca.TableName, ".", ca.ColumnName); }
				else { leftField = leftMember.Member.Name; }

				ColumnMappingAttribute rightMapping = (ColumnMappingAttribute)Attribute.GetCustomAttribute(rightMember.Member, typeof(ColumnMappingAttribute));
				ColumnAttribute rightColumn = (ColumnAttribute)Attribute.GetCustomAttribute(rightMember.Member, typeof(ColumnAttribute));
				if (rightMapping != null) { rightField = string.IsNullOrEmpty(rightMapping.TableAlias) ? rightMapping.SourceColumn : string.Concat(rightMapping.TableAlias, ".", rightMapping.SourceColumn); }
				else if (rightColumn != null) { rightField = string.IsNullOrEmpty(rightColumn.TableName) ? rightColumn.ColumnName : string.Concat(rightColumn.TableName, ".", rightColumn.ColumnName); }
				else { rightField = rightMember.Member.Name; }

				switch (body.NodeType)
				{
					case ExpressionType.Equal:
						_JoinOnField = string.Concat(leftField, " = ", rightField);
						break;
					case ExpressionType.NotEqual:
						_JoinOnField = string.Concat(leftField, " <> ", rightField);
						break;
					case ExpressionType.GreaterThan:
						_JoinOnField = string.Concat(leftField, " > ", rightField);
						break;
					case ExpressionType.GreaterThanOrEqual:
						_JoinOnField = string.Concat(leftField, " >= ", rightField);
						break;
					case ExpressionType.LessThan:
						_JoinOnField = string.Concat(leftField, " < ", rightField);
						break;
					case ExpressionType.LessThanOrEqual:
						_JoinOnField = string.Concat(leftField, " <= ", rightField);
						break;
				}
			}
		}

		/// <summary>
		/// 表示数据库两表连接条件
		/// </summary>
		public string JoinOnCondition { get { return _JoinOnField; } }

		/// <summary>
		/// 清空 JOIN 条件
		/// </summary>
		public void ClearCondition() { _JoinOnField = null; }

		/// <summary>
		/// 需要执行的参数信息。
		/// </summary>
		public DbParameter[] Parameters { get { return m_Parameters; } }

		private string _SelectText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 SELECT 数据库字段部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 SELECT 部分，默认值为空字符串。</value>
		public string SelectText { get { return _SelectText; } }

		private string _FromText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 FROM 数据库表部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 FROM 部分，默认值为空字符串。</value>
		public string FromText { get { return string.Concat("JOIN ", _FromText, " ON ", _JoinOnField); } }

		private string _WhereText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 WHERE 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 WHERE 部分，默认值为空字符串。</value>
		public string WhereText { get { return _WhereText; } }

		private string _GroupText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 GROUP 部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 GROUP 部分，默认值为空字符串。</value>
		public string GroupText { get { return _GroupText; } }

		private string _HavingText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 HANVING 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 HANVING 部分，默认值为空字符串。</value>
		public string HavingText { get { return _HavingText; } }

		private string _OrderText = null;
		/// <summary>
		/// 获取或设置要对数据源执行的 Transact-SQL 语句中 ORDER BY 条件部分。
		/// </summary>
		/// <value>要执行的 Transact-SQL 语句的 ORDER BY 部分，默认值为空字符串。</value>
		public string OrderText { get { return _OrderText; } }

		#region 接口 IXmlSerializable 默认实现
		/// <summary>
		/// 从对象的 XML 表示形式读取属性。
		/// </summary>
		/// <param name="name">属性名称。</param>
		/// <param name="value">属性值</param>
		/// <returns>如果属性存在读取成功则返回true，否则返回false，有子类读取。</returns>
		internal bool ReadAttribute(string name, string value)
		{
			//if (name == CommandTypeAttribute)
			//{
			//    CommandType commandType = CommandType.Text;
			//    if (Enum.TryParse<CommandType>(value, true, out commandType))
			//        dataDbCommand.CommandType = commandType;
			//    return true;
			//}
			//else if (name == CommandTimeoutAttribute)
			//{
			//    dataDbCommand.CommandTimeout = Convert.ToInt32(value);
			//    return true;
			//}
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象扩展信息。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		/// <returns>判断当前对象是否已经读取完成，如果读取完成则返回true，否则返回false。</returns>
		internal bool ReadContent(System.Xml.XmlReader reader)
		{
			//if (reader.NodeType == XmlNodeType.Element && reader.Name == ParametersElement)
			//{
			//    System.Xml.XmlReader reader2 = reader.ReadSubtree();
			//    while (reader2.Read())	//读取所有静态命令节点信息
			//    {
			//        if (reader2.NodeType == XmlNodeType.Element && reader2.Name == DataCommand.ParameterConfig)
			//        {
			//            DbParameter parameter = dataDbCommand.CreateParameter();
			//            CreateParameterReader(parameter, reader2);
			//            dataDbCommand.Parameters.Add(parameter);
			//        }
			//        else if (reader2.NodeType == XmlNodeType.EndElement && reader2.Name == ParametersElement)
			//            break;
			//    }
			//}
			return false;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		internal void ReadXml(System.Xml.XmlReader reader)
		{
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
				if (ReadContent(reader)) { break; }
			}
		}

		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
		/// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { ReadXml(reader); }

		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) { }
		#endregion

		#region 接口 INotifyPropertyChanged 的默认实现
		/// <summary>
		/// 在更改属性值时发生。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 引发 PropertyChanged 事件
		/// </summary>
		/// <param name="propertyName">已更改的属性名。</param>
		internal void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}
}
