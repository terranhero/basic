using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Basic.DataAccess;
using Basic.EntityLayer;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Basic.Options
{
	/// <summary>表示允许集成的抽象条件类</summary>
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class AbstractClassesOptions : DialogPage
	{
		public AbstractClassesOptions() { }

		/// <summary>条件模型基类</summary>
		public BindingList<string> BaseConditions { get; set; } = new BindingList<string>();

		/// <summary>实体模型基类</summary>
		public BindingList<string> BaseEntities { get; set; } = new BindingList<string>();

		/// <summary>实体模型数据持久基类</summary>
		public BindingList<string> BaseAccess { get; set; } = new BindingList<string>();

		public override void LoadSettingsFromStorage()
		{
			base.LoadSettingsFromStorage();
			if (BaseConditions.Count == 0) { BaseConditions.Add(typeof(AbstractCondition).FullName); }
			if (BaseEntities.Count == 0) { BaseEntities.Add(typeof(AbstractEntity).FullName); }
			if (BaseAccess.Count == 0)
			{
				BaseAccess.Add(typeof(AbstractAccess).FullName);
				BaseAccess.Add(typeof(AbstractDbAccess).FullName);
			}
		}

		public override void LoadSettingsFromXml(IVsSettingsReader reader)
		{
			base.LoadSettingsFromXml(reader);
		}

		public override void SaveSettingsToXml(IVsSettingsWriter writer)
		{
			base.SaveSettingsToXml(writer);
		}

		public override void SaveSettingsToStorage()
		{
			base.SaveSettingsToStorage();
		}
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
