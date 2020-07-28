// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public struct ENHMETARECORD
        {
            public EMR iType;
            public uint nSize;
            public uint _dParm;

            public ReadOnlySpan<uint> dParm => TrailingArray<uint>.GetBuffer(ref _dParm, nSize / 4);
        }
    }
}
