// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct EMRRESTOREDC
{
    public EMR emr;
    public int iRelative;

    public override readonly string ToString() => $"[{nameof(EMRRESTOREDC)}] Index: {iRelative}";
}
