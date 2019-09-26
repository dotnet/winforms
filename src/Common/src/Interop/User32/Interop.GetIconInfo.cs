// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        private static extern BOOL GetIconInfo(IntPtr hIcon, out ICONINFO info);

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
            public IntPtr hbmMask;
            public IntPtr hbmColor;

            public void Dispose()
            {
                if (hbmMask != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(hbmMask);
                    hbmMask = IntPtr.Zero;
                }

                if (hbmColor != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(hbmColor);
                    hbmColor = IntPtr.Zero;
                }
            }
        }
    }
}
