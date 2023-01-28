using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic.Builders
{
	/// <summary>
	/// 表示下拉框中文件信息
	/// </summary>
	public class DropDownFile : AbstractPropertyChanged
	{
		/// <summary>
		/// 初始化 DropDownFile 类实例。
		/// </summary>
		/// <param name="name">表示数据持久类名称</param>
		/// <param name="path">表示数据持久类路径含文件名</param>
		internal DropDownFile(string name, string path) { _Name = name; _Path = path; }

		private readonly string _Name = null;

		/// <summary>
		/// 表示数据持久类文件名称。
		/// </summary>
		public string Name
		{
			get { return _Name; }
		}

		private readonly string _Path = null;

		/// <summary>
		/// 表示数据持久类文件路径。
		/// </summary>
		public string Path
		{
			get { return _Path; }
		}
	}
}
