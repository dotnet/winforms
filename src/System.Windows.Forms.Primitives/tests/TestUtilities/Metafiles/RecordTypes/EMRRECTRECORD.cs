// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
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
}
