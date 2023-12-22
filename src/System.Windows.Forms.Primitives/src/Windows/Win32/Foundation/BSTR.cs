// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.Win32.Foundation;

internal readonly unsafe partial struct BSTR : IDisposable
{
    // Use Marshal here to allocate/free as that is cross-plat which can come into play with our ComNativeDescriptor.

    public BSTR(string? value) : this((char*)Marshal.StringToBSTR(value))
    {
    }

    public void Dispose()
    {
        if (Value is not null)
        {
            Marshal.FreeBSTR((nint)Value);
            Unsafe.AsRef(in this) = default;
        }
    }

    /// <summary>
    ///  Converts the <see cref="BSTR"/> to string and frees it.
    /// </summary>
    public readonly string ToStringAndFree()
    {
        string result = ToString() ?? string.Empty;
        Dispose();
        return result;
    }

    /// <summary>
    ///  Converts the <see cref="BSTR"/> to a nullable string and frees it.
    /// </summary>
    public readonly string? ToNullableStringAndFree()
    {
        string? result = ToString();
        Dispose();
        return result;
    }

    public bool IsNull => Value is null;

    public bool IsNullOrEmpty => Length == 0;
}
