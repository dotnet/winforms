// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class ComCtl32
    {
        public static partial class ImageList
        {
            [LibraryImport(Libraries.Comctl32, EntryPoint = "ImageList_Draw")]
            public static partial BOOL Draw(IntPtr himl, int i, Gdi32.HDC hdcDst, int x, int y, ILD fStyle);

            public static BOOL Draw(IHandle himl, int i, Gdi32.HDC hdcDst, int x, int y, ILD fStyle)
            {
                BOOL result = Draw(himl.Handle, i, hdcDst, x, y, fStyle);
                GC.KeepAlive(himl);
                return result;
            }
        }
    }
}
