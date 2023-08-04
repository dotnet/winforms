﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public struct NMDAYSTATE
        {
            public NMHDR nmhdr;
            public SYSTEMTIME stStart;
            public int cDayState;
            public IntPtr prgDayState;
        }
    }
}
