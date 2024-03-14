// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

internal sealed class UnicodeCharBuffer
{
    private readonly char[] _buffer;
    private int _offset;

    public UnicodeCharBuffer(int size)
    {
        _buffer = new char[size];
    }

    public IntPtr AllocCoTaskMem()
    {
        IntPtr result = Marshal.AllocCoTaskMem(_buffer.Length * 2);
        Marshal.Copy(_buffer, 0, result, _buffer.Length);
        return result;
    }

    public string GetString()
    {
        int i = _offset;
        while (i < _buffer.Length && _buffer[i] != 0)
        {
            i++;
        }

        string result = new(_buffer, _offset, i - _offset);
        if (i < _buffer.Length)
        {
            i++;
        }

        _offset = i;
        return result;
    }

    public void PutCoTaskMem(IntPtr ptr)
    {
        Marshal.Copy(ptr, _buffer, 0, _buffer.Length);
        _offset = 0;
    }

    public void PutString(string s)
    {
        int count = Math.Min(s.Length, _buffer.Length - _offset);
        s.CopyTo(0, _buffer, _offset, count);
        _offset += count;
        if (_offset < _buffer.Length)
        {
            _buffer[_offset++] = (char)0;
        }
    }
}
