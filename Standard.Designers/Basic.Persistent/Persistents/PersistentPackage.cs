using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Basic.Options;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using STT = System.Threading.Tasks;

namespace Basic.Configuration
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the 
	/// IVsPackage interface and uses the registration attributes defined in the framework to 
	/// register itself and its components with the shell.
	/// </summary>
	[ProvideService(typeof(IClassesOptions), IsCacheable = true)]
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[ProvideLoadKey("Standard", "4.0.0.0", "PersistentPackage", "SIP GoldSoft Technology Limited", 1)]
	[InstalledProductRegistration("#210", "#221", "4.0.0.0", IconResourceID = 510)]
	[ProvideEditorExtension(typeof(PersistentFactory), ".dpdl", 50, ProjectGuid = "{A2FE74E1-B743-11d0-AE1A-00A0C90FFFC3}",
		TemplateDir = "Templates", NameResourceID = 210, DefaultName = "ASP.NET MVC Data Persistent")]
	[ProvideKeyBindingTable(ConfirugationConsts.guidFactoryString, 210)]
	[ProvideEditorLogicalView(typeof(PersistentFactory), ConfirugationConsts.guidEditorLvString)]
	[ProvideMenuResource("PersistentMenus.ctmenu", 1)]
	[ProvideAutoLoad(UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
	[System.Runtime.InteropServices.Guid(ConfirugationConsts.guidPackageString)]
	[ProvideOptionPage(typeof(AClassesOptions), "Persistent Designer", "General", 307, 308, true)]
	[ProvideProfileAttribute(typeof(AClassesOptions), "Persistent Designer", "General", 307, 308, true)]
	public sealed class PersistentPackage : AsyncPackage, IVsSolutionEvents2//, IVsInstalledProduct
	{
		private readonly PersistentFactory factory;
		private readonly PersistentService _Commands;
		private uint solutionEventsCookie = 0;
		private readonly string assemblyFolder;
		/// <summary>
		/// 初始化 PersistentPackage 类实例。
		/// </summary>
		public PersistentPackage()
		{
			_Commands = new PersistentService(this);
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
			assemblyFolder = Path.GetDirectoryName(typeof(PersistentPackage).Assembly.Location);
			factory = new PersistentFactory(this, _Commands);
		}

		#region Package Members,Overriden Package Implementation
		protected override async STT.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			//string bcFilepath = string.Concat(assemblyFolder, "/Basic.Configuration.dll");
			//if (File.Exists(bcFilepath)) { AppDomain.CurrentDomain.Load(Assembly.LoadFile(bcFilepath).GetName()); }

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((sender, args) =>
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				Assembly ass = assemblies.FirstOrDefault(m => m.GetName().FullName == args.Name);
				if (ass != null) { return ass; }
				ass = assemblies.FirstOrDefault(m => m.GetName().Name == args.Name);
				if (ass != null) { return ass; }
				string fileName = args.Name.Substring(0, args.Name.IndexOf(','));
				string filePath = string.Concat(assemblyFolder, "/", fileName, ".dll");
				if (File.Exists(filePath)) { return Assembly.LoadFile(filePath); }
				return ass;
			});
			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			IVsSolution vsSolution = await _Commands.GetServiceAsync<SVsSolution, IVsSolution>();
			await _Commands.InitializeAsync(cancellationToken, progress);
			await _Commands.InitializeOptionsAsync(cancellationToken, progress);
			//await _Commands.InitializeTemplateAsync(progress);
			await _Commands.InitializeMenuAsync(cancellationToken, progress);

			//IClassesOptions opts = await _Commands.GetServiceAsync<AClassesOptions, IClassesOptions>();
			RegisterEditorFactory(factory);
			Assumes.Present(vsSolution);
			vsSolution.AdviseSolutionEvents(this, out solutionEventsCookie);
		}

		/// <summary>
		/// Called to ask the package if the shell can be closed.
		/// </summary>
		/// <param name="canClose">Returns true if the shell can be closed, otherwise false.</param>
		/// <returns>Microsoft.VisualStudio.VSConstants.S_OK if the method succeeded, otherwise an error code.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD104:提供异步方法", Justification = "<挂起>")]
		protected override int QueryClose(out bool canClose)
		{
			if (solutionEventsCookie != 0)
			{
				JoinableTaskFactory.Run(async () =>
				{
					await JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
					IVsSolution vsSolution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
					Assumes.Present(vsSolution);
					vsSolution.UnadviseSolutionEvents(solutionEventsCookie);
				});

			}
			return base.QueryClose(out canClose);
		}
		#endregion

		#region 接口 IVsSolutionEvents2 实现
		int IVsSolutionEvents2.OnAfterCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnAfterMergeSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnBeforeCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents2.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}
		#endregion
	}
}
