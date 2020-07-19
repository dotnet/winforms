// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TDN : int
        {
            CREATED = 0,
            NAVIGATED = 1,
            /// <summary>
            ///   "wParam = Button ID"
            /// </summary>
            BUTTON_CLICKED = 2,
            /// <summary>
            ///   "lParam = (LPCWSTR)pszHREF"
            /// </summary>
            HYPERLINK_CLICKED = 3,
            /// <summary>
            ///   "wParam = Milliseconds since dialog created or timer reset"
            /// </summary>
            TIMER = 4,
            DESTROYED = 5,
            /// <summary>
            ///   "wParam = Radio Button ID"
            /// </summary>
            RADIO_BUTTON_CLICKED = 6,
            DIALOG_CONSTRUCTED = 7,
            /// <summary>
            ///   "wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0"
            /// </summary>
            VERIFICATION_CLICKED = 8,
            HELP = 9,
            /// <summary>
            ///   "wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)"
            /// </summary>
            EXPANDO_BUTTON_CLICKED = 10,
        }
    }
}
