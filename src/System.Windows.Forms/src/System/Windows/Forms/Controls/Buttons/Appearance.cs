// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the appearance of a control.
/// </summary>
public enum Appearance
{
    /// <summary>
    ///  The default appearance defined by the control class.
    /// </summary>
    Normal = 0,

    /// <summary>
    ///  The appearance of a Windows button.
    /// </summary>
    Button = 1,

    /// <summary>
    ///  The appearance of a Modern UI toggle switch.
    ///  This setting is not taken into account, when <see cref="VisualStylesMode"/> is set
    ///  to <see cref="VisualStylesMode.Disabled"/> or <see cref="VisualStylesMode.Classic"/>.
    /// </summary>
    [Experimental("WFO9000")]
    ToggleSwitch = 2
}
