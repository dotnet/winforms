// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;

namespace Windows.Win32.UI.Accessibility;

internal static class UIAHelper
{
    /// <summary>
    ///  Converts a window handle to a <see cref="VARIANT"/> for UIA purposes.
    /// </summary>
    /// <param name="handle">The handle to the window</param>
    /// <returns>The <see cref="VARIANT"/> version of the window handle.</returns>
    public static VARIANT WindowHandleToVariant(nint handle)
        => new()
        {
            vt = VARENUM.VT_I4,
            data = new() { intVal = (int)handle }
        };
}
