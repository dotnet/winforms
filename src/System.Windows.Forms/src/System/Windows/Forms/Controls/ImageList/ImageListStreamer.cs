// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO.Compression;
using System.Runtime.Serialization;
using System.Private.Windows.Core.BinaryFormat;
using Windows.Win32.System.Com;

namespace System.Windows.Forms;

[Serializable] // This type is participating in resx serialization scenarios.
public sealed class ImageListStreamer : ISerializable, IDisposable
{
    // Compressed magic header. If we see this, the image stream is compressed.
    private static ReadOnlySpan<byte> HeaderMagic => "MSFt"u8;
    private static readonly Lock s_syncObject = new();

    private readonly ImageList? _imageList;
    private ImageList.NativeImageList? _nativeImageList;

    internal ImageListStreamer(ImageList imageList) => _imageList = imageList;

    // Used by binary serialization
    private ImageListStreamer(SerializationInfo info, StreamingContext context)
    {
        if (info.GetValue<byte[]>("Data") is { } data)
        {
            Deserialize(data);
        }
    }

    internal ImageListStreamer(byte[] data) => Deserialize(data);

    /// <summary>
    ///  Compresses the given input, returning a new array that represents the compressed data.
    /// </summary>
    private static byte[] Compress(ReadOnlySpan<byte> input)
    {
        int length = RunLengthEncoder.GetEncodedLength(input) + HeaderMagic.Length;
        byte[] output = new byte[length];
        SpanWriter<byte> writer = new(output);
        writer.TryWrite(HeaderMagic);
        RunLengthEncoder.TryEncode(input, writer.Span[writer.Position..], out int written);
        Debug.Assert(written == length - HeaderMagic.Length, "RLE compression failure");
        return output;
    }

    /// <summary>
    ///  Decompresses the given input, returning a new array that represents the uncompressed data.
    /// </summary>
    private static byte[] Decompress(byte[] input)
    {
        SpanReader<byte> reader = new(input);
        if (!reader.TryAdvancePast(HeaderMagic))
        {
            // Not compressed, return the original
            return input;
        }

        ReadOnlySpan<byte> remaining = reader.Span[reader.Position..];
        int length = RunLengthEncoder.GetDecodedLength(remaining);
        byte[] output = new byte[length];
        RunLengthEncoder.TryDecode(remaining, output, out int written);
        Debug.Assert(written == length, "RLE decompression failure");
        return output;
    }

    private void Deserialize(byte[] data)
    {
        // We enclose this ImageList handle create in a theming scope.
        using ThemingScope scope = new(Application.UseVisualStyles);
        using MemoryStream memoryStream = new(Decompress(data));

        lock (s_syncObject)
        {
            PInvoke.InitCommonControls();
            _nativeImageList = new ImageList.NativeImageList(new ComManagedStream(memoryStream));
        }

        if (_nativeImageList.HIMAGELIST.IsNull)
        {
            throw new InvalidOperationException(SR.ImageListStreamerLoadFailed);
        }
    }

    public void GetObjectData(SerializationInfo si, StreamingContext context) =>
        si.AddValue("Data", Serialize());

    internal byte[] Serialize()
    {
        using MemoryStream stream = new();
        if (!WriteImageList(stream))
        {
            throw new InvalidOperationException(SR.ImageListStreamerSaveFailed);
        }

        ReadOnlySpan<byte> buffer = stream.GetBuffer().AsSpan()[..(int)stream.Length];
        return Compress(buffer);
    }

    internal void GetObjectData(Stream stream)
    {
        if (!WriteImageList(stream))
        {
            throw new InvalidOperationException(SR.ImageListStreamerSaveFailed);
        }
    }

    internal ImageList.NativeImageList? GetNativeImageList() => _nativeImageList;

    private bool WriteImageList(Stream stream)
    {
        HandleRef<HIMAGELIST> handle = default;
        if (_imageList is not null)
        {
            handle = new(_imageList, (HIMAGELIST)_imageList.Handle);
        }
        else if (_nativeImageList is not null)
        {
            handle = new(_nativeImageList, _nativeImageList.HIMAGELIST);
        }

        if (handle.IsNull)
        {
            return false;
        }

        try
        {
            return PInvoke.ImageList.WriteEx(
                handle,
                IMAGE_LIST_WRITE_STREAM_FLAGS.ILP_DOWNLEVEL,
                stream).Succeeded;
        }
        catch (EntryPointNotFoundException)
        {
            // Not running on ComCtl32 v6, fall back to the old API.
        }

        return PInvoke.ImageList.Write(handle, stream);
    }

    public void Dispose()
    {
        _nativeImageList?.Dispose();
        _nativeImageList = null;
    }
}
