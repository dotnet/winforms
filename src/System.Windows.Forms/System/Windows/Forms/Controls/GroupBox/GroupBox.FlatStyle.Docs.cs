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
    ///   surface, <see cref="FlatStyle.Flat"/> renders a rounded accent outline, and <see cref="FlatStyle.Popup"/>
    ///   renders a rounded accent outline with a subtle accent-tinted header band.
    ///   <see cref="FlatStyle.System"/> remains a native <c>BS_GROUPBOX</c> in every mode.
    ///  </para>
    ///  <para>
    ///   The modern Standard and Popup surfaces preserve the classic <see cref="Control.DisplayRectangle"/> so
    ///   existing child layouts remain stable when switching visual styles. Modern captions preserve the ambient
    ///   font family, size, and style while still following system text scale. See the
    ///   <see href="https://github.com/dotnet/winforms/blob/main/docs/net11-visualstyles-layout-guidance.md">
    ///   .NET 11 VisualStyles layout guidance</see>.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [SRDescription(nameof(SR.ButtonFlatStyleDescr))]
    public partial FlatStyle FlatStyle { get; set; }
}
