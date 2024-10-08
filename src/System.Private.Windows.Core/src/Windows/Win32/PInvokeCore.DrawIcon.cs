// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    public static BOOL DrawIcon<T>(HDC hDC, int x, int y, T hIcon)
        where T : IHandle<HICON>
    {
        // DrawIcon effectively calls DrawIconEx with the following parameters:
        //
        //    DrawIconEx(hdc, x, y, hIcon, 0, 0, 0, 0, DI_NORMAL | DI_DEFAULTSIZE );

        BOOL result = DrawIconEx(hDC, x, y, hIcon, 0, 0, DI_FLAGS.DI_NORMAL | DI_FLAGS.DI_DEFAULTSIZE);
        GC.KeepAlive(hIcon.Wrapper);
        return result;
    }
}
