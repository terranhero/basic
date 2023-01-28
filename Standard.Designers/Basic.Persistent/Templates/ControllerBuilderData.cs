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
	public sealed class ControllerBuilderData : MarshalByRefObject
	{
		private readonly List<BuilderParameterData> _NewEntityKeys = new List<BuilderParameterData>(5);
		private readonly List<BuilderParameterData> _EditEntityKeys = new List<BuilderParameterData>(5);
		private readonly List<BuilderParameterData> _SearchEntityKeys = new List<BuilderParameterData>(5);
		public ControllerBuilderData()
			: base()
		{
			IndexEnabled = true;
			GridEnabled = false;
			CreateEnabled = true;
			EditEnabled = true;
			DeleteEnabled = true;
			SearchEnabled = true;
			ComplexSearchEnabled = true;
			SearchAsyncEnabled = true;
			ComplexSearchAsyncEnabled = true;
			Connection = false;
			BaseController = "AsyncController";
		}

		/// <summary>
		/// 当前项的默认命名空间
		/// </summary>
		public string DefaultNamespance { get; set; }

		/// <summary>
		/// 当前项的默认命名空间
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// 当前项的默认命名空间
		/// </summary>
		public string EntityNamespace { get; set; }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string ControllerDescription { get; set; }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string NewEntityName { get; set; }

		public List<BuilderParameterData> NewEntityKeys { get { return _NewEntityKeys; } }

		public string EditEntityName { get; set; }

		public List<BuilderParameterData> EditEntityKeys { get { return _EditEntityKeys; } }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string SearchEntityName { get; set; }

		public List<BuilderParameterData> SearchEntityKeys { get { return _SearchEntityKeys; } }

		public string DeleteEntityName { get; set; }

		public string SearchConditionName { get; set; }

		/// <summary>是否指定数据库连接</summary>
		public bool Connection { get; set; }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string ControllerClass { get; set; }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string BaseController { get; set; }

		/// <summary>
		/// 表示控制器名称。
		/// </summary>
		public string ContextName { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool IndexEnabled { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool GridEnabled { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool CreateEnabled { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool EditEnabled { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool DeleteEnabled { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool SearchAsyncEnabled { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool ComplexSearchAsyncEnabled { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool SearchEnabled { get; set; }

		/// <summary>
		/// 表示控制器名称文本框是否显示。
		/// </summary>
		public bool ComplexSearchEnabled { get; set; }

	}
}
