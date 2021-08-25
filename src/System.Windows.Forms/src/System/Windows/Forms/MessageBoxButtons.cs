// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

namespace System.Windows.Forms
{
    public enum MessageBoxButtons
    {
        /// <summary>
        ///  Specifies that the message box contains an OK button.
        /// </summary>
        OK = (int)MB.OK,

        /// <summary>
        ///  Specifies that the message box contains OK and Cancel buttons.
        /// </summary>
        OKCancel = (int)MB.OKCANCEL,

        /// <summary>
        ///  Specifies that the message box contains Abort, Retry, and Ignore buttons.
        /// </summary>
        AbortRetryIgnore = (int)MB.ABORTRETRYIGNORE,

        /// <summary>
        ///  Specifies that the message box contains Yes, No, and Cancel buttons.
        /// </summary>
        YesNoCancel = (int)MB.YESNOCANCEL,

        /// <summary>
        ///  Specifies that the message box contains Yes and No buttons.
        /// </summary>
        YesNo = (int)MB.YESNO,

        /// <summary>
        ///  Specifies that the message box contains Retry and Cancel buttons.
        /// </summary>
        RetryCancel = (int)MB.RETRYCANCEL,

        /// <summary>
        ///  Specifies that the message box contains Cancel, Try Again, and Continue buttons.
        /// </summary>
        CancelTryContinue = (int)MB.CANCELTRYCONTINUE,
    }
}
