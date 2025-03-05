// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies identifiers to indicate the return value of a dialog box.
/// </summary>
public enum DialogResult
{
    /// <summary>
    ///  Nothing is returned from the dialog box. This means that the modal dialog continues running.
    /// </summary>
    None = 0,

    /// <summary>
    ///  The dialog box return value is OK (usually sent from a button labeled OK).
    /// </summary>
    OK = MESSAGEBOX_RESULT.IDOK,

    /// <summary>
    ///  The dialog box return value is Cancel (usually sent from a button labeled Cancel).
    /// </summary>
    Cancel = MESSAGEBOX_RESULT.IDCANCEL,

    /// <summary>
    ///  The dialog box return value is Abort (usually sent from a button labeled Abort).
    /// </summary>
    Abort = MESSAGEBOX_RESULT.IDABORT,

    /// <summary>
    ///  The dialog box return value is Retry (usually sent from a button labeled Retry).
    /// </summary>
    Retry = MESSAGEBOX_RESULT.IDRETRY,

    /// <summary>
    ///  The dialog box return value is Ignore (usually sent from a button labeled Ignore).
    /// </summary>
    Ignore = MESSAGEBOX_RESULT.IDIGNORE,

    /// <summary>
    ///  The dialog box return value is Yes (usually sent from a button labeled Yes).
    /// </summary>
    Yes = MESSAGEBOX_RESULT.IDYES,

    /// <summary>
    ///  The dialog box return value is No (usually sent from a button labeled No).
    /// </summary>
    No = MESSAGEBOX_RESULT.IDNO,

    /// <summary>
    ///  The dialog box return value is Try Again (usually sent from a button labeled Try Again).
    /// </summary>
    TryAgain = MESSAGEBOX_RESULT.IDTRYAGAIN,

    /// <summary>
    ///  The dialog box return value is Continue (usually sent from a button labeled Continue).
    /// </summary>
    Continue = MESSAGEBOX_RESULT.IDCONTINUE,
}
