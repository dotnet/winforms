// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Drawing2D;
using Xunit;
using static Interop;
using Moq;

namespace System.Windows.Forms.Tests
{
    public class DeviceContextHdcScopeTests
    {
        [Fact]
        public void CreateWithGraphicsBasedOnImageAppliesRequestedParameters()
        {
            using Bitmap b = new Bitmap(10, 10);
            using Graphics g = Graphics.FromImage(b);

            Rectangle clipRectangle = new Rectangle(1, 1, 5, 5);
            using Region r = new Region(clipRectangle);
            g.Clip = r;

            Matrix transform = new Matrix();
            transform.Translate(1.0f, 2.0f);
            g.Transform = transform;

            // Just the translation transform
            using (var scope = new DeviceContextHdcScope(g, ApplyGraphicsProperties.TranslateTransform))
            {
                Gdi32.GetViewportOrgEx(scope, out Point origin);
                Assert.Equal(new Point(1, 2), origin);

                RECT clipRect = default;
                RegionType regionType = Gdi32.GetClipBox(scope, ref clipRect);
                Assert.Equal(RegionType.SIMPLEREGION, regionType);
                Assert.Equal(new Rectangle(-1, -2, 10, 10), (Rectangle)clipRect);

                Assert.Equal(g, scope.DeviceContext);
            }

            // Just the clipping
            using (var scope = new DeviceContextHdcScope(g, ApplyGraphicsProperties.Clipping))
            {
                Gdi32.GetViewportOrgEx(scope, out Point origin);
                Assert.Equal(new Point(0, 0), origin);

                RECT clipRect = default;
                RegionType regionType = Gdi32.GetClipBox(scope, ref clipRect);
                Assert.Equal(RegionType.SIMPLEREGION, regionType);
                Assert.Equal(clipRectangle, (Rectangle)clipRect);

                Assert.Equal(g, scope.DeviceContext);
            }

            // Both
            using (var scope = new DeviceContextHdcScope(g, ApplyGraphicsProperties.All))
            {
                Gdi32.GetViewportOrgEx(scope, out Point origin);
                Assert.Equal(new Point(1, 2), origin);

                RECT clipRect = default;
                RegionType regionType = Gdi32.GetClipBox(scope, ref clipRect);
                Assert.Equal(RegionType.SIMPLEREGION, regionType);
                Assert.Equal(new Rectangle(0, -1, 5, 5), (Rectangle)clipRect);

                Assert.Equal(g, scope.DeviceContext);
            }

            // Nothing
            using (var scope = new DeviceContextHdcScope(g, ApplyGraphicsProperties.None))
            {
                Gdi32.GetViewportOrgEx(scope, out Point origin);
                Assert.Equal(new Point(0, 0), origin);

                RECT clipRect = default;
                RegionType regionType = Gdi32.GetClipBox(scope, ref clipRect);
                Assert.Equal(RegionType.SIMPLEREGION, regionType);
                Assert.Equal(new Rectangle(0, 0, 10, 10), (Rectangle)clipRect);

                Assert.Equal(g, scope.DeviceContext);
            }

            // Validating we've unlocked the graphics object
            g.DrawLine(Pens.DarkBlue, default, default);
        }

        [Fact]
        public void CreateWithGraphicsBasedOnHdcAppliesRequestedParameters()
        {
            using var hdc = User32.GetDcScope.ScreenDC;
            RECT originalClipRect = default;
            RegionType originalRegionType = Gdi32.GetClipBox(hdc, ref originalClipRect);
            Gdi32.GetViewportOrgEx(hdc, out Point originalOrigin);

            using Graphics g = Graphics.FromHdcInternal(hdc);

            Rectangle clipRectangle = new Rectangle(1, 1, 5, 5);
            using Region r = new Region(clipRectangle);
            g.Clip = r;

            Matrix transform = new Matrix();
            transform.Translate(1.0f, 2.0f);
            g.Transform = transform;

            // Just the translation transform
            using (var scope = new DeviceContextHdcScope(g, ApplyGraphicsProperties.TranslateTransform))
            {
                Gdi32.GetViewportOrgEx(scope, out Point origin);
                Assert.Equal(new Point(1, 2), origin);

                RECT clipRect = default;
                RegionType regionType = Gdi32.GetClipBox(scope, ref clipRect);
                Assert.Equal(originalRegionType, regionType);
                Rectangle expectedClipRect = originalClipRect;
                expectedClipRect.X -= 1;
                expectedClipRect.Y -= 2;
                Assert.Equal(expectedClipRect, (Rectangle)clipRect);

                Assert.Equal(g, scope.DeviceContext);
                Assert.Equal(hdc, scope.HDC);
            }

            // Should be in original state
            RECT currentClipRect = default;
            Gdi32.GetClipBox(hdc, ref currentClipRect);
            Gdi32.GetViewportOrgEx(hdc, out Point currentOrigin);
            Assert.Equal(originalClipRect, currentClipRect);
            Assert.Equal(originalOrigin, currentOrigin);

            // Just the clipping
            using (var scope = new DeviceContextHdcScope(g, ApplyGraphicsProperties.Clipping))
            {
                Gdi32.GetViewportOrgEx(scope, out Point origin);
                Assert.Equal(new Point(0, 0), origin);

                RECT clipRect = default;
                RegionType regionType = Gdi32.GetClipBox(scope, ref clipRect);
                Assert.Equal(RegionType.SIMPLEREGION, regionType);
                Assert.Equal(clipRectangle, (Rectangle)clipRect);

                Assert.Equal(g, scope.DeviceContext);
                Assert.Equal(hdc, scope.HDC);
            }

            // Should be in original state
            currentClipRect = default;
            Gdi32.GetClipBox(hdc, ref currentClipRect);
            Gdi32.GetViewportOrgEx(hdc, out currentOrigin);
            Assert.Equal(originalClipRect, currentClipRect);
            Assert.Equal(originalOrigin, currentOrigin);

            // Both
            using (var scope = new DeviceContextHdcScope(g, ApplyGraphicsProperties.All))
            {
                Gdi32.GetViewportOrgEx(scope, out Point origin);
                Assert.Equal(new Point(1, 2), origin);

                RECT clipRect = default;
                RegionType regionType = Gdi32.GetClipBox(scope, ref clipRect);
                Assert.Equal(RegionType.SIMPLEREGION, regionType);
                Assert.Equal(new Rectangle(0, -1, 5, 5), (Rectangle)clipRect);

                Assert.Equal(g, scope.DeviceContext);
                Assert.Equal(hdc, scope.HDC);
            }

            // Should be in original state
            currentClipRect = default;
            Gdi32.GetClipBox(hdc, ref currentClipRect);
            Gdi32.GetViewportOrgEx(hdc, out currentOrigin);
            Assert.Equal(originalClipRect, currentClipRect);
            Assert.Equal(originalOrigin, currentOrigin);

            // Nothing
            using (var scope = new DeviceContextHdcScope(g, ApplyGraphicsProperties.None))
            {
                Gdi32.GetViewportOrgEx(scope, out Point origin);
                Assert.Equal(new Point(0, 0), origin);

                RECT clipRect = default;
                RegionType regionType = Gdi32.GetClipBox(scope, ref clipRect);
                Assert.Equal(originalRegionType, regionType);
                Assert.Equal((Rectangle)originalClipRect, (Rectangle)clipRect);

                Assert.Equal(g, scope.DeviceContext);
                Assert.Equal(hdc, scope.HDC);
            }

            // Should be in original state
            currentClipRect = default;
            Gdi32.GetClipBox(hdc, ref currentClipRect);
            Gdi32.GetViewportOrgEx(hdc, out currentOrigin);
            Assert.Equal(originalClipRect, currentClipRect);
            Assert.Equal(originalOrigin, currentOrigin);

            // Validating we've unlocked the graphics object
            g.DrawLine(Pens.DarkBlue, default, default);
        }

        [Theory]
        [InlineData((int)ApplyGraphicsProperties.TranslateTransform)]
        [InlineData((int)ApplyGraphicsProperties.Clipping)]
        [InlineData((int)ApplyGraphicsProperties.All)]
        [InlineData((int)ApplyGraphicsProperties.None)]
        public void CreateFromCleanIGraphicsHdcProviderDoesNotCreateGraphics(int apply)
        {
            Gdi32.HDC mockHdc = new Gdi32.HDC((IntPtr)1234);
            var mockHdcProvider = new Mock<IGraphicsHdcProvider>();
            var mockIDeviceContext = mockHdcProvider.As<IDeviceContext>();
            mockHdcProvider
                .Setup(p => p.IsGraphicsStateClean)
                .Returns(true);
            mockHdcProvider
                .Setup(p => p.GetHDC())
                .Returns(mockHdc);

            using (var scope = new DeviceContextHdcScope(mockIDeviceContext.Object, (ApplyGraphicsProperties)apply))
            {
                Assert.Equal(mockHdc, scope.HDC);
                Assert.Equal(mockIDeviceContext.Object, scope.DeviceContext);

                mockHdcProvider.VerifyGet(p => p.IsGraphicsStateClean, Times.AtLeastOnce());
                mockHdcProvider.Verify(p => p.GetHDC(), Times.Once());
                mockHdcProvider.VerifyNoOtherCalls();
                mockIDeviceContext.VerifyNoOtherCalls();
            }

            // If we didn't create a graphics there is no need to release the HDC
            mockHdcProvider.VerifyNoOtherCalls();
            mockIDeviceContext.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData((int)ApplyGraphicsProperties.TranslateTransform)]
        [InlineData((int)ApplyGraphicsProperties.Clipping)]
        [InlineData((int)ApplyGraphicsProperties.All)]
        public void CreateFromDirtyIGraphicsHdcProviderCreatesGraphics(int apply)
        {
            using Bitmap b = new Bitmap(10, 10);
            using Graphics g = Graphics.FromImage(b);

            Gdi32.HDC mockHdc = new Gdi32.HDC((IntPtr)1234);
            var mockHdcProvider = new Mock<IGraphicsHdcProvider>();
            var mockIDeviceContext = mockHdcProvider.As<IDeviceContext>();
            mockHdcProvider
                .Setup(p => p.IsGraphicsStateClean)
                .Returns(false);
            mockHdcProvider
                .Setup(p => p.GetHDC())
                .Returns(mockHdc);
            mockHdcProvider
                .Setup(p => p.GetGraphics(true))
                .Returns(g);

            using (var scope = new DeviceContextHdcScope(mockIDeviceContext.Object, (ApplyGraphicsProperties)apply))
            {
                Assert.NotEqual(mockHdc, scope.HDC);
                Assert.Equal(g, scope.DeviceContext);

                mockHdcProvider.VerifyGet(p => p.IsGraphicsStateClean, Times.AtLeastOnce());
                mockHdcProvider.Verify(p => p.GetGraphics(true), Times.Once());
                mockHdcProvider.VerifyNoOtherCalls();
                mockIDeviceContext.VerifyNoOtherCalls();
            }

            // The graphics object itself will be called to release
            mockHdcProvider.VerifyNoOtherCalls();
            mockIDeviceContext.VerifyNoOtherCalls();

            // Validating we've unlocked the graphics object
            g.DrawLine(Pens.DarkBlue, default, default);
        }

        [Fact]
        public void CreateFromDirtyIGraphicsHdcProviderDoesNotCreateGraphics()
        {
            // If we don't request to apply properties, there is no need to get the graphics.

            Gdi32.HDC mockHdc = new Gdi32.HDC((IntPtr)1234);
            var mockHdcProvider = new Mock<IGraphicsHdcProvider>();
            var mockIDeviceContext = mockHdcProvider.As<IDeviceContext>();
            mockHdcProvider
                .Setup(p => p.IsGraphicsStateClean)
                .Returns(false);
            mockHdcProvider
                .Setup(p => p.GetHDC())
                .Returns(mockHdc);

            using (var scope = new DeviceContextHdcScope(mockIDeviceContext.Object, ApplyGraphicsProperties.None))
            {
                Assert.Equal(mockHdc, scope.HDC);
                Assert.Equal(mockIDeviceContext.Object, scope.DeviceContext);

                mockHdcProvider.VerifyGet(p => p.IsGraphicsStateClean, Times.AtLeastOnce());
                mockHdcProvider.Verify(p => p.GetHDC(), Times.Once());
                mockHdcProvider.VerifyNoOtherCalls();
                mockIDeviceContext.VerifyNoOtherCalls();
            }

            // We don't release anything as we haven't locked a Graphics object
            mockHdcProvider.VerifyNoOtherCalls();
            mockIDeviceContext.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData((int)ApplyGraphicsProperties.TranslateTransform)]
        [InlineData((int)ApplyGraphicsProperties.Clipping)]
        [InlineData((int)ApplyGraphicsProperties.All)]
        [InlineData((int)ApplyGraphicsProperties.None)]
        public void CreateFromIDeviceContext(int apply)
        {
            Gdi32.HDC mockHdc = new Gdi32.HDC((IntPtr)1234);
            var mockIDeviceContext = new Mock<IDeviceContext>();
            mockIDeviceContext
                .Setup(p => p.GetHdc())
                .Returns((IntPtr)mockHdc);
            mockIDeviceContext
                .Setup(p => p.ReleaseHdc());

            using (var scope = new DeviceContextHdcScope(mockIDeviceContext.Object, (ApplyGraphicsProperties)apply))
            {
                Assert.Equal(mockHdc, scope.HDC);
                Assert.Equal(mockIDeviceContext.Object, scope.DeviceContext);

                mockIDeviceContext.Verify(p => p.GetHdc(), Times.Once);
                mockIDeviceContext.VerifyNoOtherCalls();
            }

            mockIDeviceContext.Verify(p => p.ReleaseHdc(), Times.Once);
        }
    }
}
