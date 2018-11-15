// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Remoting;

    using System.Diagnostics;

    using System;
    using System.Windows.Forms;
    using Microsoft.Win32;

    /// <include file='doc\IWindowTarget.uex' path='docs/doc[@for="IWindowTarget"]/*' />
    /// <devdoc>
    ///      This interface defines the communication layer between
    ///      a Control object and the Win32 API.  Each Control object
    ///      has an internal implementation this interface that is called
    ///      by the Win32 window.
    /// </devdoc>
    /// <internalonly/>
    public interface IWindowTarget {

        /// <include file='doc\IWindowTarget.uex' path='docs/doc[@for="IWindowTarget.OnHandleChange"]/*' />
        /// <devdoc>
        ///      Called when the window handle of the control has changed.
        /// </devdoc>
        void OnHandleChange(IntPtr newHandle);

        /// <include file='doc\IWindowTarget.uex' path='docs/doc[@for="IWindowTarget.OnMessage"]/*' />
        /// <devdoc>
        ///      Called to do control-specific processing for this window.
        /// </devdoc>
        void OnMessage(ref Message m);
    }

}
