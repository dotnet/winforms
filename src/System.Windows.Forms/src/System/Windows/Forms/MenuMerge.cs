// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the behavior of a <see cref='System.Windows.Forms.MenuItem'/> when it is merged with items in another menu.
    /// </devdoc>
    public enum MenuMerge
    {
        /// <devdoc>
        /// The <see cref='System.Windows.Forms.MenuItem'/> is added to the
        /// existing <see cref='System.Windows.Forms.MenuItem'/> objects in a
        /// merged menu.
        /// </devdoc>
        Add = 0,

        /// <devdoc>
        /// The <see cref='System.Windows.Forms.MenuItem'/> replaces the
        /// existing <see cref='System.Windows.Forms.MenuItem'/> at the same
        /// position in a merged menu.
        /// </devdoc>
        Replace = 1,

        /// <devdoc>
        /// Subitems of this <see cref='System.Windows.Forms.MenuItem'/> are merged
        /// with those of existing <see cref='System.Windows.Forms.MenuItem'/>
        /// objects at the same position in a merged menu.
        /// </devdoc>
        MergeItems = 2,

        /// <devdoc>
        /// The <see cref='System.Windows.Forms.MenuItem'/> is not included in a
        /// merged menu.
        /// </devdoc>
        Remove = 3,
    }
}
