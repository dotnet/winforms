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
    ///  - EMRPOLYLINE16
    ///  - EMRPOLYBEZIER16
    ///  - EMRPOLYGON16
    ///  - EMRPOLYBEZIERTO16
    ///  - EMRPOLYLINETO16
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRPOLY16
    {
        public EMR emr;
        public RECT rclBounds;          // Inclusive-inclusive bounds in device units
        public uint cpts;
        public POINTS apts;

        public override string ToString()
        {
            return $"[EMR{emr.iType}] Bounds: {rclBounds} Points: {cpts}";
        }
    }
}
