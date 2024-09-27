using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SelectOption : Selector
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4127;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectOption"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SelectOption(AsyncPackage package, OleMenuCommandService commandService) : base(package)
        {
            AddCommand(commandService, CommandSet, CommandId);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SelectOption Instance
        {
            get;
            private set;
        }


        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            OleMenuCommandService commandService = await GetCommandServiceAsync(package) as OleMenuCommandService;
            Instance = new SelectOption(package, commandService);
        }

        public override void DoTheJob() 
        { 
        } // do nothing
    }
}
