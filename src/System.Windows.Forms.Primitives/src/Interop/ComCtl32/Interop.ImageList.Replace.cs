﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class ComCtl32
    {
        public static partial class ImageList
        {
            [LibraryImport(Libraries.Comctl32, EntryPoint = "ImageList_Replace")]
            public static partial BOOL Replace(IntPtr himl, int i, IntPtr hbmImage, IntPtr hbmMask);

            public static BOOL Replace(IHandle himl, int i, IntPtr hbmImage, IntPtr hbmMask)
            {
                BOOL result = Replace(himl.Handle, i, hbmImage, hbmMask);
                GC.KeepAlive(himl);
                return result;
            }
        }
    }
}
