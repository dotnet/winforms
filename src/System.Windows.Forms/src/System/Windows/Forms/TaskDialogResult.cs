// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// 
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
        OK = Interop.TaskDialog.IDOK,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Note: Adding a Cancel button will automatically add a close button
        /// to the task dialog's title bar and will allow to close the dialog by
        /// pressing ESC or Alt+F4 (just as if you enabled
        /// <see cref="TaskDialogPage.AllowCancel"/>).
        /// </remarks>
        Cancel = Interop.TaskDialog.IDCANCEL,

        /// <summary>
        /// 
        /// </summary>
        Abort = Interop.TaskDialog.IDABORT,

        /// <summary>
        /// 
        /// </summary>
        Retry = Interop.TaskDialog.IDRETRY,

        /// <summary>
        /// 
        /// </summary>
        Ignore = Interop.TaskDialog.IDIGNORE,

        /// <summary>
        /// 
        /// </summary>
        Yes = Interop.TaskDialog.IDYES,

        /// <summary>
        /// 
        /// </summary>
        No = Interop.TaskDialog.IDNO,

        /// <summary>
        /// 
        /// </summary>
        Close = Interop.TaskDialog.IDCLOSE,

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Note: Clicking this button will not close the dialog, but will raise the
        /// <see cref="TaskDialogPage.Help"/> event.
        /// </remarks>
        Help = Interop.TaskDialog.IDHELP,

        /// <summary>
        /// 
        /// </summary>
        TryAgain = Interop.TaskDialog.IDTRYAGAIN,

        /// <summary>
        /// 
        /// </summary>
        Continue = Interop.TaskDialog.IDCONTINUE
    }
}
