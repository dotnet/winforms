// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies identifiers to indicate the return value of a task dialog.
    /// </summary>
    internal enum TaskDialogResult : int
    {
        /// <summary>
        ///  A custom button was selected.
        /// </summary>
        None = 0,

        /// <summary>
        ///  The <c>OK</c> button was selected.
        /// </summary>
        OK = User32.IDOK,

        /// <summary>
        ///  The <c>Cancel</c> button was selected.
        /// </summary>
        Cancel = User32.IDCANCEL,

        /// <summary>
        ///  The <c>Abort</c> button was selected.
        /// </summary>
        Abort = User32.IDABORT,

        /// <summary>
        ///  The <c>Retry</c> button was selected.
        /// </summary>
        Retry = User32.IDRETRY,

        /// <summary>
        ///  The <c>Ignore</c> button was selected.
        /// </summary>
        Ignore = User32.IDIGNORE,

        /// <summary>
        ///  The <c>Yes</c> button was selected.
        /// </summary>
        Yes = User32.IDYES,

        /// <summary>
        ///  The <c>No</c> button was selected.
        /// </summary>
        No = User32.IDNO,

        /// <summary>
        ///  The <c>Close</c> button was selected.
        /// </summary>
        Close = User32.IDCLOSE,

        /// <summary>
        ///  The <c>Help</c> button was selected.
        /// </summary>
        Help = User32.IDHELP,

        /// <summary>
        ///  The <c>Try Again</c> button was selected.
        /// </summary>
        TryAgain = User32.IDTRYAGAIN,

        /// <summary>
        ///  The <c>Continue</c> button was selected.
        /// </summary>
        Continue = User32.IDCONTINUE
    }
}
