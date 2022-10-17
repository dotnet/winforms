﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Numerics;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct EMRMODIFYWORLDTRANSFORM
    {
        public EMR emr;
        public Matrix3x2 xform;
        public MODIFY_WORLD_TRANSFORM_MODE iMode;

        public override string ToString() => $"[{nameof(EMRMODIFYWORLDTRANSFORM)}] Mode: {iMode} Transform: {xform}";
    }
}
