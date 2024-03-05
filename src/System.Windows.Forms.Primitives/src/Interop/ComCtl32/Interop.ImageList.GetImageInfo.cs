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
            [LibraryImport(Libraries.Comctl32, EntryPoint = "ImageList_GetImageInfo")]
            public static partial BOOL GetImageInfo(IntPtr himl, int i, ref IMAGEINFO pImageInfo);

            public static BOOL GetImageInfo(HandleRef himl, int i, ref IMAGEINFO pImageInfo)
            {
                BOOL result = GetImageInfo(himl.Handle, i, ref pImageInfo);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }
        }
    }
}