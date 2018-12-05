// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;

    /// <internalonly/>
    /// <devdoc>
    ///    <para>
    ///       Provides data for the UpDownEvent
    ///    </para>
    /// </devdoc>
    public class UpDownEventArgs : EventArgs {

        int buttonID;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public UpDownEventArgs(int buttonPushed) {
            buttonID = buttonPushed;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int ButtonID {
            get {
                return buttonID;
            }
        }
    }
}
