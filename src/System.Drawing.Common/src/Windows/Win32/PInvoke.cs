// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32;

internal static partial class PInvoke
{
    static PInvoke()
    {
        // Ensure GDI+ is initialized before the first PInvoke call. Note that this has to happen after
        // the DPI awareness context is set for scaling to occur correctly.
        bool initialized = Gdip.Initialized;
        Debug.Assert(initialized);
    }
}
