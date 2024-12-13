// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.Graphics.GdiPlus;

namespace System.Drawing;

internal static class ImageCodecInfoHelper
{
    // Getting the entire array of ImageCodecInfo objects is expensive and not necessary when all we need are the
    // encoder CLSIDs. There are only 5 encoders: PNG, JPEG, GIF, BMP, and TIFF. We'll just build the small cache
    // here to avoid all of the overhead. (We could probably just hard-code the values, but on the very small chance
    // that the list of encoders changes, we'll keep it dynamic.)
    private static (Guid Format, Guid Encoder)[]? s_encoders;

    /// <summary>
    ///  Get the encoder guid for the given image format guid.
    /// </summary>
    internal static Guid GetEncoderClsid(Guid format)
    {
        foreach ((Guid Format, Guid Encoder) in Encoders)
        {
            if (Format == format)
            {
                return Encoder;
            }
        }

        return Guid.Empty;
    }

    private static unsafe (Guid Format, Guid Encoder)[] Encoders
    {
        get
        {
            if (s_encoders is null)
            {
                GdiPlusInitialization.EnsureInitialized();

                uint numEncoders;
                uint size;

                PInvokeGdiPlus.GdipGetImageEncodersSize(&numEncoders, &size).ThrowIfFailed();

                using BufferScope<byte> buffer = new((int)size);

                fixed (byte* b = buffer)
                {
                    PInvokeGdiPlus.GdipGetImageEncoders(numEncoders, size, (ImageCodecInfo*)b).ThrowIfFailed();
                    ReadOnlySpan<ImageCodecInfo> codecInfo = new((ImageCodecInfo*)b, (int)numEncoders);
                    s_encoders = new (Guid Format, Guid Encoder)[codecInfo.Length];

                    for (int i = 0; i < codecInfo.Length; i++)
                    {
                        s_encoders[i] = (codecInfo[i].FormatID, codecInfo[i].Clsid);
                    }
                }
            }

            return s_encoders;
        }
    }
}
