// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    internal class MergeHistoryItem
    {
        public MergeHistoryItem(MergeAction mergeAction)
        {
            MergeAction = mergeAction;
        }

        public MergeAction MergeAction { get; }

        public ToolStripItem TargetItem { get; set; }

        public int Index { get; set; } = -1;

        public int PreviousIndex { get; set; } = -1;

        public ToolStripItemCollection PreviousIndexCollection { get; set; }

        public ToolStripItemCollection IndexCollection { get; set; }

        public override string ToString()
            => $"MergeAction: {MergeAction} | TargetItem: {(TargetItem is null ? "null" : TargetItem.Text)} Index: {Index}";
    }
}
