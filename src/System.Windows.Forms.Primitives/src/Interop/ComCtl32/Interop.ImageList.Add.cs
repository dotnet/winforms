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
            [DllImport(Libraries.Comctl32, EntryPoint = "ImageList_Add")]
            public static extern int Add(IntPtr himl, HBITMAP hbmImage, HBITMAP hbmMask);

            public static int Add(IHandle himl, HBITMAP hbmImage, HBITMAP hbmMask)
            {
                int result = Add(himl.Handle, hbmImage, hbmMask);
                GC.KeepAlive(himl);
                return result;
            }
        }
    }
}
