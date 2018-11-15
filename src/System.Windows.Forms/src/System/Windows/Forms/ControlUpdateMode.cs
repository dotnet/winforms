// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;

    /// <include file='doc\ControlUpdateMode.uex' path='docs/doc[@for="ControlUpdateMode"]/*' />
    /// <devdoc>
    ///     Determines when changes to a data source property get propagated up to the corresponding data-bound control property.
    /// </devdoc>
    public enum ControlUpdateMode {

        /// <include file='doc\ControlUpdateMode.uex' path='docs/doc[@for="ControlUpdateMode.Always"]/*' />
        /// <devdoc>
        ///     Control property is updated whenever the data source property changes, or the data source position changes.
        ///     This is the default update mode.
        /// </devdoc>
        OnPropertyChanged = 0,

        /// <include file='doc\ControlUpdateMode.uex' path='docs/doc[@for="ControlUpdateMode.Never"]/*' />
        /// <devdoc>
        ///     Control property is never updated. Binding is "write-only" with respect to the data source.
        ///     To force the control property to be updated, use the Binding.ReadValue method.
        /// </devdoc>
        Never = 1,
    }
}
