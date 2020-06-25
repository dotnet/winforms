// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  We need a special way to defer to the ListView's image list for indexing purposes.
    ///  ListViewItemImageIndexer is a class used to support <see cref="ListViewItem.ImageIndex"/> and
    ///  <see cref="ListViewItem.ImageKey"/>.
    /// </summary>
    internal class ListViewItemImageIndexer : ImageList.Indexer
    {
        private readonly ListViewItem _owner;

        /// <summary>
        ///  Initializes a new instance of the <see cref="ListViewItemImageIndexer"/> class.
        /// </summary>
        /// <param name="item">The <see cref="ListViewItem"/> that this object belongs to.</param>
        public ListViewItemImageIndexer(ListViewItem item)
        {
            _owner = item;
        }

        /// <summary>
        ///  Gets the <see cref="ListViewItem.ImageList"/> associated with the item.
        /// </summary>
        public override ImageList? ImageList
        {
            get => _owner.ImageList;
            set => Debug.Fail("We should never set the image list");
        }
    }
}
