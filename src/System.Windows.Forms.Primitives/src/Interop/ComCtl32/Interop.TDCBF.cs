// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TDCBF : uint
        {
            /// <summary>
            ///   "selected control return value IDOK"
            /// </summary>
            OK_BUTTON = 0x00000001,
            /// <summary>
            ///   "selected control return value IDYES"
            /// </summary>
            YES_BUTTON = 0x00000002,
            /// <summary>
            ///   "selected control return value IDNO"
            /// </summary>
            NO_BUTTON = 0x00000004,
            /// <summary>
            ///   "selected control return value IDCANCEL"
            /// </summary>
            CANCEL_BUTTON = 0x00000008,
            /// <summary>
            ///   "selected control return value IDRETRY"
            /// </summary>
            RETRY_BUTTON = 0x00000010,
            /// <summary>
            ///   "selected control return value IDCLOSE"
            /// </summary>
            CLOSE_BUTTON = 0x00000020,
            // Note: The following values are not documented, but have been tested
            // to work correctly on every version of Windows that supports the
            // Task Dialog (from Windows Vista to Windows 10 Version 1909).
            /// <summary>
            ///   "selected control return value IDABORT"
            /// </summary>
            ABORT_BUTTON = 0x00010000,
            /// <summary>
            ///   "selected control return value IDIGNORE"
            /// </summary>
            IGNORE_BUTTON = 0x00020000,
            /// <summary>
            ///   "selected control return value IDTRYAGAIN"
            /// </summary>
            TRYAGAIN_BUTTON = 0x00040000,
            /// <summary>
            ///   "selected control return value IDCONTINUE"
            /// </summary>
            CONTINUE_BUTTON = 0x00080000,
            /// <summary>
            ///   "selected control return value IDHELP"
            /// </summary>
            HELP_BUTTON = 0x00100000,
        }
    }
}
