// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    /// <summary>
    ///  Record that represents an index.
    /// </summary>
    /// <remarks>
    ///   Not an actual Win32 define, encapsulates:
    ///
    ///   - EMRSELECTOBJECT
    ///   - EMRDELETEOBJECT
    ///   - EMRSELECTPALETTE
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRINDEXRECORD
    {
        public EMR emr;
        public uint index;

        public override string ToString()
            => (index & 0x80000000) != 0
                ? $"[EMR{emr.iType}] StockObject: {(User32.StockObject)index}"
                : $"[EMR{emr.iType}] Index: {index}";
    }
}
