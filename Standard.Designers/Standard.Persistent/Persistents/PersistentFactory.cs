﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Basic.Configuration
{
	/// <summary>
	/// Factory for creating our editor object. Extends from the IVsEditoryFactory interface
	/// </summary>
	[Guid(ConfirugationConsts.guidFactoryString)]
    public sealed class PersistentFactory : IVsEditorFactory, IVsRefactorNotify, IDisposable
    {
        private readonly PersistentPackage editorPackage;
        private readonly PersistentService _Commands;
        private ServiceProvider vsServiceProvider;


        public PersistentFactory(PersistentPackage package, PersistentService commands)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering {0} constructor", this.ToString()));
            this._Commands = commands;
            this.editorPackage = package;
        }

        /// <summary>
        /// Since we create a ServiceProvider which implements IDisposable we
        /// also need to implement IDisposable to make sure that the ServiceProvider's
        /// Dispose method gets called.
        /// </summary>
        public void Dispose()
        {
            if (vsServiceProvider != null)
            {
                vsServiceProvider.Dispose();
            }
        }

        #region IVsEditorFactory Members

        /// <summary>
        /// Used for initialization of the editor in the environment
        /// </summary>
        /// <param name="psp">pointer to the service provider. Can be used to obtain instances of other interfaces
        /// </param>
        /// <returns></returns>
        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            vsServiceProvider = new ServiceProvider(psp);
            return VSConstants.S_OK;
        }

        public object GetService(Type serviceType)
        {
            return vsServiceProvider.GetService(serviceType);
        }

        // This method is called by the Environment (inside IVsUIShellOpenDocument::
        // OpenStandardEditor and OpenSpecificEditor) to map a LOGICAL view to a 
        // PHYSICAL view. A LOGICAL view identifies the purpose of the view that is
        // desired (e.g. a view appropriate for Debugging [LOGVIEWID_Debugging], or a 
        // view appropriate for text view manipulation as by navigating to a find
        // result [LOGVIEWID_TextView]). A PHYSICAL view identifies an actual type 
        // of view implementation that an IVsEditorFactory can create. 
        //
        // NOTE: Physical views are identified by a string of your choice with the 
        // one constraint that the default/primary physical view for an editor  
        // *MUST* use a NULL string as its physical view name (*pbstrPhysicalView = NULL).
        //
        // NOTE: It is essential that the implementation of MapLogicalView properly
        // validates that the LogicalView desired is actually supported by the editor.
        // If an unsupported LogicalView is requested then E_NOTIMPL must be returned.
        //
        // NOTE: The special Logical Views supported by an Editor Factory must also 
        // be registered in the local registry hive. LOGVIEWID_Primary is implicitly 
        // supported by all editor types and does not need to be registered.
        // For example, an editor that supports a ViewCode/ViewDesigner scenario
        // might register something like the following:
        //        HKLM\Software\Microsoft\VisualStudio\<version>\Editors\
        //            {...guidEditor...}\
        //                LogicalViews\
        //                    {...LOGVIEWID_TextView...} = s ''
        //                    {...LOGVIEWID_Code...} = s ''
        //                    {...LOGVIEWID_Debugging...} = s ''
        //                    {...LOGVIEWID_Designer...} = s 'Form'
        //
        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null;    // initialize out parameter

            // we support only a single physical view
            if (VSConstants.LOGVIEWID_Primary == rguidLogicalView)
                return VSConstants.S_OK;        // primary view uses NULL as pbstrPhysicalView
            else
                return VSConstants.E_NOTIMPL;   // you must return E_NOTIMPL for any unrecognized rguidLogicalView values
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Used by the editor factory to create an editor instance. the environment first determines the 
        /// editor factory with the highest priority for opening the file and then calls 
        /// IVsEditorFactory.CreateEditorInstance. If the environment is unable to instantiate the document data 
        /// in that editor, it will find the editor with the next highest priority and attempt to so that same 
        /// thing. 
        /// NOTE: The priority of our editor is 32 as mentioned in the attributes on the package class.
        /// 
        /// Since our editor supports opening only a single view for an instance of the document data, if we 
        /// are requested to open document data that is already instantiated in another editor, or even our 
        /// editor, we return a value VS_E_INCOMPATIBLEDOCDATA.
        /// </summary>
        /// <param name="grfCreateDoc">Flags determining when to create the editor. Only open and silent flags 
        /// are valid
        /// </param>
        /// <param name="pszMkDocument">path to the file to be opened</param>
        /// <param name="pszPhysicalView">name of the physical view</param>
        /// <param name="pvHier">pointer to the IVsHierarchy interface</param>
        /// <param name="itemid">Item identifier of this editor instance</param>
        /// <param name="punkDocDataExisting">This parameter is used to determine if a document buffer 
        /// (DocData object) has already been created
        /// </param>
        /// <param name="ppunkDocView">Pointer to the IUnknown interface for the DocView object</param>
        /// <param name="ppunkDocData">Pointer to the IUnknown interface for the DocData object</param>
        /// <param name="pbstrEditorCaption">Caption mentioned by the editor for the doc window</param>
        /// <param name="pguidCmdUI">the Command UI Guid. Any UI element that is visible in the editor has 
        /// to use this GUID. This is specified in the .vsct file
        /// </param>
        /// <param name="pgrfCDW">Flags for CreateDocumentWindow</param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public int CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, string pszPhysicalView,
            IVsHierarchy pvHier, uint itemid, System.IntPtr punkDocDataExisting,
            out System.IntPtr ppunkDocView, out System.IntPtr ppunkDocData, out string pbstrEditorCaption,
            out Guid pguidCmdUI, out int pgrfCDW)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering {0} CreateEditorInstace()", this.ToString()));

            // Initialize to null
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pguidCmdUI = ConfirugationConsts.guidFactory;
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
            if (pvHier.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_ExtObject, out object outProject) >= 0)
            {
                EnvDTE.ProjectItem item = outProject as EnvDTE.ProjectItem;
                PersistentPane NewEditor = new PersistentPane(editorPackage, _Commands, pvHier, item, pszMkDocument);
                ppunkDocView = Marshal.GetIUnknownForObject(NewEditor);
                ppunkDocData = Marshal.GetIUnknownForObject(NewEditor);
            }
            pbstrEditorCaption = "";
            return VSConstants.S_OK;
        }
        #endregion

		#region 接口 IVsRefactorNotify 的实现
		int IVsRefactorNotify.OnAddParams(IVsHierarchy pHier, uint itemid, string lpszRQName,
    uint cParams, uint[] rgszParamIndexes, string[] rgszRQTypeNames, string[] rgszParamNames)
        {
            return VSConstants.S_OK;
        }

        int IVsRefactorNotify.OnBeforeAddParams(IVsHierarchy pHier, uint itemid, string lpszRQName, uint cParams,
            uint[] rgszParamIndexes, string[] rgszRQTypeNames, string[] rgszParamNames, out Array prgAdditionalCheckoutVSITEMIDs)
        {
            prgAdditionalCheckoutVSITEMIDs = null;
            return VSConstants.S_OK;
        }

        int IVsRefactorNotify.OnBeforeGlobalSymbolRenamed(IVsHierarchy pHier, uint itemid, uint cRQNames,
            string[] rglpszRQName, string lpszNewName, out Array prgAdditionalCheckoutVSITEMIDs)
        {
            prgAdditionalCheckoutVSITEMIDs = null;
            return VSConstants.S_OK;
        }

        int IVsRefactorNotify.OnBeforeRemoveParams(IVsHierarchy pHier, uint itemid, string lpszRQName,
            uint cParamIndexes, uint[] rgParamIndexes, out Array prgAdditionalCheckoutVSITEMIDs)
        {
            prgAdditionalCheckoutVSITEMIDs = null;
            return VSConstants.S_OK;
        }

        int IVsRefactorNotify.OnBeforeReorderParams(IVsHierarchy pHier, uint itemid, string lpszRQName,
            uint cParamIndexes, uint[] rgParamIndexes, out Array prgAdditionalCheckoutVSITEMIDs)
        {
            prgAdditionalCheckoutVSITEMIDs = null;
            return VSConstants.S_OK;
        }

        int IVsRefactorNotify.OnGlobalSymbolRenamed(IVsHierarchy pHier, uint itemid, uint cRQNames, string[] rglpszRQName, string lpszNewName)
        {
            return VSConstants.S_OK;
        }

        int IVsRefactorNotify.OnRemoveParams(IVsHierarchy pHier, uint itemid, string lpszRQName, uint cParamIndexes, uint[] rgParamIndexes)
        {
            return VSConstants.S_OK;
        }

        int IVsRefactorNotify.OnReorderParams(IVsHierarchy pHier, uint itemid, string lpszRQName, uint cParamIndexes, uint[] rgParamIndexes)
        {
            return VSConstants.S_OK;
		}
		#endregion
	}
}
