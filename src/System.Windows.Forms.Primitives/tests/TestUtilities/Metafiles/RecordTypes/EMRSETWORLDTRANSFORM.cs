// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;
using System.Numerics;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRSETWORLDTRANSFORM
    {
        public EMR emr;
        public Matrix3x2 xform;

        public override string ToString() => $"[{nameof(EMRSETWORLDTRANSFORM)}] Transform: {xform}";
    }
}
