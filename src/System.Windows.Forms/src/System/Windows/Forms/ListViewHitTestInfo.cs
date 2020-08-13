// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the return value for HITTEST on ListView.
    /// </summary>
    public class ListViewHitTestInfo
    {
        private readonly ListViewHitTestLocations loc;
        private readonly ListViewItem item;
        private readonly ListViewItem.ListViewSubItem subItem;

        /// <summary>
        ///  Creates a ListViewHitTestInfo instance.
        /// </summary>
        public ListViewHitTestInfo(ListViewItem hitItem, ListViewItem.ListViewSubItem hitSubItem, ListViewHitTestLocations hitLocation)
        {
            item = hitItem;
            subItem = hitSubItem;
            loc = hitLocation;
        }

        /// <summary>
        ///  This gives the exact location returned by hit test on listview.
        /// </summary>
        public ListViewHitTestLocations Location
        {
            get
            {
                return loc;
            }
        }

        /// <summary>
        ///  This gives the ListViewItem returned by hit test on listview.
        /// </summary>
        public ListViewItem Item
        {
            get
            {
                return item;
            }
        }

        /// <summary>
        ///  This gives the ListViewSubItem returned by hit test on listview.
        /// </summary>
        public ListViewItem.ListViewSubItem SubItem
        {
            get
            {
                return subItem;
            }
        }
    }
}
