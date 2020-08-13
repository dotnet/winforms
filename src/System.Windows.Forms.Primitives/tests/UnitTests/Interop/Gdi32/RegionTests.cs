// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.Gdi32;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.Tests.Interop.Gdi32
{
    public class RegionTests
    {
        [Fact]
        public void GetClipRgn_NoRegion()
        {
            // Create a bitmap using the screen's stats
            HDC hdc = CreateCompatibleDC((HDC)default);
            Assert.False(hdc.IsNull);

            try
            {
                HBITMAP hbitmap = CreateCompatibleBitmap(hdc, 20, 20);
                Assert.False(hdc.IsNull);

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
            HDC hdc = CreateCompatibleDC((HDC)default);
            Assert.False(hdc.IsNull);

            try
            {
                HBITMAP hbitmap = CreateCompatibleBitmap(hdc, 20, 20);
                Assert.False(hdc.IsNull);

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

        [Fact]
        public void RegionScope_GetRegion()
        {
            // Create a bitmap using the screen's stats
            HDC hdc = CreateCompatibleDC((HDC)default);
            Assert.False(hdc.IsNull);

            try
            {
                HBITMAP hbitmap = CreateCompatibleBitmap(hdc, 20, 20);
                Assert.False(hdc.IsNull);

                try
                {
                    Rectangle rectangle = new Rectangle(1, 2, 3, 4);
                    using var originalRegion = new RegionScope(rectangle);
                    SelectClipRgn(hdc, originalRegion);
                    using var retrievedRegion = new RegionScope(hdc);
                    RECT rect = default;
                    RegionType type = GetRgnBox(retrievedRegion, ref rect);
                    Assert.Equal(RegionType.SIMPLEREGION, type);
                    Assert.Equal(rectangle, (Rectangle)rect);
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
