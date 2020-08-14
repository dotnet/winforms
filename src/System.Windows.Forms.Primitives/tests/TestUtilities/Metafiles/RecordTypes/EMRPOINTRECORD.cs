// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles
{
    /// <summary>
    ///  Record that just has a single point value.
    /// </summary>
    /// <remarks>
    ///   Not an actual Win32 define, encapsulates:
    ///
    ///    - EMRLINETO
    ///    - EMRMOVETOEX
    ///    - EMROFFSETCLIPRGN
    ///    - EMRSETVIEWPORTORGEX
    ///    - EMRSETWINDOWORGEX
    ///    - EMRSETBRUSHORGEX
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRPOINTRECORD
    {
        public EMR emr;
        public Point point;

        public override string ToString() => $"[EMR{emr.iType}] Point: {point}";
    }
}
