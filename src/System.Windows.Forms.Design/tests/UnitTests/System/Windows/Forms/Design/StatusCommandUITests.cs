// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class StatusCommandUITests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IMenuCommandService> _mockMenuCommandService;
    private readonly Mock<MenuCommand> _mockMenuCommand;
    private readonly StatusCommandUI _statusCommandUI;

    public StatusCommandUITests()
    {
        _mockServiceProvider = new();
        _mockMenuCommandService = new();
        _mockMenuCommand = new(null!, null!);

        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IMenuCommandService)))
            .Returns(_mockMenuCommandService.Object);

        _mockMenuCommandService
            .Setup(mcs => mcs.FindCommand(MenuCommands.SetStatusRectangle))
            .Returns(_mockMenuCommand.Object);

        _statusCommandUI = new(_mockServiceProvider.Object);
    }

    [Fact]
    public void SetStatusInformation_WithNullComponent_ShouldNotInvokeCommand()
    {
        _statusCommandUI.SetStatusInformation(selectedComponent: null);
        _mockMenuCommand.Verify(mc => mc.Invoke(It.IsAny<Rectangle>()), Times.Never);
    }

    [Fact]
    public void SetStatusInformation_WithControlComponent_ShouldInvokeWithOriginalBounds()
    {
        using Control control = new() { Bounds = new(10, 20, 30, 40) };
        TestSetStatusInformation(() => _statusCommandUI.SetStatusInformation(control), new(10, 20, 30, 40));
        control.IsHandleCreated.Should().BeFalse();
    }

    [Fact]
    public void SetStatusInformation_WithCustomComponentHavingBounds_ShouldInvokeWithOriginalBounds()
    {
        using CustomComponent component = new() { Bounds = new(10, 20, 30, 40) };
        TestSetStatusInformation(() => _statusCommandUI.SetStatusInformation(component), new(10, 20, 30, 40));
    }

    [Fact]
    public void SetStatusInformation_WithComponentWithoutBounds_ShouldInvokeWithEmptyRectangle()
    {
        using Component component = new();
        TestSetStatusInformation(() => _statusCommandUI.SetStatusInformation(component), Rectangle.Empty);
    }

    [Fact]
    public void SetStatusInformation_WithRectangle_ShouldInvokeWithSameRectangle()
    {
        Rectangle bounds = new(10, 20, 30, 40);
        TestSetStatusInformation(() => _statusCommandUI.SetStatusInformation(bounds), bounds);
    }

    [Fact]
    public void SetStatusInformation_WithEmptyRectangle_ShouldInvokeWithEmptyRectangle()
    {
        Rectangle bounds = Rectangle.Empty;
        TestSetStatusInformation(() => _statusCommandUI.SetStatusInformation(bounds), bounds);
    }

    [Fact]
    public void SetStatusInformation_WithControlAndLocation_ShouldInvokeWithUpdatedBounds()
    {
        using Control control = new() { Bounds = new(10, 20, 30, 40) };
        Point location = new(50, 60);
        TestSetStatusInformation(() => _statusCommandUI.SetStatusInformation(control, location), new(50, 60, 30, 40));
    }

    [Fact]
    public void SetStatusInformation_WithComponentHavingBoundsAndLocation_ShouldInvokeWithUpdatedBounds()
    {
        using CustomComponent component = new() { Bounds = new(10, 20, 30, 40) };
        Point location = new(50, 60);
        TestSetStatusInformation(() => _statusCommandUI.SetStatusInformation(component, location), new(50, 60, 30, 40));
    }

    [Fact]
    public void SetStatusInformation_WithComponentHavingLocationButNoBounds_ShouldInvokeWithEmptyRectangle()
    {
        using Component component = new();
        Point location = new(50, 60);
        _statusCommandUI.SetStatusInformation(component, location);
        _mockMenuCommand.Verify(cmd => cmd.Invoke(new Rectangle(50, 60, 0, 0)), Times.Once);
    }

    [Fact]
    public void SetStatusInformation_WithLocationAsEmpty_ShouldNotChangeBounds()
    {
        using Control control = new() { Bounds = new(10, 20, 30, 40) };
        Point location = Point.Empty;
        TestSetStatusInformation(() => _statusCommandUI.SetStatusInformation(control, location), new(10, 20, 30, 40));
    }

    [Fact]
    public void SetStatusInformation_NullComponentWithLocation_ShouldNotInvokeCommand()
    {
        _statusCommandUI.SetStatusInformation(null, new Point(10, 10));
        _mockMenuCommand.Verify(cmd => cmd.Invoke(It.IsAny<Rectangle>()), Times.Never);
    }

    private void TestSetStatusInformation(Action action, Rectangle expectedRectangle)
    {
        action.Invoke();
        VerifyInvoke(expectedRectangle);
    }

    private void VerifyInvoke(Rectangle expectedRectangle)
    {
        _mockMenuCommand.Verify(mc => mc.Invoke(It.Is<Rectangle>(r =>
              r.X == expectedRectangle.X
           && r.Y == expectedRectangle.Y
           && r.Width == expectedRectangle.Width
           && r.Height == expectedRectangle.Height)), Times.Once);
    }

    private class CustomComponent : Component
    {
        public Rectangle Bounds { get; set; }
    }
}
