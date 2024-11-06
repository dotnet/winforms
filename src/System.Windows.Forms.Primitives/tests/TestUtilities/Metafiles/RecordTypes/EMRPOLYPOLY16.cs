// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms.Metafiles;

/// <summary>
///  Record that represents a 16 bit Poly record.
/// </summary>
/// <devdoc>
///   Not an actual Win32 define, encapsulates:
///
///  - EMRPOLYPOLYLINE16
///  - EMRPOLYPOLYBEZIER16
///  - EMRPOLYPOLYGON16
///  - EMRPOLYPOLYLINETO16
/// </devdoc>
[StructLayout(LayoutKind.Sequential)]
internal struct EMRPOLYPOLY16
{
    public EMR emr;
    public RECT rclBounds;          // Inclusive-inclusive bounds in device units
    public uint nPolys;             // Number of polys
    public uint cpts;               // Total number of points in all polys
    public uint _aPolyCounts;       // Array of point counts for each poly

    // Can't represent this as a field as it comes nPolys uints after cpts
    // public POINTS apts[1];        // Array of points

    public override string ToString() => ToString(null);

    public string ToString(DeviceContextState? state)
    {
        StringBuilder sb = new(512);
        sb.Append($"[EMR{emr.iType}] Bounds: {rclBounds} Poly count: {nPolys} Total points: {cpts}");

        for (int i = 0; i < nPolys; i++)
        {
            if (state is null)
            {
                sb.Append($"\n\tPoly index {i}: {string.Join(' ', GetPointsForPoly(i).ToArray())}");
            }
            else
            {
                sb.Append($"\n\tPoly index {i}: {string.Join(' ', GetPointsForPoly(i).Transform(p => state.TransformPoint(p)))}");
            }
        }

        return sb.ToString();
    }

    public unsafe ReadOnlySpan<uint> aPolyCounts
    {
        get
        {
            fixed (uint* c = &_aPolyCounts)
            {
                return new(c, checked((int)nPolys));
            }
        }
    }

    public unsafe ReadOnlySpan<POINTS> GetPointsForPoly(int index)
    {
        if (index < 0 || index >= nPolys)
            throw new ArgumentOutOfRangeException(nameof(index));

        int current = 0;
        fixed (void* s = &emr)
        {
            POINTS* currentPoint = (POINTS*)((byte*)s + sizeof(EMRPOLYPOLY16) + (sizeof(uint) * (nPolys - 1)));
            var counts = aPolyCounts;
            while (current != index)
            {
                currentPoint += counts[current];
                current++;
            }

            return new ReadOnlySpan<POINTS>(currentPoint, (int)counts[current]);
        }
    }
}
