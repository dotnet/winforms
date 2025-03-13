// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the style of the sizing grip on a <see cref="Form"/>.
/// </summary>
public enum SizeGripStyle
{
    /// <summary>
    ///  The size grip is automatically display when needed.
    /// </summary>
    Auto = 0,

    /// <summary>
    ///  The sizing grip is always shown on the form.
    /// </summary>
    Show = 1,

    /// <summary>
    ///  The sizing grip is hidden.
    /// </summary>
    Hide = 2,
}
