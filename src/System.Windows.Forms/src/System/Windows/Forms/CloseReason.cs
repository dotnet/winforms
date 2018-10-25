// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    
    /// <include file='doc\CloseReason.uex' path='docs/doc[@for="CloseReason"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the reason for the Form Closing.
    ///    </para>
    /// </devdoc>
    public enum CloseReason {

        /// <include file='doc\CloseReason.uex' path='docs/doc[@for="CloseReason.FormDisposing"]/*' />
        /// <devdoc>
        ///    <para>
        ///      No reason for closure of the Form.
        ///    </para>
        /// </devdoc>
        None = 0,
        
        /// <include file='doc\CloseReason.uex' path='docs/doc[@for="CloseReason.WindowsShutDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       In the process of shutting down, Windows has closed the application.
        ///    </para>
        /// </devdoc>
        // Verb: Shut down, not noun Shutdown.
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")] 
        WindowsShutDown = 1,

        /// <include file='doc\CloseReason.uex' path='docs/doc[@for="CloseReason.MdiFormClosing"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The parent form of this MDI form is closing.
        ///    </para>
        /// </devdoc>
        MdiFormClosing = 2,

        /// <include file='doc\CloseReason.uex' path='docs/doc[@for="CloseReason.UserClosing"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The user has clicked the close button on the form window, selected Close from the window's control menu or
        ///       hit Alt + F4.
        ///    </para>
        /// </devdoc>
        UserClosing = 3,

        /// <include file='doc\CloseReason.uex' path='docs/doc[@for="CloseReason.TaskManagerClosing"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The Microsoft Windows Task Manager is closing the application.
        ///    </para>
        /// </devdoc>
        TaskManagerClosing = 4,

        /// <include file='doc\CloseReason.uex' path='docs/doc[@for="CloseReason.FormOwnerClosing"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A form is closing because its owner is closing.
        ///    </para>
        /// </devdoc>
        FormOwnerClosing = 5,

        /// <include file='doc\CloseReason.uex' path='docs/doc[@for="CloseReason.ApplicationExitCall"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A form is closing because Application.Exit() was called.
        ///    </para>
        /// </devdoc>
        ApplicationExitCall = 6
    }
}


