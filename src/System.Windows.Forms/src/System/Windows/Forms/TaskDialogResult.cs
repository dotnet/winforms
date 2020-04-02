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
        ///  No button was selected.
        /// </summary>
        None = 0,

        /// <summary>
        ///  The <c>OK</c> button was selected.
        /// </summary>
        OK = User32.ID.OK,

        /// <summary>
        ///  The <c>Cancel</c> button was selected.
        /// </summary>
        Cancel = User32.ID.CANCEL,

        /// <summary>
        ///  The <c>Abort</c> button was selected.
        /// </summary>
        Abort = User32.ID.ABORT,

        /// <summary>
        ///  The <c>Retry</c> button was selected.
        /// </summary>
        Retry = User32.ID.RETRY,

        /// <summary>
        ///  The <c>Ignore</c> button was selected.
        /// </summary>
        Ignore = User32.ID.IGNORE,

        /// <summary>
        ///  The <c>Yes</c> button was selected.
        /// </summary>
        Yes = User32.ID.YES,

        /// <summary>
        ///  The <c>No</c> button was selected.
        /// </summary>
        No = User32.ID.NO,

        /// <summary>
        ///  The <c>Close</c> button was selected.
        /// </summary>
        Close = User32.ID.CLOSE,

        /// <summary>
        ///  The <c>Help</c> button was selected.
        /// </summary>
        Help = User32.ID.HELP,

        /// <summary>
        ///  The <c>Try Again</c> button was selected.
        /// </summary>
        TryAgain = User32.ID.TRYAGAIN,

        /// <summary>
        ///  The <c>Continue</c> button was selected.
        /// </summary>
        Continue = User32.ID.CONTINUE
    }
}
