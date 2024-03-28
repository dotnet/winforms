// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
///  Specifies a printer resolution.
/// </summary>
public enum PrinterResolutionKind
{
    /// <summary>
    ///  High resolution.
    /// </summary>
    High = PInvoke.DMRES_HIGH,

    /// <summary>
    ///  Medium resolution.
    /// </summary>
    Medium = PInvoke.DMRES_MEDIUM,

    /// <summary>
    ///  Low resolution.
    /// </summary>
    Low = PInvoke.DMRES_LOW,

    /// <summary>
    ///  Draft-quality resolution.
    /// </summary>
    Draft = PInvoke.DMRES_DRAFT,

    /// <summary>
    ///  Custom resolution.
    /// </summary>
    Custom = 0,
}
