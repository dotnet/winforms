// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class ComCtl32
    {
        public static partial class ImageList
        {
            [LibraryImport(Libraries.Comctl32, EntryPoint = "ImageList_Add")]
            public static partial int Add(IntPtr himl, Gdi32.HBITMAP hbmImage, Gdi32.HBITMAP hbmMask);

            public static int Add(IHandle himl, Gdi32.HBITMAP hbmImage, Gdi32.HBITMAP hbmMask)
            {
                int result = Add(himl.Handle, hbmImage, hbmMask);
                GC.KeepAlive(himl);
                return result;
            }
        }
    }
}
