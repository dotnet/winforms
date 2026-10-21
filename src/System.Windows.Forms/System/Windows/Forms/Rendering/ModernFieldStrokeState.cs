// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Interaction state for a modern (Net11) editable-control border stroke.
///  Multiple states use priority Disabled > Focused > ReadOnly > Hover > Rest.
/// </summary>
internal enum ModernFieldStrokeState
{
    /// <summary>Normal appearance.</summary>
    Rest,

    /// <summary>Pointer is over the control.</summary>
    Hover,

    /// <summary>Control has keyboard focus.</summary>
    Focused,

    /// <summary>Control is disabled.</summary>
    Disabled,

    /// <summary>Control is read-only.</summary>
    ReadOnly,
}
