// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Tests;

public class WebBrowserContainerTests
{
    [WinFormsFact]
    public void AddControl_AddsControlToCache()
    {
        using WebBrowser parent = new();
        WebBrowserContainer container = new(parent);
        using Control control = new()
        {
            Name = "control1"
        };

        container.AddControl(control);

        Action act = () => container.AddControl(control);
        act.Should().Throw<ArgumentException>().Which
            .Message.Should().Contain(string.Format(SR.AXDuplicateControl, control.Name));
    }

    [WinFormsFact]
    public void RemoveControl_RemovesControlFromCache()
    {
        using WebBrowser parent = new();
        WebBrowserContainer container = new(parent);
        using Control control = new()
        {
            Name = "controlToRemove"
        };

        HashSet<Control> containerCache = container.TestAccessor.Dynamic._containerCache;

        container.AddControl(control);
        containerCache.Contains(control).Should().BeTrue();

        container.RemoveControl(control);
        containerCache.Contains(control).Should().BeFalse();
    }

    [WinFormsFact]
    public void FindContainerForControl_ReturnsExistingContainer_IfPresent()
    {
        using WebBrowser control = new();
        WebBrowserContainer expectedContainer = new(control);
        control._container = expectedContainer;

        WebBrowserContainer? result = WebBrowserContainer.FindContainerForControl(control);

        result.Should().BeSameAs(expectedContainer);
    }

    [WinFormsFact]
    public void FindContainerForControl_CreatesAndRegistersContainerFailed_IfNotPresent()
    {
        using WebBrowser control = new();
        using ContainerControl parent = new();
        parent.Controls.Add(control);
        control._container = null;

        WebBrowserContainer? result = WebBrowserContainer.FindContainerForControl(control);

        result.Should().BeNull();
    }

    [WinFormsFact]
    public void FindContainerForControl_ReturnsNull_IfControlIsNull()
    {
        WebBrowserContainer? result = WebBrowserContainer.FindContainerForControl(null!);
        result.Should().BeNull();
    }

    [WinFormsFact]
    public void FindContainerForControl_ReturnsNull_IfNoContainingControl()
    {
        using WebBrowser control = new();
        control._container = null;

        WebBrowserContainer? result = WebBrowserContainer.FindContainerForControl(control);

        result.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData("SiteName", "ControlName", "SiteName")]
    [InlineData(null, "ControlName", "ControlName")]
    [InlineData(null, null, "")]
    [InlineData("", "ControlName", "")]
    [InlineData("SiteName", null, "SiteName")]
    public void GetNameForControl_ReturnsExpectedName(string? siteName, string? controlName, string expected)
    {
        using Control control = new()
        {
            Name = controlName
        };

        if (siteName is not null)
        {
            Mock<ISite> mockSite = new Mock<ISite>();
            mockSite.Setup(s => s.Name).Returns(siteName);
            control.Site = mockSite.Object;
        }

        string result = WebBrowserContainer.GetNameForControl(control);

        result.Should().Be(expected);
    }
}
