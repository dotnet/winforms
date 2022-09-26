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
            [DllImport(Libraries.Comctl32, EntryPoint = "ImageList_GetIconSize")]
            public static extern BOOL GetIconSize(IntPtr himl, out int x, out int y);

            public static BOOL GetIconSize(HandleRef himl, out int x, out int y)
            {
                BOOL result = GetIconSize(himl.Handle, out x, out y);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }
        }
    }
}
