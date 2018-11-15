// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;

    /// <include file='doc\BindingCompleteContext.uex' path='docs/doc[@for="BindingCompleteContext"]/*' />
    /// <devdoc>
    ///     Indicates the direction of a binding operation.
    /// </devdoc>
    public enum BindingCompleteContext {

        /// <include file='doc\BindingCompleteContext.uex' path='docs/doc[@for="BindingCompleteContext.ControlUpdate"]/*' />
        /// <devdoc>
        ///     Control value is being updated from data source value.
        /// </devdoc>
        ControlUpdate = 0,

        /// <include file='doc\BindingCompleteContext.uex' path='docs/doc[@for="BindingCompleteContext.DataSourceUpdate"]/*' />
        /// <devdoc>
        ///     Data source value is being updated from control value.
        /// </devdoc>
        DataSourceUpdate = 1,
    }
}