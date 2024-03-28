// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

/// <summary>
///  Record that represents a 16 bit Poly record.
/// </summary>
/// <devdoc>
///   Not an actual Win32 define, encapsulates:
///
///   - EMRSETTEXTCOLOR
///   - EMRSETBKCOLOR
/// </devdoc>
[StructLayout(LayoutKind.Sequential)]
internal struct EMRSETCOLOR
{
    public EMR emr;
    public COLORREF crColor;

    public override readonly string ToString()
    {
        return $"[EMR{emr.iType}] Color: {crColor.ToSystemColorString()}";
    }
}
