// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.HiDpi;

internal readonly partial struct DPI_AWARENESS_CONTEXT
{
    internal static DPI_AWARENESS_CONTEXT UNSPECIFIED_DPI_AWARENESS_CONTEXT { get; } = Null;

    /// <summary>
    ///  Compares <see cref="DPI_AWARENESS"/> for equality.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the specified context is equal; otherwise, <see langword="false"/> if not equal
    ///  or the underlying OS does not support comparing context.
    /// </returns>
    public bool IsEquivalent(DPI_AWARENESS_CONTEXT dpiContext) =>
        (this == UNSPECIFIED_DPI_AWARENESS_CONTEXT && dpiContext == UNSPECIFIED_DPI_AWARENESS_CONTEXT)
            || (OsVersion.IsWindows10_1607OrGreater()
                && (bool)PInvoke.AreDpiAwarenessContextsEqual(this, dpiContext));
}
