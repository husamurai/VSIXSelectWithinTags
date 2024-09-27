﻿using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SelectLTHashHashGT : Selector
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129; // 9001; instead of 0x0100 does not work;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectLTHashHashGT"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SelectLTHashHashGT(AsyncPackage package, OleMenuCommandService commandService) : base(package, StartHasher(), EndHasher())
        {
            AddCommand(commandService, CommandSet, CommandId);
        }
        public static string EndHasher() => "#>";
        public static string StartHasher() => "<#";

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SelectLTHashHashGT Instance
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
            Instance = new SelectLTHashHashGT(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            DoTheJob();
        }
    }
}
