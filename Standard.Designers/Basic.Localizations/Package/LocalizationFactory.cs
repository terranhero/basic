using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Basic.Localizations
{
	/// <summary>
	/// 
	/// </summary>
	[System.Runtime.InteropServices.Guid(GuidString)]
	public sealed class LocalizationFactory : IVsEditorFactory, IDisposable
	{
		/// <summary>
		/// 表示 Factory 类 Guid 常量值。
		/// </summary>
		public const string GuidString = "B76814ED-D201-46A4-99EA-3A31588FAD03";

		private readonly LocalizationPackage resourcePackage;
		private readonly LocalizationService _Service;
		private ServiceProvider vsServiceProvider;

		public LocalizationFactory(LocalizationPackage package, LocalizationService commands)
		{
			_Service = commands;
			this.resourcePackage = package;
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering {0} constructor", this.ToString()));
		}

		int IVsEditorFactory.Close()
		{
			return VSConstants.S_OK;
		}

		[SuppressMessage("Usage", "VSTHRD010:在主线程上调用单线程类型", Justification = "<挂起>")]
		int IVsEditorFactory.CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView,
			IVsHierarchy pvHier, uint itemid, IntPtr punkDocDataExisting, out IntPtr ppunkDocView,
			out IntPtr ppunkDocData, out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
		{
			Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering {0} CreateEditorInstace()", this.ToString()));

			// Initialize to null
			ppunkDocView = IntPtr.Zero;
			ppunkDocData = IntPtr.Zero;
			pguidCmdUI = Consts.guidFactory;
			pgrfCDW = 0;
			pbstrEditorCaption = null;

			// Validate inputs
			if ((grfCreateDoc & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
			{
				return VSConstants.E_INVALIDARG;
			}
			if (punkDocDataExisting != IntPtr.Zero)
			{
				return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
			}

			// Create the Document (editor)
			if (pvHier.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_ExtObject, out object item) >= 0)
			{
				LocalizationPane resourceEditor = new LocalizationPane(resourcePackage, _Service, item as EnvDTE.ProjectItem);
				ppunkDocView = Marshal.GetIUnknownForObject(resourceEditor);
				ppunkDocData = Marshal.GetIUnknownForObject(resourceEditor);
			}

			pbstrEditorCaption = "";
			return VSConstants.S_OK;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rguidLogicalView"></param>
		/// <param name="pbstrPhysicalView"></param>
		/// <returns></returns>
		int IVsEditorFactory.MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
		{
			pbstrPhysicalView = null;    // initialize out parameter
										 // we support only a single physical view
			if (VSConstants.LOGVIEWID_Primary == rguidLogicalView)
				return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
			else
				return VSConstants.E_NOTIMPL;   // you must return E_NOTIMPL for any unrecognized rguidLogicalView values
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="psp"></param>
		/// <returns></returns>
		int IVsEditorFactory.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
		{
			vsServiceProvider = new ServiceProvider(psp);
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Since we create a ServiceProvider which implements IDisposable we
		/// also need to implement IDisposable to make sure that the ServiceProvider's
		/// Dispose method gets called.
		/// </summary>
		public void Dispose()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (vsServiceProvider != null)
			{
				vsServiceProvider.Dispose();
			}
		}
	}
}
