// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct LVINSERTMARK
        {
            public uint cbSize;
            public LVIM dwFlags;
            public int iItem;
            public uint dwReserved;
        }
    }
}
