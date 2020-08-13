// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class ComCtl32
    {
        public static partial class ImageList
        {
            [DllImport(Libraries.Comctl32, ExactSpelling = true, EntryPoint = "ImageList_WriteEx")]
            public static extern HRESULT WriteEx(IntPtr himl, ILP dwFlags, Ole32.IStream pstm);

            public static HRESULT WriteEx(HandleRef himl, ILP dwFlags, Ole32.IStream pstm)
            {
                HRESULT result = WriteEx(himl.Handle, dwFlags, pstm);
                GC.KeepAlive(himl);
                return result;
            }
        }
    }
}
