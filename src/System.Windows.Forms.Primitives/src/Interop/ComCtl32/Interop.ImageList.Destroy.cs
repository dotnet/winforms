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
            [LibraryImport(Libraries.Comctl32, EntryPoint = "ImageList_Destroy")]
            public static partial BOOL Destroy(IntPtr himl);

            public static BOOL Destroy(HandleRef himl)
            {
                BOOL result = Destroy(himl.Handle);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }
        }
    }
}
