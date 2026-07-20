// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Gets or sets how the control renders itself when visual styles are applied. This is an ambient property.
    /// </summary>
    /// <value>
    ///  The <see cref="Forms.VisualStylesMode"/> for the control. When not explicitly set, the value is inherited
    ///  from the parent control, or, for a top-level control, from <see cref="Application.DefaultVisualStylesMode"/>.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   A control without an explicit value inherits from its parent or from
    ///   <see cref="Application.DefaultVisualStylesMode"/>. Derived controls can override
    ///   <see cref="DefaultVisualStylesMode"/> to pin a renderer version for compatibility.
    ///  </para>
    ///  <para>
    ///   Modern modes can increase the space required for chrome without reducing the client area. Controls with
    ///   intrinsic sizing, or with <see cref="AutoSize"/> enabled, request layout automatically. Multiline
    ///   <see cref="TextBox"/> and <see cref="RichTextBox"/> controls retain fixed bounds, so their client area can
    ///   shrink slightly. Prefer <see cref="TableLayoutPanel"/> or <see cref="FlowLayoutPanel"/> for adaptive layouts.
    ///  </para>
    ///  <para>
    ///   High Contrast resolves the effective mode to classic rendering. See the
    ///   <see href="https://github.com/dotnet/winforms/blob/main/docs/net11-visualstyles-layout-guidance.md">
    ///   .NET 11 VisualStyles layout guidance</see> for migration patterns.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatAppearance))]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [AmbientValue(VisualStylesMode.Inherit)]
    [SRDescription(nameof(SR.ControlVisualStylesModeDescr))]
    public virtual partial VisualStylesMode VisualStylesMode { get; set; }
}
