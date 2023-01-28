// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace Windows.Win32.Foundation;

internal readonly unsafe partial struct BSTR : IDisposable
{
    public BSTR(string value) : this((char*)Marshal.StringToBSTR(value))
    {
    }

    public void Dispose()
    {
        Marshal.FreeBSTR((nint)Value);
        fixed (char** c = &Value)
        {
            *c = null;
        }
    }

    /// <summary>
    ///  Converts the <see cref="BSTR"/> to string and frees it.
    /// </summary>
    public readonly string ToStringAndFree()
    {
        string result = ToString();
        Dispose();
        return result;
    }

    public bool IsNull => Value is null;
}
