﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal partial class Interop
{
    internal static partial class Richedit
    {
        public delegate int EDITSTREAMCALLBACK(IntPtr dwCookie, IntPtr pbBuff, int cb, out int pcb);
    }
}
