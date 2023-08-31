// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.Foundation;

internal readonly unsafe partial struct PWSTR
{
    public string? ToStringAndCoTaskMemFree()
    {
        if (Value is null)
        {
            return null;
        }

        string value = new(Value);
        Marshal.FreeCoTaskMem((IntPtr)Value);
        return value;
    }

    /// <summary>
    ///  The length of the string when it is a null separated list of values that is terminated by
    ///  a double null. Does not include the final double null.
    /// </summary>
    internal int StringListLength
    {
        get
        {
            char* p = Value;
            if (p is null)
            {
                return 0;
            }

            while (!(*p == '\0' && *(p + 1) == '\0'))
            {
                p++;
            }

            return checked((int)(p - Value));
        }
    }

    public bool IsNull => Value is null;
}
