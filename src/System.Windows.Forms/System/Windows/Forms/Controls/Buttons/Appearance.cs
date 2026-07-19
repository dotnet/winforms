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
    ///  The appearance of a modern UI toggle switch.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This value has no effect when <see cref="Control.VisualStylesMode"/> is set to
    ///   <see cref="VisualStylesMode.Disabled"/> or <see cref="VisualStylesMode.Classic"/>.
    ///  </para>
    ///  <para>
    ///   For later visual styles versions, each control determines whether and how it supports this value.
    ///   Setting it does not require every control or every control state to render as a toggle switch.
    ///  </para>
    /// </remarks>
    ToggleSwitch = 2
}
