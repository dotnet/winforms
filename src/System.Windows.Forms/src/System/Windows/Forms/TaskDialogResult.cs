// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies identifiers to indicate the return value of a task dialog.
    /// </summary>
    public enum TaskDialogResult : int
    {
        /// <summary>
        ///
        /// </summary>
        None = 0,

        /// <summary>
        ///
        /// </summary>
        OK = User32.IDOK,

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Note: Adding a Cancel button will automatically add a close button
        ///   to the task dialog's title bar and will allow to close the dialog by
        ///   pressing ESC or Alt+F4 (just as if you enabled
        ///   <see cref="TaskDialogPage.AllowCancel"/>).
        /// </para>
        /// </remarks>
        Cancel = User32.IDCANCEL,

        /// <summary>
        ///
        /// </summary>
        Abort = User32.IDABORT,

        /// <summary>
        ///
        /// </summary>
        Retry = User32.IDRETRY,

        /// <summary>
        ///
        /// </summary>
        Ignore = User32.IDIGNORE,

        /// <summary>
        ///
        /// </summary>
        Yes = User32.IDYES,

        /// <summary>
        ///
        /// </summary>
        No = User32.IDNO,

        /// <summary>
        ///
        /// </summary>
        Close = User32.IDCLOSE,

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Note: Clicking this button will not close the dialog, but will raise the
        ///   <see cref="TaskDialogPage.HelpRequest"/> event.
        /// </para>
        /// </remarks>
        Help = User32.IDHELP,

        /// <summary>
        ///
        /// </summary>
        TryAgain = User32.IDTRYAGAIN,

        /// <summary>
        ///
        /// </summary>
        Continue = User32.IDCONTINUE
    }
}
