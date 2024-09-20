using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;

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

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.Package;
            }
        }

        public AsyncPackage Package => package;

        private IWpfTextView GetTextView()
        {
            IWpfTextView textView = null;
            var textManager = (IVsTextManager)ServiceProvider.GetServiceAsync(typeof(SVsTextManager)).Result;
            textManager.GetActiveView(1, null, out IVsTextView vTextView);
            var userData = vTextView as IVsUserData;
            if (userData != null)
            {
                var guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out var holder);
                var viewHost = (IWpfTextViewHost)holder;
                textView = viewHost.TextView;
            }
            return textView;
        }
        private SnapshotSpan? FindTagContent(ITextSnapshot snapshot, SnapshotPoint startPoint)
        {
            var text = snapshot.GetText();
            int startIndex = text.IndexOf(StartTag, startPoint.Position, StringComparison.OrdinalIgnoreCase);
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
                if (next_start > -1)
                {
                    if (next_start < next_end)
                    {
                        tagStack.Push(next_start);
                        current_start = next_start + StartTag.Length;
                    }
                    else
                    {
                        tagStack.Pop();
                        current_start = next_end + EndTag.Length;
                    }
                }
                else
                {
                    tagStack.Pop();
                    current_start = next_end + EndTag.Length;
                }
            }

            if (tagStack.Count == 0)
            {
                return new SnapshotSpan(snapshot, new Span(startIndex, current_start - startIndex));
            }
            ShowMsg("No matching end tag found!");

            return null; // No matching end tag found
        }

        private static string alpabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private string GetStartOfWord(IWpfTextView textView)
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
        private void SetSelectedTags(IWpfTextView textView)
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

        public void DoTheJob()
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
        private int ShowMsg(string msg)
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
