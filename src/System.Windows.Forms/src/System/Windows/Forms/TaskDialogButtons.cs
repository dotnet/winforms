// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents standard buttons of a task dialog.
    /// </summary>
    [Flags]
    public enum TaskDialogButtons : int
    {
        /// <summary>
        ///   No standard button is added to the task dialog.
        /// </summary>
        None = 0,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.OK"/> result.
        /// </summary>
        OK = ComCtl32.TDCBF.OK_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.Yes"/> result.
        /// </summary>
        Yes = ComCtl32.TDCBF.YES_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.No"/> result.
        /// </summary>
        No = ComCtl32.TDCBF.NO_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.Cancel"/> result.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Note: Adding a Cancel button will automatically add a close button
        ///   to the task dialog's title bar and will allow to close the dialog by
        ///   pressing ESC or Alt+F4 (just as if you enabled
        ///   <see cref="TaskDialogPage.AllowCancel"/>).
        /// </para>
        /// </remarks>
        Cancel = ComCtl32.TDCBF.CANCEL_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.Retry"/> result.
        /// </summary>
        Retry = ComCtl32.TDCBF.RETRY_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.Close"/> result.
        /// </summary>
        Close = ComCtl32.TDCBF.CLOSE_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.Abort"/> result.
        /// </summary>
        Abort = ComCtl32.TDCBF.ABORT_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.Ignore"/> result.
        /// </summary>
        Ignore = ComCtl32.TDCBF.IGNORE_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.TryAgain"/> result.
        /// </summary>
        TryAgain = ComCtl32.TDCBF.TRYAGAIN_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.Continue"/> result.
        /// </summary>
        Continue = ComCtl32.TDCBF.CONTINUE_BUTTON,

        /// <summary>
        ///   The task dialog contains a standard button representing an <see cref="TaskDialogResult.Help"/> result.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Note: Clicking this button will not close the dialog, but will raise the
        ///   <see cref="TaskDialogPage.HelpRequest"/> event.
        /// </para>
        /// </remarks>
        Help = ComCtl32.TDCBF.HELP_BUTTON
    }
}
