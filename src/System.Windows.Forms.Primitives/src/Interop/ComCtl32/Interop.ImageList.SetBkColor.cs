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
            [LibraryImport(Libraries.Comctl32, EntryPoint = "ImageList_SetBkColor")]
            public static partial int SetBkColor(IntPtr himl, int clrBk);

            public static int SetBkColor(IHandle himl, int clrBk)
            {
                int result = SetBkColor(himl.Handle, clrBk);
                GC.KeepAlive(himl);
                return result;
            }
        }
    }
}
