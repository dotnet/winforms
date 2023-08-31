// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public enum MessageBoxButtons
{
    /// <summary>
    ///  Specifies that the message box contains an OK button.
    /// </summary>
    OK = (int)MESSAGEBOX_STYLE.MB_OK,

    /// <summary>
    ///  Specifies that the message box contains OK and Cancel buttons.
    /// </summary>
    OKCancel = (int)MESSAGEBOX_STYLE.MB_OKCANCEL,

    /// <summary>
    ///  Specifies that the message box contains Abort, Retry, and Ignore buttons.
    /// </summary>
    AbortRetryIgnore = (int)MESSAGEBOX_STYLE.MB_ABORTRETRYIGNORE,

    /// <summary>
    ///  Specifies that the message box contains Yes, No, and Cancel buttons.
    /// </summary>
    YesNoCancel = (int)MESSAGEBOX_STYLE.MB_YESNOCANCEL,

    /// <summary>
    ///  Specifies that the message box contains Yes and No buttons.
    /// </summary>
    YesNo = (int)MESSAGEBOX_STYLE.MB_YESNO,

    /// <summary>
    ///  Specifies that the message box contains Retry and Cancel buttons.
    /// </summary>
    RetryCancel = (int)MESSAGEBOX_STYLE.MB_RETRYCANCEL,

    /// <summary>
    ///  Specifies that the message box contains Cancel, Try Again, and Continue buttons.
    /// </summary>
    CancelTryContinue = (int)MESSAGEBOX_STYLE.MB_CANCELTRYCONTINUE,
}
