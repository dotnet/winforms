// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

/// <summary>
///  Determines whether visual styles are enabled.
/// </summary>
[Flags]
public enum VisualStyleState
{
    /// <summary>
    ///  Visual styles are not enabled.
    /// </summary>
    NoneEnabled = 0,

    /// <summary>
    ///  Visual styles enabled only for client area.
    /// </summary>
    ClientAreaEnabled = (int)SET_THEME_APP_PROPERTIES_FLAGS.ALLOW_CONTROLS,

    /// <summary>
    ///  Visual styles enabled only for non-client area.
    /// </summary>
    NonClientAreaEnabled = (int)SET_THEME_APP_PROPERTIES_FLAGS.ALLOW_NONCLIENT,

    /// <summary>
    ///  Visual styles enabled only for client and non-client areas.
    /// </summary>
    ClientAndNonClientAreasEnabled = (int)(SET_THEME_APP_PROPERTIES_FLAGS.ALLOW_NONCLIENT | SET_THEME_APP_PROPERTIES_FLAGS.ALLOW_CONTROLS)
}
