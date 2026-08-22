// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  The interaction state of a modern (Net11) editable-control border, used to resolve its per-state
///  stroke. When several apply, precedence is Disabled > Focused > ReadOnly > Hover > Rest.
/// </summary>
internal enum ModernFieldStrokeState
{
    /// <summary>Default resting appearance.</summary>
    Rest,

    /// <summary>The pointer is over the control.</summary>
    Hover,

    /// <summary>The control has keyboard focus.</summary>
    Focused,

    /// <summary>The control is disabled.</summary>
    Disabled,

    /// <summary>The control is read-only.</summary>
    ReadOnly,
}
