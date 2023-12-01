// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

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
    OK = MESSAGEBOX_RESULT.IDOK,

    /// <summary>
    ///  The <c>Cancel</c> button was selected.
    /// </summary>
    Cancel = MESSAGEBOX_RESULT.IDCANCEL,

    /// <summary>
    ///  The <c>Abort</c> button was selected.
    /// </summary>
    Abort = MESSAGEBOX_RESULT.IDABORT,

    /// <summary>
    ///  The <c>Retry</c> button was selected.
    /// </summary>
    Retry = MESSAGEBOX_RESULT.IDRETRY,

    /// <summary>
    ///  The <c>Ignore</c> button was selected.
    /// </summary>
    Ignore = MESSAGEBOX_RESULT.IDIGNORE,

    /// <summary>
    ///  The <c>Yes</c> button was selected.
    /// </summary>
    Yes = MESSAGEBOX_RESULT.IDYES,

    /// <summary>
    ///  The <c>No</c> button was selected.
    /// </summary>
    No = MESSAGEBOX_RESULT.IDNO,

    /// <summary>
    ///  The <c>Close</c> button was selected.
    /// </summary>
    Close = MESSAGEBOX_RESULT.IDCLOSE,

    /// <summary>
    ///  The <c>Help</c> button was selected.
    /// </summary>
    Help = MESSAGEBOX_RESULT.IDHELP,

    /// <summary>
    ///  The <c>Try Again</c> button was selected.
    /// </summary>
    TryAgain = MESSAGEBOX_RESULT.IDTRYAGAIN,

    /// <summary>
    ///  The <c>Continue</c> button was selected.
    /// </summary>
    Continue = MESSAGEBOX_RESULT.IDCONTINUE
}
