// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public enum MessageBoxButtons
    {
        /// <summary>
        /// Specifies that the message box contains an OK button. This field is
        /// constant.
        /// </devdoc>
        OK = 0x00000000,

        /// <summary>
        /// Specifies that the message box contains OK and Cancel buttons. This
        /// field is constant.
        /// </devdoc>
        OKCancel = 0x00000001,

        /// <summary>
        /// Specifies that the message box contains Abort, Retry, and Ignore
        /// buttons.
        /// This field is constant.
        /// </devdoc>
        AbortRetryIgnore = 0x00000002,

        /// <summary>
        /// Specifies that the message box contains Yes, No, and Cancel buttons.
        /// This field is constant.
        /// </devdoc>
        YesNoCancel = 0x00000003,

        /// <summary>
        /// Specifies that the
        /// message box contains Yes and No buttons. This field is
        /// constant.
        /// </devdoc>
        YesNo = 0x00000004,

        /// <summary>
        /// Specifies that the message box contains Retry and Cancel buttons.
        /// This field is constant.
        /// </devdoc>
        RetryCancel = 0x00000005
    }
}
