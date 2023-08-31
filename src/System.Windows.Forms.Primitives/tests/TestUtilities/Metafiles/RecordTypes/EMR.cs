// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct EMR
{
    public ENHANCED_METAFILE_RECORD_TYPE iType;
    public uint nSize;
}
