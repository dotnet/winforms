// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    
    
    /// <include file='doc\ListViewHitTestInfo.uex' path='docs/doc[@for="ListViewHitTestInfo"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the return value for HITTEST on ListView.
    ///    </para>
    /// </devdoc>
    public class ListViewHitTestInfo {

        private ListViewHitTestLocations loc;
        private ListViewItem item;
        private ListViewItem.ListViewSubItem subItem;

        /// <include file='doc\ListViewHitTestInfo.uex' path='docs/doc[@for="ListViewHitTestInfo.ListViewHitTestInfo"]/*' />
        /// <devdoc>
        ///     Creates a ListViewHitTestInfo instance.
        /// </devdoc>
        public ListViewHitTestInfo(ListViewItem hitItem, ListViewItem.ListViewSubItem hitSubItem, ListViewHitTestLocations hitLocation) {
            this.item = hitItem;
            this.subItem = hitSubItem;
            this.loc = hitLocation;
        }
        

        /// <include file='doc\ListViewHitTestInfo.uex' path='docs/doc[@for="ListViewHitTestInfo.Location"]/*' />
        /// <devdoc>
        ///     This gives the exact location returned by hit test on listview.
        /// </devdoc>
        public ListViewHitTestLocations Location {
            get {
                return loc;
            }
        }
        
        /// <include file='doc\ListViewHitTestInfo.uex' path='docs/doc[@for="ListViewHitTestInfo.Item"]/*' />
        /// <devdoc>
        ///     This gives the ListViewItem returned by hit test on listview.
        /// </devdoc>
        public ListViewItem Item {
            get {
                return item;
            }
        }
        
        /// <include file='doc\ListViewHitTestInfo.uex' path='docs/doc[@for="ListViewHitTestInfo.SubItem"]/*' />
        /// <devdoc>
        ///     This gives the ListViewSubItem returned by hit test on listview.
        /// </devdoc>
        public ListViewItem.ListViewSubItem SubItem {
            get {
                return subItem;
            }
        }
    }
}
