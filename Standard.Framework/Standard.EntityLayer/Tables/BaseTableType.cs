using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Basic.EntityLayer;
using Basic.Interfaces;
using System.Linq.Expressions;
using Basic.Collections;

namespace Basic.Tables
{
	/// <summary>
	/// 表示内存中的数据表,此为抽象表,只能有子类继承
	/// </summary>
	/// <typeparam name="TR">继承于BaseTableRowType类型</typeparam>
	[System.Serializable()]
	public abstract partial class BaseTableType<TR> : DataTable, ISerializable, IXmlSerializable, IPagination<TR> where TR : BaseTableRowType
	{
		#region 构造函数和类初始化方法
		/// <summary>
		/// 使用将AbstractEntity对象序列化所需的数据填充 SerializationInfo。
		/// </summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
		protected BaseTableType(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			InitColumns();
		}

		/// <summary>
		/// 使用带参数的构造方法，创建AbstractEntity类实例
		/// </summary>
		/// <param name="entity">DataTable 类实例</param>
		protected BaseTableType(DataTable entity)
			: this(entity.MinimumCapacity, entity.TableName)
		{
			if ((entity.CaseSensitive != entity.DataSet.CaseSensitive))
				this.CaseSensitive = entity.CaseSensitive;
			if ((entity.Locale.ToString() != entity.DataSet.Locale.ToString()))
				this.Locale = entity.Locale;
			if ((entity.Namespace != entity.DataSet.Namespace))
				this.Namespace = entity.Namespace;
			this.Prefix = entity.Prefix;
		}

		/// <summary>
		/// 使用tableName参数的构造函数初始化一个AbstractEntity实例。
		/// </summary>
		/// <param name="tableName">数据表名称</param>
		protected BaseTableType(string tableName) : this(50, tableName) { }

		/// <summary>
		/// 使用tableName参数的构造函数初始化一个AbstractEntity实例。
		/// </summary>
		/// <param name="capacity"></param>
		/// <param name="tableName"></param>
		protected BaseTableType(int capacity, string tableName)
			: base(tableName)
		{
			//FillMode = FillMode.Truncate;
			this.MinimumCapacity = capacity;
			this.BeginInit();
			this.InitClass();
			this.EndInit();
		}

		/// <summary>
		/// 使用不带参数的构造函数初始化一个AbstractEntity实例。
		/// </summary>
		protected internal BaseTableType()
			: base()
		{
			//FillMode = FillMode.Truncate;
			this.MinimumCapacity = 50;
			this.BeginInit();
			this.InitClass();
			this.EndInit();
		}

		/// <summary>
		/// 初始化列信息
		/// </summary>
		protected abstract void InitColumns();
		/// <summary>
		/// 初始化类实例
		/// </summary>
		protected abstract void InitClass();
		#endregion

		#region 属性
		/// <summary>
		/// 表中行记录数
		/// </summary>
		public int Count { get { return Rows.Count; } }
		/// <summary>
		/// 读取AbstractEntity中第一条记录
		/// </summary>
		public TR FirstRow { get { return GetFirstRow(); } }

		/// <summary>
		/// 读取AbstractEntity中最后一条记录
		/// </summary>
		public TR LastRow { get { return GetLastRow(); } }

		/// <summary>
		/// 获取指定索引处的行
		/// </summary>
		/// <param name="index">要返回的行的从零开始的索引。</param>
		/// <returns>返回指定索引的TableRow实例</returns>
		public TR this[int index]
		{
			get { return this.Rows[index] as TR; }
		}

		/// <summary>
		/// 获取包含指定的主键值的行
		/// </summary>
		/// <param name="keyValue">要查找的 DataRow 的主键值</param>
		/// <exception cref="System.Data.MissingPrimaryKeyException">该表没有主键</exception>
		/// <returns>包含指定的主键值的 DataRow；否则为空值（如果 GoldSoftEntity 中不存在主键值）。 </returns>
		public TR this[object keyValue]
		{
			get { return this.FindByKey(keyValue); }
		}

		/// <summary>
		/// 获取包含指定的主键值的行。 
		/// </summary>
		/// <param name="keyArray">要查找的主键值的数组。数组的类型为 Object。</param>
		/// <exception cref="System.Data.MissingPrimaryKeyException">该表没有主键</exception>
		/// <returns>包含指定的主键值的 DataRow；否则为空值（如果 GoldSoftEntity 中不存在主键值）。 </returns>
		public TR this[params object[] keyArray]
		{
			get { return this.FindByKey(keyArray); }
		}

		/// <summary>
		/// 创建与该表具有相同架构的新 TR。 
		/// </summary>
		/// <returns>TR，其架构与 AbstractEntity 的架构相同。 </returns>
		public TR NewDataRow()
		{
			return this.NewRow() as TR;
		}
		#endregion

		#region 行操作方法
		/// <summary>
		/// 转换目标值
		/// </summary>
		/// <param name="dataType">数据类型</param>
		/// <param name="formValue">页面字符串值</param>
		/// <returns></returns>
		private object ConvertObjectValue(Type dataType, string formValue)
		{
			if (formValue == string.Empty)
				return DBNull.Value;
			if (dataType == typeof(bool))
				return Convert.ToBoolean(formValue);
			else if (dataType == typeof(byte))
				return Convert.ToByte(formValue);
			else if (dataType == typeof(short))
				return Convert.ToInt16(formValue);
			else if (dataType == typeof(int))
				return Convert.ToInt32(formValue);
			else if (dataType == typeof(long))
				return Convert.ToInt64(formValue);
			else if (dataType == typeof(decimal))
				return Convert.ToDecimal(formValue);
			else if (dataType == typeof(Guid))
				return new Guid(formValue);
			else if (dataType == typeof(DateTime))
				return Convert.ToDateTime(formValue);
			return formValue;
		}

		/// <summary>
		/// 读取AbstractEntity中第一条记录
		/// </summary>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public TR GetFirstRow()
		{
			if (Count > 0)
				return this.Rows[0] as TR;
			return null;
		}

		/// <summary>
		/// 读取AbstractEntity中第一条记录
		/// </summary>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public TR GetLastRow()
		{
			if (Count > 0)
				return this.Rows[0] as TR;
			return null;
		}

		/// <summary>
		/// <![CDATA[按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的所有 <TR> 对象的数组。]]>
		/// </summary>
		/// <param name="formatString">复合格式字符串。</param>
		/// <param name="paramArray">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>DataRow 对象的数组。</returns>
		public virtual TR[] Select(string formatString, params object[] paramArray)
		{
			string filter = string.Format(formatString, paramArray);
			return base.Select(filter) as TR[];
		}

		/// <summary>
		/// <![CDATA[按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的所有 <TR> 对象的数组。]]>
		/// </summary>
		/// <param name="expression">一个Lambda表达式，该 Lambda 表达式表示需要筛选的条件。</param>
		/// <param name="sortexp">一个Lambda表达式，该 Lambda 表达式指定列和排序方向。</param>
		/// <example>table.Select(row=>row.UserKey == 1 AND row.Test == true);</example>
		/// <returns><![CDATA[<TR>对象的数组。]]></returns>
		public TR[] Select(Expression<Func<TR, bool>> expression, Expression<Func<TR, object>> sortexp)
		{
			string sort = this.AnalyzeMemberExpression(sortexp);
			return Select(expression, sort);
		}

		/// <summary>
		/// <![CDATA[按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的所有 <TR> 对象的数组。]]>
		/// </summary>
		/// <param name="expression">一个Lambda表达式，该 Lambda 表达式表示需要筛选的条件。</param>
		/// <param name="sort">一个字符串，它指定列和排序方向。</param>
		/// <example>table.Select(row=>row.UserKey == 1 AND row.Test == true);</example>
		/// <returns><![CDATA[<TR>对象的数组。]]></returns>
		public TR[] Select(Expression<Func<TR, bool>> expression, string sort)
		{
			string filter = this.AnalyzeExpression(expression);
			return base.Select(filter, sort) as TR[];
		}

		/// <summary>
		/// <![CDATA[按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的所有 <TR> 对象的数组。]]>
		/// </summary>
		/// <param name="expression">一个Lambda表达式，该 Lambda 表达式表示需要筛选的条件。</param>
		/// <example>table.Select(row=>row.UserKey == 1 AND row.Test == true);</example>
		/// <returns><![CDATA[<TR>对象的数组。]]></returns>
		public TR[] Select(Expression<Func<TR, bool>> expression)
		{
			return Select(expression, string.Empty);
		}

		/// <summary>
		/// <![CDATA[按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的所有 <TR> 对象的数组。]]>
		/// </summary>
		/// <param name="expression">一个Lambda表达式，该 Lambda 表达式表示需要筛选的条件。</param>
		/// <param name="sortexp">一个Lambda表达式，该 Lambda 表达式指定列和排序方向。</param>
		/// <example>table.Select(row=>row.UserKey == 1 AND row.Test == true);</example>
		/// <returns><![CDATA[<TR>对象的数组。]]></returns>
		public TR SelectRow(Expression<Func<TR, bool>> expression, Expression<Func<TR, object>> sortexp)
		{
			string sort = this.AnalyzeMemberExpression(sortexp);
			return SelectRow(expression, sort);
		}

		/// <summary>
		/// <![CDATA[按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的第一个 <TR> 对象。]]>
		/// </summary>
		/// <param name="expression">一个Lambda表达式，该 Lambda 表达式表示需要筛选的条件。</param>
		/// <param name="sort">一个字符串，它指定列和排序方向。</param>
		/// <example>table.Select(row=>row.UserKey == 1 AND row.Test == true);</example>
		/// <returns><![CDATA[<TR>对象的数组。]]></returns>
		public TR SelectRow(Expression<Func<TR, bool>> expression, string sort)
		{
			string filter = this.AnalyzeExpression(expression);
			DataRow[] rowArray = base.Select(filter, sort);
			return (rowArray != null && rowArray.Length > 0) ? rowArray[0] as TR : null;
		}

		/// <summary>
		/// <![CDATA[按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的第一个 <TR> 对象。]]>
		/// </summary>
		/// <param name="expression">一个Lambda表达式，该 Lambda 表达式表示需要筛选的条件。</param>
		/// <example>table.Select(row=>row.UserKey == 1 AND row.Test == true);</example>
		/// <returns><![CDATA[返回找到的第一个 <TR> 对象。]]></returns>
		public TR SelectRow(Expression<Func<TR, bool>> expression)
		{
			return SelectRow(expression, string.Empty);
		}

		/// <summary>
		/// 按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的所有 TR 对象的数组。
		/// </summary>
		/// <param name="formatString">复合格式字符串。</param>
		/// <param name="paramArray">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>TR 对象的数组。</returns>
		public TR[] SelectRows(string formatString, params object[] paramArray)
		{
			return this.Select(formatString, paramArray) as TR[];
		}

		/// <summary>
		/// 按照主键顺序（如果没有主键，则按照添加顺序）获取与筛选条件相匹配的第一条 TR 对象的数组。
		/// </summary>
		/// <param name="formatString">复合格式字符串。</param>
		/// <param name="paramArray">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
		/// <returns>TR 对象的数组。</returns>
		public TR SelectSingleRow(string formatString, params object[] paramArray)
		{
			DataRow[] rowArray = this.Select(formatString, paramArray);
			if (rowArray == null || rowArray.Length <= 0)
				return null;
			return rowArray[0] as TR;
		}

		/// <summary>
		/// 将指定的 EntityRow子类实例添加到表中
		/// </summary>
		/// <param name="row">需要添加的行记录</param>
		public void AddRow(TR row)
		{
			this.Rows.Add(row);
		}

		/// <summary>
		/// 创建使用指定值的行，并将其添加到 System.Data.DataRowCollection 中。
		/// </summary>
		/// <param name="values">用于创建新行的值的数组。</param>
		/// <exception cref="ArgumentException">数组大于表中的列数。</exception>
		/// <exception cref="InvalidCastException">值与其各自的列类型不匹配。</exception>
		/// <exception cref="ConstraintException">添加行将使约束无效。</exception>
		/// <exception cref="NoNullAllowedException">尝试将空值放到 System.Data.DataColumn.AllowDBNull 为 false 的列中。</exception>
		/// <returns>返回创建成功的行</returns>
		protected TR AddRow(object[] values)
		{
			return this.Rows.Add(values) as TR;
		}

		/// <summary>
		/// 从表中移除指定的行
		/// </summary>
		/// <param name="row">要移除的行实例</param>
		public void RemoveRow(TR row)
		{
			this.Rows.Remove(row);
		}

		/// <summary>
		/// 获取包含指定的主键值的行。 
		/// </summary>
		/// <param name="key">要查找的 DataRow 的主键值</param>
		/// <exception cref="System.Data.MissingPrimaryKeyException">该表没有主键</exception>
		/// <returns>包含指定的主键值的 DataRow；否则为空值（如果 GoldSoftEntity 中不存在主键值）。 </returns>
		protected TR FindByKey(object key)
		{
			return this.Rows.Find(key) as TR;
		}

		/// <summary>
		/// 获取包含指定的主键值的行。 
		/// </summary>
		/// <param name="keys">要查找的主键值的数组。数组的类型为 Object。</param>
		/// <exception cref="System.Data.MissingPrimaryKeyException">该表没有主键</exception>
		/// <returns>包含指定的主键值的 DataRow；否则为空值（如果 GoldSoftEntity 中不存在主键值）。 </returns>
		protected TR FindByKey(params object[] keys)
		{
			return this.Rows.Find(keys) as TR;
		}

		/// <summary>
		/// 获取表架构的行类型
		/// </summary>
		/// <returns>返回此表架构的行类型</returns>
		protected override System.Type GetRowType()
		{
			return typeof(TR);
		}

		/// <summary>
		/// 返回 TRow 的数组
		/// </summary>
		/// <param name="size">描述数组大小的 System.Int32 值</param>
		/// <returns>新数组</returns>
		public new TR[] NewRowArray(int size)
		{
			return base.NewRowArray(size) as TR[];
		}
		#endregion

		#region 接口 IEnumerable<TR>
		/// <summary>
		/// 获取该集合的 System.Collections.IEnumerator
		/// </summary>
		/// <returns>该集合的 System.Collections.IEnumerator</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		/// <summary>
		/// 返回一个循环访问集合的枚举器。
		/// </summary>
		/// <returns>可用于循环访问集合的 System.Collections.Generic.IEnumerator&lt;T&gt;。</returns>
		IEnumerator<TR> IEnumerable<TR>.GetEnumerator()
		{
			return base.Rows.Cast<TR>().GetEnumerator();
		}
		#endregion

		#region 将表记录转换为实体数组
		/// <summary>
		/// 将Basic.Table.TableEntity中所有行转换成实体类数组
		/// </summary>
		/// <typeparam name="TE">继承于 AbstractEntity 的实体类</typeparam>
		/// <returns>返回TE类型的数组</returns>
		public TE[] ToArray<TE>() where TE : AbstractEntity, new()
		{
			if (this.Count > 0)
			{
				IReadOnlyCollection<EntityPropertyMeta> propertyArray = EntityPropertyProvidor.GetProperties<TE>();
				List<TE> list = new List<TE>(this.Count);
				foreach (TR row in this)
				{
					TE entity = new TE();
					foreach (EntityPropertyMeta pInfo in propertyArray)
					{
						if (!pInfo.CanWrite) { continue; }
						string name = pInfo.Name;
						if (pInfo.Mapping != null) { name = pInfo.Mapping.SourceColumn; }
						if (row.IsNull(name))
							pInfo.SetValue(entity, null);
						else
							pInfo.SetValue(entity, row[name]);
					}
					list.Add(entity);
				}
				return list.ToArray();
			}
			return null;
		}

		/// <summary>
		/// 从目标数组的指定索引处开始将整个 IPagination&lt;T&gt;复制到兼容的一维 Array。 
		/// </summary>
		/// <param name="index">array 中从零开始的索引，从此索引处开始进行复制。</param>
		/// <returns>一个数组，它包含 IPagination&lt;T&gt;的元素的副本。</returns>
		public TR[] ToArray(int index)
		{
			return base.Rows.Cast<TR>().ToArray();
		}

		/// <summary><![CDATA[将 IPagination<T>的元素复制到 List<T> 中。]]></summary>
		/// <returns><![CDATA[一个 List<T>实例，它包含 IPagination<T>的元素的副本。]]></returns>
		public List<TR> ToList() { return new List<TR>(base.Rows.Cast<TR>()); }

		/// <summary>
		/// 将 IPagination&lt;T&gt;的元素复制到新数组中。
		/// </summary>
		/// <returns>一个数组，它包含 IPagination&lt;T&gt;的元素的副本。</returns>
		public TR[] ToArray()
		{
			return ToArray(0);
		}
		#endregion

		#region 接口 ISerializable
		/// <summary>
		/// 使用将TableEntity对象序列化所需的数据填充 SerializationInfo。
		/// </summary>
		/// <param name="info">要填充数据的 SerializationInfo。</param>
		/// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
		#endregion

		#region IPagination 成员
		/// <summary>
		/// 当前记录总数.
		/// </summary>
		public int Capacity { get; set; }

		/// <summary>
		/// 当前页面索引
		/// </summary>
		public int PageIndex { get; internal set; }

		/// <summary>
		/// 每页记录数量
		/// </summary>
		public int PageSize { get; internal set; }

		/// <summary><![CDATA[将指定集合的元素添加到 Pagination<T> 的末尾。]]></summary>
		/// <param name="collection"><![CDATA[一个集合，其元素应被添加到 Pagination<T> 的末尾。 
		/// 集合自身不能为 null，但它可以包含为 null 的元素（如果类型 T 为引用类型）。]]></param>
		public void AddRange(IEnumerable<TR> collection)
		{
			foreach (TR row in collection) { this.ImportRow(row); }
		}

		/// <summary><![CDATA[将对象添加到 IPagination<T> 的结尾处]]></summary>
		/// <param name="item"><![CDATA[要添加到 IPagination<T> 末尾的对象。 对于引用类型，该值可以为 null。]]></param>
		/// <returns>返回添加到集合末尾的元素。</returns>
		public TR Add(TR item) { this.ImportRow(item); return item; }

		/// <summary><![CDATA[确定某元素是否在 IPagination<T> 中]]></summary>
		/// <param name="item"><![CDATA[要在 IPagination<T>中定位的对象。 对于引用类型，该值可以为 null。]]></param>
		public bool Contains(TR item)
		{
			return this.Rows.Cast<TR>().Contains(item);
		}

		/// <summary>
		/// <![CDATA[对 Pagination<TR> 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 Pagination<TR> 的每个元素执行的 Action<TR> 委托。]]></param>
		public void ForEach(Action<TR> action)
		{
			if (action == null) { throw new ArgumentNullException("action"); }
			foreach (TR row in base.Rows) { action(row); }
		}

		/// <summary>
		/// <![CDATA[对 Pagination<T> 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 Pagination<T> 的每个元素执行的 System.Action<TR, int> 委托。]]></param>
		public void ForEach(Action<TR, int> action)
		{
			if (action == null) { throw new ArgumentNullException("action"); }
			for (int index = 0; index < Count; index++) { action(base.Rows[index] as TR, index); }
		}

		/// <summary>
		/// <![CDATA[对 Pagination<TR> 的每个元素执行指定操作。在执行此方法期间不允许更改集合大小和集合元素。]]>
		/// </summary>
		/// <param name="action"><![CDATA[要对 Pagination<TR> 的每个元素执行的 System.Action<TR> 委托。]]></param>
		/// <param name="match"><![CDATA[要对 Pagination<TR> 的每个元素执行的 System.Predicate<TR> 委托。]]></param>
		public void ForEach(Action<TR> action, System.Predicate<TR> match)
		{
			if (action == null) { throw new ArgumentNullException("action"); }
			if (match == null) { throw new ArgumentNullException("match"); }

			for (int index = 0; index < Count; index++)
			{
				TR entity = this[index]; if (match(entity)) { action(entity); }
			}
		}

		#endregion

		#region IXmlSerializable Members
		/// <summary>
		/// 此方法是保留方法，请不要使用。在实现 IXmlSerializable 接口时，
		/// 应从此方法返回 null（在 Visual Basic 中为 Nothing），
		/// 如果需要指定自定义架构，应向该类应用 XmlSchemaProviderAttribute。
		/// </summary>
		/// <returns>XmlSchema，描述由 WriteXml 方法产生并由 ReadXml 方法使用的对象的 XML 表示形式。</returns>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return base.GetSchema();
		}
		/// <summary>
		/// 从对象的 XML 表示形式生成该对象。
		/// </summary>
		/// <param name="reader">对象从中进行反序列化的 XmlReader 流。</param>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			base.ReadXml(reader);
		}
		/// <summary>
		/// 将对象转换为其 XML 表示形式。
		/// </summary>
		/// <param name="writer">对象要序列化为的 XmlWriter 流。</param>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			base.WriteXml(writer);
		}

		#endregion

		#region 接口 INotifyCollectionChanged 实现
		/// <summary>
		/// 集合更改时引发的事件
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// 当集合更改时发生，即DataTable内行数量更改是引发的事件。
		/// </summary>
		/// <param name="e">引发集合更改时的事件参数。</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, e);
			}
		}
		#endregion
	}
}
