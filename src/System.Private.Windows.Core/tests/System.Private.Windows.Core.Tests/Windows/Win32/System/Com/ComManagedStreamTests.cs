// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

    private class TestStream : MemoryStream
    {
        private readonly bool _canSeek;

        public override bool CanSeek => _canSeek;

        public TestStream(bool canSeek, int numBytes) : base(new byte[numBytes])
        {
            _canSeek = canSeek;
        }
    }
}
