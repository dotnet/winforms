// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System;
    using System.Windows.Forms;

    /// <include file='doc\RTLAwareMessageBox.uex' path='docs/doc[@for="RTLAwareMessageBox"]/*' />
    /// <devdoc>
    ///    <para>
    ///       The Show method displays a message box that can contain text, buttons, and symbols that
    ///       inform and instruct the user. This MessageBox will be RTL, if the resources
    ///       for this dll have been localized to a RTL language.
    ///    </para>
    /// </devdoc>
    internal sealed class RTLAwareMessageBox {

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///       Makes the dialog RTL if the resources for this dll have been localized to a RTL language.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options) {
            if (RTLAwareMessageBox.IsRTLResources) {
                options |= (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
            return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options);
        }

        /// <devdoc>
        ///     Tells whether the current resources for this dll have been
        ///     localized for a RTL language.
        /// </devdoc>
        public static bool IsRTLResources {
            get {
                return SR.RTL != "RTL_False";
            }
        }
    }
}


