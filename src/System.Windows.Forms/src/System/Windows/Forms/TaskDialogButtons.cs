// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommonButtonFlags = Interop.TaskDialog.TASKDIALOG_COMMON_BUTTON_FLAGS;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum TaskDialogButtons : int
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        OK = CommonButtonFlags.TDCBF_OK_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        Yes = CommonButtonFlags.TDCBF_YES_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        No = CommonButtonFlags.TDCBF_NO_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Note: Adding a Cancel button will automatically add a close button
        /// to the task dialog's title bar and will allow to close the dialog by
        /// pressing ESC or Alt+F4 (just as if you enabled
        /// <see cref="TaskDialogPage.AllowCancel"/>).
        /// </remarks>
        Cancel = CommonButtonFlags.TDCBF_CANCEL_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        Retry = CommonButtonFlags.TDCBF_RETRY_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        Close = CommonButtonFlags.TDCBF_CLOSE_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        Abort = CommonButtonFlags.TDCBF_ABORT_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        Ignore = CommonButtonFlags.TDCBF_IGNORE_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        TryAgain = CommonButtonFlags.TDCBF_TRYAGAIN_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        Continue = CommonButtonFlags.TDCBF_CONTINUE_BUTTON,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Note: Clicking this button will not close the dialog, but will raise the
        /// <see cref="TaskDialogPage.HelpRequest"/> event.
        /// </remarks>
        Help = CommonButtonFlags.TDCBF_HELP_BUTTON
    }
}
