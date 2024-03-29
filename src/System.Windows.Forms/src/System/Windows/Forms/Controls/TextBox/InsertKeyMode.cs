﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents the mode characters are entered in a text box.
/// </summary>
public enum InsertKeyMode
{
    /// <summary>
    ///  Honors the Insert key mode.
    /// </summary>
    Default,

    /// <summary>
    ///  Forces insertion mode to be 'on' regardless of the Insert key mode.
    /// </summary>
    Insert,

    /// <summary>
    ///  Forces insertion mode to be 'off' regardless of the Insert key mode.
    /// </summary>
    Overwrite
}
