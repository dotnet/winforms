using Moq;
using System.ComponentModel;
using System.ComponentModel.Design;
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
}

