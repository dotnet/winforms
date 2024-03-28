// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualStudio.Shell;

/// <summary>
///  Predefined property categories for <see cref="ICategorizeProperties"/>. User defined categories
///  must have a positive value.
/// </summary>
internal enum PROPCAT : int
{
#pragma warning disable format
    Nil         = unchecked((int)0xFFFFFFFF),
    Misc        = unchecked((int)0xFFFFFFFE),
    Font        = unchecked((int)0xFFFFFFFD),
    Position    = unchecked((int)0xFFFFFFFC),
    Appearance  = unchecked((int)0xFFFFFFFB),
    Behavior    = unchecked((int)0xFFFFFFFA),
    Data        = unchecked((int)0xFFFFFFF9),
    List        = unchecked((int)0xFFFFFFF8),
    Text        = unchecked((int)0xFFFFFFF7),
    Scale       = unchecked((int)0xFFFFFFF6),
    DDE         = unchecked((int)0xFFFFFFF5),
#pragma warning restore format
}
