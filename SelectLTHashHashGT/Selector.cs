using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.RpcContracts.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace SelectLTHashHashGT
{
    internal class Selector : Titler
    {
        private readonly AsyncPackage package;
        public Selector(AsyncPackage package) : base()
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }
        public Selector(AsyncPackage package, string startTag, string endTag) : base(startTag, endTag)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }
        protected MenuCommand selectAllCommand = null;
        protected void GetSelectAll(OleMenuCommandService commandService)
        {
            // Get the status of the "Select All" command
            var selectAllCommandID = new CommandID(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.SelectAll);
            if (selectAllCommandID != null)
                selectAllCommand = commandService.FindCommand(selectAllCommandID);
        }
        protected static async Task<OleMenuCommandService> GetCommandServiceAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in SelectOption's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            return commandService;
        }
        private void SelectOptionBeforeQueryStatus(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;
            if (command != null)
            {
                if (selectAllCommand == null)
                {
                    OleMenuCommandService commandService = GetCommandServiceAsync((AsyncPackage)ServiceProvider).Result;
                    GetSelectAll(commandService);
                }
                // Enable or disable the "Select Option" command based on the "Select All" command's status
                command.Enabled = selectAllCommand != null && selectAllCommand.Enabled;
            }
        }

        public AsyncPackage Package => package;
        protected IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.Package;
            }
        }
        private CommandID menuCommandID = null;
        public MenuCommand MenuItem { get => menuItem; set => menuItem = value; }
        public CommandID MenuCommandID { get => menuCommandID; set => menuCommandID = value; }

        protected void AddCommand(OleMenuCommandService commandService, Guid CommandSet, int CommandId)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            MenuCommandID = new CommandID(CommandSet, CommandId);
            MenuItem = new MenuCommand(this.Execute, MenuCommandID);
            commandService.AddCommand(MenuItem);
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

        private MenuCommand menuItem = null;
        protected IWpfTextView GetTextView()
        {
            IWpfTextView textView = null;
            IVsTextManager vIVsTextManager = ServiceProvider.GetServiceAsync(typeof(SVsTextManager)).Result as IVsTextManager;
            if (vIVsTextManager != null)
            {
                vIVsTextManager.GetActiveView(1, null, out IVsTextView vTextView);
                var userData = vTextView as IVsUserData;
                if (userData != null)
                {
                    var guidViewHost = DefGuidList.guidIWpfTextViewHost;
                    userData.GetData(ref guidViewHost, out var holder);
                    var viewHost = (IWpfTextViewHost)holder;
                    textView = viewHost.TextView;
                }
            }
            return textView;
        }
        virtual protected SnapshotSpan? FindTagContent(ITextSnapshot snapshot, SnapshotPoint startPoint)
        {
            var text = snapshot.GetText();
            int startIndex = startPoint.Position - StartTag.Length;
            if (startIndex < 0)
                startIndex = 0;
            startIndex = text.IndexOf(StartTag, startIndex, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1)
            {
                ShowMsg("No Tag is present from position {startPoint.Position}!");
                return null;
            }

            Stack<int> tagStack = new Stack<int>();
            tagStack.Push(startIndex);

            int current_start = startIndex + StartTag.Length;

            while (tagStack.Count > 0 && current_start < text.Length)
            {
                int next_start = text.IndexOf(StartTag, current_start, StringComparison.OrdinalIgnoreCase);
                int next_end = text.IndexOf(EndTag, current_start, StringComparison.OrdinalIgnoreCase);
                if (next_end == -1)
                    break; // No matching end tag found
                if (next_start > -1 && (next_start < next_end))
                {
                    tagStack.Push(next_start);
                    Progress(next_start + StartTag.Length);
                }
                else
                {
                    tagStack.Pop();
                    Progress(next_end + EndTag.Length);
                }
            }

            if (tagStack.Count == 0)
            {
                return new SnapshotSpan(snapshot, new Span(startIndex, current_start - startIndex));
            }
            ShowMsg("No matching end tag found!");

            return null; // No matching end tag found

            void Progress(int s)
            {
                if (s < text.Length)
                    current_start = s;
                else
                    current_start = text.Length;
            }
        }

        private static string alpabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        protected string GetStartOfWord(IWpfTextView textView)
        {
            string sw = string.Empty;
            var bp = textView.Caret.Position.BufferPosition;
            var line = bp.GetContainingLine();
            var lineText = line.GetText();
            int x = bp.Position - line.Start.Position;
            string betas = alpabet.ToLower();
            while (x > 0 && (alpabet.IndexOf(lineText[x - 1]) > -1) || betas.IndexOf(lineText[x - 1]) > -1)
                x--;
            int y = x;
            while (y >= 0 && y < lineText.Length && (alpabet.IndexOf(lineText[y]) > -1) || betas.IndexOf(lineText[y]) > -1)
                y++;
            //0123456789
            //01Z3456789
            if (y > x)
                sw = lineText.Substring(x, y - x);

            if (x > 0)
                x--;
            SnapshotPoint ss = new SnapshotPoint(bp.Snapshot, line.Start + x);
            textView.Caret.MoveTo(ss);

            return sw;
        }
        protected void SetSelectedTags(IWpfTextView textView)
        {
            StartTag = string.Empty; EndTag = string.Empty;
            string sp = GetStartOfWord(textView);
            if (string.IsNullOrEmpty(sp))
            {
                StartTag = "<#";
                EndTag = "#>";
            }
            else
            {
                StartTag = string.Concat('<', sp, ' ');
                EndTag = string.Concat('/', sp, '>');
            }
        }

        public virtual void DoTheJob()
        {
            IWpfTextView textView = GetTextView();
            if (textView != null)
            {
                if (this is SelectedTag)
                    SetSelectedTags(textView);
                if (Valid)
                {
                    var textSnapshot = textView.TextSnapshot;
                    var caretPosition = textView.Caret.Position.BufferPosition;
                    var newSelection = FindTagContent(textSnapshot, caretPosition);
                    if (newSelection != null)
                    {
                        textView.Selection.Select(newSelection.Value, false);
                        textView.Caret.MoveTo(newSelection.Value.End);
                    }
                }
            }
        }
        protected int ShowMsg(string msg)
        {
            int answer = VsShellUtilities.ShowMessageBox(
                Package,
                msg,
                Title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            return answer;
        }
    }
}
