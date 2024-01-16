// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using Windows.Win32.System.Com;

namespace System.Windows.Forms;

[Serializable] // This type is participating in resx serialization scenarios.
public sealed class ImageListStreamer : ISerializable, IDisposable
{
    // Compressed magic header. If we see this, the image stream is compressed.
    private static ReadOnlySpan<byte> HeaderMagic => "MSFt"u8;
    private static readonly object s_syncObject = new();

    private readonly ImageList? _imageList;
    private ImageList.NativeImageList? _nativeImageList;

    internal ImageListStreamer(ImageList il) => _imageList = il;

    private ImageListStreamer(SerializationInfo info, StreamingContext context)
    {
        if (info.GetEnumerator() is not { } enumerator)
        {
            return;
        }

        while (enumerator.MoveNext())
        {
            if (!string.Equals(enumerator.Name, "Data", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            try
            {
                if (enumerator.Value is byte[] data)
                {
                    Deserialize(data);
                }
            }
            catch (Exception e)
            {
                Debug.Fail($"ImageList serialization failure: {e}");
                throw;
            }
        }
    }

    internal ImageListStreamer(Stream stream)
    {
        if (stream is MemoryStream memoryStream
            && memoryStream.TryGetBuffer(out ArraySegment<byte> buffer)
            && buffer.Offset == 0
            && buffer.Array is { } array)
        {
            Deserialize(array);
        }
        else
        {
            stream.Position = 0;
            using MemoryStream copyStream = new(checked((int)stream.Length));
            stream.CopyTo(copyStream);
            Deserialize(copyStream.GetBuffer());
        }
    }

    /// <summary>
    ///  Compresses the given input, returning a new array that represents the compressed data.
    /// </summary>
    private static byte[] Compress(ReadOnlySpan<byte>input)
    {
        int finalLength = 0;
        int idx = 0;
        int compressedIdx = 0;

        while (idx < input.Length)
        {
            byte current = input[idx++];
            byte runLength = 1;

            while (idx < input.Length && input[idx] == current && runLength < 0xFF)
            {
                runLength++;
                idx++;
            }

            finalLength += 2;
        }

        byte[] output = new byte[finalLength + HeaderMagic.Length];

        HeaderMagic.CopyTo(output);
        int idxOffset = HeaderMagic.Length;
        idx = 0;

        while (idx < input.Length)
        {
            byte current = input[idx++];
            byte runLength = 1;

            while (idx < input.Length && input[idx] == current && runLength < 0xFF)
            {
                runLength++;
                idx++;
            }

            output[idxOffset + compressedIdx++] = runLength;
            output[idxOffset + compressedIdx++] = current;
        }

        Debug.Assert(idxOffset + compressedIdx == output.Length, "RLE Compression failure in ImageListStreamer -- didn't fill array");

        // Validate that our compression routine works
#if DEBUG
        byte[] debugCompare = Decompress(output);
        Debug.Assert(debugCompare.Length == input.Length, "RLE Compression in ImageListStreamer is broken.");
        int debugMaxCompare = input.Length;
        for (int debugIdx = 0; debugIdx < debugMaxCompare; debugIdx++)
        {
            if (debugCompare[debugIdx] != input[debugIdx])
            {
                Debug.Fail($"RLE Compression failure in ImageListStreamer at byte offset {debugIdx}");
                break;
            }
        }
#endif

        return output;
    }

    /// <summary>
    ///  Decompresses the given input, returning a new array that represents the uncompressed data.
    /// </summary>
    private static byte[] Decompress(byte[] input)
    {
        int finalLength = 0;
        int idx = 0;
        int outputIdx = 0;

        // Check for our header. If we don't have one, we're not actually compressed, so just return the original.
        if (!input.AsSpan().StartsWith(HeaderMagic))
        {
            return input;
        }

        // Ok, we passed the magic header test.

        for (idx = HeaderMagic.Length; idx < input.Length; idx += 2)
        {
            finalLength += input[idx];
        }

        byte[] output = new byte[finalLength];

        idx = HeaderMagic.Length;

        while (idx < input.Length)
        {
            byte runLength = input[idx++];
            byte current = input[idx++];

            int startIdx = outputIdx;
            int endIdx = outputIdx + runLength;

            while (startIdx < endIdx)
            {
                output[startIdx++] = current;
            }

            outputIdx += runLength;
        }

        return output;
    }

    private void Deserialize(byte[] data)
    {
        // We enclose this imagelist handle create in a theming scope.
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

#pragma warning disable CA1725 // Parameter names should match base declaration (previously shipped public API)
    public void GetObjectData(SerializationInfo si, StreamingContext context)
#pragma warning restore CA1725
    {
        using MemoryStream stream = new();
        if (!WriteImageList(stream))
        {
            throw new InvalidOperationException(SR.ImageListStreamerSaveFailed);
        }

        si.AddValue("Data", Compress(stream.GetBuffer().AsSpan(0, (int)stream.Length)));
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
        HIMAGELIST handle = HIMAGELIST.Null;
        if (_imageList is not null)
        {
            handle = (HIMAGELIST)_imageList.Handle;
        }
        else if (_nativeImageList is not null)
        {
            handle = _nativeImageList.HIMAGELIST;
        }

        if (handle.IsNull)
        {
            return false;
        }

        // What we need to do here is use WriteEx if comctl 6 or above, and Write otherwise. However, till we can fix
        // There isn't a reliable way to tell which version of comctl fusion is binding to.
        // So for now, we try to bind to WriteEx, and if that entry point isn't found, we use Write.

        try
        {
            return PInvoke.ImageList.WriteEx(
                new HandleRef<HIMAGELIST>(this, handle),
                IMAGE_LIST_WRITE_STREAM_FLAGS.ILP_DOWNLEVEL,
                new ComManagedStream(stream)).Succeeded;
        }
        catch (EntryPointNotFoundException)
        {
            // WriteEx wasn't found - that's fine - we will use Write.
        }

        return PInvoke.ImageList.Write(new HandleRef<HIMAGELIST>(this, handle), new ComManagedStream(stream));
    }

    /// <summary>
    ///  Disposes the native image list handle.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _nativeImageList?.Dispose();
            _nativeImageList = null;
        }
    }
}
