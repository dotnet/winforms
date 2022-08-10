﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal partial class Comdlg32
    {
        public struct OFNOTIFYW
        {
            public NMHDR hdr;
            public IntPtr lpOFN;
            public IntPtr pszFile;
        }
    }
}
