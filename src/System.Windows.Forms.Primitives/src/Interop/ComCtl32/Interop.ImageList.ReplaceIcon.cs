// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal partial class ComCtl32
    {
        public static partial class ImageList
        {
            [DllImport(Libraries.Comctl32, ExactSpelling = true, EntryPoint = "ImageList_ReplaceIcon")]
            public static extern int ReplaceIcon(IntPtr himl, int i, IntPtr hicon);

            public static int ReplaceIcon(IHandle himl, int i, HandleRef hicon)
            {
                int result = ReplaceIcon(himl.Handle, i, hicon.Handle);
                GC.KeepAlive(himl);
                GC.KeepAlive(hicon.Wrapper);
                return result;
            }
        }
    }
}
