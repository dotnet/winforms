// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.GdiPlus;
using System.Private.Windows.GdiPlus.Resources;

namespace Windows.Win32;

internal static partial class PInvokeGdiPlus
{
    static PInvokeGdiPlus()
    {
        // Ensure GDI+ is initialized before the first PInvoke call. Note that this has to happen after
        // the DPI awareness context is set for scaling to occur correctly.
        if (!OperatingSystem.IsWindows())
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (_, _, _) =>
                throw new PlatformNotSupportedException(SR.PlatformNotSupported_Unix));
        }

        bool initialized = GdiPlusInitialization.EnsureInitialized();

        Debug.Assert(initialized);
    }
}
