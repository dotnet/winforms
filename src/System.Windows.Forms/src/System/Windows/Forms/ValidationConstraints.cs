// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;

    /// <include file='doc\ValidationConstraints.uex' path='docs/doc[@for="ValidationConstraints"]/*' />
    /// <devdoc>
    ///     Determines which child controls in a ContainerControl will be validated
    ///     by the <see cref='ContainerControl.ValidateChildren'/> method.
    /// </devdoc>
    [Flags]
    public enum ValidationConstraints {
        /// <include file='doc\ValidationConstraints.uex' path='docs/doc[@for="ValidationConstraints.All"]/*' />
        /// <devdoc>
        ///     All child controls and their descendants are validated.
        /// </devdoc>
        None = 0x00,

        /// <include file='doc\ValidationConstraints.uex' path='docs/doc[@for="ValidationConstraints.Selectable"]/*' />
        /// <devdoc>
        ///     Child control must be selectable to be validated.
        ///
        ///     Note: This flag allows validation of a control that has the ControlStyles.Selectable style, ie. has
        ///     the ability to be selected, even when that control is currently unselectable because it has been
        ///     disabled or hidden. To prevent validation of selectable controls that are currently disabled or hidden,
        ///     include the <see cref='ValidationConstraints.Enabled'/> or <see cref='ValidationConstraints.Visible'/> modes.
        /// </devdoc>
        Selectable = 0x01,

        /// <include file='doc\ValidationConstraints.uex' path='docs/doc[@for="ValidationConstraints.Enabled"]/*' />
        /// <devdoc>
        ///     Child control must be enabled to be validated (Control.Enabled = true).
        /// </devdoc>
        Enabled = 0x02,

        /// <include file='doc\ValidationConstraints.uex' path='docs/doc[@for="ValidationConstraints.Visible"]/*' />
        /// <devdoc>
        ///     Child control must be visible to be validated (Control.Visible = true).
        /// </devdoc>
        Visible = 0x04,

        /// <include file='doc\ValidationConstraints.uex' path='docs/doc[@for="ValidationConstraints.TabStop"]/*' />
        /// <devdoc>
        ///     Child control must be a tab stops to be validated (Control.TabStop = true).
        /// </devdoc>
        TabStop = 0x08,

        /// <include file='doc\ValidationConstraints.uex' path='docs/doc[@for="ValidationConstraints.ImmediateChildren"]/*' />
        /// <devdoc>
        ///     Only immediate children of container control are validated. Descendants are not validated.
        /// </devdoc>
        ImmediateChildren = 0x10,
    }
}
