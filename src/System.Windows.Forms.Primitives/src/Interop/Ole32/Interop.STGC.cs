// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum STGC : uint
        {
            STGC_DEFAULT = 0x0,
            STGC_OVERWRITE = 0x1,
            STGC_ONLYIFCURRENT = 0x2,
            STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 0x4,
            STGC_CONSOLIDATE = 0x8
        }
    }
}
