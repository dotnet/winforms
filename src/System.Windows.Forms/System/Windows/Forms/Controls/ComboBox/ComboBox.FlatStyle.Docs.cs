// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Gets or sets the flat-style appearance of the combo box.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   In an effective .NET 11-or-later mode, <see cref="FlatStyle.Standard"/> uses a rounded field matching
    ///   the TextBoxBase color scheme, <see cref="FlatStyle.Flat"/> uses the same colors with a square border,
    ///   and <see cref="FlatStyle.Popup"/> uses Standard geometry with a Windows-accent border. Each style adds
    ///   a one-logical-pixel internal inset on top of <see cref="Control.Padding"/>. The native
    ///   <see cref="FlatStyle.System"/> style is not modernized.
    ///  </para>
    ///  <para>
    ///   Modern non-<see cref="ComboBoxStyle.Simple"/> controls share field metrics with an AutoSize,
    ///   single-line <see cref="BorderStyle.Fixed3D"/> TextBox whose Padding includes the same inset. Prefer
    ///   TableLayoutPanel rows for mixed TextBox and ComboBox layouts. See the
    ///   <see href="https://github.com/dotnet/winforms/blob/main/docs/net11-visualstyles-layout-guidance.md">
    ///   .NET 11 VisualStyles layout guidance</see>.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxFlatStyleDescr))]
    public partial FlatStyle FlatStyle { get; set; }
}
