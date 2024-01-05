// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
///  Specifies the printer's duplex setting.
/// </summary>
public enum Duplex
{
    /// <summary>
    ///  The printer's default duplex setting.
    /// </summary>
    Default = -1,

    /// <summary>
    ///  Single-sided printing.
    /// </summary>
    Simplex = DEVMODE_DUPLEX.DMDUP_SIMPLEX,

    /// <summary>
    ///  Double-sided, horizontal printing.
    /// </summary>
    Horizontal = DEVMODE_DUPLEX.DMDUP_HORIZONTAL,

    /// <summary>
    ///  Double-sided, vertical printing.
    /// </summary>
    Vertical = DEVMODE_DUPLEX.DMDUP_VERTICAL,
}
