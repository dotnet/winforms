// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct PICTDESC
        {
            public uint cbSizeofstruct;
            public PICTYPE picType;
            public UnionType Union;

            [StructLayout(LayoutKind.Explicit)]
            public struct UnionType
            {
                [FieldOffset(0)]
                public bmptype bmp;
                [FieldOffset(0)]
                public wmftype wmf;
                [FieldOffset(0)]
                public icontype icon;
                [FieldOffset(0)]
                public emftype emf;

                public struct bmptype
                {
                    public IntPtr hbitmap;
                    public IntPtr hpal;
                }

                public struct wmftype
                {
                    public IntPtr hmeta;
                    public int xExt;
                    public int yExt;
                }

                public struct icontype
                {
                    public IntPtr hicon;
                }

                public struct emftype
                {
                    public IntPtr hemf;
                }
            }

            public static PICTDESC FromBitmap(Bitmap bitmap, IntPtr paletteHandle = default)
            {
                PICTDESC desc = new PICTDESC
                {
                    picType = PICTYPE.BITMAP
                };

                desc.Union.bmp.hbitmap = bitmap.GetHbitmap();
                desc.Union.bmp.hpal = paletteHandle;
                return desc;
            }

            public static PICTDESC FromIcon(Icon icon, bool copy)
            {
                PICTDESC desc = new PICTDESC
                {
                    picType = PICTYPE.ICON
                };

                desc.Union.icon.hicon = copy ?
                    User32.CopyImage(
                        icon.Handle,
                        User32.IMAGE.ICON,
                        icon.Width,
                        icon.Height,
                        User32.LR.DEFAULTCOLOR)
                    : icon.Handle;

                GC.KeepAlive(icon);
                return desc;
            }

            public static PICTDESC FromMetafile(Metafile metafile)
            {
                PICTDESC desc = new PICTDESC
                {
                    picType = PICTYPE.ENHMETAFILE
                };

                desc.Union.emf.hemf = metafile.GetHenhmetafile();
                return desc;
            }
        }
    }
}
