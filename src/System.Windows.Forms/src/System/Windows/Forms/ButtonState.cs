// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\ButtonState.uex' path='docs/doc[@for="ButtonState"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the appearance of a button.
    ///       
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum ButtonState {

        /// <include file='doc\ButtonState.uex' path='docs/doc[@for="ButtonState.Checked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The button has a
        ///       checked or latched appearance. Use
        ///       this appearance to show that a toggle button has been pressed.
        ///       
        ///    </para>
        /// </devdoc>
        Checked = NativeMethods.DFCS_CHECKED,

        /// <include file='doc\ButtonState.uex' path='docs/doc[@for="ButtonState.Flat"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The button has a flat, two-dimensional appearance.
        ///       
        ///    </para>
        /// </devdoc>
        Flat = NativeMethods.DFCS_FLAT,

        /// <include file='doc\ButtonState.uex' path='docs/doc[@for="ButtonState.Inactive"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The button is inactive (grayed).
        ///       
        ///    </para>
        /// </devdoc>
        Inactive = NativeMethods.DFCS_INACTIVE,

        /// <include file='doc\ButtonState.uex' path='docs/doc[@for="ButtonState.Normal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       
        ///       The button has its normal appearance
        ///       (three-dimensional and not pressed).
        ///       
        ///    </para>
        /// </devdoc>
        Normal = 0,

        /// <include file='doc\ButtonState.uex' path='docs/doc[@for="ButtonState.Pushed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       
        ///       The button is currently pressed.
        ///       
        ///    </para>
        /// </devdoc>
        Pushed = NativeMethods.DFCS_PUSHED,

        /// <include file='doc\ButtonState.uex' path='docs/doc[@for="ButtonState.All"]/*' />
        /// <devdoc>
        ///    <para>
        ///       All viable
        ///       flags in the bit mask are used.
        ///       
        ///    </para>
        /// </devdoc>
        All = Flat | Checked | Pushed | Inactive,

    }
}
