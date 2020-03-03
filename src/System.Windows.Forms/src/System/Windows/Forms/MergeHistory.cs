// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;

namespace System.Windows.Forms
{
    internal class MergeHistory
    {
        private Stack<MergeHistoryItem> mergeHistoryItemsStack;
        private readonly ToolStrip mergedToolStrip;

        public MergeHistory(ToolStrip mergedToolStrip)
        {
            this.mergedToolStrip = mergedToolStrip;
        }
        public Stack<MergeHistoryItem> MergeHistoryItemsStack
        {
            get
            {
                if (mergeHistoryItemsStack == null)
                {
                    mergeHistoryItemsStack = new Stack<MergeHistoryItem>();
                }
                return mergeHistoryItemsStack;
            }
        }
        public ToolStrip MergedToolStrip
        {
            get
            {
                return mergedToolStrip;
            }
        }
    }
}
