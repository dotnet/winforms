// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Windows.Forms;

internal unsafe class DataStreamFromComStream : Stream
{
    private IStream* _comStream;

    /// <summary>
    ///  Initializes a new instance that does not take ownership of <paramref name="comStream"/>.
    /// </summary>
    public DataStreamFromComStream(IStream* comStream) : base()
    {
        _comStream = comStream;
    }

    public override long Position
    {
        get => Seek(0, SeekOrigin.Current);
        set => Seek(value, SeekOrigin.Begin);
    }

    public override bool CanWrite => true;

    public override bool CanSeek => true;

    public override bool CanRead => true;

    public override long Length
    {
        get
        {
            long curPos = Position;
            long endPos = Seek(0, SeekOrigin.End);
            Position = curPos;
            return endPos - curPos;
        }
    }

    public override void Flush()
    {
    }

    /// <summary>
    ///  Read the data into the given buffer
    /// </summary>
    /// <param name="buffer">The buffer receiving the data</param>
    /// <param name="index">The offset from the beginning of the buffer</param>
    /// <param name="count">The number of bytes to read</param>
    /// <returns>The number of bytes read</returns>
    public override int Read(byte[] buffer, int index, int count)
    {
        int bytesRead = 0;
        if (count > 0 && index >= 0 && (count + index) <= buffer.Length)
        {
            Span<byte> span = new(buffer, index, count);
            bytesRead = Read(span);
        }

        return bytesRead;
    }

    /// <summary>
    ///  Read the data into the given buffer
    /// </summary>
    /// <param name="buffer">The buffer receiving the data</param>
    /// <returns>The number of bytes read</returns>
    public override int Read(Span<byte> buffer)
    {
        uint bytesRead = 0;
        if (!buffer.IsEmpty)
        {
            fixed (byte* ch = &buffer[0])
            {
                _comStream->Read(ch, (uint)buffer.Length, &bytesRead);
            }
        }

        return (int)bytesRead;
    }

    public override int ReadByte()
    {
        byte data = default;
        int r = Read(new Span<byte>(ref data));
        return r == 0 ? -1 : data;
    }

    public override void SetLength(long value)
    {
        _comStream->SetSize((ulong)value);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ulong newPosition = 0;
        _comStream->Seek(offset, origin, &newPosition);
        return (long)newPosition;
    }

    /// <summary>
    ///  Writes the data contained in the given buffer
    /// </summary>
    /// <param name="buffer">The buffer containing the data to write</param>
    /// <param name="index">The offset from the beginning of the buffer</param>
    /// <param name="count">The number of bytes to write</param>
    public override void Write(byte[] buffer, int index, int count)
    {
        if (count <= 0)
        {
            return;
        }

        if (count > 0 && index >= 0 && (count + index) <= buffer.Length)
        {
            ReadOnlySpan<byte> span = new(buffer, index, count);
            Write(span);
            return;
        }

        throw new IOException(SR.DataStreamWrite);
    }

    /// <summary>
    ///  Writes the data contained in the given buffer
    /// </summary>
    /// <param name="buffer">The buffer to write</param>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (buffer.IsEmpty)
        {
            return;
        }

        uint bytesWritten = 0;
        fixed (byte* b = &buffer[0])
        {
            _comStream->Write(b, (uint)buffer.Length, &bytesWritten);
        }

        if (bytesWritten < buffer.Length)
        {
            throw new IOException(SR.DataStreamWrite);
        }
    }

    public override void WriteByte(byte value) => Write([value]);

    protected override void Dispose(bool disposing)
    {
        if (disposing && _comStream is not null)
        {
            _comStream->Commit((uint)STGC.STGC_DEFAULT);
        }

        _comStream = null;
        base.Dispose(disposing);
    }
}
