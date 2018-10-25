// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\AccessibleSelection.uex' path='docs/doc[@for="AccessibleSelection"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how an accessible object will be selected or receive focus.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum AccessibleSelection {

        /// <include file='doc\AccessibleSelection.uex' path='docs/doc[@for="AccessibleSelection.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The selection or focus of an object is unchanged.
        ///    </para>
        /// </devdoc>
        None = 0,

        /// <include file='doc\AccessibleSelection.uex' path='docs/doc[@for="AccessibleSelection.TakeFocus"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Assigns focus to an object and makes
        ///       it the anchor, which is the starting point for
        ///       the selection. Can be combined with <see langword='TakeSelection'/>,
        ///    <see langword='ExtendSelection'/>, <see langword='AddSelection'/>, or 
        ///    <see langword='RemoveSelection'/>.
        ///    </para>
        /// </devdoc>
        TakeFocus = 1,

        /// <include file='doc\AccessibleSelection.uex' path='docs/doc[@for="AccessibleSelection.TakeSelection"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Selects the object and deselects all other objects in the container.
        ///    </para>
        /// </devdoc>
        TakeSelection = 2,

        /// <include file='doc\AccessibleSelection.uex' path='docs/doc[@for="AccessibleSelection.ExtendSelection"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Selects all objects between the anchor and the selected object.
        ///    </para>
        /// </devdoc>
        ExtendSelection = 4,

        /// <include file='doc\AccessibleSelection.uex' path='docs/doc[@for="AccessibleSelection.AddSelection"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Adds the object to the selection.
        ///    </para>
        /// </devdoc>
        AddSelection = 8,
        
        /// <include file='doc\AccessibleSelection.uex' path='docs/doc[@for="AccessibleSelection.RemoveSelection"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Removes the object from the selection.
        ///    </para>
        /// </devdoc>
        RemoveSelection = 16,
    }
}
