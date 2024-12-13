// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.GdiPlus;

internal partial struct GdiplusStartupInputEx
{
    public static GdiplusStartupInputEx GetDefault()
    {
        OperatingSystem os = Environment.OSVersion;
        GdiplusStartupInputEx result = default;

        // In Windows 7 GDI+ 1.1 story is different as there are different binaries per GDI+ version.
        bool isWindows7 = os.Platform == PlatformID.Win32NT && os.Version.Major == 6 && os.Version.Minor == 1;
        result.Base.GdiplusVersion = isWindows7 ? 1u : 2;
        result.Base.SuppressBackgroundThread = BOOL.FALSE;
        result.Base.SuppressExternalCodecs = BOOL.FALSE;
        result.StartupParameters = 0;
        return result;
    }
}
