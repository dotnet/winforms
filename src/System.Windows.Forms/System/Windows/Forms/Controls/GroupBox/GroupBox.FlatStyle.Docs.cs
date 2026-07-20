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
    ///   In an effective .NET 11-or-later mode, <see cref="FlatStyle.Standard"/> renders a filled card with its
    ///   caption above the frame, <see cref="FlatStyle.Flat"/> renders an inline-caption outline, and
    ///   <see cref="FlatStyle.Popup"/> renders an accent-derived header band. <see cref="FlatStyle.System"/>
    ///   remains a native <c>BS_GROUPBOX</c> in every mode.
    ///  </para>
    ///  <para>
    ///   The modern card intentionally moves <see cref="Control.DisplayRectangle"/> down. AutoSize layouts
    ///   remeasure automatically; manually bold fonts appear additionally emphasized because the modern caption
    ///   derives a scaled semibold face from the ambient font. See the
    ///   <see href="https://github.com/dotnet/winforms/blob/main/docs/net11-visualstyles-layout-guidance.md">
    ///   .NET 11 VisualStyles layout guidance</see>.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [SRDescription(nameof(SR.ButtonFlatStyleDescr))]
    public partial FlatStyle FlatStyle { get; set; }
}
