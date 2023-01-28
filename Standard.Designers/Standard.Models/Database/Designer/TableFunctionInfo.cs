using Basic.Designer;
using Basic.Collections;
using System.Text;

namespace Basic.Database
{
	/// <summary>
	/// 表示表值函数
	/// </summary>
	public class TableFunctionInfo : TableDesignerInfo
	{
		private readonly FunctionParameterCollection _Parameters;
		/// <summary>
		/// 初始化 TableFunctionInfo 类实例。
		/// </summary>
		internal TableFunctionInfo(TableDesignerCollection tables) : this(tables, null) { }

		/// <summary>
		/// 初始化 TableDesignerInfo 类实例。
		/// </summary>
		/// <param name="tables">需要通知 AbstractCustomTypeDescriptor 类实例当前类的属性已更改。</param>
		/// <param name="aliasName">aliasName</param>
		internal TableFunctionInfo(TableDesignerCollection tables, string aliasName)
			: base(tables, aliasName, false)
		{
			_Parameters = new FunctionParameterCollection(this);
		}

		/// <summary>
		/// 创建新的参数。
		/// </summary>
		public FunctionParameterInfo CreateParameter()
		{
			return new FunctionParameterInfo(this);
		}

		/// <summary>
		/// 数据库列集合
		/// </summary>
		public FunctionParameterCollection Parameters { get { return _Parameters; } }

		/// <summary>
		/// 
		/// </summary>
		public override string FromName
		{
			get
			{
				if (_Parameters.Count > 0)
				{
					StringBuilder fromBuilder = new StringBuilder(Name, 200);
					fromBuilder.Append("("); int fromLength = fromBuilder.Length;
					foreach (FunctionParameterInfo parameter in _Parameters)
					{
						if (fromLength == fromBuilder.Length)
							fromBuilder.Append("{%").Append(parameter.Name).Append("%}");
						else
							fromBuilder.Append(",{%").Append(parameter.Name).Append("%}");
					}
					return fromBuilder.Append(") ").Append(Alias).ToString();
				}
				return base.FromName;
			}
		}
	}
}
