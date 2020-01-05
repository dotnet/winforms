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
            [DllImport(Libraries.Comctl32, ExactSpelling = true, EntryPoint = "ImageList_Remove")]
            public static extern BOOL Remove(IntPtr himl, int i);

            public static BOOL Remove(IHandle himl, int i)
            {
                BOOL result = Remove(himl.Handle, i);
                GC.KeepAlive(himl);
                return result;
            }
        }
    }
}
