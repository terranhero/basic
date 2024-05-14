using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Basic.Options
{
	/// <summary>表示允许集成的抽象条件类</summary>
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class AbstractClassesOptions : DialogPage
	{
		/// <summary>
		/// 
		/// </summary>
		public List<string> BaseConditions { get; set; } = new List<string>(10);

		/// <summary>
		/// 
		/// </summary>
		public List<string> BaseEntities { get; set; } = new List<string>(10);


		/// <summary>
		/// 
		/// </summary>
		public List<string> BaseAccess { get; set; } = new List<string>(10);

		protected override IWin32Window Window
		{
			get
			{
				AbstractClassesControl page = new AbstractClassesControl(this);
				page.Dock = DockStyle.Fill;
				page.Initialize();
				return page;
			}
		}
	}
}
