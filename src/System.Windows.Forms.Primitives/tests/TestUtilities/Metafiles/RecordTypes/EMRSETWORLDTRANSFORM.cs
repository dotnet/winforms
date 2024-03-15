// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Numerics;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct EMRSETWORLDTRANSFORM
{
    public EMR emr;
    public Matrix3x2 xform;

    public override readonly string ToString() => $"[{nameof(EMRSETWORLDTRANSFORM)}] Transform: {xform}";
}
