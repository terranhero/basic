using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using Basic.Tables;

namespace Basic.DataAccess
{
	/// <summary>
	/// 实体类填充模式，此枚举仅限于BaseEntityType类中的Fill方法
	/// </summary>
	public enum FillMode
	{
		/// <summary>
		/// 追加模式，内存表中的数据记录将被保留
		/// </summary>
		Append = 0,
		/// <summary>
		/// 覆盖模式，内存表中的数据记录将被清空
		/// </summary>
		Truncate
	}
	/// <summary>
	/// 表示内存中的数据表,此为抽象表,只能有子类继承
	/// </summary>
	/// <typeparam name="TR">继承于Basic.Table.TableEntityRow类型</typeparam>
	public abstract partial class BaseEntityType<TR> : BaseTableType<TR> where TR : BaseRowType
	{
		#region 构造函数和类初始化方法
#if NET6_0 || NETSTANDARD2_0
      /// <summary>
        /// 使用将Basic.Entity.AbstractEntity对象序列化所需的数据填充 SerializationInfo。
        /// </summary>
        /// <param name="info">要填充数据的 SerializationInfo。</param>
        /// <param name="context">此序列化的目标（请参见 StreamingContext）。</param>
        protected BaseEntityType(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
		/// <summary>
		/// 使用带参数的构造方法，创建Basic.Entity.AbstractEntity类实例
		/// </summary>
		/// <param name="entity">GoldSoftEntity类实例</param>
		protected BaseEntityType(DataTable entity)
			: base(entity) { }

		/// <summary>
		/// 使用tableName参数的构造函数初始化一个Basic.Entity.AbstractEntity实例。
		/// </summary>
		/// <param name="tableName">数据表名称</param>
		protected BaseEntityType(string tableName) : base(50, tableName) { }

		/// <summary>
		/// 使用tableName参数的构造函数初始化一个Basic.Entity.AbstractEntity实例。
		/// </summary>
		/// <param name="capacity"></param>
		/// <param name="tableName"></param>
		protected BaseEntityType(int capacity, string tableName) : base(capacity, tableName) { }

		/// <summary>
		/// 使用不带参数的构造函数初始化一个Basic.Entity.AbstractEntity实例。
		/// </summary>
		protected internal BaseEntityType() : base() { }

		#endregion
	}

	/// <summary>
	/// 表示强类型的抽象的Basic.Table.TableEntityRow类
	/// </summary>
	public abstract class BaseRowType : BaseTableRowType, INotifyPropertyChanging, INotifyPropertyChanged
	{
		/// <summary>
		/// 初始化 AbstractRow 的新实例。从生成器中构造行。仅供内部使用
		/// </summary>
		/// <param name="rb">生成器</param>
		protected BaseRowType(DataRowBuilder rb) : base(rb) { }

		/// <summary>
		/// 获取或设置存储在由名称指定的列中的数据。
		/// </summary>
		/// <param name="columnName">列的名称。</param>
		/// <returns> System.Object，包含该数据。</returns>
		/// <exception cref="System.ArgumentException">该列不属于此表。</exception>
		/// <exception cref="System.ArgumentNullException">column 为 null。</exception>
		/// <exception cref="System.Data.DeletedRowInaccessibleException">尝试对已删除的行设置值。</exception>
		/// <exception cref="System.InvalidCastException">值与列的数据类型不匹配。</exception>
		public new object this[string columnName]
		{
			get { return base[columnName]; }
			set
			{
				if (base[columnName] != value)
				{
					OnPropertyChanging(columnName);
					base[columnName] = value;
					OnPropertyChanged(columnName);
				}
			}
		}

		/// <summary>
		/// 获取或设置存储在指定的 System.Data.DataColumn 中的数据。
		/// </summary>
		/// <param name="column">一个包含数据的 System.Data.DataColumn。</param>
		/// <returns> System.Object，包含该数据。</returns>
		/// <exception cref="System.ArgumentException">该列不属于此表。</exception>
		/// <exception cref="System.ArgumentNullException">column 为 null。</exception>
		/// <exception cref="System.Data.DeletedRowInaccessibleException">尝试对已删除的行设置值。</exception>
		/// <exception cref="System.InvalidCastException">值与列的数据类型不匹配。</exception>
		public new object this[DataColumn column]
		{
			get { return base[column]; }
			set
			{
				if (base[column] != value)
				{
					OnPropertyChanging(column.ColumnName);
					base[column] = value;
					OnPropertyChanged(column.ColumnName);
				}
			}
		}

		#region INotifyPropertyChanging 成员, INotifyPropertyChanged 成员
		///// <summary>
		///// 在属性值更改时发生。
		///// </summary>
		//public event PropertyChangingEventHandler PropertyChanging;
		///// <summary>
		///// 引发PropertyChanging事件
		///// </summary>
		///// <param name="propertyName">其值将更改的属性的名称。</param>
		//protected virtual void OnPropertyChanging(string propertyName)
		//{
		//    if ((this.PropertyChanging != null))
		//    {
		//        this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
		//    }
		//}

		///// <summary>
		///// 在更改属性值时发生。
		///// </summary>
		//public event PropertyChangedEventHandler PropertyChanged;
		///// <summary>
		///// 引发PropertyChanged事件
		///// </summary>
		///// <param name="propertyName">其值将更改的属性的名称。</param>
		//protected virtual void OnPropertyChanged(string propertyName)
		//{
		//    if ((this.PropertyChanged != null))
		//    {
		//        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		//    }
		//}
		#endregion
	}

	/// <summary>
	/// 表示 System.Data.DataTable 中列的架构。
	/// </summary>
	[DefaultProperty("ColumnName"), DesignTimeVisible(false), ToolboxItem(false)]
	public sealed class EntityColumn : DataColumn
	{
		/// <summary>
		/// 当前列是否数据库表列
		/// </summary>
		public bool IsTableColumn { get; private set; }

		/// <summary>
		/// 将 System.Data.DataColumn 类的新实例初始化为类型字符串。
		/// </summary>
		/// <param name="isTableColumn">表示该列是否数据库基表列</param>
		public EntityColumn(bool isTableColumn)
			: base() { IsTableColumn = isTableColumn; }

		/// <summary>
		/// 使用指定的列名称将 System.Data.DataColumn 类的新实例初始化为类型字符串。
		/// </summary>
		/// <param name="columnName">一个字符串，它表示要创建的列的名称。如果设置为 null 或空字符串 ("")，则当添加到列集合中时，将指定一个默认名称。</param>
		/// <param name="isTableColumn">表示该列是否数据库基表列</param>
		public EntityColumn(string columnName, bool isTableColumn)
			: base() { IsTableColumn = isTableColumn; }

		/// <summary>
		/// 使用指定列名称和数据类型初始化 System.Data.DataColumn 类的新实例。
		/// </summary>
		/// <param name="columnName">一个字符串，它表示要创建的列的名称。如果设置为 null 或空字符串 ("")，则当添加到列集合中时，将指定一个默认名称。</param>
		/// <param name="dataType">支持的 System.Data.DataColumn.DataType。</param>
		/// <param name="isTableColumn">表示该列是否数据库基表列</param>
		/// <exception cref="System.ArgumentNullException">未指定任何 dataType。</exception>
		public EntityColumn(string columnName, Type dataType, bool isTableColumn)
			: base() { IsTableColumn = isTableColumn; }

		/// <summary>
		/// 使用指定的名称、数据类型和表达式初始化 System.Data.DataColumn 类的新实例。
		/// </summary>
		/// <param name="columnName">一个字符串，它表示要创建的列的名称。如果设置为 null 或空字符串 ("")，则当添加到列集合中时，将指定一个默认名称。</param>
		/// <param name="dataType">支持的 System.Data.DataColumn.DataType。</param>
		/// <param name="expr">用于创建该列的表达式。有关更多信息，请参见 System.Data.DataColumn.Expression 属性。</param>
		/// <param name="isTableColumn">表示该列是否数据库基表列</param>
		/// <exception cref="System.ArgumentNullException">未指定任何 dataType。</exception>
		public EntityColumn(string columnName, Type dataType, string expr, bool isTableColumn)
			: base() { IsTableColumn = isTableColumn; }

		/// <summary>
		/// 使用指定名称、数据类型、表达式和确定列是否为属性的值，初始化 System.Data.DataColumn 类的新实例。
		/// </summary>
		/// <param name="columnName">一个字符串，它表示要创建的列的名称。如果设置为 null 或空字符串 ("")，则当添加到列集合中时，将指定一个默认名称。</param>
		/// <param name="dataType">支持的 System.Data.DataColumn.DataType。</param>
		/// <param name="expr">用于创建该列的表达式。有关更多信息，请参见 System.Data.DataColumn.Expression 属性。</param>
		/// <param name="type">System.Data.MappingType 值之一。</param>
		/// <param name="isTableColumn">表示该列是否数据库基表列</param>
		/// <exception cref="System.ArgumentNullException">未指定任何 dataType。</exception>
		public EntityColumn(string columnName, Type dataType, string expr, MappingType type, bool isTableColumn)
			: base() { IsTableColumn = isTableColumn; }

		/// <summary>
		/// 将 System.Data.DataColumn 类的新实例初始化为类型字符串。
		/// </summary>
		public EntityColumn() : base() { }

		/// <summary>
		/// 使用指定的列名称将 System.Data.DataColumn 类的新实例初始化为类型字符串。
		/// </summary>
		/// <param name="columnName">一个字符串，它表示要创建的列的名称。如果设置为 null 或空字符串 ("")，则当添加到列集合中时，将指定一个默认名称。</param>
		public EntityColumn(string columnName) : base() { }

		/// <summary>
		/// 使用指定列名称和数据类型初始化 System.Data.DataColumn 类的新实例。
		/// </summary>
		/// <param name="columnName">一个字符串，它表示要创建的列的名称。如果设置为 null 或空字符串 ("")，则当添加到列集合中时，将指定一个默认名称。</param>
		/// <param name="dataType">支持的 System.Data.DataColumn.DataType。</param>
		/// <exception cref="System.ArgumentNullException">未指定任何 dataType。</exception>
		public EntityColumn(string columnName, Type dataType) : base() { }

		/// <summary>
		/// 使用指定的名称、数据类型和表达式初始化 System.Data.DataColumn 类的新实例。
		/// </summary>
		/// <param name="columnName">一个字符串，它表示要创建的列的名称。如果设置为 null 或空字符串 ("")，则当添加到列集合中时，将指定一个默认名称。</param>
		/// <param name="dataType">支持的 System.Data.DataColumn.DataType。</param>
		/// <param name="expr">用于创建该列的表达式。有关更多信息，请参见 System.Data.DataColumn.Expression 属性。</param>
		/// <exception cref="System.ArgumentNullException">未指定任何 dataType。</exception>
		public EntityColumn(string columnName, Type dataType, string expr) : base() { }

		/// <summary>
		/// 使用指定名称、数据类型、表达式和确定列是否为属性的值，初始化 System.Data.DataColumn 类的新实例。
		/// </summary>
		/// <param name="columnName">一个字符串，它表示要创建的列的名称。如果设置为 null 或空字符串 ("")，则当添加到列集合中时，将指定一个默认名称。</param>
		/// <param name="dataType">支持的 System.Data.DataColumn.DataType。</param>
		/// <param name="expr">用于创建该列的表达式。有关更多信息，请参见 System.Data.DataColumn.Expression 属性。</param>
		/// <param name="type">System.Data.MappingType 值之一。</param>
		/// <exception cref="System.ArgumentNullException">未指定任何 dataType。</exception>
		public EntityColumn(string columnName, Type dataType, string expr, MappingType type) : base() { }
	}
}
