// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        // We need a special way to defer to the ListView's image
        // list for indexing purposes.
        internal class ListViewItemImageIndexer : ImageList.Indexer
        {
            private readonly ListViewItem _owner;

            public ListViewItemImageIndexer(ListViewItem owner)
            {
                _owner = owner;
            }

            public override ImageList ImageList
                => _owner?.ImageList;
        }
    }
}
