// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Mshtml
    {
        public struct DOCHOSTUIINFO
        {
            public uint cbSize;
            public Mshtml.DOCHOSTUIFLAG dwFlags;
            public Mshtml.DOCHOSTUIDBLCLK dwDoubleClick;
            public uint dwReserved1;
            public uint dwReserved2;
        }
    }
}
