using System.Collections.Generic;
using System.Collections.ObjectModel;
using Basic.EntityLayer;

using Basic.Enums;
using Basic.DataAccess;


namespace Basic.Expressions
{
	/// <summary>
	/// lambda 表达式参数解析结果。
	/// </summary>
	public sealed class LambdaExpressionCollection : ObservableCollection<LambdaConditionExpression>
	{
		internal readonly DataCommand dataCommand;
		#region 构造函数
		/// <summary>
		/// 初始化 BaseDictionary&lt;LambdaExprssion&gt; 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
		/// </summary>
		/// <param name="cmd">获取当前 Lambda 表达式关联的动态命令。</param>
		public LambdaExpressionCollection(DataCommand cmd)
			: base() { dataCommand = cmd; }

		/// <summary>
		/// 初始化 ExpressionParemeterCollection 类的新实例，该类包含从指定集合中复制的元素。
		/// </summary>
		/// <param name="cmd">获取当前 Lambda 表达式关联的动态命令。</param>
		/// <param name="collection">从中复制元素的集合。</param>
		/// <exception cref="System.ArgumentNullException">collection 参数不能为 null。</exception>
		public LambdaExpressionCollection(DataCommand cmd, IEnumerable<LambdaConditionExpression> collection)
			: base(collection) { dataCommand = cmd; }

		/// <summary>
		/// 初始化 ExpressionParemeterCollection 类的新实例，该类包含从指定列表中复制的元素。
		/// </summary>
		/// <param name="cmd">获取当前 Lambda 表达式关联的动态命令。</param>
		/// <param name="list">从中复制元素的列表。</param>
		/// <exception cref="System.ArgumentNullException">list 参数不能为 null。</exception>
		public LambdaExpressionCollection(DataCommand cmd, List<LambdaConditionExpression> list)
			: base(list) { dataCommand = cmd; }
		#endregion

		private readonly SortedDictionary<string, int> fieldCollection = new SortedDictionary<string, int>();
		/// <summary>
		/// 判断指定列名是否已经解析过，需要重定义参数名称。
		/// </summary>
		/// <param name="columnName">数据库列名称。</param>
		internal string GetParameterName(string columnName)
		{
			if (fieldCollection.ContainsKey(columnName))
			{
				int count = fieldCollection[columnName] += 1;
				return string.Format("{0}{1}", columnName, count);
			}
			fieldCollection[columnName] = 0;
			return columnName;
		}

		/// <summary>
		/// 向表达式集合中添加ExpressionParemeter 类实例，添加成功后返回ExpressionParemeter 类实例。
		/// </summary>
		/// <param name="cma">字段描述</param>
		/// <returns>ExpressionParemeter 类实例</returns>
		public ConditionExpression AddConditionExpression(ColumnMappingAttribute cma)
		{
			ConditionExpression ep = null;
			if (cma.Size > 0)
				ep = new ConditionExpression(this, cma.TableAlias, cma.SourceColumn, cma.DataType, cma.Size, cma.Nullable);
			else
				ep = new ConditionExpression(this, cma.TableAlias, cma.SourceColumn, cma.DataType, cma.Precision, cma.Scale, cma.Nullable);
			return AddConditionExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <returns>ConditionExpression 类实例</returns>
		public ConditionExpression AddConditionExpression()
		{
			ConditionExpression ep = new ConditionExpression(this);
			return AddConditionExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">表示 Value 属性的最大位数。</param>
		/// <param name="scale">数据库字段的小数位数</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		/// <returns>ExpressionParemeter 类实例</returns>
		public ConditionExpression AddConditionExpression(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
		{
			ConditionExpression ep = new ConditionExpression(this, tableAlias, name, dataType, precision, scale, nullable);
			return AddConditionExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		/// <returns>ExpressionParemeter 类实例</returns>
		public ConditionExpression AddConditionExpression(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable)
		{
			ConditionExpression ep = new ConditionExpression(this, tableAlias, name, dataType, size, nullable);
			return AddConditionExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 ConditionExpression 类实例，添加成功后返回 ConditionExpression 类实例。
		/// </summary>
		/// <param name="ep">需要添加的 ParemeterExpression 类实例。</param>
		/// <returns>ExpressionParemeter 类实例</returns>
		private ConditionExpression AddConditionExpression(ConditionExpression ep)
		{
			base.Add(ep); return ep;
		}

		/// <summary>
		/// 向表达式集合中添加 CalculateExpression 类实例，添加成功后返回 CalculateExpression 类实例。
		/// </summary>
		/// <param name="cma">字段描述</param>
		/// <returns>CalculateExpression 类实例</returns>
		public CalculateExpression AddCalculateExpression(ColumnMappingAttribute cma)
		{
			CalculateExpression ep = null;
			if (cma.Size > 0)
				ep = new CalculateExpression(this, cma.TableAlias, cma.ColumnName, cma.DataType, cma.Size, cma.Nullable);
			else
				ep = new CalculateExpression(this, cma.TableAlias, cma.ColumnName, cma.DataType, cma.Precision, cma.Scale, cma.Nullable);
			return AddCalculateExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 CalculateExpression 类实例，添加成功后返回 CalculateExpression 类实例。
		/// </summary>
		/// <returns>ConditionExpression 类实例</returns>
		public CalculateExpression AddCalculateExpression()
		{
			CalculateExpression ep = new CalculateExpression(this);
			return AddCalculateExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 CalculateExpression 类实例，添加成功后返回 CalculateExpression 类实例。
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">表示 Value 属性的最大位数。</param>
		/// <param name="scale">数据库字段的小数位数</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		/// <returns>CalculateExpression 类实例</returns>
		public CalculateExpression AddCalculateExpression(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
		{
			CalculateExpression ep = new CalculateExpression(this, tableAlias, name, dataType, precision, scale, nullable);
			return AddCalculateExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 CalculateExpression 类实例，添加成功后返回 CalculateExpression 类实例。
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		/// <returns>CalculateExpression 类实例</returns>
		public CalculateExpression AddCalculateExpression(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable)
		{
			CalculateExpression ep = new CalculateExpression(this, tableAlias, name, dataType, size, nullable);
			return AddCalculateExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 CalculateExpression 类实例，添加成功后返回 CalculateExpression 类实例。
		/// </summary>
		/// <param name="ep">需要添加的 ParemeterExpression 类实例。</param>
		/// <returns>CalculateExpression 类实例</returns>
		private CalculateExpression AddCalculateExpression(CalculateExpression ep)
		{
			base.Add(ep); return ep;
		}

		/// <summary>
		/// 向表达式集合中添加 BetweenExpression 类实例，添加成功后返回 BetweenExpression 类实例。
		/// </summary>
		/// <returns>BetweenExpression 类实例</returns>
		public BetweenExpression AddBetweenExpression()
		{
			BetweenExpression ep = new BetweenExpression(this);
			return AddBetweenExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 BetweenExpression 类实例，添加成功后返回 BetweenExpression 类实例。
		/// </summary>
		/// <param name="cma">字段描述</param>
		/// <returns>BetweenExpression 类实例</returns>
		public BetweenExpression AddBetweenExpression(ColumnMappingAttribute cma)
		{
			BetweenExpression ep = null;
			if (cma.Size > 0)
				ep = new BetweenExpression(this, cma.TableAlias, cma.ColumnName, cma.DataType, cma.Size, cma.Nullable);
			else
				ep = new BetweenExpression(this, cma.TableAlias, cma.ColumnName, cma.DataType, cma.Precision, cma.Scale, cma.Nullable);
			return AddBetweenExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 BetweenExpression 类实例，添加成功后返回 BetweenExpression 类实例。
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="precision">表示 Value 属性的最大位数。</param>
		/// <param name="scale">数据库字段的小数位数</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		/// <returns>BetweenExpression 类实例</returns>
		public BetweenExpression AddBetweenExpression(string tableAlias, string name, DbTypeEnum dataType, byte precision, byte scale, bool nullable)
		{
			BetweenExpression ep = new BetweenExpression(this, tableAlias, name, dataType, precision, scale, nullable);
			return AddBetweenExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加 BetweenExpression 类实例，添加成功后返回 BetweenExpression 类实例。
		/// </summary>
		/// <param name="tableAlias">当前字段所属表名称或别名</param>
		/// <param name="name">数据库字段名称</param>
		/// <param name="dataType">数据库字段类型</param>
		/// <param name="size">数据的最大大小，以字节为单位。</param>
		/// <param name="nullable">获取或设置一个值，该值指示参数是否接受空值。</param>
		/// <returns>BetweenExpression 类实例</returns>
		public BetweenExpression AddBetweenExpression(string tableAlias, string name, DbTypeEnum dataType, int size, bool nullable)
		{
			BetweenExpression ep = new BetweenExpression(this, tableAlias, name, dataType, size, nullable);
			return AddBetweenExpression(ep);
		}

		/// <summary>
		/// 向表达式集合中添加ExpressionParemeter 类实例，添加成功后返回ExpressionParemeter 类实例。
		/// </summary>
		/// <param name="ep">需要添加的 ParemeterExpression 类实例。</param>
		/// <returns>ExpressionParemeter 类实例</returns>
		private BetweenExpression AddBetweenExpression(BetweenExpression ep)
		{
			ep.ParameterName = this.GetParameterName(ep.ColumnName);
			ep.ToParameterName = this.GetParameterName(ep.ColumnName);
			base.Add(ep);
			return ep;
		}

		/// <summary>
		/// 向表达式集合中添加ExpressionCollection 类实例，添加成功后返回 ExpressionCollection 类实例。
		/// </summary>
		/// <returns>ExpressionCollection 类实例</returns>
		public BinaryConditionExpression AddBinaryExpression()
		{
			BinaryConditionExpression collection = new BinaryConditionExpression(this);
			this.Add(collection);
			return collection;
		}
	}

}
