// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;

namespace System.Windows.Forms
{
    internal class MergeHistory
    {
        private Stack<MergeHistoryItem> _mergeHistoryItemsStack;

        public MergeHistory(ToolStrip mergedToolStrip)
        {
            MergedToolStrip = mergedToolStrip;
        }

        public Stack<MergeHistoryItem> MergeHistoryItemsStack
            => _mergeHistoryItemsStack ??= new Stack<MergeHistoryItem>();

        public ToolStrip MergedToolStrip { get; }
    }
}
