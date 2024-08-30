using Moq;
using Moq.Protected;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using static System.Windows.Forms.Design.ComponentTray;

namespace System.Windows.Forms.Design.Tests;

public sealed class InheritanceUITests
{
    [Theory]
    [InlineData(InheritanceLevel.Inherited, "Inherited control")]
    [InlineData(InheritanceLevel.InheritedReadOnly, "Inherited control (Private)")]
    [InlineData(InheritanceLevel.NotInherited, "Inherited control")]
    public void AddInheritedControl_ShouldSetToolTipText(InheritanceLevel inheritanceLevel, string expectedText)
    {
        InheritanceUI inheritanceUI = new();
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
            inheritanceUI.AddInheritedControl(trayControl, inheritanceLevel);
        }

        ToolTip toolTip = inheritanceUI.TestAccessor().Dynamic._toolTip;
        string text = toolTip.GetToolTip(trayControl);
        text.Should().Be(expectedText);
    }

    [Fact]
    public void RemoveInheritedControl_ShouldUnsetToolTipText()
    {
        InheritanceUI inheritanceUI = new();
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
            inheritanceUI.AddInheritedControl(trayControl, InheritanceLevel.Inherited);
        }

        inheritanceUI.RemoveInheritedControl(trayControl);

        ToolTip toolTip = inheritanceUI.TestAccessor().Dynamic._toolTip;
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
        glyphRect.Should().NotBe(Rectangle.Empty);
        glyphRect.Size.Should().Be(glyph.Size);
    }


    [Theory]
    [InlineData(InheritanceLevel.Inherited, "Inherited control")]
    [InlineData(InheritanceLevel.InheritedReadOnly, "Inherited control (Private)")]
    public void AddInheritedControl_ShouldSetToolTipText_And_InitializeToolTip(InheritanceLevel inheritanceLevel, string expectedText)
    {
        InheritanceUI inheritanceUI = new();
        using Form form = new();
        using Control control = new() { Parent = form };
        using Control childControl = new() { Parent = control };

        inheritanceUI.AddInheritedControl(control, inheritanceLevel);
        ToolTip toolTip = inheritanceUI.TestAccessor().Dynamic._toolTip;

        toolTip.Should().NotBeNull().And.BeOfType<ToolTip>();
        toolTip.ShowAlways.Should().BeTrue();
        toolTip.GetToolTip(control).Should().Be(expectedText);
        toolTip.GetToolTip(childControl).Should().Be(expectedText);
    }


    [Fact]
    public void AddAndRemoveInheritedControl_ShouldSetAndUnsetToolTipText_ForNonSitedChildren()
    {
        InheritanceUI inheritanceUI = new();
        using Form form = new();
        using Control parentControl = new() { Parent = form };
        using Control childControl = new() { Parent = parentControl };
        using Control sitedChildControl = new() { Parent = parentControl, Site = new Mock<ISite>().Object };

        inheritanceUI.AddInheritedControl(parentControl, InheritanceLevel.Inherited);
        ToolTip toolTip = inheritanceUI.TestAccessor().Dynamic._toolTip;

        toolTip.Should().NotBeNull().And.BeOfType<ToolTip>();
        toolTip.GetToolTip(parentControl).Should().Be("Inherited control");
        toolTip.GetToolTip(childControl).Should().Be("Inherited control");
        toolTip.GetToolTip(sitedChildControl).Should().BeEmpty();

        inheritanceUI.RemoveInheritedControl(parentControl);

        toolTip.GetToolTip(parentControl).Should().BeEmpty();
        toolTip.GetToolTip(childControl).Should().BeEmpty();
        toolTip.GetToolTip(sitedChildControl).Should().BeEmpty();
    }


    [Fact]
    public void Dispose_ShouldDisposeToolTip_And_NotThrowIfToolTipIsNull()
    {
        var inheritanceUI = new InheritanceUI();
        var mockToolTip = new Mock<ToolTip>();
        bool isDisposed = false;

        mockToolTip.Protected().Setup("Dispose", ItExpr.IsAny<bool>()).Callback(() => isDisposed = true);

        typeof(InheritanceUI).GetField("_toolTip", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(inheritanceUI, mockToolTip.Object);

        inheritanceUI.Invoking(ui => ui.Dispose()).Should().NotThrow();
        isDisposed.Should().BeTrue();

        inheritanceUI = new InheritanceUI();
        inheritanceUI.Invoking(ui => ui.Dispose()).Should().NotThrow();
    }


    [Fact]
    public void RemoveInheritedControl_ShouldUnsetToolTipText_And_NotThrowIfToolTipIsNull()
    {
        InheritanceUI inheritanceUI = new();
        using Form form = new();
        using Control parentControl = new() { Parent = form };
        using Control sitedChildControl = new() { Parent = parentControl, Site = new Mock<ISite>().Object };

        inheritanceUI.Invoking(ui => ui.RemoveInheritedControl(parentControl)).Should().NotThrow();

        inheritanceUI.AddInheritedControl(parentControl, InheritanceLevel.Inherited);
        inheritanceUI.RemoveInheritedControl(parentControl);

        ToolTip toolTip = inheritanceUI.TestAccessor().Dynamic._toolTip;

        toolTip.Should().NotBeNull().And.BeOfType<ToolTip>();
        toolTip.GetToolTip(parentControl).Should().BeEmpty();
        toolTip.GetToolTip(sitedChildControl).Should().BeEmpty();
    }


}

#nullable disable
