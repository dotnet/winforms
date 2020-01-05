// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum VIEWSTATUS : uint
        {
            OPAQUE = 0x01,
            SOLIDBKGND = 0x02,
            DVASPECTOPAQUE = 0x04,
            DVASPECTTRANSPARENT = 0x08,
            SURFACE = 0x10,
            SURFACE3D = 0x20,
        }
    }
}
