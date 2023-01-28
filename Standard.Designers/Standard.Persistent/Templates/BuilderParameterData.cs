using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Builders
{
	/// <summary>
	/// 标准控制器生成参数数据
	/// </summary>
	[System.SerializableAttribute()]
	public sealed class BuilderParameterData : MarshalByRefObject
	{
		private System.String _TypeName;
		private System.String _Name;
		public BuilderParameterData() : base() { }

		public BuilderParameterData(string name, System.String typeName)
			: base() { _TypeName = typeName; _Name = name; }

		public System.String Name { get { return _Name; } set { _Name = value; } }

		public System.String TypeName { get { return _TypeName; } set { _TypeName = value; } }
	}
}
