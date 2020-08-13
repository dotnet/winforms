// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class GridEntryComparer : IComparer<GridEntry>
    {
        public static GridEntryComparer Default { get; } = new GridEntryComparer();

        public int Compare(GridEntry? x, GridEntry? y)
            => StringComparer.CurrentCulture.Compare(x?.ToString(), y?.ToString());
    }
}
