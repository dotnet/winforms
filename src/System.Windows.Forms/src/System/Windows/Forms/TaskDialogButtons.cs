// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommonButtonFlags = Interop.TaskDialog.TASKDIALOG_COMMON_BUTTON_FLAGS;

namespace System.Windows.Forms
{
    /// <summary>
    /// Contains constants representing standard buttons of a task dialog.
    /// </summary>
    /// <remarks>
    /// This enumeration has a <see cref="FlagsAttribute"/> attribute that allows a bitwise
    /// combination of its member values.
    /// </remarks>
    [Flags]
    public enum TaskDialogButtons : int
    {
        /// <summary>
        /// No standard button is added to the task dialog.
        /// </summary>
        None = 0,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.OK"/> result.
        /// </summary>
        OK = CommonButtonFlags.TDCBF_OK_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.Yes"/> result.
        /// </summary>
        Yes = CommonButtonFlags.TDCBF_YES_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.No"/> result.
        /// </summary>
        No = CommonButtonFlags.TDCBF_NO_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.Cancel"/> result.
        /// </summary>
        /// <remarks>
        /// Note: Adding a Cancel button will automatically add a close button
        /// to the task dialog's title bar and will allow to close the dialog by
        /// pressing ESC or Alt+F4 (just as if you enabled
        /// <see cref="TaskDialogPage.AllowCancel"/>).
        /// </remarks>
        Cancel = CommonButtonFlags.TDCBF_CANCEL_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.Retry"/> result.
        /// </summary>
        Retry = CommonButtonFlags.TDCBF_RETRY_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.Close"/> result.
        /// </summary>
        Close = CommonButtonFlags.TDCBF_CLOSE_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.Abort"/> result.
        /// </summary>
        Abort = CommonButtonFlags.TDCBF_ABORT_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.Ignore"/> result.
        /// </summary>
        Ignore = CommonButtonFlags.TDCBF_IGNORE_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.TryAgain"/> result.
        /// </summary>
        TryAgain = CommonButtonFlags.TDCBF_TRYAGAIN_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.Continue"/> result.
        /// </summary>
        Continue = CommonButtonFlags.TDCBF_CONTINUE_BUTTON,

        /// <summary>
        /// The task dialog contains a standard button representing an <see cref="TaskDialogResult.Help"/> result.
        /// </summary>
        /// <remarks>
        /// Note: Clicking this button will not close the dialog, but will raise the
        /// <see cref="TaskDialogPage.HelpRequest"/> event.
        /// </remarks>
        Help = CommonButtonFlags.TDCBF_HELP_BUTTON
    }
}
