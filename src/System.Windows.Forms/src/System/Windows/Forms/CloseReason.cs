// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the reason for the Form Closing.
    /// </devdoc>
    public enum CloseReason
    {
        /// <devdoc>
        /// No reason for closure of the Form.
        /// </devdoc>
        None = 0,
        
        /// <devdoc>
        /// In the process of shutting down, Windows has closed the application.
        /// </devdoc>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Verb: Shut down, not noun Shutdown.")]
        WindowsShutDown = 1,

        /// <devdoc>
        /// The parent form of this MDI form is closing.
        /// </devdoc>
        MdiFormClosing = 2,

        /// <devdoc>
        /// The user has clicked the close button on the form window, selected
        /// Close from the window's control menu or hit Alt + F4.
        /// </devdoc>
        UserClosing = 3,

        /// <devdoc>
        /// The Microsoft Windows Task Manager is closing the application.
        /// </devdoc>
        TaskManagerClosing = 4,

        /// <devdoc>
        /// A form is closing because its owner is closing.
        /// </devdoc>
        FormOwnerClosing = 5,

        /// <devdoc>
        /// A form is closing because Application.Exit() was called.
        /// </devdoc>
        ApplicationExitCall = 6
    }
}
