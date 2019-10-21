// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMDATETIMECHANGE
        {
            public User32.NMHDR nmhdr;
            public GDT dwFlags;
            public Kernel32.SYSTEMTIME st;
        }
    }
}
