// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

/// <summary>
///  Record that just has a single <see cref="RECT"/> value.
/// </summary>
/// <remarks>
///   Not an actual Win32 define, encapsulates:
///
///    - EMRFILLPATH
///    - EMRSTROKEANDFILLPATH
///    - EMRSTROKEPATH
///    - EMREXCLUDECLIPRECT
///    - EMRINTERSECTCLIPRECT
///    - EMRELLIPSE
///    - EMRRECTANGLE
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal struct EMRRECTRECORD
{
    public EMR emr;
    public RECT rect;

    public override string ToString() => $"[EMR{emr.iType}] RECT: {rect}";
}
