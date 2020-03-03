// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Globalization;

namespace System.Windows.Forms
{
    internal class MergeHistoryItem
    {
        private readonly MergeAction mergeAction;
        private ToolStripItem targetItem;
        private int index = -1;
        private int previousIndex = -1;
        private ToolStripItemCollection previousIndexCollection;
        private ToolStripItemCollection indexCollection;

        public MergeHistoryItem(MergeAction mergeAction)
        {
            this.mergeAction = mergeAction;
        }
        public MergeAction MergeAction
        {
            get
            {
                return mergeAction;
            }
        }

        public ToolStripItem TargetItem
        {
            get
            {
                return targetItem;
            }
            set
            {
                targetItem = value;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        public int PreviousIndex
        {
            get
            {
                return previousIndex;
            }
            set
            {
                previousIndex = value;
            }
        }

        public ToolStripItemCollection PreviousIndexCollection
        {
            get
            {
                return previousIndexCollection;
            }
            set
            {
                previousIndexCollection = value;
            }
        }

        public ToolStripItemCollection IndexCollection
        {
            get
            {
                return indexCollection;
            }
            set
            {
                indexCollection = value;
            }
        }

        public override string ToString()
        {
            return "MergeAction: " + mergeAction.ToString() + " | TargetItem: " + (TargetItem == null ? "null" : TargetItem.Text) + " Index: " + index.ToString(CultureInfo.CurrentCulture);
        }
    }
}
