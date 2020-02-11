﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        public struct ENDROPFILES
        {
            public User32.NMHDR nmhdr;
            public IntPtr hDrop;
            public int cp;
            public BOOL fProtected;
        }
    }
}
