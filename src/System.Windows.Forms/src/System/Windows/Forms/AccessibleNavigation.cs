// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///       Specifies values for navigating between accessible objects.
    ///    </para>
    /// </devdoc>
    public enum AccessibleNavigation {

        /// <devdoc>
        ///    <para>
        ///       Navigation
        ///       to a sibling object located
        ///       below the
        ///       starting object.
        ///    </para>
        /// </devdoc>
        Down = 0x2,

        /// <devdoc>
        ///    <para>
        ///       Navigation to
        ///       the
        ///       first child of the object.
        ///    </para>
        /// </devdoc>
        FirstChild = 0x7,

        /// <devdoc>
        ///    <para>
        ///       Navigation to
        ///       the
        ///       last child of the object
        ///    </para>
        /// </devdoc>
        LastChild = 0x8,

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
        
        /// <devdoc>
        ///    <para>
        ///       Navigation to the sibling object
        ///       located to the right of the
        ///       starting object.
        ///    </para>
        /// </devdoc>
        Right = 0x4,
        
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
