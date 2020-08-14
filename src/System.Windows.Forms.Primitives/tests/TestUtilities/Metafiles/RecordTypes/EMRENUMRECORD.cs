// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles
{
    /// <summary>
    ///  Record that has just a single enum value.
    /// </summary>
    /// <remarks>
    ///   Not an actual Win32 define, encapsulates:
    ///
    ///   - EMRSELECTCLIPPATH
    ///   - EMRSETBKMODE
    ///   - EMRSETMAPMODE
    ///   - EMRSETLAYOUT
    ///   - EMRSETPOLYFILLMODE
    ///   - EMRSETROP2
    ///   - EMRSETSTRETCHBLTMODE
    ///   - EMRSETTEXTALIGN
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRENUMRECORD<T> where T : Enum
    {
        public EMR emr;
        public T iMode;

        public override string ToString() => $"[EMR{emr.iType}] Mode: {typeof(T).Name}_{iMode}";
    }
}
