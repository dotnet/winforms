// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="CopyImage(HANDLE, GDI_IMAGE_TYPE, int, int, IMAGE_FLAGS)"/>
    public static HICON CopyIcon<T>(T hImage, int cx, int cy, IMAGE_FLAGS flags = default)
        where T : IHandle<HICON>
    {
        HICON result = (HICON)CopyImage(hImage.Handle, GDI_IMAGE_TYPE.IMAGE_ICON, cx, cy, flags);
        GC.KeepAlive(hImage.Wrapper);
        return result;
    }

    /// <inheritdoc cref="CopyImage(HANDLE, GDI_IMAGE_TYPE, int, int, IMAGE_FLAGS)"/>
    public static HCURSOR CopyCursor<T>(T hImage, int cx, int cy, IMAGE_FLAGS flags = default)
        where T : IHandle<HCURSOR>
    {
        HCURSOR result = (HCURSOR)CopyImage(hImage.Handle, GDI_IMAGE_TYPE.IMAGE_CURSOR, cx, cy, flags);
        GC.KeepAlive(hImage.Wrapper);
        return result;
    }
}
