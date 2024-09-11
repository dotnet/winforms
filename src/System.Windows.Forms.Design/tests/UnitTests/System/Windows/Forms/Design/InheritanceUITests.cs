// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;
using Moq.Protected;
using static System.Windows.Forms.Design.ComponentTray;

namespace System.Windows.Forms.Design.Tests;

public sealed class InheritanceUITests : IDisposable
{
    private readonly InheritanceUI _inheritanceUI;
    private readonly Form _form;
    private readonly Control _control;
    private readonly Control _childControl;
    private readonly Control _sitedChildControl;

    public InheritanceUITests()
    {
        _inheritanceUI = new InheritanceUI();
        _form = new Form();
        _control = new Control { Parent = _form };
        _childControl = new Control { Parent = _control };
        _sitedChildControl = new Control { Parent = _control, Site = new Mock<ISite>().Object };
    }

    public void Dispose()
    {
        _sitedChildControl.Dispose();
        _childControl.Dispose();
        _control.Dispose();
        _form.Dispose();
        _inheritanceUI.Dispose();
    }

    [Theory]
    [InlineData(InheritanceLevel.Inherited, "Inherited control")]
    [InlineData(InheritanceLevel.InheritedReadOnly, "Inherited control (Private)")]
    [InlineData(InheritanceLevel.NotInherited, "Inherited control")]
    public void AddInheritedControl_ShouldSetToolTipText(InheritanceLevel inheritanceLevel, string expectedText)
    {
        Mock<IDesigner> mockDesigner = new(MockBehavior.Strict);
        Mock<IExtenderProviderService> mockExtenderProviderService = new();
        Mock<IServiceProvider> mockServiceProvider = new();
        mockServiceProvider
            .Setup(s => s.GetService(typeof(IExtenderProviderService)))
            .Returns(mockExtenderProviderService.Object);
        ComponentTray componentTray = new(mockDesigner.Object, mockServiceProvider.Object);
        Mock<IComponent> mockComponent = new();
        TrayControl trayControl = new(componentTray, mockComponent.Object);

        using (new NoAssertContext())
        {
            _inheritanceUI.AddInheritedControl(trayControl, inheritanceLevel);
        }

        ToolTip toolTip = _inheritanceUI.TestAccessor().Dynamic._toolTip;
        string text = toolTip.GetToolTip(trayControl);
        text.Should().Be(expectedText);
    }

    [Fact]
    public void RemoveInheritedControl_ShouldUnsetToolTipText()
    {
        Mock<IDesigner> mockDesigner = new(MockBehavior.Strict);
        Mock<IExtenderProviderService> mockExtenderProviderService = new();
        Mock<IServiceProvider> mockServiceProvider = new();
        mockServiceProvider
            .Setup(s => s.GetService(typeof(IExtenderProviderService)))
            .Returns(mockExtenderProviderService.Object);
        ComponentTray componentTray = new(mockDesigner.Object, mockServiceProvider.Object);
        Mock<IComponent> mockComponent = new();
        TrayControl trayControl = new(componentTray, mockComponent.Object);

        using (new NoAssertContext())
        {
            _inheritanceUI.AddInheritedControl(trayControl, InheritanceLevel.Inherited);
        }

        _inheritanceUI.RemoveInheritedControl(trayControl);

        ToolTip toolTip = _inheritanceUI.TestAccessor().Dynamic._toolTip;
        string text = toolTip.GetToolTip(trayControl);
        text.Should().BeEmpty();
    }

#nullable enable

    [Fact]
    public void InheritanceGlyph_And_InheritanceGlyphRectangle_ShouldReturnExpectedValues()
    {
        Bitmap glyph = InheritanceUI.InheritanceGlyph;
        Rectangle glyphRect = InheritanceUI.InheritanceGlyphRectangle;

        glyph.Should().NotBeNull().And.BeOfType<Bitmap>();
        glyphRect.Should().NotBeEmpty();
        glyphRect.Size.Should().Be(glyph.Size);
    }

    [Theory]
    [InlineData(InheritanceLevel.Inherited, "Inherited control")]
    [InlineData(InheritanceLevel.InheritedReadOnly, "Inherited control (Private)")]
    public void AddInheritedControl_ShouldSetToolTipText_And_InitializeToolTip(InheritanceLevel inheritanceLevel, string expectedText)
    {
        _inheritanceUI.AddInheritedControl(_control, inheritanceLevel);
        ToolTip toolTip = _inheritanceUI.TestAccessor().Dynamic._toolTip;

        toolTip.Should().NotBeNull().And.BeOfType<ToolTip>();
        toolTip.ShowAlways.Should().BeTrue();
        toolTip.GetToolTip(_control).Should().Be(expectedText);
        toolTip.GetToolTip(_childControl).Should().Be(expectedText);
    }

    [Fact]
    public void AddAndRemoveInheritedControl_ShouldSetAndUnsetToolTipText_ForNonSitedChildren()
    {
        _inheritanceUI.AddInheritedControl(_control, InheritanceLevel.Inherited);
        ToolTip toolTip = _inheritanceUI.TestAccessor().Dynamic._toolTip;

        toolTip.Should().NotBeNull().And.BeOfType<ToolTip>();
        toolTip.GetToolTip(_control).Should().Be("Inherited control");
        toolTip.GetToolTip(_childControl).Should().Be("Inherited control");
        toolTip.GetToolTip(_sitedChildControl).Should().BeEmpty();

        _inheritanceUI.RemoveInheritedControl(_control);

        toolTip.GetToolTip(_control).Should().BeEmpty();
        toolTip.GetToolTip(_childControl).Should().BeEmpty();
        toolTip.GetToolTip(_sitedChildControl).Should().BeEmpty();
    }

    [Fact]
    public void Dispose_ShouldDisposeToolTip_And_NotThrowIfToolTipIsNull()
    {
        Mock<ToolTip> mockToolTip = new();
        mockToolTip.Protected().Setup("Dispose", ItExpr.IsAny<bool>());

        _inheritanceUI.TestAccessor().Dynamic._toolTip = mockToolTip.Object;

        _inheritanceUI.Invoking(ui => ui.Dispose()).Should().NotThrow();
        mockToolTip.Protected().Verify("Dispose", Times.Once(), ItExpr.IsAny<bool>());
    }

    [Fact]
    public void RemoveInheritedControl_ShouldUnsetToolTipText_And_NotThrowIfToolTipIsNull()
    {
        _inheritanceUI.Invoking(ui => ui.RemoveInheritedControl(_control)).Should().NotThrow();

        _inheritanceUI.AddInheritedControl(_control, InheritanceLevel.Inherited);
        _inheritanceUI.RemoveInheritedControl(_control);

        ToolTip toolTip = _inheritanceUI.TestAccessor().Dynamic._toolTip;

        toolTip.Should().NotBeNull().And.BeOfType<ToolTip>();
        toolTip.GetToolTip(_control).Should().BeEmpty();
        toolTip.GetToolTip(_sitedChildControl).Should().BeEmpty();
    }
}

#nullable disable
