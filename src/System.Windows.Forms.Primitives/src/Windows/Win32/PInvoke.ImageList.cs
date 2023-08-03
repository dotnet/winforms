// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace Windows.Win32;

internal partial class PInvoke
{
    public static class ImageList
    {
        public static int Add<T>(T himl, HBITMAP hbmImage, HBITMAP hbmMask) where T : IHandle<HIMAGELIST>
        {
            int result = ImageList_Add(himl.Handle, hbmImage, hbmMask);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static bool Destroy<T>(T himl) where T: IHandle<HIMAGELIST>
        {
            bool result = ImageList_Destroy(himl.Handle);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static bool Draw<T>(T himl, int i, HDC hdcDst, int x, int y, IMAGE_LIST_DRAW_STYLE fStyle)
            where T : IHandle<HIMAGELIST>
        {
            BOOL result = ImageList_Draw(himl.Handle, i, hdcDst, x, y, fStyle);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static bool DrawEx<THIML, THDC>(
            THIML himl,
            int i,
            THDC hdcDst,
            int x,
            int y,
            int dx,
            int dy,
            COLORREF rgbBk,
            COLORREF rgbFg,
            IMAGE_LIST_DRAW_STYLE fStyle) where THIML : IHandle<HIMAGELIST> where THDC : IHandle<HDC>
        {
            bool result = ImageList_DrawEx(himl.Handle, i, hdcDst.Handle, x, y, dx, dy, rgbBk, rgbFg, fStyle);
            GC.KeepAlive(himl.Wrapper);
            GC.KeepAlive(hdcDst.Wrapper);
            return result;
        }

        public static unsafe bool GetIconSize<T>(T himl, out int x, out int y) where T : IHandle<HIMAGELIST>
        {
            fixed (int* px = &x)
            fixed (int* py = &y)
            {
                bool result = ImageList_GetIconSize(himl.Handle, px, py);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }
        }

        public static int GetImageCount<T>(T himl) where T : IHandle<HIMAGELIST>
        {
            int result = ImageList_GetImageCount(himl.Handle);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static bool GetImageInfo<T>(T himl, int i, out IMAGEINFO pImageInfo) where T : IHandle<HIMAGELIST>
        {
            bool result = ImageList_GetImageInfo(himl.Handle, i, out pImageInfo);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static bool Remove<T>(T himl, int i) where T : IHandle<HIMAGELIST>
        {
            bool result = ImageList_Remove(himl.Handle, i);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static bool Replace<T>(T himl, int i, HBITMAP hbmImage, HBITMAP hbmMask) where T : IHandle<HIMAGELIST>
        {
            bool result = ImageList_Replace(himl.Handle, i, hbmImage, hbmMask);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static int ReplaceIcon<THIML, THICON>(
            THIML himl,
            int i,
            THICON hicon) where THIML : IHandle<HIMAGELIST> where THICON : IHandle<HICON>
        {
            int result = ImageList_ReplaceIcon(himl.Handle, i, hicon.Handle);
            GC.KeepAlive(himl.Wrapper);
            GC.KeepAlive(hicon.Wrapper);
            return result;
        }

        public static COLORREF SetBkColor<T>(T himl, COLORREF clrBk) where T : IHandle<HIMAGELIST>
        {
            COLORREF result = ImageList_SetBkColor(himl.Handle, clrBk);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static unsafe BOOL Write<T>(T himl, IStream.Interface pstm) where T : IHandle<HIMAGELIST>
        {
            using var stream = ComHelpers.TryGetComScope<IStream>(pstm, out HRESULT hr);
            Debug.Assert(hr.Succeeded);
            BOOL result = ImageList_Write(himl.Handle, stream);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }

        public static unsafe HRESULT WriteEx<T>(
            T himl,
            IMAGE_LIST_WRITE_STREAM_FLAGS dwFlags,
            IStream.Interface pstm) where T : IHandle<HIMAGELIST>
        {
            using var stream = ComHelpers.TryGetComScope<IStream>(pstm, out HRESULT hr);
            Debug.Assert(hr.Succeeded);
            HRESULT result = ImageList_WriteEx(himl.Handle, dwFlags, stream);
            GC.KeepAlive(himl.Wrapper);
            return result;
        }
    }
}
