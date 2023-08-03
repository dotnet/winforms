// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static HANDLE CopyImage<T>(T hImage, GDI_IMAGE_TYPE type, int cx, int cy, IMAGE_FLAGS flags)
        where T : IHandle<HANDLE>
    {
        HANDLE result = CopyImage(hImage.Handle, type, cx, cy, flags);
        GC.KeepAlive(hImage.Wrapper);
        return result;
    }
}
