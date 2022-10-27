// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Primitives.Tests.Interop.Mocks
{
    internal unsafe class MockAxHost
    {
        public MockAxHost(string clsidString)
        {
        }

        public static IPictureDisp.Interface GetIPictureDispFromPicture(Image image)
        {
            PICTDESC desc = GetPICTDESCFromPicture(image);
            using ComScope<IPictureDisp> picture = new(null);
            PInvoke.OleCreatePictureIndirect(&desc, IPictureDisp.NativeGuid, fOwn: true, picture).ThrowOnFailure();
            return (IPictureDisp.Interface)Marshal.GetObjectForIUnknown(picture);
        }

        public static IPicture.Interface GetIPictureFromCursor(IntPtr cursorHandle)
        {
            PICTDESC desc = PICTDESC.FromIcon(Icon.FromHandle(cursorHandle), copy: true);
            using ComScope<IPicture> picture = new(null);
            PInvoke.OleCreatePictureIndirect(&desc, IPicture.NativeGuid, fOwn: true, picture).ThrowOnFailure();
            return (IPicture.Interface)Marshal.GetObjectForIUnknown(picture);
        }

        public static IPicture.Interface GetIPictureFromPicture(Image image)
        {
            PICTDESC desc = GetPICTDESCFromPicture(image);
            using ComScope<IPicture> picture = new(null);
            PInvoke.OleCreatePictureIndirect(&desc, IPicture.NativeGuid, fOwn: true, picture).ThrowOnFailure();
            return (IPicture.Interface)Marshal.GetObjectForIUnknown(picture);
        }

        public static Image? GetPictureFromIPicture(object picture)
        {
            uint hPal = default;
            IPicture.Interface pict = (IPicture.Interface)picture;
            pict.get_Type(out short type).ThrowOnFailure();
            if (type == (short)PICTYPE.PICTYPE_BITMAP)
            {
                pict.get_hPal(&hPal);
            }

            pict.get_Handle(out uint handle).ThrowOnFailure();
            pict.get_Width(out int width).ThrowOnFailure();
            pict.get_Height(out int height).ThrowOnFailure();

            return GetPictureFromParams(handle, (PICTYPE)type, hPal, (uint)width, (uint)height);
        }

        public static Image? GetPictureFromIPictureDisp(object picture)
        {
            if (picture is null)
            {
                return null;
            }

            uint hPal = default;
            using var pict = ComHelpers.QueryInterface<IDispatch>(picture, out HRESULT hr);
            hr.ThrowOnFailure();
            using VARIANT variant = new();
            ComHelpers.GetDispatchProperty(pict, PInvoke.DISPID_PICT_TYPE, &variant).ThrowOnFailure();
            PICTYPE type = (PICTYPE)variant.data.iVal;
            if (type == PICTYPE.PICTYPE_BITMAP)
            {
                ComHelpers.GetDispatchProperty(pict, PInvoke.DISPID_PICT_HPAL, &variant).ThrowOnFailure();
                hPal = variant.data.uintVal;
            }

            ComHelpers.GetDispatchProperty(pict, PInvoke.DISPID_PICT_HANDLE, &variant).ThrowOnFailure();
            uint handle = variant.data.uintVal;

            ComHelpers.GetDispatchProperty(pict, PInvoke.DISPID_PICT_WIDTH, &variant).ThrowOnFailure();
            uint width = variant.data.uintVal;

            ComHelpers.GetDispatchProperty(pict, PInvoke.DISPID_PICT_HEIGHT, &variant).ThrowOnFailure();
            uint height = variant.data.uintVal;

            return GetPictureFromParams(handle, type, hPal, width, height);
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
            uint handle,
            PICTYPE type,
            uint paletteHandle,
            uint width,
            uint height)
        {
            int extendedHandle = (int)handle;
            switch (type)
            {
                case PICTYPE.PICTYPE_ICON:
                    return (Image)Icon.FromHandle(extendedHandle).Clone();
                case PICTYPE.PICTYPE_METAFILE:
                    WmfPlaceableFileHeader header = new WmfPlaceableFileHeader
                    {
                        BboxRight = (short)width,
                        BboxBottom = (short)height
                    };

                    using (var metafile = new Metafile(extendedHandle, header, deleteWmf: false))
                    {
                        return (Image)metafile.Clone();
                    }

                case PICTYPE.PICTYPE_ENHMETAFILE:
                    using (var metafile = new Metafile(extendedHandle, deleteEmf: false))
                    {
                        return (Image)metafile.Clone();
                    }

                case PICTYPE.PICTYPE_BITMAP:
                    return Image.FromHbitmap(extendedHandle, (int)paletteHandle);
                case PICTYPE.PICTYPE_NONE:
                    // MSDN says this should not be a valid value, but comctl32 returns it...
                    return null;
                case PICTYPE.PICTYPE_UNINITIALIZED:
                    return null;
                default:
                    throw new ArgumentException("AXUnknownImage", nameof(type));
            }
        }
    }
}
