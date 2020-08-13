// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  We need a special way to defer to the ListView's image list for indexing purposes.
    ///  ListViewGroupImageIndexer is a class used to support <see cref="ListViewGroup.TitleImageIndex"/> and
    ///  <see cref="ListViewGroup.TitleImageKey"/>.
    /// </summary>
    internal class ListViewGroupImageIndexer : ImageList.Indexer
    {
        private readonly ListViewGroup _owner;

        /// <summary>
        ///  Initializes a new instance of the <see cref="ListViewGroupImageIndexer"/> class.
        /// </summary>
        /// <param name="group">The <see cref="ListViewGroup"/> that this object belongs to.</param>
        public ListViewGroupImageIndexer(ListViewGroup group)
        {
            _owner = group;
        }

        /// <summary>
        ///  Gets the <see cref="ListView.GroupImageList"/> of the <see cref="ListView"/>
        ///  associated with the group.
        /// </summary>
        public override ImageList? ImageList
        {
            get => _owner.ListView?.GroupImageList;
            set => Debug.Fail("We should never set the image list");
        }
    }
}
