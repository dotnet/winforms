// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.Primitives.Tests.Interop.Mocks
{
    internal class MockAxHost
    {
        private static Guid ipictureDisp_Guid = typeof(IPictureDisp).GUID;
        private static Guid ipicture_Guid = typeof(IPicture).GUID;

        public MockAxHost(string clsidString)
        {
        }

        public static IPictureDisp GetIPictureDispFromPicture(Image image)
        {
            PICTDESC desc = GetPICTDESCFromPicture(image);
            return (IPictureDisp)OleCreatePictureIndirect(ref desc, ref ipictureDisp_Guid, fOwn: BOOL.TRUE);
        }

        public static IPicture GetIPictureFromCursor(IntPtr cursorHandle)
        {
            PICTDESC desc = PICTDESC.FromIcon(Icon.FromHandle(cursorHandle), copy: true);
            return (IPicture)OleCreatePictureIndirect(ref desc, ref ipicture_Guid, fOwn: BOOL.TRUE);
        }

        public static IPicture GetIPictureFromPicture(Image image)
        {
            PICTDESC desc = GetPICTDESCFromPicture(image);
            return (IPicture)OleCreatePictureIndirect(ref desc, ref ipicture_Guid, fOwn: BOOL.TRUE);
        }

        public static Image? GetPictureFromIPicture(object picture)
        {
            int hPal = default;
            IPicture pict = (IPicture)picture;
            PICTYPE type = (PICTYPE)pict.Type;
            if (type == PICTYPE.BITMAP)
            {
                try
                {
                    hPal = pict.hPal;
                }
                catch (COMException)
                {
                }
            }

            return GetPictureFromParams(pict.Handle, type, hPal, pict.Width, pict.Height);
        }

        public static Image? GetPictureFromIPictureDisp(object picture)
        {
            if (picture is null)
            {
                return null;
            }

            int hPal = default;
            IPictureDisp pict = (IPictureDisp)picture;
            PICTYPE type = (PICTYPE)pict.Type;
            if (type == PICTYPE.BITMAP)
            {
                try
                {
                    hPal = pict.hPal;
                }
                catch (COMException)
                {
                }
            }

            Image? image = GetPictureFromParams(pict.Handle, type, hPal, pict.Width, pict.Height);
            GC.KeepAlive(pict);
            return image;
        }

        private static PICTDESC GetPICTDESCFromPicture(Image image)
        {
            if (image is Bitmap bmp)
            {
                return PICTDESC.FromBitmap(bmp);
            }

            if (image is Metafile mf)
            {
                return PICTDESC.FromMetafile(mf);
            }

            throw new ArgumentException("AXUnknownImage", nameof(image));
        }

        private static Image? GetPictureFromParams(
            int handle,
            PICTYPE type,
            int paletteHandle,
            int width,
            int height)
        {
            switch (type)
            {
                case PICTYPE.ICON:
                    return (Image)Icon.FromHandle((IntPtr)handle).Clone();
                case PICTYPE.METAFILE:
                    WmfPlaceableFileHeader header = new WmfPlaceableFileHeader
                    {
                        BboxRight = (short)width,
                        BboxBottom = (short)height
                    };

                    using (var metafile = new Metafile((IntPtr)handle, header, deleteWmf: false))
                    {
                        return (Image)metafile.Clone();
                    }
                case PICTYPE.ENHMETAFILE:
                    using (var metafile = new Metafile((IntPtr)handle, deleteEmf: false))
                    {
                        return (Image)metafile.Clone();
                    }
                case PICTYPE.BITMAP:
                    return Image.FromHbitmap((IntPtr)handle, (IntPtr)paletteHandle);
                case PICTYPE.NONE:
                    // MSDN says this should not be a valid value, but comctl32 returns it...
                    return null;
                case PICTYPE.UNINITIALIZED:
                    return null;
                default:
                    throw new ArgumentException("AXUnknownImage", nameof(type));
            }
        }
    }
}
