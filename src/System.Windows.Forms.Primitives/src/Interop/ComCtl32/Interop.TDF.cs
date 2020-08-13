// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TDF : uint
        {
            ENABLE_HYPERLINKS = 0x00000001,
            USE_HICON_MAIN = 0x00000002,
            USE_HICON_FOOTER = 0x00000004,
            ALLOW_DIALOG_CANCELLATION = 0x00000008,
            USE_COMMAND_LINKS = 0x00000010,
            USE_COMMAND_LINKS_NO_ICON = 0x00000020,
            EXPAND_FOOTER_AREA = 0x00000040,
            EXPANDED_BY_DEFAULT = 0x00000080,
            VERIFICATION_FLAG_CHECKED = 0x00000100,
            SHOW_PROGRESS_BAR = 0x00000200,
            SHOW_MARQUEE_PROGRESS_BAR = 0x00000400,
            CALLBACK_TIMER = 0x00000800,
            POSITION_RELATIVE_TO_WINDOW = 0x00001000,
            RTL_LAYOUT = 0x00002000,
            NO_DEFAULT_RADIO_BUTTON = 0x00004000,
            CAN_BE_MINIMIZED = 0x00008000,
            /// <summary>
            ///   "Don't call SetForegroundWindow() when activating the dialog"
            /// </summary>
            /// <remarks>
            /// <para>
            ///   This flag has an effect only on Windows NT 6.2 ("Windows 8") and higher.
            /// </para>
            /// </remarks>
            NO_SET_FOREGROUND = 0x00010000,
            /// <summary>
            ///   "used by ShellMessageBox to emulate MessageBox sizing behavior"
            /// </summary>
            SIZE_TO_CONTENT = 0x01000000,
        }
    }
}
