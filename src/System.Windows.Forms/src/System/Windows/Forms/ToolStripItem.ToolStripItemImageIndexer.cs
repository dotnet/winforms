// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class ToolStripItem
    {
        internal class ToolStripItemImageIndexer : ImageList.Indexer
        {
            private readonly ToolStripItem _item;

            public ToolStripItemImageIndexer(ToolStripItem item)
            {
                _item = item;
            }

            public override ImageList ImageList
            {
                get => _item?.Owner?.ImageList;
                set => Debug.Assert(false, "We should never set the image list");
            }
        }
    }
}
