﻿using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SelectComment : Selector
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4133;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectComment"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SelectComment(AsyncPackage package, OleMenuCommandService commandService) : base(package, StartComment(), EndComment())
        {
            AddCommand(commandService, CommandSet, CommandId);
        }

        public static string EndComment() => "*/";
        public static string StartComment() => "/*";

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SelectComment Instance
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
            Instance = new SelectComment(package, commandService);
        }

    }
}
