// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class CheckBox
{
    /// <summary>
    ///  Gets or sets the value that determines the appearance of the check box.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   <see cref="Forms.Appearance.ToggleSwitch"/> selects the framework's owner-drawn toggle renderer for a
    ///   two-state check box in an effective .NET 11-or-later mode. Because a native check box cannot render a
    ///   toggle switch, this appearance intentionally takes precedence over <see cref="ButtonBase.FlatStyle"/> when
    ///   that property is <see cref="FlatStyle.System"/>. A check box with <see cref="ThreeState"/> enabled retains
    ///   classic check-box behavior instead.
    ///  </para>
    ///  <para>
    ///   High Contrast resolves to classic rendering. Modern toggle metrics can increase preferred size; see the
    ///   <see href="https://github.com/dotnet/winforms/blob/main/docs/net11-visualstyles-layout-guidance.md">
    ///   .NET 11 VisualStyles layout guidance</see>.
    ///  </para>
    /// </remarks>
    [DefaultValue(Appearance.Normal)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.CheckBoxAppearanceDescr))]
    public partial Appearance Appearance { get; set; }
}
