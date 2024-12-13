// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

// sdkinc\imaging.h
public sealed unsafe class ImageCodecInfo
{
    internal ImageCodecInfo()
    {
    }

    public Guid Clsid { get; set; }

    public Guid FormatID { get; set; }

    public string? CodecName { get; set; }

    public string? DllName { get; set; }

    public string? FormatDescription { get; set; }

    public string? FilenameExtension { get; set; }

    public string? MimeType { get; set; }

    public ImageCodecFlags Flags { get; set; }

    public int Version { get; set; }

    [CLSCompliant(false)]
    public byte[][]? SignaturePatterns { get; set; }

    [CLSCompliant(false)]
    public byte[][]? SignatureMasks { get; set; }

    // Encoder/Decoder selection APIs

    public static ImageCodecInfo[] GetImageDecoders()
    {
        GdiPlusInitialization.EnsureInitialized();

        ImageCodecInfo[] imageCodecs;
        uint numDecoders;
        uint size;

        PInvokeGdiPlus.GdipGetImageDecodersSize(&numDecoders, &size).ThrowIfFailed();

        using BufferScope<byte> buffer = new((int)size);

        fixed (byte* b = buffer)
        {
            PInvokeGdiPlus.GdipGetImageDecoders(numDecoders, size, (GdiPlus.ImageCodecInfo*)b).ThrowIfFailed();
            imageCodecs = FromNative(new((GdiPlus.ImageCodecInfo*)b, (int)numDecoders));
        }

        return imageCodecs;
    }

    public static ImageCodecInfo[] GetImageEncoders()
    {
        GdiPlusInitialization.EnsureInitialized();

        ImageCodecInfo[] imageCodecs;
        uint numEncoders;
        uint size;

        PInvokeGdiPlus.GdipGetImageEncodersSize(&numEncoders, &size).ThrowIfFailed();

        using BufferScope<byte> buffer = new((int)size);

        fixed (byte* b = buffer)
        {
            PInvokeGdiPlus.GdipGetImageEncoders(numEncoders, size, (GdiPlus.ImageCodecInfo*)b).ThrowIfFailed();
            imageCodecs = FromNative(new((GdiPlus.ImageCodecInfo*)b, (int)numEncoders));
        }

        return imageCodecs;
    }

    private static unsafe ImageCodecInfo[] FromNative(ReadOnlySpan<GdiPlus.ImageCodecInfo> codecInfo)
    {
        ImageCodecInfo[] codecs = new ImageCodecInfo[codecInfo.Length];

        for (int i = 0; i < codecInfo.Length; i++)
        {
            int signatureCount = (int)codecInfo[i].SigCount;

            ImageCodecInfo codec = new()
            {
                Clsid = codecInfo[i].Clsid,
                FormatID = codecInfo[i].FormatID,
                CodecName = codecInfo[i].CodecName.ToString(),
                DllName = codecInfo[i].DllName.ToString(),
                FormatDescription = codecInfo[i].FormatDescription.ToString(),
                FilenameExtension = codecInfo[i].FilenameExtension.ToString(),
                MimeType = codecInfo[i].MimeType.ToString(),
                Flags = (ImageCodecFlags)codecInfo[i].Flags,
                Version = (int)codecInfo[i].Version,

                SignaturePatterns = new byte[signatureCount][],
                SignatureMasks = new byte[signatureCount][]
            };

            for (int j = 0; j < signatureCount; j++)
            {
                codec.SignaturePatterns[j] = new ReadOnlySpan<byte>(codecInfo[i].SigPattern + j * codecInfo[i].SigSize, (int)codecInfo[i].SigSize).ToArray();
                codec.SignatureMasks[j] = new ReadOnlySpan<byte>(codecInfo[i].SigMask + j * codecInfo[i].SigSize, (int)codecInfo[i].SigSize).ToArray();
            }

            codecs[i] = codec;
        }

        return codecs;
    }
}
