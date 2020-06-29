// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.Gdi32;

namespace System.Windows.Forms.Tests.InteropTests
{
    public class RegionTests
    {
        [Fact]
        public void GetClipRgn_NoRegion()
        {
            // Create a bitmap using the screen's stats
            IntPtr hdc = CreateCompatibleDC(IntPtr.Zero);
            Assert.NotEqual(IntPtr.Zero, hdc);

            try
            {
                IntPtr hbitmap = CreateCompatibleBitmap(hdc, 20, 20);
                Assert.NotEqual(IntPtr.Zero, hbitmap);

                try
                {
                    SelectObject(hdc, hbitmap);
                    HRGN hregion = CreateRectRgn(0, 0, 0, 0);

                    Assert.False(hregion.IsNull);
                    try
                    {
                        int result = GetClipRgn(hdc, hregion);

                        // We should have no clipping region
                        Assert.Equal(0, result);
                    }
                    finally
                    {
                        DeleteObject(hregion);
                    }
                }
                finally
                {
                    DeleteObject(hbitmap);
                }
            }
            finally
            {
                DeleteDC(hdc);
            }
        }

        [Fact]
        public void RegionScope_NullWithNoClippingRegion()
        {
            // Create a bitmap using the screen's stats
            IntPtr hdc = CreateCompatibleDC(IntPtr.Zero);
            Assert.NotEqual(IntPtr.Zero, hdc);

            try
            {
                IntPtr hbitmap = CreateCompatibleBitmap(hdc, 20, 20);
                Assert.NotEqual(IntPtr.Zero, hbitmap);
                try
                {
                    using var hregion = new RegionScope(hdc);
                    Assert.True(hregion.IsNull);
                }
                finally
                {
                    DeleteObject(hbitmap);
                }
            }
            finally
            {
                DeleteDC(hdc);
            }
        }
    }
}
