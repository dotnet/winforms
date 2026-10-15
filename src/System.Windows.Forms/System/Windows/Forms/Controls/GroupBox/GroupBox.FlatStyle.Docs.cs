// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class GroupBox
{
    /// <summary>
    ///  Gets or sets the flat-style appearance of the group box.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   In an effective .NET 11-or-later mode, <see cref="FlatStyle.Standard"/> renders a borderless rectangular
    ///   surface with an enlarged caption, <see cref="FlatStyle.Flat"/> renders a rounded accent outline with an
    ///   ambient-size inline caption, and <see cref="FlatStyle.Popup"/> renders a Windows-accent header band.
    ///   <see cref="FlatStyle.System"/> remains a native <c>BS_GROUPBOX</c> in every mode.
    ///  </para>
    ///  <para>
    ///   The modern Standard surface intentionally moves <see cref="Control.DisplayRectangle"/> down and reserves
    ///   more space above its content than below. Its caption aligns with <see cref="Control.Padding"/>. When the
    ///   ambient font is regular and a matching installed Semibold family exists, modern captions use that real
    ///   face; otherwise they preserve the ambient weight. AutoSize layouts remeasure automatically. See the
    ///   <see href="https://github.com/dotnet/winforms/blob/main/docs/net11-visualstyles-layout-guidance.md">
    ///   .NET 11 VisualStyles layout guidance</see>.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [SRDescription(nameof(SR.ButtonFlatStyleDescr))]
    public partial FlatStyle FlatStyle { get; set; }
}
