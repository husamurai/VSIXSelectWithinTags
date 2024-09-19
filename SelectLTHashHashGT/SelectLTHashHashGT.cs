using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using Task = System.Threading.Tasks.Task;

namespace SelectLTHashHashGT
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SelectLTHashHashGT
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100; // 9001; instead of 0x0100 does not work;
        private const string Title = "Select <# ... #>";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("765c9d5b-9b9b-4e24-8310-11270bb6041e");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectLTHashHashGT"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private SelectLTHashHashGT(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SelectLTHashHashGT Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in SelectLTHashHashGT's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
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
            string message = string.Format(CultureInfo.CurrentCulture, "Do you want to select content of <# ... #> ?", this.GetType().FullName);
            string title = Title;

            //// Show a message box to prove we were here
            //var answer = ShowMsg(message);
            //ShowMsg($"The answer is {answer}");
            //if (answer == 1)
            DoTheJob();
        }
        private IWpfTextView GetTextView()
        {
            //ShowMsg("GetTextView!");
            var textManager = (IVsTextManager)ServiceProvider.GetServiceAsync(typeof(SVsTextManager)).Result;
            textManager.GetActiveView(1, null, out IVsTextView vTextView);
            var userData = vTextView as IVsUserData;
            if (userData == null) return null;

            var guidViewHost = DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out var holder);
            var viewHost = (IWpfTextViewHost)holder;
            return viewHost.TextView;
        }
        private SnapshotSpan? FindTagContent(ITextSnapshot snapshot, SnapshotPoint startPoint, string startTag, string endTag)
        {
            //ShowMsg("FindTagContent!");
            var text = snapshot.GetText();
            int startIndex = text.IndexOf(startTag, startPoint.Position);
            if (startIndex == -1)
            {
                ShowMsg("No Tag is present!");
                return null;
            }

            Stack<int> tagStack = new Stack<int>();
            tagStack.Push(startIndex);

            int currentIndex = startIndex + startTag.Length;

            while (tagStack.Count > 0 && currentIndex < text.Length)
            {
                int nextStartIndex = text.IndexOf(startTag, currentIndex);
                int nextEndIndex = text.IndexOf(endTag, currentIndex);

                if (nextEndIndex == -1)
                {
                    ShowMsg("No matching end Tag is present!");
                    return null; // No matching end tag found
                }

                if (nextStartIndex != -1 && nextStartIndex < nextEndIndex)
                {
                    tagStack.Push(nextStartIndex);
                    currentIndex = nextStartIndex + startTag.Length;
                }
                else
                {
                    tagStack.Pop();
                    currentIndex = nextEndIndex + endTag.Length;
                }
            }

            if (tagStack.Count == 0)
            {
                return new SnapshotSpan(snapshot, new Span(startIndex, currentIndex - startIndex));
            }

            return null; // No matching end tag found
        }
        private void DoTheJob()
        {
            //ShowMsg("DoTheJob!");
            var textView = GetTextView();
            if (textView == null)
            {
                ShowMsg("Could not obtain Text View!");
                return;
            }

            var textSnapshot = textView.TextSnapshot;
            var caretPosition = textView.Caret.Position.BufferPosition;
            var selection = FindTagContent(textSnapshot, caretPosition, "<#", "#>");
            if (selection != null)
            {
                textView.Selection.Select(selection.Value, false);
                textView.Caret.MoveTo(selection.Value.End);
                //ShowMsg("Selected|");
            }
            //else
            //    ShowMsg("Not Selected|");
        }
        private int ShowMsg(string msg)
        {
            int answer = VsShellUtilities.ShowMessageBox(
                this.package,
                msg,
                Title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            return answer;
        }
    }
}
