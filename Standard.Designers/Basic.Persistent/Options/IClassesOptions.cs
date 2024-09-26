using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Options
{
	/// <summary></summary>
	public interface IClassesOptions
	{
		/// <summary>条件模型基类</summary>
		BindingList<string> BaseConditions { get; set; }

		/// <summary>实体模型基类</summary>
		BindingList<string> BaseEntities { get; set; }

		/// <summary>实体模型数据持久基类</summary>
		BindingList<string> BaseAccess { get; set; }
	}
}
