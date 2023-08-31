// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ColumnHeader
{
    internal class ColumnHeaderImageListIndexer : ImageList.Indexer
    {
        private readonly ColumnHeader _owner;

        public ColumnHeaderImageListIndexer(ColumnHeader ch)
        {
            _owner = ch;
        }

        public override ImageList? ImageList
        {
            get
            {
                return _owner.ListView?.SmallImageList;
            }
            set
            {
                Debug.Assert(false, "We should never set the image list");
            }
        }
    }
}
