// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using Moq;

namespace System.Windows.Forms.Tests;

public class DeviceContextHdcScopeTests
{
    [Fact]
    public unsafe void CreateWithGraphicsBasedOnImageAppliesRequestedParameters()
    {
        using Bitmap b = new(10, 10);
        using Graphics g = Graphics.FromImage(b);

        Rectangle clipRectangle = new(1, 1, 5, 5);
        using Region r = new(clipRectangle);
        g.Clip = r;

        Matrix transform = new();
        transform.Translate(1.0f, 2.0f);
        g.Transform = transform;

        // Just the translation transform
        using (DeviceContextHdcScope scope = new(g, ApplyGraphicsProperties.TranslateTransform))
        {
            Point origin = default;
            PInvokeCore.GetViewportOrgEx(scope, &origin);
            Assert.Equal(new Point(1, 2), origin);

            RECT clipRect = default;
            GDI_REGION_TYPE regionType = PInvoke.GetClipBox(scope, &clipRect);
            Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, regionType);
            Assert.Equal(new Rectangle(-1, -2, 10, 10), (Rectangle)clipRect);

            Assert.Equal(g, scope.DeviceContext);
        }

        // Just the clipping
        using (DeviceContextHdcScope scope = new(g, ApplyGraphicsProperties.Clipping))
        {
            Point origin = default;
            PInvokeCore.GetViewportOrgEx(scope, &origin);
            Assert.Equal(new Point(0, 0), origin);

            RECT clipRect = default;
            GDI_REGION_TYPE regionType = PInvoke.GetClipBox(scope, &clipRect);
            Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, regionType);
            Assert.Equal(clipRectangle, (Rectangle)clipRect);

            Assert.Equal(g, scope.DeviceContext);
        }

        // Both
        using (DeviceContextHdcScope scope = new(g, ApplyGraphicsProperties.All))
        {
            Point origin = default;
            PInvokeCore.GetViewportOrgEx(scope, &origin);
            Assert.Equal(new Point(1, 2), origin);

            RECT clipRect = default;
            GDI_REGION_TYPE regionType = PInvoke.GetClipBox(scope, &clipRect);
            Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, regionType);
            Assert.Equal(new Rectangle(0, -1, 5, 5), (Rectangle)clipRect);

            Assert.Equal(g, scope.DeviceContext);
        }

        // Nothing
        using (DeviceContextHdcScope scope = new(g, ApplyGraphicsProperties.None))
        {
            Point origin = default;
            PInvokeCore.GetViewportOrgEx(scope, &origin);
            Assert.Equal(new Point(0, 0), origin);

            RECT clipRect = default;
            GDI_REGION_TYPE regionType = PInvoke.GetClipBox(scope, &clipRect);
            Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, regionType);
            Assert.Equal(new Rectangle(0, 0, 10, 10), (Rectangle)clipRect);

            Assert.Equal(g, scope.DeviceContext);
        }

        // Validating we've unlocked the graphics object
        g.DrawLine(Pens.DarkBlue, default, default);
    }

    [Fact]
    public unsafe void CreateWithGraphicsBasedOnHdcAppliesRequestedParameters()
    {
        using var hdc = GetDcScope.ScreenDC;
        RECT originalClipRect = default;
        GDI_REGION_TYPE originalRegionType = PInvoke.GetClipBox(hdc, &originalClipRect);
        Point originalOrigin = default;
        PInvokeCore.GetViewportOrgEx(hdc, &originalOrigin);

        using Graphics g = Graphics.FromHdcInternal(hdc);

        Rectangle clipRectangle = new(1, 1, 5, 5);
        using Region r = new(clipRectangle);
        g.Clip = r;

        Matrix transform = new();
        transform.Translate(1.0f, 2.0f);
        g.Transform = transform;

        // Just the translation transform
        using (DeviceContextHdcScope scope = new(g, ApplyGraphicsProperties.TranslateTransform))
        {
            Point origin = default;
            PInvokeCore.GetViewportOrgEx(scope, &origin);
            Assert.Equal(new Point(1, 2), origin);

            RECT clipRect = default;
            GDI_REGION_TYPE regionType = PInvoke.GetClipBox(scope, &clipRect);
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
        PInvoke.GetClipBox(hdc, &currentClipRect);
        Point currentOrigin = default;
        PInvokeCore.GetViewportOrgEx(hdc, &currentOrigin);
        Assert.Equal(originalClipRect, currentClipRect);
        Assert.Equal(originalOrigin, currentOrigin);

        // Just the clipping
        using (DeviceContextHdcScope scope = new(g, ApplyGraphicsProperties.Clipping))
        {
            Point origin = default;
            PInvokeCore.GetViewportOrgEx(scope, &origin);
            Assert.Equal(new Point(0, 0), origin);

            RECT clipRect = default;
            GDI_REGION_TYPE regionType = PInvoke.GetClipBox(scope, &clipRect);
            Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, regionType);
            Assert.Equal(clipRectangle, (Rectangle)clipRect);

            Assert.Equal(g, scope.DeviceContext);
            Assert.Equal(hdc, scope.HDC);
        }

        // Should be in original state
        currentClipRect = default;
        PInvoke.GetClipBox(hdc, &currentClipRect);
        PInvokeCore.GetViewportOrgEx(hdc, &currentOrigin);
        Assert.Equal(originalClipRect, currentClipRect);
        Assert.Equal(originalOrigin, currentOrigin);

        // Both
        using (DeviceContextHdcScope scope = new(g, ApplyGraphicsProperties.All))
        {
            Point origin = default;
            PInvokeCore.GetViewportOrgEx(scope, &origin);
            Assert.Equal(new Point(1, 2), origin);

            RECT clipRect = default;
            GDI_REGION_TYPE regionType = PInvoke.GetClipBox(scope, &clipRect);
            Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, regionType);
            Assert.Equal(new Rectangle(0, -1, 5, 5), (Rectangle)clipRect);

            Assert.Equal(g, scope.DeviceContext);
            Assert.Equal(hdc, scope.HDC);
        }

        // Should be in original state
        currentClipRect = default;
        PInvoke.GetClipBox(hdc, &currentClipRect);
        PInvokeCore.GetViewportOrgEx(hdc, &currentOrigin);
        Assert.Equal(originalClipRect, currentClipRect);
        Assert.Equal(originalOrigin, currentOrigin);

        // Nothing
        using (DeviceContextHdcScope scope = new(g, ApplyGraphicsProperties.None))
        {
            Point origin = default;
            PInvokeCore.GetViewportOrgEx(scope, &origin);
            Assert.Equal(new Point(0, 0), origin);

            RECT clipRect = default;
            GDI_REGION_TYPE regionType = PInvoke.GetClipBox(scope, &clipRect);
            Assert.Equal(originalRegionType, regionType);
            Assert.Equal((Rectangle)originalClipRect, (Rectangle)clipRect);

            Assert.Equal(g, scope.DeviceContext);
            Assert.Equal(hdc, scope.HDC);
        }

        // Should be in original state
        currentClipRect = default;
        PInvoke.GetClipBox(hdc, &currentClipRect);
        PInvokeCore.GetViewportOrgEx(hdc, &currentOrigin);
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
        using var hdc = GetDcScope.ScreenDC;
        Mock<IGraphicsHdcProvider> mockHdcProvider = new();
        var mockIDeviceContext = mockHdcProvider.As<IHdcContext>();
        mockHdcProvider
            .Setup(p => p.IsGraphicsStateClean)
            .Returns(true);
        mockHdcProvider
            .Setup(p => p.GetHdc())
            .Returns(hdc);

        using (DeviceContextHdcScope scope = new(mockIDeviceContext.Object, (ApplyGraphicsProperties)apply))
        {
            Assert.Equal(hdc, scope.HDC);
            Assert.Equal(mockIDeviceContext.Object, scope.DeviceContext);

            mockHdcProvider.VerifyGet(p => p.IsGraphicsStateClean, Times.AtLeastOnce());
            mockHdcProvider.Verify(p => p.GetHdc(), Times.Once());
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
        using Bitmap b = new(10, 10);
        using Graphics g = Graphics.FromImage(b);

        HDC mockHdc = new(1234);
        Mock<IGraphicsHdcProvider> mockHdcProvider = new();
        var mockIDeviceContext = mockHdcProvider.As<IHdcContext>();
        mockHdcProvider
            .Setup(p => p.IsGraphicsStateClean)
            .Returns(false);
        mockHdcProvider
            .Setup(p => p.GetHdc())
            .Returns(mockHdc);
        mockHdcProvider
            .Setup(p => p.GetGraphics(true))
            .Returns(g);

        using (DeviceContextHdcScope scope = new(mockIDeviceContext.Object, (ApplyGraphicsProperties)apply))
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

        using var hdc = GetDcScope.ScreenDC;
        Mock<IGraphicsHdcProvider> mockHdcProvider = new();
        var mockIDeviceContext = mockHdcProvider.As<IHdcContext>();
        mockHdcProvider
            .Setup(p => p.IsGraphicsStateClean)
            .Returns(false);
        mockHdcProvider
            .Setup(p => p.GetHdc())
            .Returns(hdc);

        using (DeviceContextHdcScope scope = new(mockIDeviceContext.Object, ApplyGraphicsProperties.None))
        {
            Assert.Equal(hdc, scope.HDC);
            Assert.Equal(mockIDeviceContext.Object, scope.DeviceContext);

            mockHdcProvider.VerifyGet(p => p.IsGraphicsStateClean, Times.AtLeastOnce());
            mockHdcProvider.Verify(p => p.GetHdc(), Times.Once());
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
        using var hdc = GetDcScope.ScreenDC;
        Mock<IHdcContext> mockIDeviceContext = new();
        mockIDeviceContext
            .Setup(p => p.GetHdc())
            .Returns(hdc);
        mockIDeviceContext
            .Setup(p => p.ReleaseHdc());

        using (DeviceContextHdcScope scope = new(mockIDeviceContext.Object, (ApplyGraphicsProperties)apply))
        {
            Assert.Equal(hdc, scope.HDC);
            Assert.Equal(mockIDeviceContext.Object, scope.DeviceContext);

            mockIDeviceContext.Verify(p => p.GetHdc(), Times.Once);
            mockIDeviceContext.VerifyNoOtherCalls();
        }

        mockIDeviceContext.Verify(p => p.ReleaseHdc(), Times.Once);
    }
}
