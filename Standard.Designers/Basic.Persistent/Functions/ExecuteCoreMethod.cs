using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basic.Configuration;

namespace Basic.Functions
{/// <summary>
	/// 表示调用基类的 ExecuteNonQuery 方法。
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class ExecuteCoreMethod : AbstractExecutableMethod
	{
		internal const string XmlElementName = "ExecuteCore";
		private readonly StaticCommandElement staticCommand;
		internal ExecuteCoreMethod(StaticCommandElement element)
			: base(element)
		{
			staticCommand = element;
		}

		public override string MethodName
		{
			get { throw new NotImplementedException(); }
		}

		protected internal override void WriteCode(System.CodeDom.CodeMemberMethod codeMethod)
		{
			throw new NotImplementedException();
		}

		public override string GetComponentName()
		{
			throw new NotImplementedException();
		}

		protected internal override string ElementName
		{
			get { throw new NotImplementedException(); }
		}

		protected internal override bool ReadAttribute(string name, string value)
		{
			throw new NotImplementedException();
		}

		protected internal override void WriteAttribute(System.Xml.XmlWriter writer)
		{
			throw new NotImplementedException();
		}
	}
}
