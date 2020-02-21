// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern IntPtr CopyImage(IntPtr hImage, IMAGE type, int cx, int cy, LR flags);

        public static IntPtr CopyImage(IHandle hImage, IMAGE type, int cx, int cy, LR flags)
        {
            IntPtr result = CopyImage(hImage.Handle, type, cx, cy, flags);
            GC.KeepAlive(hImage);
            return result;
        }

        public enum IMAGE : uint
        {
            BITMAP       = 0,
            ICON         = 1,
            CURSOR       = 2,
            ENHMETAFILE  = 3
        }

        [Flags]
        public enum LR : uint
        {
            DEFAULTCOLOR     = 0x00000000,
            MONOCHROME       = 0x00000001,
            COLOR            = 0x00000002,
            COPYRETURNORG    = 0x00000004,
            COPYDELETEORG    = 0x00000008,
            LOADFROMFILE     = 0x00000010,
            LOADTRANSPARENT  = 0x00000020,
            DEFAULTSIZE      = 0x00000040,
            VGACOLOR         = 0x00000080,
            LOADMAP3DCOLORS  = 0x00001000,
            CREATEDIBSECTION = 0x00002000,
            COPYFROMRESOURCE = 0x00004000,
            SHARED           = 0x00008000
        }
    }
}
