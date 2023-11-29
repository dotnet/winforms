// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the case of characters in a Textbox control.
/// </summary>
public enum CharacterCasing
{
    /// <summary>
    ///  The case of characters is left unchanged.
    /// </summary>
    Normal = 0,

    /// <summary>
    ///  Converts all characters to uppercase.
    /// </summary>
    Upper = 1,

    /// <summary>
    ///  Converts all characters to lowercase.
    /// </summary>
    Lower = 2,
}
