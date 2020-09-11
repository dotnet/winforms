// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

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

        public bool IsStockObject => (index & 0x80000000) != 0;
        public Gdi32.StockObject StockObject => (Gdi32.StockObject)(index & ~0x80000000);

        public override string ToString()
            => IsStockObject
                ? $"[EMR{emr.iType}] StockObject: {StockObject}"
                : $"[EMR{emr.iType}] Index: {index}";
    }
}
