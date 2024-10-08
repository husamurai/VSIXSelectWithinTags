﻿using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;

namespace SelectLTHashHashGT
{
    internal class SelectorReverse : Selector
    {
        public SelectorReverse(SelectBlocksPackage package) : base(package)
        {
        }
        public SelectorReverse(SelectBlocksPackage package, string startTag, string endTag) : base(package, startTag, endTag)
        {
        }

        override protected SnapshotSpan? FindTagContent(ITextSnapshot snapshot, SnapshotPoint startPoint)
        {
            var text = snapshot.GetText();
            int finishIndex = startPoint.Position + EndTag.Length;
            if (finishIndex >= text.Length)
                finishIndex = text.Length - 1;
            finishIndex = text.LastIndexOf(EndTag, finishIndex, StringComparison.OrdinalIgnoreCase);
            if (finishIndex == -1)
            {
                ShowMsg("No Tag is present from position {finishIndex}!");
                return null;
            }

            Stack<int> tagStack = new Stack<int>();
            tagStack.Push(finishIndex);

            int current_start = finishIndex - 1;

            while (tagStack.Count > 0 && current_start > 0)
            {
                int previous_end = text.LastIndexOf(EndTag, current_start, StringComparison.OrdinalIgnoreCase);
                int previous_start = text.LastIndexOf(StartTag, current_start, StringComparison.OrdinalIgnoreCase);
                if (previous_start == -1)
                    break; // No matching start tag found
                if (previous_end > -1 && (previous_start < previous_end))
                {
                    tagStack.Push(previous_end);
                    Progress(previous_end);
                }
                else
                {
                    tagStack.Pop();
                    Progress(previous_start);
                }
            }

            if (tagStack.Count == 0)
            {
                return new SnapshotSpan(snapshot, new Span(current_start, (finishIndex + EndTag.Length) - current_start));
            }
            ShowMsg("No matching end tag found!");

            return null; // No matching end tag found
            void Progress(int s)
            {
                if (s > 0)
                    current_start = (tagStack.Count > 0) ? --s : s;
                else
                    current_start = 0;
            }
        }
    }
}
