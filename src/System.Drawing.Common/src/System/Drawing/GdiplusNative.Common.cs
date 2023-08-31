// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Drawing;

// Raw function imports for gdiplus
internal static partial class SafeNativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupInputEx
    {
        public int GdiplusVersion;                        // Must be 1 or 2

        public IntPtr DebugEventCallback;

        public BOOL SuppressBackgroundThread;     // FALSE unless you're prepared to call
                                                          // the hook/unhook functions properly

        public BOOL SuppressExternalCodecs;       // FALSE unless you want GDI+ only to use
                                                          // its internal image codecs.
        public int StartupParameters;

        public static StartupInputEx GetDefault()
        {
            OperatingSystem os = Environment.OSVersion;
            StartupInputEx result = default;

            // In Windows 7 GDI+1.1 story is different as there are different binaries per GDI+ version.
            bool isWindows7 = os.Platform == PlatformID.Win32NT && os.Version.Major == 6 && os.Version.Minor == 1;
            result.GdiplusVersion = isWindows7 ? 1 : 2;
            result.SuppressBackgroundThread = BOOL.FALSE;
            result.SuppressExternalCodecs = BOOL.FALSE;
            result.StartupParameters = 0;
            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct StartupOutput
    {
        // The following 2 fields won't be used.  They were originally intended
        // for getting GDI+ to run on our thread - however there are marshalling
        // dealing with function *'s and what not - so we make explicit calls
        // to gdi+ after the fact, via the GdiplusNotificationHook and
        // GdiplusNotificationUnhook methods.
        public IntPtr hook; //not used
        public IntPtr unhook; //not used.
    }
}
