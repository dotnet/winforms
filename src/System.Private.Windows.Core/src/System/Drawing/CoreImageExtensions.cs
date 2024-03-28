// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.Graphics.GdiPlus;

namespace System.Drawing;

// We also have an ImageExtensions in Primitives

internal static unsafe class CoreImageExtensions
{
    internal static void Save(this IImage image, Stream stream, Guid encoder, Guid format, EncoderParameters* encoderParameters)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (encoder == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(encoder));
        }

        if (format == PInvokeCore.ImageFormatGIF && image.Data is { } rawData && rawData.Length > 0)
        {
            stream.Write(rawData);
            return;
        }

        using var iStream = stream.ToIStream();
        PInvokeCore.GdipSaveImageToStream(image.Pointer, iStream, &encoder, encoderParameters).ThrowIfFailed();
    }

    internal static void Save(this IImage image, MemoryStream stream)
    {
        Guid format = default;
        PInvokeCore.GdipGetImageRawFormat(image.Pointer, &format).ThrowIfFailed();

        Guid encoder = ImageCodecInfoHelper.GetEncoderClsid(format);

        // Jpeg loses data, so we don't want to use it to serialize. We'll use PNG instead.
        // If we don't find an Encoder (for things like Icon), we just switch back to PNG.
        if (format == PInvokeCore.ImageFormatJPEG || encoder == Guid.Empty)
        {
            format = PInvokeCore.ImageFormatPNG;
            encoder = ImageCodecInfoHelper.GetEncoderClsid(format);
        }

        image.Save(stream, encoder, format, null);
    }
}
