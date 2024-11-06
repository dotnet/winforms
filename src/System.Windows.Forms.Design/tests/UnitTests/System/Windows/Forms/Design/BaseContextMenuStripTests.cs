// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class BaseContextMenuStripTests
{
    [Fact]
    public void RefreshItems_SetsFontFromIUIService()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<IUIService> uiServiceMock = new();
        Font expectedFont = new Font("Arial", 12.0f);
        uiServiceMock.Setup(uis => uis.Styles["DialogFont"]).Returns(expectedFont);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IUIService))).Returns(uiServiceMock.Object);

        using BaseContextMenuStrip contextMenuStrip = new(serviceProviderMock.Object);
        contextMenuStrip.Font = new Font("Times New Roman", 10.0f, FontStyle.Italic);
        for (int i = 0; i < 3; i++)
        {
            contextMenuStrip.Items.Add(new ToolStripMenuItem());
        }

        contextMenuStrip.RefreshItems();

        contextMenuStrip.Font.Should().Be(expectedFont);
        foreach (var item in contextMenuStrip.Items.OfType<ToolStripMenuItem>())
        {
            item.Font.Should().Be(expectedFont);
        }
    }
}
