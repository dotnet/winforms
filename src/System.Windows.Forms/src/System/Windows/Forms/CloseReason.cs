// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the reason for the Form Closing.
    /// </summary>
    public enum CloseReason
    {
        /// <summary>
        ///  No reason for closure of the Form.
        /// </summary>
        None = 0,

        /// <summary>
        ///  In the process of shutting down, Windows has closed the application.
        /// </summary>
        WindowsShutDown = 1,

        /// <summary>
        ///  The parent form of this MDI form is closing.
        /// </summary>
        MdiFormClosing = 2,

        /// <summary>
        ///  The user has clicked the close button on the form window, selected
        ///  Close from the window's control menu or hit Alt + F4.
        /// </summary>
        UserClosing = 3,

        /// <summary>
        ///  The Microsoft Windows Task Manager is closing the application.
        /// </summary>
        TaskManagerClosing = 4,

        /// <summary>
        ///  A form is closing because its owner is closing.
        /// </summary>
        FormOwnerClosing = 5,

        /// <summary>
        ///  A form is closing because Application.Exit() was called.
        /// </summary>
        ApplicationExitCall = 6
    }
}
