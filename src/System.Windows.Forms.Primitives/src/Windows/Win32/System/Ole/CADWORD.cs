// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.System.Ole;

internal partial struct CADWORD
{
    public readonly unsafe uint[] ConvertAndFree()
    {
        try
        {
            return (cElems == 0 || pElems is null)
                ? []
                : new Span<uint>(pElems, (int)cElems).ToArray();
        }
        finally
        {
            Marshal.FreeCoTaskMem((nint)pElems);
        }
    }
}
