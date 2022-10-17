// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Windows.Win32.Foundation;

internal unsafe readonly partial struct BSTR : IDisposable
{
    public BSTR(string value) : this((char*)Marshal.StringToBSTR(value))
    {
    }

    /// <summary>
    ///  Returns the length of the native BSTR.
    /// </summary>
    public unsafe uint Length => Value is null ? 0 : *(((uint*)Value) - 1) / 2;

    public void Dispose()
    {
        Marshal.FreeBSTR((nint)Value);
        fixed (char** c = &Value)
        {
            *c = null;
        }
    }
}
