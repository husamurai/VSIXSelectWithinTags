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
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        private static MenuCommand selectOleMenuCommand = null;
        public static MenuCommand SelectOleMenuCommand { get => selectOleMenuCommand; set => selectOleMenuCommand = value; }

        private static IVsTextManager vIVsTextManager = null;
        protected static IVsTextManager VIVsTextManager { get => vIVsTextManager; set => vIVsTextManager = value; }
        protected static async Task<OleMenuCommandService> GetCommandServiceAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in SelectOption's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (VIVsTextManager == null)
                VIVsTextManager = await package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
            if (commandService != null && SelectOleMenuCommand == null)
            {
                //Guid selectGroupGuid = new Guid("{5EFC7975-14BC-11CF-9B2B-00AA00573819}"); // IDG_VS_EDIT_SELECT
                //for (int i = 0; i < int.MaxValue; i++)
                {
                    // in C:\Program Files\Microsoft Visual Studio\2022\Community\VSSDK\VisualStudioIntegration\Common\Inc\vsshlids.h #define IDG_VS_EDIT_SELECT            0x012B in 
                    // in C:\Program Files\Microsoft Visual Studio\2022\Community\VSSDK\VisualStudioIntegration\Common\Inc\stdidcmd.h #define cmdidSelectAll          31
                    //CommandID menuCommandID = new CommandID(VsMenus.guidSHLMainMenu, 0x012B);// 
                    //CommandID menuCommandID = new CommandID(VsMenus.guidStandardCommandSet2K, 31);// 
                    //CommandID menuCommandID = new CommandID(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.SelectAll);
                    CommandID menuCommandID = new CommandID(VsMenus.guidStandardCommandSet97, 15);
                    if (menuCommandID != null)
                    {
                        SelectOleMenuCommand = commandService.FindCommand(menuCommandID);
                        //if (SelectOleMenuCommand != null)
                        //    break;
                    }
                }
            }
            return commandService;
        }

        public AsyncPackage Package => package;
        protected IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.Package;
            }
        }


        /// <summary>
        /// If you need to dynamically enable or disable a command based on certain conditions, you should use OleMenuCommand. For simpler scenarios where the command’s status does not change, MenuCommand might be sufficient.
        //Here’s a quick comparison:
        //MenuCommand: Basic command handling, no dynamic status.
        //OleMenuCommand: Advanced command handling, supports dynamic status via BeforeQueryStatus.
        //Would you like an example of how to use OleMenuCommand in your extension?
        /// </summary>
        /// <param name="commandService"></param>
        /// <param name="CommandSet"></param>
        /// <param name="CommandId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected void AddCommand(OleMenuCommandService commandService, Guid CommandSet, int CommandId)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuCommand = new OleMenuCommand(this.Execute, menuCommandID);
            menuCommand.BeforeQueryStatus += DoBeforeQueryStatus;
            menuCommand.Enabled = false;
            commandService.AddCommand(menuCommand);
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

        protected IWpfTextView GetTextView()
        {
            IWpfTextView textView = null;
            if (VIVsTextManager != null)// && sm is IVsTextManager vIVsTextManager)
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
        private void DoBeforeQueryStatus(object sender, EventArgs e)
        {
            var command = sender as OleMenuCommand;
            if (command != null)
            {
                if (SelectOleMenuCommand == null)
                {
                    // works fine
                    IWpfTextView textView = GetTextView();
                    command.Enabled = textView != null;
                }
                else
                {
                    command.Enabled = SelectOleMenuCommand.Enabled;
                }
            }
        }
    }
}
