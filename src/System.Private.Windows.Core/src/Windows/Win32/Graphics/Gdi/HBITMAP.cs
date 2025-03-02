// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

internal readonly partial struct HBITMAP : IDisposable
{
    public void Dispose()
    {
        if (!IsNull)
        {
            PInvokeCore.DeleteObject(this);
        }
    }

    /// <summary>
    ///  Creates a compatible bitmap copying the content of the current bitmap.
    /// </summary>
    public HBITMAP CreateCompatibleBitmap(int width, int height)
    {
        using var screenDC = GetDcScope.ScreenDC;

        // Create a compatible DC to render the source bitmap.
        using CreateDcScope sourceDC = new(screenDC);
        using SelectObjectScope sourceBitmapSelection = new(sourceDC, this);

        // Create a compatible DC and a new compatible bitmap.
        using CreateDcScope destinationDC = new(screenDC);
        HBITMAP compatibleBitmap = PInvokeCore.CreateCompatibleBitmap(screenDC, width, height);

        // Select the new bitmap into a compatible DC and blit in the original bitmap.
        using SelectObjectScope destinationBitmapSelection = new(destinationDC, compatibleBitmap);
        PInvokeCore.BitBlt(
            destinationDC,
            0,
            0,
            width,
            height,
            sourceDC,
            0,
            0,
            ROP_CODE.SRCCOPY);

        return compatibleBitmap;
    }
}
