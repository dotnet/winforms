// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public class SearchForVirtualItemEventArgs : EventArgs
    {
        public SearchForVirtualItemEventArgs(bool isTextSearch, bool isPrefixSearch, bool includeSubItemsInSearch, string text, Point startingPoint, SearchDirectionHint direction, int startIndex)
        {
            IsTextSearch = isTextSearch;
            IsPrefixSearch = isPrefixSearch;
            IncludeSubItemsInSearch = includeSubItemsInSearch;
            Text = text;
            StartingPoint = startingPoint;
            Direction = direction;
            StartIndex = startIndex;
        }

        public bool IsTextSearch { get; }

        public bool IsPrefixSearch { get; }

        public bool IncludeSubItemsInSearch { get; }

        public int Index { get; set; } = -1;

        public string Text { get; }

        public Point StartingPoint { get; }

        public SearchDirectionHint Direction { get; }

        public int StartIndex { get; }
    }
}
