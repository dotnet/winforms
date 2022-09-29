// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal partial class PInvoke
    {
        public static class ImageList
        {
            public static int Add<T>(in T himl, HBITMAP hbmImage, HBITMAP hbmMask) where T : IHandle<HIMAGELIST>
            {
                int result = ImageList_Add(himl.Handle, hbmImage, hbmMask);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }

            public static bool Destroy<T>(in T himl) where T: IHandle<HIMAGELIST>
            {
                bool result = ImageList_Destroy(himl.Handle);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }

            public static bool Draw<T>(in T himl, int i, HDC hdcDst, int x, int y, IMAGE_LIST_DRAW_STYLE fStyle)
                where T : IHandle<HIMAGELIST>
            {
                BOOL result = ImageList_Draw(himl.Handle, i, hdcDst, x, y, fStyle);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }

            public static bool DrawEx<THIML, THDC>(
                in THIML himl,
                int i,
                in THDC hdcDst,
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

            public static unsafe bool GetIconSize<T>(in T himl, out int x, out int y) where T : IHandle<HIMAGELIST>
            {
                fixed (int* px = &x)
                fixed (int* py = &y)
                {
                    bool result = ImageList_GetIconSize(himl.Handle, px, py);
                    GC.KeepAlive(himl.Wrapper);
                    return result;
                }
            }

            public static int GetImageCount<T>(in T himl) where T : IHandle<HIMAGELIST>
            {
                int result = ImageList_GetImageCount(himl.Handle);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }

            public static bool GetImageInfo<T>(in T himl, int i, out IMAGEINFO pImageInfo) where T : IHandle<HIMAGELIST>
            {
                bool result = ImageList_GetImageInfo(himl.Handle, i, out pImageInfo);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }

            public static bool Remove<T>(in T himl, int i) where T : IHandle<HIMAGELIST>
            {
                bool result = ImageList_Remove(himl.Handle, i);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }

            public static bool Replace<T>(in T himl, int i, HBITMAP hbmImage, HBITMAP hbmMask) where T : IHandle<HIMAGELIST>
            {
                bool result = ImageList_Replace(himl.Handle, i, hbmImage, hbmMask);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }

            public static int ReplaceIcon<THIML, THICON>(
                in THIML himl,
                int i,
                in THICON hicon) where THIML : IHandle<HIMAGELIST> where THICON : IHandle<HICON>
            {
                int result = ImageList_ReplaceIcon(himl.Handle, i, hicon.Handle);
                GC.KeepAlive(himl.Wrapper);
                GC.KeepAlive(hicon.Wrapper);
                return result;
            }

            public static COLORREF SetBkColor<T>(in T himl, COLORREF clrBk) where T : IHandle<HIMAGELIST>
            {
                COLORREF result = ImageList_SetBkColor(himl.Handle, clrBk);
                GC.KeepAlive(himl.Wrapper);
                return result;
            }
        }
    }
}
