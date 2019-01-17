// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;

    /// <include file='doc\UpDownEvent.uex' path='docs/doc[@for="UpDownEventArgs"]/*' />
    /// <internalonly/>
    /// <devdoc>
    ///    <para>
    ///       Provides data for the UpDownEvent
    ///    </para>
    /// </devdoc>
    public class UpDownEventArgs : EventArgs {

        int buttonID;

        /// <include file='doc\UpDownEvent.uex' path='docs/doc[@for="UpDownEventArgs.UpDownEventArgs"]/*' />
        public UpDownEventArgs(int buttonPushed) {
            buttonID = buttonPushed;
        }

        /// <include file='doc\UpDownEvent.uex' path='docs/doc[@for="UpDownEventArgs.ButtonID"]/*' />
        public int ButtonID {
            get {
                return buttonID;
            }
        }
    }
}
