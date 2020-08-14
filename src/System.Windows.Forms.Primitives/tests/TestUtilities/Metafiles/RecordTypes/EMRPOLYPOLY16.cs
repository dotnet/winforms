// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    /// <summary>
    ///  Record that represents a 16 bit Poly record.
    /// </summary>
    /// <remarks>
    ///   Not an actual Win32 define, encapsulates:
    ///
    ///  - EMRPOLYPOLYLINE16
    ///  - EMRPOLYPOLYBEZIER16
    ///  - EMRPOLYPOLYGON16
    ///  - EMRPOLYPOLYBEZIERTO16
    ///  - EMRPOLYPOLYLINETO16
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRPOLYPOLY16
    {
        public EMR emr;
        public RECT rclBounds;          // Inclusive-inclusive bounds in device units
        public uint nPolys;             // Number of polys
        public uint cpts;               // Total number of points in all polys
        public uint aPolyCounts;        // Array of point counts for each poly

        // Can't represent this as it comes nPolys uints after cpts
        //public POINTS apts[1];        // Array of points

        public override string ToString()
        {
            return $"[EMR{emr.iType}] Bounds: {rclBounds} Polys: {nPolys} Points: {cpts}";
        }
    }
}
