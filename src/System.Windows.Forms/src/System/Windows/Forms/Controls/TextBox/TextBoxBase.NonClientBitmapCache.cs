// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public abstract partial class TextBoxBase
{
    private class NonClientBitmapCache : IDisposable
    {
        private Bitmap _bitmap;

        public NonClientBitmapCache(Control control, int width, int height)
        {
            using Graphics graphics = control.CreateGraphics();

            HDC hdc = (HDC)graphics.GetHdc();

            try
            {
                _bitmap = CreateCompatibleBitmap(hdc, width, height);
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }

        public Graphics GetNewGraphics() => Graphics.FromImage(_bitmap);

        public Bitmap Bitmap => _bitmap;

        public void EnsureSize(int width, int height)
        {
            if (_bitmap.Width == width && _bitmap.Height == height)
            {
                return;
            }

            using Graphics graphics = Graphics.FromImage(_bitmap);

            HDC hdc = (HDC)graphics.GetHdc();

            try
            {
                var newBitmap = CreateCompatibleBitmap(hdc, width, height);
                _bitmap?.Dispose();
                _bitmap = newBitmap;
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }

        private static Bitmap CreateCompatibleBitmap(HDC hdc, int width, int height)
        {
            HBITMAP compatibleBitmap = PInvokeCore.CreateCompatibleBitmap(hdc, width, height);
            Bitmap imageToReturn = Image.FromHbitmap(compatibleBitmap);
            PInvokeCore.DeleteObject(compatibleBitmap);

            return imageToReturn;
        }

        void IDisposable.Dispose()
        {
            try
            {
                _bitmap.Dispose();
            }
            catch (Exception)
            {
            }
            finally
            {
                _bitmap = null!;
            }
        }
    }
}
