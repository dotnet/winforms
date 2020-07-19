// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TDM : uint
        {
            NAVIGATE_PAGE = User32.WM.USER + 101,
            /// <summary>
            ///   "wParam = Button ID"
            /// </summary>
            CLICK_BUTTON = User32.WM.USER + 102,
            /// <summary>
            ///   "wParam = 0 (nonMarque) wParam != 0 (Marquee)"
            /// </summary>
            SET_MARQUEE_PROGRESS_BAR = User32.WM.USER + 103,
            /// <summary>
            ///   "wParam = new progress state"
            /// </summary>
            SET_PROGRESS_BAR_STATE = User32.WM.USER + 104,
            /// <summary>
            ///   "lParam = MAKELPARAM(nMinRange, nMaxRange)"
            /// </summary>
            SET_PROGRESS_BAR_RANGE = User32.WM.USER + 105,
            /// <summary>
            ///   "wParam = new position"
            /// </summary>
            SET_PROGRESS_BAR_POS = User32.WM.USER + 106,
            /// <summary>
            ///   "wParam = 0 (stop marquee), wParam != 0 (start marquee),
            ///   lparam = speed (milliseconds between repaints)"
            /// </summary>
            SET_PROGRESS_BAR_MARQUEE = User32.WM.USER + 107,
            /// <summary>
            ///   "wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)"
            /// </summary>
            SET_ELEMENT_TEXT = User32.WM.USER + 108,
            /// <summary>
            ///   "wParam = Radio Button ID"
            /// </summary>
            CLICK_RADIO_BUTTON = User32.WM.USER + 110,
            /// <summary>
            ///   "lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID"
            /// </summary>
            ENABLE_BUTTON = User32.WM.USER + 111,
            /// <summary>
            ///   "lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID"
            /// </summary>
            ENABLE_RADIO_BUTTON = User32.WM.USER + 112,
            /// <summary>
            ///   "wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)"
            /// </summary>
            CLICK_VERIFICATION = User32.WM.USER + 113,
            /// <summary>
            ///   "wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)"
            /// </summary>
            UPDATE_ELEMENT_TEXT = User32.WM.USER + 114,
            /// <summary>
            ///   "wParam = Button ID, lParam = 0 (elevation not required),
            ///   lParam != 0 (elevation required)"
            /// </summary>
            SET_BUTTON_ELEVATION_REQUIRED_STATE = User32.WM.USER + 115,
            /// <summary>
            ///   "wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon
            ///   (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)"
            /// </summary>
            UPDATE_ICON = User32.WM.USER + 116,
        }
    }
}
