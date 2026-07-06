// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Foundation;

namespace Windows.Win32.System.Com.Tests;

public class ComManagedStreamTests
{
    [Fact]
    public void Ctor_NonSeekableStream_WrapsWithSeekableStreamAtPositionZero()
    {
        using TestStream nonSeekableStream = new(canSeek: false, numBytes: 4);
        ComManagedStream comManagedStream = new(nonSeekableStream, makeSeekable: true);
        comManagedStream.GetDataStream().CanSeek.Should().Be(true);
        comManagedStream.GetDataStream().Position.Should().Be(0);
    }

    [Fact]
    public void Ctor_SeekableStream_UsesOriginalStream()
    {
        using TestStream seekableStream = new(canSeek: true, numBytes: 4);
        ComManagedStream comManagedStream = new(seekableStream, makeSeekable: true);
        comManagedStream.GetDataStream().Should().BeSameAs(seekableStream);
    }

    [Fact]
    public unsafe void Read_StreamReturnsShortReads_FillsBuffer()
    {
        // Streams are allowed to return fewer bytes than requested. Simulate a chunked/network stream
        // and verify the wrapper keeps reading until the requested buffer is filled. See issue #14064.
        byte[] sourceBytes = new byte[1024];
        for (int index = 0; index < sourceBytes.Length; index++)
        {
            sourceBytes[index] = (byte)index;
        }

        using ChunkingStream shortReadStream = new(sourceBytes, chunkSize: 100);
        ComManagedStream comManagedStream = new(shortReadStream);

        byte[] destinationBuffer = new byte[sourceBytes.Length];
        uint bytesReadCount;
        fixed (byte* destinationPointer = destinationBuffer)
        {
            ((IStream.Interface)comManagedStream).Read(destinationPointer, (uint)destinationBuffer.Length, &bytesReadCount).Should().Be(HRESULT.S_OK);
        }

        bytesReadCount.Should().Be((uint)sourceBytes.Length);
        destinationBuffer.Should().Equal(sourceBytes);
    }

    [Fact]
    public unsafe void Read_RequestPastEndOfStream_ReturnsOnlyAvailableBytes()
    {
        // When more bytes are requested than remain, only the available bytes are returned (no throw on EOF).
        byte[] sourceBytes = new byte[50];
        for (int index = 0; index < sourceBytes.Length; index++)
        {
            sourceBytes[index] = (byte)index;
        }

        using ChunkingStream shortReadStream = new(sourceBytes, chunkSize: 10);
        ComManagedStream comManagedStream = new(shortReadStream);

        byte[] destinationBuffer = new byte[sourceBytes.Length * 2];
        uint bytesReadCount;
        fixed (byte* destinationPointer = destinationBuffer)
        {
            ((IStream.Interface)comManagedStream).Read(destinationPointer, (uint)destinationBuffer.Length, &bytesReadCount).Should().Be(HRESULT.S_OK);
        }

        bytesReadCount.Should().Be((uint)sourceBytes.Length);
    }

    private class TestStream : MemoryStream
    {
        private readonly bool _canSeek;

        public override bool CanSeek => _canSeek;

        public TestStream(bool canSeek, int numBytes) : base(new byte[numBytes])
        {
            _canSeek = canSeek;
        }
    }

    // Seekable stream that never returns more than a fixed chunk per Read, simulating chunked/network streams.
    // Derives from Stream (not MemoryStream) so the span-based Read routes through this Read override on all targets.
    private sealed class ChunkingStream : Stream
    {
        private readonly byte[] _sourceData;
        private readonly int _maxBytesPerRead;
        private int _position;

        public ChunkingStream(byte[] sourceData, int chunkSize)
        {
            _sourceData = sourceData;
            _maxBytesPerRead = chunkSize;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => _sourceData.Length;

        public override long Position
        {
            get => _position;
            set => _position = (int)value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesToRead = Math.Min(Math.Min(count, _maxBytesPerRead), _sourceData.Length - _position);
            if (bytesToRead <= 0)
            {
                return 0;
            }

            Array.Copy(_sourceData, _position, buffer, offset, bytesToRead);
            _position += bytesToRead;
            return bytesToRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _position = origin switch
            {
                SeekOrigin.Begin => (int)offset,
                SeekOrigin.Current => _position + (int)offset,
                SeekOrigin.End => _sourceData.Length + (int)offset,
                _ => _position,
            };

            return _position;
        }

        public override void Flush() { }
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
