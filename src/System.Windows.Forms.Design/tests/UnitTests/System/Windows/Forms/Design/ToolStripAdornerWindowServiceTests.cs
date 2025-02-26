// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using Moq;
using System.Windows.Forms.Design.Behavior;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripAdornerWindowServiceTests
{
    private class TestDesignerFrame : DesignerFrame
    {
        public TestDesignerFrame(ISite site) : base(site) { }
    }

    private class BehaviorServiceWrapper : IDisposable
    {
        public Mock<IServiceProvider> ServiceProviderMock { get; }
        public Mock<IOverlayService> OverlayServiceMock { get; }
        public BehaviorService BehaviorService { get; }

        public BehaviorServiceWrapper()
        {
            ServiceProviderMock = new Mock<IServiceProvider>();
            OverlayServiceMock = new Mock<IOverlayService>();

            Mock<ISite> siteMock = new();
            siteMock.Setup(s => s.GetService(typeof(IOverlayService))).Returns(OverlayServiceMock.Object);

            TestDesignerFrame designerFrame = new(siteMock.Object);
            BehaviorService = new(ServiceProviderMock.Object, designerFrame);

            SetupCommonMocks(ServiceProviderMock, OverlayServiceMock);
        }

        private void SetupCommonMocks(Mock<IServiceProvider> serviceProviderMock, Mock<IOverlayService> overlayServiceMock)
        {
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IOverlayService))).Returns(overlayServiceMock.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(BehaviorService))).Returns(BehaviorService);
        }

        public void Dispose()
        {
            BehaviorService?.Dispose();
        }
    }

    private class TestControl : Control
    {
        private readonly Graphics _graphics;

        public TestControl(Graphics graphics) => _graphics = graphics;

        public Graphics TestGraphics => _graphics;

        public new Point PointToScreen(Point point) => new Point(point.X + 8, point.Y + 31);
    }

    private readonly BehaviorServiceWrapper _behaviorServiceWrapper;
    private readonly ToolStripAdornerWindowService _service;

    public ToolStripAdornerWindowServiceTests() => (_behaviorServiceWrapper, _service) = Initialize();

    private (BehaviorServiceWrapper, ToolStripAdornerWindowService) Initialize()
    {
        BehaviorServiceWrapper behaviorServiceWrapper = new();
        using Control control = new();
        var service = new ToolStripAdornerWindowService(behaviorServiceWrapper.ServiceProviderMock.Object, control);
        return (behaviorServiceWrapper, service);
    }

    private void RunInStaThread(Action action)
    {
        Exception? exception = null;
        Thread thread = new(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (exception is not null)
        {
            throw exception;
        }
    }

    private void RunTest(Action test) => RunInStaThread(test);

    private TestControl CreateTestControlWithGraphics()
    {
        Bitmap bitmap = new(100, 100);
        Graphics graphics = Graphics.FromImage(bitmap);
        return new(graphics);
    }

    [Fact]
    public void ToolStripAdornerWindowGraphics_BasicTest() => RunTest(() =>
    {
        TestControl testControl = CreateTestControlWithGraphics();

        Graphics resultGraphics = testControl.TestGraphics;

        resultGraphics.Should().NotBeNull();
    });

    [Fact]
    public void DropDownAdorner_ReturnsAdornerObject() => RunTest(() =>
    {
        Adorner adorner = _service.DropDownAdorner;

        adorner.Should().NotBeNull();
    });

    [Fact]
    public void ToolStripAdornerWindowGraphics_ReturnsGraphicsObject() => RunTest(() =>
    {
        using Graphics graphics = _service.ToolStripAdornerWindowGraphics;

        graphics.Should().NotBeNull();
    });

    [Fact]
    public void Dispose_DisposesResourcesCorrectly() => RunTest(() =>
    {
        _behaviorServiceWrapper.BehaviorService.Adorners.Add(_service.DropDownAdorner);

        _service.Dispose();

        _behaviorServiceWrapper.OverlayServiceMock.Verify(o => o.RemoveOverlay(It.IsAny<Control>()), Times.Once);
        _behaviorServiceWrapper.ServiceProviderMock.Verify(sp => sp.GetService(typeof(BehaviorService)), Times.Once);
        _behaviorServiceWrapper.ServiceProviderMock.Verify(sp => sp.GetService(typeof(IOverlayService)), Times.Exactly(2));
        _behaviorServiceWrapper.BehaviorService.Adorners.Cast<Adorner>().Should().NotContain(_service.DropDownAdorner);
    });

    [Fact]
    public void AdornerWindowPointToScreen_TranslatesPointCorrectly() => RunTest(() =>
    {
        TestControl testControl = CreateTestControlWithGraphics();
        Point point = new(10, 20);

        Point screenPoint = _service.AdornerWindowPointToScreen(point);

        Point expectedScreenPoint = new(18, 51);
        screenPoint.Should().Be(expectedScreenPoint);
    });

    [Fact]
    public void AdornerWindowToScreen_ReturnsCorrectScreenCoordinates() => RunTest(() =>
    {
        TestControl control = new TestControl(Graphics.FromImage(new Bitmap(100, 100)));

        Point screenPoint = _service.AdornerWindowToScreen();

        Point expectedScreenPoint = new(8, 31);
        screenPoint.Should().Be(expectedScreenPoint);
    });

    [Fact]
    public void ControlToAdornerWindow_TranslatesPointCorrectly() => RunTest(() =>
    {
        Control parentControl = new Control { Left = 10, Top = 20 };
        Control control = new Control { Parent = parentControl, Left = 30, Top = 40 };

        Point adornerWindowPoint = _service.ControlToAdornerWindow(control);

        Point expectedPoint = new(40, 60);
        adornerWindowPoint.Should().Be(expectedPoint);
    });

    [Fact]
    public void Invalidate_InvokesInvalidateOnAdornerWindow() => RunTest(_service.Invalidate);

    [Fact]
    public void InvalidateRegion_InvokesInvalidateOnAdornerWindow() => RunTest(() =>
    {
        Region region = new(new Rectangle(10, 10, 50, 50));

        _service.Invalidate(region);
    });

    [Fact]
    public void ProcessPaintMessage_InvokesInvalidateOnAdornerWindow() => RunTest(() =>
    {
        Rectangle paintRect = new Rectangle(10, 10, 50, 50);

        _service.ProcessPaintMessage(paintRect);
    });
}
