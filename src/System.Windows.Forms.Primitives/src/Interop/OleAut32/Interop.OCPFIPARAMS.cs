﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        public unsafe struct OCPFIPARAMS
        {
            public uint cbStructSize;
            public IntPtr hwndOwner;
            public int x;
            public int y;
            public char* lpszCaption;
            public uint cObjects;
            public IntPtr ppUnk;
            public uint cPages;
            public IntPtr lpPages;
            public PInvoke.LCID lcid;
            public Ole32.DispatchID dispidInitialProperty;
        }
    }
}
