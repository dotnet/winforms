// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

/// <summary>
///  Record that represents a 16 bit Poly record.
/// </summary>
/// <devdoc>
///   Not an actual Win32 define, encapsulates:
///
///  - EMRPOLYLINE16
///  - EMRPOLYBEZIER16
///  - EMRPOLYGON16
///  - EMRPOLYBEZIERTO16
///  - EMRPOLYLINETO16
/// </devdoc>
[StructLayout(LayoutKind.Sequential)]
internal struct EMRPOLY16
{
    public EMR emr;
    public RECT rclBounds;          // Inclusive-inclusive bounds in device units
    public uint cpts;
    private POINTS _apts;

    public unsafe ReadOnlySpan<POINTS> points
    {
        get
        {
            fixed (POINTS* p = &_apts)
            {
                return new(p, checked((int)cpts));
            }
        }
    }

    public override string ToString() => $"[EMR{emr.iType}] Bounds: {rclBounds} Points: {string.Join(' ', points.ToArray())}";

    public string ToString(DeviceContextState state)
    {
        Point[] transformedPoints = points.Transform(point => state.TransformPoint(point));
        return $"[EMR{emr.iType}] Bounds: {rclBounds} Points: {string.Join(' ', transformedPoints)}";
    }
}
