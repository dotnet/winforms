// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Graphics.Dwm;

namespace System.Windows.Forms;

/// <summary>
///  Specifies the corner preference for a <see cref="Form"/> which can be
///  set using the <see cref="Form.FormCornerPreference"/> property.
/// </summary>
public enum FormCornerPreference
{
    /// <summary>
    ///  The default corner preference.
    /// </summary>
    Default = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT,

    /// <summary>
    ///  Do not round the corners of the form window.
    /// </summary>
    DoNotRound = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND,

    /// <summary>
    ///  Round the corners of the form window.
    /// </summary>
    Round = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND,

    /// <summary>
    ///  Round the corners of the form window with a small radius.
    /// </summary>
    RoundSmall = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL
}
