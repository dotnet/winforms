// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies values for navigating between accessible objects.
    ///    </para>
    /// </devdoc>
    public enum AccessibleNavigation {

        /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation.Down"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Navigation
        ///       to a sibling object located
        ///       below the
        ///       starting object.
        ///    </para>
        /// </devdoc>
        Down = 0x2,

        /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation.FirstChild"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Navigation to
        ///       the
        ///       first child of the object.
        ///    </para>
        /// </devdoc>
        FirstChild = 0x7,

        /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation.LastChild"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Navigation to
        ///       the
        ///       last child of the object
        ///    </para>
        /// </devdoc>
        LastChild = 0x8,

        /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation.Left"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Navigation
        ///       to the sibling object located
        ///       to the left
        ///       of the
        ///       starting object.
        ///    </para>
        /// </devdoc>
        Left = 0x3,

        /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation.Next"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Navigation
        ///       to the next logical object, generally from a
        ///       sibling
        ///       object to the
        ///       starting object.
        ///    </para>
        /// </devdoc>
        Next = 0x5,
        
        /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation.Previous"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Navigation to the previous logical object, generally
        ///       from
        ///       a sibling
        ///       object to the
        ///       starting object.
        ///    </para>
        /// </devdoc>
        Previous = 0x6,
        
        /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation.Right"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Navigation to the sibling object
        ///       located to the right of the
        ///       starting object.
        ///    </para>
        /// </devdoc>
        Right = 0x4,
        
        /// <include file='doc\AccessibleNavigation.uex' path='docs/doc[@for="AccessibleNavigation.Up"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Navigation to a sibling object
        ///       located above the
        ///       starting object.
        ///    </para>
        /// </devdoc>
        Up = 0x1,
    }
}
