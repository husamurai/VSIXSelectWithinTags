using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(SelectBlocksPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public class SelectBlocksPackage : AsyncPackage // , IVsRunningDocTableEvents
    {
        /// <summary>
        /// SelectBlocksPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "d0a577cf-be8c-44cf-9d0d-1223d56243d2";
        private MenuCommand referenceMenuCommand = null;
        public MenuCommand ReferenceMenuCommand { get => referenceMenuCommand; set => referenceMenuCommand = value; }
        private IVsTextManager vIVsTextManager = null;
        public IVsTextManager VIVsTextManager { get => vIVsTextManager; set => vIVsTextManager = value; }
        private OleMenuCommandService commandService = null;
        public OleMenuCommandService CommandService { get => commandService; set => commandService = value; }
        internal SelectOption VSelectOption { get => vSelectOption; set => vSelectOption = value; }
        internal SelectLTHashHashGT VSelectLTHashHashGT { get => vSelectLTHashHashGT; set => vSelectLTHashHashGT = value; }
        internal SelectParanthesisedBlock VSelectParanthesisedBlock { get => vSelectParanthesisedBlock; set => vSelectParanthesisedBlock = value; }
        internal SelectedTag VSelectedTag { get => vSelectedTag; set => vSelectedTag = value; }
        internal SelectRegion VSelectRegion { get => vSelectRegion; set => vSelectRegion = value; }
        internal SelectComment VSelectComment { get => vSelectComment; set => vSelectComment = value; }
        internal ReverseSelectLTHashHashGT VReverseSelectLTHashHashGT { get => vReverseSelectLTHashHashGT; set => vReverseSelectLTHashHashGT = value; }
        internal ReverseSelectComment VReverseSelectComment { get => vReverseSelectComment; set => vReverseSelectComment = value; }
        internal ReverseSelectedTag VReverseSelectedTag { get => vReverseSelectedTag; set => vReverseSelectedTag = value; }
        internal ReverseSelectParanthesisedBlock VReverseSelectParanthesisedBlock { get => vReverseSelectParanthesisedBlock; set => vReverseSelectParanthesisedBlock = value; }
        internal ReverseSelectRegion VReverseSelectRegion { get => vReverseSelectRegion; set => vReverseSelectRegion = value; }
        internal BlockSelection VBlockSelection { get => vBlockSelection; set => vBlockSelection = value; }

        protected async Task<OleMenuCommandService> GetServicesAsync()
        {
            // Switch to the main thread - the call to AddCommand in SelectOption's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
            if (vIVsTextManager == null)
                vIVsTextManager = await GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
            if (commandService == null)
                commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            return CommandService;
        }
        private SelectOption vSelectOption = null;
        private SelectLTHashHashGT vSelectLTHashHashGT = null;
        private SelectParanthesisedBlock vSelectParanthesisedBlock = null;
        private SelectedTag vSelectedTag = null;
        private SelectRegion vSelectRegion = null;
        private SelectComment vSelectComment = null;
        private ReverseSelectLTHashHashGT vReverseSelectLTHashHashGT = null;
        private ReverseSelectComment vReverseSelectComment = null;
        private ReverseSelectedTag vReverseSelectedTag = null;
        private ReverseSelectParanthesisedBlock vReverseSelectParanthesisedBlock = null;
        private ReverseSelectRegion vReverseSelectRegion = null;
        private BlockSelection vBlockSelection = null;
        #region Package Members

        protected override async Task OnAfterPackageLoadedAsync(CancellationToken cancellationToken)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
        #endregion

        #region
        //int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        //{
        //    return VSConstants.S_OK;
        //}
        //int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        //{
        //    return VSConstants.S_OK;
        //}
        //int IVsRunningDocTableEvents.OnAfterSave(uint docCookie)
        //{
        //    return VSConstants.S_OK;
        //}
        //int IVsRunningDocTableEvents.OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        //{
        //    return VSConstants.S_OK;
        //}
        //int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        //{
        //    return VSConstants.S_OK;
        //}
        //int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        //{
        //    return VSConstants.S_OK;
        //}

        //private uint rdtCookie;
        #endregion
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await GetServicesAsync();
            vBlockSelection = new BlockSelection(this);
            vSelectOption = new SelectOption(this);
            vSelectLTHashHashGT = new SelectLTHashHashGT(this);
            vSelectParanthesisedBlock = new SelectParanthesisedBlock(this);
            vSelectedTag = new SelectedTag(this);
            vSelectRegion = new SelectRegion(this);
            vSelectComment = new SelectComment(this);
            vReverseSelectLTHashHashGT = new ReverseSelectLTHashHashGT(this);
            vReverseSelectComment = new ReverseSelectComment(this);
            vReverseSelectedTag = new ReverseSelectedTag(this);
            vReverseSelectParanthesisedBlock = new ReverseSelectParanthesisedBlock(this);
            vReverseSelectRegion = new ReverseSelectRegion(this);
            await Task.CompletedTask;
        }
    }
}
