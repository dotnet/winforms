// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolStripItem
{
    internal class ToolStripItemImageIndexer : ImageList.Indexer
    {
        private readonly ToolStripItem _item;

        public ToolStripItemImageIndexer(ToolStripItem item)
        {
            Debug.Assert(item is not null, $"{nameof(item)} should not be null.");
            _item = item;
        }

        public override ImageList? ImageList
        {
            get => _item.Owner?.ImageList;
            set => Debug.Assert(false, "We should never set the image list");
        }
    }
}
