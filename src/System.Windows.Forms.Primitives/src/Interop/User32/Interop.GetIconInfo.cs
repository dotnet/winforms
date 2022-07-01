﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        private static partial BOOL GetIconInfo(IntPtr hIcon, out ICONINFO info);

        public static ICONINFO GetIconInfo(IntPtr hIcon)
        {
            GetIconInfo(hIcon, out ICONINFO info);
            return info;
        }

        public static ICONINFO GetIconInfo(IHandle cursor)
        {
            GetIconInfo(cursor.Handle, out ICONINFO info);
            GC.KeepAlive(cursor);
            return info;
        }

        [StructLayout(LayoutKind.Sequential)]
        public ref struct ICONINFO
        {
            public BOOL fIcon;
            public uint xHotspot;
            public uint yHotspot;
            public Gdi32.HBITMAP hbmMask;
            public Gdi32.HBITMAP hbmColor;

            public void Dispose()
            {
                if (!hbmMask.IsNull)
                {
                    Gdi32.DeleteObject(hbmMask);
                    hbmMask = default;
                }

                if (!hbmColor.IsNull)
                {
                    Gdi32.DeleteObject(hbmColor);
                    hbmColor = default;
                }
            }
        }
    }
}
