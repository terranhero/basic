using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using STT = System.Threading.Tasks;
namespace Basic.Localizations
{
	/// <summary>本地化资源管理器</summary>
	[ProvideAutoLoad(UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[ProvideLoadKey("Standard", Consts.ProductVersion, "ResourcePackage", "SIP GoldSoft Technology Limited", 1)]
	[InstalledProductRegistrationAttribute("#100", "#111", Consts.ProductVersion, IconResourceID = 510)]
	[ProvideEditorExtension(typeof(LocalizationFactory), ".localresx", 50, NameResourceID = 112, TemplateDir = "Templates",
		 ProjectGuid = "{A2FE74E1-B743-11d0-AE1A-00A0C90FFFC3}", DefaultName = "ASP.NET MVC Localization Resource")]
	[ProvideKeyBindingTable(Consts.guidFactoryString, 112)]
	[ProvideEditorLogicalView(typeof(LocalizationFactory), Consts.guidLogicalViewString)]
	[System.Runtime.InteropServices.Guid(Consts.guidPackageString)]
	[ProvideMenuResource("ResourcesMenus.ctmenu", 1)]
	public sealed class LocalizationPackage : AsyncPackage
	{
		private readonly LocalizationService _Commands;
		private readonly LocalizationFactory _ResourceFactory;
		/// <summary>
		/// 初始化 ResourcePackage 类实例。
		/// </summary>
		public LocalizationPackage()
		{
			_Commands = new LocalizationService(this);
			_ResourceFactory = new LocalizationFactory(this, _Commands);
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async STT.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((sender, args) =>
				{
					Trace.WriteLine(string.Format("缺少程序集：{0}", args.Name));
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						if (assembly.FullName == args.Name)
							return assembly;
					}
					return null;
				});
			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			await _Commands.InitializeAsync(cancellationToken, progress);
			base.RegisterEditorFactory(_ResourceFactory);
		}


		///// <summary>
		///// Initialization of the package; this method is called right after the package is sited, so this is the place
		///// where you can put all the initilaization code that rely on services provided by VisualStudio.
		///// </summary>
		//protected override void Initialize()
		//{
		//	AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
		//	{
		//		Trace.WriteLine(string.Format("缺少程序集：{0}", args.Name));
		//		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		//		{
		//			if (assembly.FullName == args.Name)
		//				return assembly;
		//		}
		//		return null;
		//	};
		//	Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
		//	base.Initialize();
		//	_Commands.Initialize();
		//	base.RegisterEditorFactory(_ResourceFactory);
		//}
	}
}
