// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.ComponentModel;

    using System.Diagnostics;

    using System;

    /// <include file='doc\IContainerControl.uex' path='docs/doc[@for="IContainerControl"]/*' />
    /// <devdoc>
    ///    <para> Provides functionality for a control 
    ///       to parent other controls.</para>
    /// </devdoc>
    public interface IContainerControl {
        /// <include file='doc\IContainerControl.uex' path='docs/doc[@for="IContainerControl.ActiveControl"]/*' />
        /// <devdoc>
        ///    <para>Indicates the control that is currently active on the container control.</para>
        /// </devdoc>
        Control ActiveControl { get; set; }
        /// <include file='doc\IContainerControl.uex' path='docs/doc[@for="IContainerControl.ActivateControl"]/*' />
        /// <devdoc>
        ///    <para>Activates the specified control.</para>
        /// </devdoc>
        bool ActivateControl(Control active);
    }
}
