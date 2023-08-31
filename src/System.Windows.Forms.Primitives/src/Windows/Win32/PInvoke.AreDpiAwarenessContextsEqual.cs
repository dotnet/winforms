// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <summary>
    ///  Compares two DPI awareness context values for equality.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if <paramref name="dpiContextA"/> and <paramref name="dpiContextB"/> are equal;
    ///  otherwise, <see langword="false"/> if not equal or the underlying OS does not support this API.
    /// </returns>
    public static bool AreDpiAwarenessContextsEqualInternal(DPI_AWARENESS_CONTEXT dpiContextA, DPI_AWARENESS_CONTEXT dpiContextB)
    {
        if (dpiContextA == DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT && dpiContextB == DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT)
        {
            return true;
        }

        if (OsVersion.IsWindows10_1607OrGreater())
        {
            return AreDpiAwarenessContextsEqual(dpiContextA, dpiContextB);
        }

        return false;
    }
}
