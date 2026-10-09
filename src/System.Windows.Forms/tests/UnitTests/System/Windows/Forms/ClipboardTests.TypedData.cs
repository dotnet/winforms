// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections.Specialized;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class ClipboardTests
{
    // Verifies that SetAudio snapshots the supplied bytes without mutating or retaining the caller's array.
    [WinFormsFact]
    public void Clipboard_SetAudio_ByteArray_DoesNotMutateCallerBuffer()
    {
        byte[] audioBytes = [1, 2, 3, 4];
        byte[] expected = [.. audioBytes];

        Clipboard.SetAudio(audioBytes);

        Assert.Equal(expected, audioBytes);
        audioBytes[0] = 42;

        using Stream result = Assert.IsAssignableFrom<Stream>(Clipboard.GetAudioStream());
        Assert.True(result.CanRead);
        Assert.Equal(0, result.Position);
        Assert.Equal(expected, ReadAllBytes(result));
    }

    // Verifies that SetAudio reads the full stream from its beginning and does not assume ownership of it.
    [WinFormsFact]
    public void Clipboard_SetAudio_Stream_UsesEntireStreamAndLeavesStreamOpen()
    {
        byte[] expected = [1, 2, 3, 4];
        using ChunkedReadStream audioStream = new(expected, maxBytesPerRead: 1);
        audioStream.Position = 2;

        Clipboard.SetAudio(audioStream);

        Assert.True(audioStream.ReadCallCount > 1);
        Assert.True(audioStream.CanRead);
        Assert.Equal(audioStream.Length, audioStream.Position);
        audioStream.Position = 0;
        Assert.Equal(expected[0], audioStream.ReadByte());

        using Stream result = Assert.IsAssignableFrom<Stream>(Clipboard.GetAudioStream());
        Assert.True(result.CanRead);
        Assert.Equal(0, result.Position);
        Assert.Equal(expected, ReadAllBytes(result));
    }

    // Verifies that format presence is independent of payload type and an incompatible audio value reads as null.
    [WinFormsFact]
    public void Clipboard_GetAudioStream_WrongStoredType_ReturnsNull()
    {
        DataObject data = new(DataFormats.WaveAudio, 42);

        Clipboard.SetDataObject(data, copy: false);

        Assert.True(Clipboard.ContainsAudio());
        Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
        Assert.Null(Clipboard.GetAudioStream());
    }

    // Verifies that file-drop data preserves ordering, duplicates, unusual paths, and the caller's collection.
    [WinFormsFact]
    public void Clipboard_SetFileDropList_OrderDuplicatesAndSourceRemainUnchanged()
    {
        StringCollection filePaths =
        [
            "relative.txt",
            " ",
            @"\\server\share\file.txt",
            "relative.txt"
        ];
        string[] expected = [.. filePaths.Cast<string>()];

        Clipboard.SetFileDropList(filePaths);

        Assert.Equal(expected, filePaths.Cast<string>());
        Assert.True(Clipboard.ContainsFileDropList());
        Assert.Equal(expected, Clipboard.GetFileDropList().Cast<string>());
    }

    // Verifies both legacy file-name aliases are converted only by the typed file-drop APIs.
    [WinFormsTheory]
    [InlineData("FileName")]
    [InlineData("FileNameW")]
    public void Clipboard_FileDropList_ConvertibleOnlyFormat_AutoConverts(string storedFormat)
    {
        string[] expected = ["first.txt", "second.txt"];
        DataObject data = new();
        data.SetData(storedFormat, autoConvert: true, expected);

        Assert.True(data.GetDataPresent(DataFormats.FileDrop, autoConvert: true));
        Assert.False(data.GetDataPresent(DataFormats.FileDrop, autoConvert: false));

        Clipboard.SetDataObject(data, copy: false);

        Assert.False(Clipboard.ContainsData(DataFormats.FileDrop));
        Assert.Null(Clipboard.GetData(DataFormats.FileDrop));
        Assert.True(Clipboard.ContainsFileDropList());
        Assert.Equal(expected, Clipboard.GetFileDropList().Cast<string>());
    }

    // Verifies that an incompatible file-drop payload returns a non-null empty collection without hiding its format.
    [WinFormsFact]
    public void Clipboard_GetFileDropList_WrongPayloadType_ReturnsEmptyCollection()
    {
        DataObject data = new(DataFormats.FileDrop, 42);

        Clipboard.SetDataObject(data, copy: false);

        Assert.True(Clipboard.ContainsFileDropList());
        StringCollection result = Clipboard.GetFileDropList();
        Assert.NotNull(result);
        Assert.Empty(result.Cast<string>());
    }

    // Verifies that GetImage returns null for both an empty clipboard and unrelated stored data.
    [WinFormsFact]
    public void Clipboard_GetImage_EmptyOrWrongFormat_ReturnsNull()
    {
        Assert.False(Clipboard.ContainsImage());
        Assert.Null(Clipboard.GetImage());

        Clipboard.SetData("WinForms.ClipboardTests.NotAnImage", 42);

        Assert.False(Clipboard.ContainsImage());
        Assert.Null(Clipboard.GetImage());
    }

    // Verifies the CLR Bitmap alias is converted only by the typed image APIs.
    [WinFormsFact]
    public void Clipboard_Image_ConvertibleOnlyFormat_AutoConverts()
    {
        using Bitmap expected = new(3, 2);
        DataObject data = new();
        data.SetData(typeof(Bitmap).FullName!, autoConvert: true, expected);

        Assert.True(data.GetDataPresent(DataFormats.Bitmap, autoConvert: true));
        Assert.False(data.GetDataPresent(DataFormats.Bitmap, autoConvert: false));

        Clipboard.SetDataObject(data, copy: false);

        Assert.False(Clipboard.ContainsData(DataFormats.Bitmap));
        Assert.Null(Clipboard.GetData(DataFormats.Bitmap));
        Assert.True(Clipboard.ContainsImage());
        Assert.Same(expected, Clipboard.GetImage());
    }

    // Verifies that SetImage does not own the source and that its flushed copy remains valid after source disposal.
    [WinFormsFact]
    public void Clipboard_SetImage_DoesNotDisposeCallerAndFlushedCopySurvivesCallerDispose()
    {
        Bitmap source = new(3, 2);
        try
        {
            source.SetPixel(1, 1, Color.Red);

            Clipboard.SetImage(source);

            Assert.Equal(new Size(3, 2), source.Size);
        }
        finally
        {
            source.Dispose();
        }

        using Image result = Assert.IsAssignableFrom<Image>(Clipboard.GetImage());
        Assert.Equal(new Size(3, 2), result.Size);
        Assert.True(Clipboard.ContainsImage());
    }

    private static byte[] ReadAllBytes(Stream stream)
    {
        using MemoryStream copy = new();
        stream.CopyTo(copy);
        return copy.ToArray();
    }

    private sealed class ChunkedReadStream(byte[] buffer, int maxBytesPerRead) : Stream
    {
        private readonly MemoryStream _innerStream = new(buffer);

        public int ReadCallCount { get; private set; }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;

        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        public override void Flush() => _innerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count)
        {
            ReadCallCount++;

            return _innerStream.Read(buffer, offset, Math.Min(count, maxBytesPerRead));
        }

        public override int Read(Span<byte> buffer)
        {
            ReadCallCount++;

            return _innerStream.Read(buffer[..Math.Min(buffer.Length, maxBytesPerRead)]);
        }

        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

        public override void SetLength(long value) => _innerStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) =>
            _innerStream.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerStream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
