﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Hashing;
using System.Text;

namespace System.Windows.Forms.UITests;

internal class ScreenRecordService : IDisposable
{
    private static readonly Stopwatch s_timer = Stopwatch.StartNew();

    /// <summary>
    ///  Frames captured for the current test. The elapsed time field is a timestamp relative to a fixed but unspecified
    ///  point in time.
    /// </summary>
    /// <remarks>
    ///  <para>Lock on this object before accessing to prevent concurrent accesses.</para>
    /// </remarks>
    private static readonly List<(TimeSpan elapsed, Bitmap image)> s_frames = [];
    private static readonly ArrayPool<byte> s_pool = ArrayPool<byte>.Shared;

    private static ScreenRecordService? s_currentInstance;
    private Form? _form;

    private static readonly ReadOnlyMemory<byte> s_pngHeader = new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G', (byte)'\r', (byte)'\n', 0x1A, (byte)'\n' };

    private static ReadOnlySpan<byte> Ihdr => "IHDR"u8;
    private static ReadOnlySpan<byte> Idat => "IDAT"u8;
    private static ReadOnlySpan<byte> Iend => "IEND"u8;
    private static ReadOnlySpan<byte> Srgb => "sRGB"u8;
    private static ReadOnlySpan<byte> Gama => "gAMA"u8;
    private static ReadOnlySpan<byte> Phys => "pHYs"u8;
    private static ReadOnlySpan<byte> Actl => "acTL"u8;
    private static ReadOnlySpan<byte> Fctl => "fcTL"u8;
    private static ReadOnlySpan<byte> Fdat => "fdAT"u8;

    private enum PngColorType : byte
    {
        Grayscale = 0,
        TrueColor = 2,
        Indexed = 3,
        GrayscaleWithAlpha = 4,
        TrueColorWithAlpha = 6,
    }

    private enum PngCompressionMethod : byte
    {
        /// <summary>
        ///  <see href="https://www.w3.org/TR/png/#dfn-deflate">
        ///   deflate
        ///  </see>
        ///  compression with a sliding window of at most 32768 bytes.
        /// </summary>
        Deflate = 0,
    }

    private enum PngFilterMethod : byte
    {
        /// <summary>
        ///  Adaptive filtering with five basic filter types.
        /// </summary>
        Adaptive = 0,
    }

    private enum PngInterlaceMethod : byte
    {
        /// <summary>
        ///  No interlacing.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Adam7 interlace.
        /// </summary>
        Adam7 = 1,
    }

    private enum ApngDisposeOp : byte
    {
        /// <summary>
        ///  No disposal is done on this frame before rendering the next;
        ///  the contents of the output buffer are left as-is.
        /// </summary>
        None = 0,

        /// <summary>
        ///  The frame's region of the output buffer is to be cleared to
        ///  fully transparent black before rendering the next frame.
        /// </summary>
        Background = 1,

        /// <summary>
        ///  The frame's region of the output buffer is to be reverted
        ///  to the previous contents before rendering the next frame.
        /// </summary>
        Previous = 2,
    }

    private enum ApngBlendOp : byte
    {
        /// <summary>
        ///  All color components of the frame, including alpha,
        ///  overwrite the current contents of the frame's output buffer region.
        /// </summary>
        Source = 0,

        /// <summary>
        ///  The frame should be composited onto the output buffer based on its alpha, using a simple OVER operation as
        ///  described in
        ///  <see href="https://www.w3.org/TR/png/#13Alpha-channel-processing">
        ///   Alpha Channel Processing.
        ///  </see>
        /// </summary>
        Over = 1,
    }

    static ScreenRecordService()
    {
        DataCollectionService.RegisterCustomLogger(
            static fullPath =>
            {
                lock (s_frames)
                {
                    if (s_frames.Count == 0)
                        return;
                }

                // Try to capture an additional frame at the end
                s_currentInstance?.CaptureFrame();

                (TimeSpan elapsed, Bitmap image)[] frames;
                lock (s_frames)
                {
                    if (s_frames.Count < 2)
                    {
                        // No animation available
                        return;
                    }

                    frames = [.. s_frames];
                }

                // Make sure the frames are processed in order of their timestamps
                Array.Sort(frames, (x, y) => x.elapsed.CompareTo(y.elapsed));
                (TimeSpan elapsed, Bitmap image, Size offset)[] croppedFrames = DetectChangedRegions(frames);

                try
                {
                    using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    var crc = new Crc32();
                    byte[] buffer = s_pool.Rent(4096);

                    try
                    {
                        (TimeSpan elapsed, Bitmap image, Size offset) firstFrame = croppedFrames[0];
                        (ReadOnlyMemory<byte> signature, ReadOnlyMemory<byte> ihdr, ImmutableArray<ReadOnlyMemory<byte>> idat, ReadOnlyMemory<byte> iend) firstEncoded = EncodeFrame(firstFrame.image);

                        // PNG Signature (8 bytes)
                        WritePngSignature(fileStream, buffer);

                        // IHDR
                        Write(fileStream, buffer, crc: null, firstEncoded.ihdr.Span);

                        // acTL
                        WriteActl(fileStream, buffer, crc, croppedFrames.Length, playCount: 1);

                        // Write the first frame data as IDAT
                        WriteFctl(fileStream, buffer, crc, sequenceNumber: 0, size: new Size(firstFrame.image.Width, firstFrame.image.Height), offset: firstFrame.offset, delay: TimeSpan.Zero, ApngDisposeOp.None, ApngBlendOp.Source);
                        foreach (ReadOnlyMemory<byte> idat in firstEncoded.idat)
                        {
                            Write(fileStream, buffer, crc: null, idat.Span);
                        }

                        // Write the remaining frames as fDAT
                        int sequenceNumber = 1;
                        for (int i = 1; i < croppedFrames.Length; i++)
                        {
                            TimeSpan elapsed = croppedFrames[i].elapsed - croppedFrames[i - 1].elapsed;
                            WriteFrame(fileStream, buffer, crc, ref sequenceNumber, croppedFrames[i].image, croppedFrames[i].offset, elapsed);
                        }

                        WriteIend(fileStream, buffer, crc);
                    }
                    finally
                    {
                        s_pool.Return(buffer);
                    }
                }
                finally
                {
                    foreach (var (_, image, offset) in croppedFrames)
                    {
                        if (offset != Size.Empty || image.Size != frames[0].image.Size)
                        {
                            // This is a cloned/cropped bitmap that needs to be disposed separately from s_frames
                        }
                    }
                }
            },
            "",
            "apng");
    }

    public ScreenRecordService()
    {
        // Release the previous instance, if any
        s_currentInstance?.UnregisterEvents();
        s_currentInstance = this;

        TimeSpan elapsed = s_timer.Elapsed;
        if (ScreenshotService.TryCaptureFullScreen() is { } image)
        {
            lock (s_frames)
            {
                s_frames.ForEach(frame => frame.image.Dispose());
                s_frames.Clear();
                s_frames.Add((elapsed, image));
            }
        }
        else
        {
            lock (s_frames)
            {
                s_frames.ForEach(frame => frame.image.Dispose());
                s_frames.Clear();
            }
        }
    }

    private static (TimeSpan elapsed, Bitmap image, Size offset)[] DetectChangedRegions((TimeSpan elapsed, Bitmap image)[] frames)
    {
        int width = frames[0].image.Width;
        int height = frames[0].image.Height;
        var bounds = new Rectangle(0, 0, width, height);

        const int BytesPerPixel = 4;

        List<(TimeSpan elapsed, Bitmap image, Size offset)> resultFrames = new(frames.Length);

        var lockedBitmaps = new (Bitmap image, BitmapData data)[2];
        try
        {
            // Even frame indexes go into the first slot of the image buffer. Odd frame indexes go into the second slot.
            lockedBitmaps[0] = (frames[0].image, frames[0].image.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb));
            int stride = lockedBitmaps[0].data.Stride;
            Assert.Equal(0, stride % BytesPerPixel);
            int stridePixels = stride / 4;
            int totalLockedPixels = stridePixels * lockedBitmaps[0].data.Height;

            resultFrames.Add((frames[0].elapsed, frames[0].image, offset: Size.Empty));

            for (int i = 1; i < frames.Length; i++)
            {
                Assert.True(frames[i].image.Width == width);
                Assert.True(frames[i].image.Height == height);

                int previousFrameBufferIndex = (i - 1) % 2;
                int currentFrameBufferIndex = i % 2;

                // Call UnlockBits if the buffer is already holding image data in the slot we are about to overwrite
                lockedBitmaps[currentFrameBufferIndex].image?.UnlockBits(lockedBitmaps[currentFrameBufferIndex].data);

                lockedBitmaps[currentFrameBufferIndex] = (frames[i].image, frames[i].image.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb));
                Assert.Equal(stride, lockedBitmaps[currentFrameBufferIndex].data.Stride);

                ReadOnlySpan<uint> previousImageData;
                ReadOnlySpan<uint> currentImageData;
                unsafe
                {
                    previousImageData = new ReadOnlySpan<uint>((void*)lockedBitmaps[previousFrameBufferIndex].data.Scan0, totalLockedPixels);
                    currentImageData = new ReadOnlySpan<uint>((void*)lockedBitmaps[currentFrameBufferIndex].data.Scan0, totalLockedPixels);
                }

                int firstChangedLine = -1;
                int lastChangedLine = -1;
                int firstChangedColumn = -1;
                int lastChangedColumn = -1;
                for (int line = 0; line < height; line++)
                {
                    ReadOnlySpan<uint> previousFrameLine = previousImageData.Slice(line * stridePixels, width);
                    ReadOnlySpan<uint> currentFrameLine = currentImageData.Slice(line * stridePixels, width);
                    for (int column = 0; column < previousFrameLine.Length; column++)
                    {
                        if (previousFrameLine[column] != currentFrameLine[column])
                        {
                            if (firstChangedLine == -1)
                                firstChangedLine = line;

                            lastChangedLine = line;
                            if (firstChangedColumn == -1 || column < firstChangedColumn)
                                firstChangedColumn = column;
                            lastChangedColumn = Math.Max(lastChangedColumn, column);
                            break;
                        }
                    }

                    for (int column = previousFrameLine.Length - 1; column > lastChangedColumn; column--)
                    {
                        if (previousFrameLine[column] != currentFrameLine[column])
                        {
                            lastChangedColumn = column;
                            break;
                        }
                    }
                }

                if (firstChangedLine == -1)
                {
                    // This image is identical to the previous one and can be skipped
                    continue;
                }
                else if (firstChangedLine == 0 && firstChangedColumn == 0 && lastChangedLine == height - 1 && lastChangedColumn == width - 1)
                {
                    // This image does not need to be cropped
                    resultFrames.Add((frames[i].elapsed, frames[i].image, Size.Empty));
                }
                else
                {
                    var offset = new Size(firstChangedColumn, firstChangedLine);
                    var cropArea = new Rectangle(firstChangedColumn, firstChangedLine, lastChangedColumn - firstChangedColumn + 1, lastChangedLine - firstChangedLine + 1);
                    Bitmap croppedSource = frames[i].image.Clone(cropArea, frames[i].image.PixelFormat);
                    resultFrames.Add((frames[i].elapsed, croppedSource, offset));
                }
            }
        }
        finally
        {
            foreach ((Bitmap image, BitmapData data) in lockedBitmaps)
                image?.UnlockBits(data);
        }

        return [.. resultFrames];
    }

    private static void WritePngSignature(Stream stream, byte[] buffer)
    {
        Write(stream, buffer, crc: null, s_pngHeader.Span);
    }

    private static void WriteIend(Stream stream, byte[] buffer, Crc32 crc)
    {
        crc.Reset();

        WriteChunkHeader(stream, buffer, crc, Iend, dataLength: 0);

        WriteCrc(stream, buffer, crc);
    }

    private static void WriteActl(Stream stream, byte[] buffer, Crc32 crc, int frameCount, int playCount)
    {
        crc.Reset();

        WriteChunkHeader(stream, buffer, crc, Actl, 8);

        // num_frames (4 bytes)
        WritePngUInt32(stream, buffer, crc, checked((uint)frameCount));
        // num_plays (4 bytes)
        WritePngUInt32(stream, buffer, crc, checked((uint)playCount));

        WriteCrc(stream, buffer, crc);
    }

    private static void WriteFrame(Stream stream, byte[] buffer, Crc32 crc, ref int sequenceNumber, Bitmap frame, Size offset, TimeSpan delay)
    {
        WriteFctl(stream, buffer, crc, sequenceNumber++, size: new Size(frame.Width, frame.Height), offset: offset, delay, ApngDisposeOp.None, ApngBlendOp.Source);

        (ReadOnlyMemory<byte> _, ReadOnlyMemory<byte> _, ImmutableArray<ReadOnlyMemory<byte>> idats, ReadOnlyMemory<byte> _) = EncodeFrame(frame);
        foreach (ReadOnlyMemory<byte> idat in idats)
        {
            WriteFdat(stream, buffer, crc, sequenceNumber++, idat.Span[8..^4]);
        }
    }

    private static void WriteFdat(Stream stream, byte[] buffer, Crc32 crc, int sequenceNumber, ReadOnlySpan<byte> data)
    {
        crc.Reset();

        WriteChunkHeader(stream, buffer, crc, Fdat, (uint)(data.Length + 4));

        // fdAT is sequence number followed by IDAT
        WritePngUInt32(stream, buffer, crc, (uint)sequenceNumber);
        Write(stream, buffer, crc, data);

        WriteCrc(stream, buffer, crc);
    }

    private static void WriteChunkHeader(Stream stream, byte[] buffer, Crc32 crc, ReadOnlySpan<byte> chunkType, uint dataLength)
    {
        WriteChunkDataLength(stream, buffer, dataLength);
        Write(stream, buffer, crc, chunkType);
    }

    private static void WriteChunkDataLength(Stream stream, byte[] buffer, uint dataLength)
    {
        WritePngUInt32(stream, buffer, crc: null, dataLength);
    }

    private static void WriteCrc(Stream stream, byte[] buffer, Crc32 crc)
    {
        WritePngUInt32(stream, buffer, crc: null, crc.GetCurrentHashAsUInt32());
    }

    private static (ReadOnlyMemory<byte> signature, ReadOnlyMemory<byte> ihdr, ImmutableArray<ReadOnlyMemory<byte>> idat, ReadOnlyMemory<byte> iend) EncodeFrame(Bitmap frame)
    {
        using var stream = new MemoryStream();

        frame.Save(stream, ImageFormat.Png);

        Memory<byte> memory = stream.GetBuffer().AsMemory()[..(int)stream.Length];
        Memory<byte> signature = memory[..8];
        if (!signature.Span.SequenceEqual(s_pngHeader.Span))
            throw new InvalidOperationException();

        ReadOnlyMemory<byte> ihdr = ReadOnlyMemory<byte>.Empty;
        List<ReadOnlyMemory<byte>> idat = [];
        ReadOnlyMemory<byte> iend = ReadOnlyMemory<byte>.Empty;
        for (Memory<byte> remaining = memory[signature.Length..]; !remaining.IsEmpty; remaining = remaining[GetChunkLength(remaining)..])
        {
            Memory<byte> chunk = remaining[..GetChunkLength(remaining)];
            ReadOnlyMemory<byte> chunkType = GetChunkType(chunk);
            if (chunkType.Span.SequenceEqual(Ihdr))
            {
                Assert.True(ihdr.IsEmpty);
                ihdr = chunk;
            }
            else if (chunkType.Span.SequenceEqual(Srgb)
                || chunkType.Span.SequenceEqual(Gama)
                || chunkType.Span.SequenceEqual(Phys))
            {
                // These are expected chunks in the PNG, but not needed for the final APNG
                continue;
            }
            else if (chunkType.Span.SequenceEqual(Idat))
            {
                idat.Add(chunk);
            }
            else if (chunkType.Span.SequenceEqual(Iend))
            {
                Assert.True(iend.IsEmpty);
                iend = chunk;
            }
            else
            {
                string type = Encoding.ASCII.GetString(chunkType.ToArray());
                throw new NotSupportedException($"Chunk \"{type}\" is not supported.");
            }
        }

        Assert.False(ihdr.IsEmpty);
        Assert.NotEqual(0, idat.Count);
        Assert.False(iend.IsEmpty);

        return (signature, ihdr, idat.ToImmutableArray(), iend);

        static ReadOnlyMemory<byte> GetChunkType(ReadOnlyMemory<byte> chunk)
            => chunk[4..8];

        static int GetDataLength(ReadOnlyMemory<byte> memory)
            => (int)BinaryPrimitives.ReadUInt32BigEndian(memory.Span);

        static int GetChunkLength(ReadOnlyMemory<byte> memory)
        {
            // Total chunk length = length field + chunk type + data length + crc
            return GetDataLength(memory) + 12;
        }
    }

    private static void WriteFctl(Stream stream, byte[] buffer, Crc32 crc, int sequenceNumber, Size size, Size offset, TimeSpan delay, ApngDisposeOp disposeOp, ApngBlendOp blendOp)
    {
        crc.Reset();

        WriteChunkHeader(stream, buffer, crc, Fctl, dataLength: 26);

        // sequence_number (4 bytes)
        WritePngUInt32(stream, buffer, crc, checked((uint)sequenceNumber));
        // width (4 bytes)
        WritePngUInt32(stream, buffer, crc, checked((uint)size.Width));
        // height (4 bytes)
        WritePngUInt32(stream, buffer, crc, checked((uint)size.Height));
        // x_offset (4 bytes)
        WritePngUInt32(stream, buffer, crc, checked((uint)offset.Width));
        // y_offset (4 bytes)
        WritePngUInt32(stream, buffer, crc, checked((uint)offset.Height));

        if (delay.TotalMilliseconds > ushort.MaxValue)
        {
            // Specify delay in 1/30 second (max allowed is a bit over 36 minutes)
            // delay_num (2 bytes)
            WritePngUInt16(stream, buffer, crc, checked((ushort)(delay.TotalMilliseconds / 100)));
            // delay_den (2 bytes)
            WritePngUInt16(stream, buffer, crc, 10);
        }
        else
        {
            // Specify delay in 1/1000 second
            // delay_num (2 bytes)
            WritePngUInt16(stream, buffer, crc, checked((ushort)delay.TotalMilliseconds));
            // delay_den (2 bytes)
            WritePngUInt16(stream, buffer, crc, 1000);
        }

        // dispose_op (1 bytes)
        WritePngByte(stream, crc, (byte)disposeOp);

        // blend_op (1 bytes)
        WritePngByte(stream, crc, (byte)blendOp);

        WriteCrc(stream, buffer, crc);
    }

    private static void WritePngByte(Stream stream, Crc32? crc, byte value)
    {
        Span<byte> buffer = [value];
        crc?.Append(buffer);
        stream.WriteByte(value);
    }

    private static void WritePngUInt16(Stream stream, byte[] buffer, Crc32? crc, ushort value)
    {
        Span<byte> encoded = stackalloc byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16BigEndian(encoded, value);
        Write(stream, buffer, crc, encoded);
    }

    private static void WritePngUInt32(Stream stream, byte[] buffer, Crc32? crc, uint value)
    {
        Span<byte> encoded = stackalloc byte[sizeof(uint)];
        BinaryPrimitives.WriteUInt32BigEndian(encoded, value);
        Write(stream, buffer, crc, encoded);
    }

    private static void Write(Stream stream, byte[] buffer, Crc32? crc, ReadOnlySpan<byte> bytes)
    {
        crc?.Append(bytes);

        if (bytes.Length < buffer.Length)
        {
            bytes.CopyTo(buffer);
            stream.Write(buffer, 0, bytes.Length);
        }
        else
        {
            for (ReadOnlySpan<byte> remaining = bytes; !remaining.IsEmpty; remaining = remaining[Math.Min(buffer.Length, remaining.Length)..])
            {
                ReadOnlySpan<byte> current = remaining[..Math.Min(buffer.Length, remaining.Length)];
                current.CopyTo(buffer);
                stream.Write(buffer, 0, current.Length);
            }
        }
    }

    public void Dispose()
    {
        UnregisterEvents();
        s_currentInstance = null;
    }

    public void RegisterEvents(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        Assert.Null(_form);

        _form = form;
        form.Layout += OnThrottledInteraction;
        form.Paint += OnThrottledInteraction;
        form.MouseEnter += OnThrottledInteraction;
        form.MouseLeave += OnThrottledInteraction;
        form.MouseMove += OnThrottledInteraction;
        form.MouseDown += OnInteraction;
        form.MouseUp += OnInteraction;
        form.KeyDown += OnInteraction;
    }

    public void UnregisterEvents()
    {
        if (_form is { } form)
        {
            form.Layout -= OnThrottledInteraction;
            form.Paint -= OnThrottledInteraction;
            form.MouseEnter -= OnThrottledInteraction;
            form.MouseLeave -= OnThrottledInteraction;
            form.MouseMove -= OnThrottledInteraction;
            form.MouseDown -= OnInteraction;
            form.MouseUp -= OnInteraction;
            form.KeyDown -= OnInteraction;
        }
    }

    private void OnThrottledInteraction(object? sender, EventArgs e)
    {
        // This can be increased to 0.5 seconds if tests are capturing too many images
        var throttle = TimeSpan.Zero;

        lock (s_frames)
        {
            // Avoid taking too many screenshots for layout updates
            if (s_frames.Count > 0 && (s_timer.Elapsed - s_frames[^1].elapsed) < throttle)
                return;
        }

        CaptureFrame();
    }

    private void OnInteraction(object? sender, EventArgs e)
    {
        CaptureFrame();
    }

    public void CaptureFrame()
    {
        TimeSpan elapsed = s_timer.Elapsed;
        if (ScreenshotService.TryCaptureFullScreen() is { } image)
        {
            lock (s_frames)
            {
                s_frames.Add((elapsed, image));
            }
        }
    }
}
