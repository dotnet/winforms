// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    public ref struct BSTR
    {
        private IntPtr _bstr;

        public BSTR(string value)
        {
            _bstr = Marshal.StringToBSTR(value);
        }

        /// <summary>
        ///  Returns the length of the native BSTR.
        /// </summary>
        public unsafe uint Length
            => _bstr == IntPtr.Zero ? 0 :  *(((uint*)_bstr) - 1) / 2;

        public unsafe ReadOnlySpan<char> String
            => _bstr == IntPtr.Zero
                ? ReadOnlySpan<char>.Empty
                : new ReadOnlySpan<char>((void*)_bstr, checked((int)Length));
        public void Dispose()
        {
            Marshal.FreeBSTR(_bstr);
            _bstr = IntPtr.Zero;
        }
    }
}
