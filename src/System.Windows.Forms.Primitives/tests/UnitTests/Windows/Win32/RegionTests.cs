﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;
using static Interop.Gdi32;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32
{
    public class RegionTests
    {
        [Fact]
        public void GetClipRgn_NoRegion()
        {
            // Create a bitmap using the screen's stats
            HDC hdc = PInvoke.CreateCompatibleDC((HDC)default);
            Assert.False(hdc.IsNull);

            try
            {
                HBITMAP hbitmap = PInvoke.CreateCompatibleBitmap(hdc, 20, 20);
                Assert.False(hdc.IsNull);

                try
                {
                    SelectObject(hdc, hbitmap);
                    HRGN hregion = PInvoke.CreateRectRgn(0, 0, 0, 0);

                    Assert.False(hregion.IsNull);
                    try
                    {
                        int result = GetClipRgn(hdc, hregion);

                        // We should have no clipping region
                        Assert.Equal(0, result);
                    }
                    finally
                    {
                        PInvoke.DeleteObject(hregion);
                    }
                }
                finally
                {
                    PInvoke.DeleteObject(hbitmap);
                }
            }
            finally
            {
                PInvoke.DeleteDC(hdc);
            }
        }

        [Fact]
        public void RegionScope_NullWithNoClippingRegion()
        {
            // Create a bitmap using the screen's stats
            HDC hdc = PInvoke.CreateCompatibleDC((HDC)default);
            Assert.False(hdc.IsNull);

            try
            {
                HBITMAP hbitmap = PInvoke.CreateCompatibleBitmap(hdc, 20, 20);
                Assert.False(hdc.IsNull);

                try
                {
                    using var hregion = new PInvoke.RegionScope(hdc);
                    Assert.True(hregion.IsNull);
                }
                finally
                {
                    PInvoke.DeleteObject(hbitmap);
                }
            }
            finally
            {
                PInvoke.DeleteDC(hdc);
            }
        }

        [Fact]
        public void RegionScope_GetRegion()
        {
            // Create a bitmap using the screen's stats
            HDC hdc = PInvoke.CreateCompatibleDC((HDC)default);
            Assert.False(hdc.IsNull);

            try
            {
                HBITMAP hbitmap = PInvoke.CreateCompatibleBitmap(hdc, 20, 20);
                Assert.False(hdc.IsNull);

                try
                {
                    Rectangle rectangle = new(1, 2, 3, 4);
                    using PInvoke.RegionScope originalRegion = new(rectangle);
                    SelectClipRgn(hdc, originalRegion);
                    using PInvoke.RegionScope retrievedRegion = new(hdc);
                    RECT rect = default;
                    RegionType type = GetRgnBox(retrievedRegion, ref rect);
                    Assert.Equal(RegionType.SIMPLEREGION, type);
                    Assert.Equal(rectangle, (Rectangle)rect);
                }
                finally
                {
                    PInvoke.DeleteObject(hbitmap);
                }
            }
            finally
            {
                PInvoke.DeleteDC(hdc);
            }
        }
    }
}
