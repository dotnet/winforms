﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class ComCtl32
    {
        public static partial class ImageList
        {
            [LibraryImport(Libraries.Comctl32, EntryPoint = "ImageList_DrawEx")]
            public static partial BOOL DrawEx(IntPtr himl, int i, IntPtr hdcDst, int x, int y, int dx, int dy, int rgbBk, int rgbFg, ILD fStyle);

            public static BOOL DrawEx(IntPtr himl, int i, HandleRef hdcDst, int x, int y, int dx, int dy, int rgbBk, int rgbFg, ILD fStyle)
            {
                BOOL result = DrawEx(himl, i, hdcDst.Handle, x, y, dx, dy, rgbBk, rgbFg, fStyle);
                GC.KeepAlive(hdcDst.Wrapper);
                return result;
            }

            public static BOOL DrawEx(IHandle himl, int i, HandleRef hdcDst, int x, int y, int dx, int dy, int rgbBk, int rgbFg, ILD fStyle)
            {
                BOOL result = DrawEx(himl.Handle, i, hdcDst.Handle, x, y, dx, dy, rgbBk, rgbFg, fStyle);
                GC.KeepAlive(himl);
                GC.KeepAlive(hdcDst.Wrapper);
                return result;
            }
        }
    }
}
