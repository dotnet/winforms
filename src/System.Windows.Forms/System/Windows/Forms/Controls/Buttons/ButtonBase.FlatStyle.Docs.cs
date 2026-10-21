// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ButtonBase
{
    /// <summary>
    ///  Gets or sets the flat-style appearance of the button control.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   In an effective .NET 11-or-later visual-styles mode, <see cref="FlatStyle.Standard"/>,
    ///   <see cref="FlatStyle.Flat"/>, and <see cref="FlatStyle.Popup"/> select framework-defined modern
    ///   renderers. Their colors and metrics derive from system state, including dark mode, accent color,
    ///   text scale, and DPI.
    ///  </para>
    ///  <para>
    ///   <see cref="FlatStyle.System"/> requests the native control and is not modernized. A two-state
    ///   <see cref="CheckBox"/> with <see cref="Forms.Appearance.ToggleSwitch"/> is the exception: its appearance
    ///   requires owner drawing and therefore takes precedence over System. Modern chrome can increase preferred
    ///   size, so adaptive layout containers are recommended. See the
    ///   <see href="https://github.com/dotnet/winforms/blob/main/docs/net11-visualstyles-layout-guidance.md">
    ///   .NET 11 VisualStyles layout guidance</see>.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ButtonFlatStyleDescr))]
    public partial FlatStyle FlatStyle { get; set; }
}
