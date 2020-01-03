// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMLVODSTATECHANGE
        {
            public User32.NMHDR hdr;
            public int iFrom;
            public int iTo;
            public ComCtl32.LVIS uNewState;
            public ComCtl32.LVIS uOldState;
        }
    }
}
