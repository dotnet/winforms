// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class NotifyIconDesignerTests
{
    [Fact]
    public void Visible_WithDefaultNotifyIcon_ShouldBeTrue()
    {
        using NotifyIconDesigner notifyIconDesigner = new();
        using NotifyIcon icon = new();
        notifyIconDesigner.Initialize(icon);
        notifyIconDesigner.InitializeNewComponent(null);

        icon.Visible.Should().BeTrue();
    }

    [Fact]
    public void ActionLists_WithDefaultNotifyIcon_ShouldReturnExpectedValue()
    {
        using NotifyIconDesigner notifyIconDesigner = new();
        using NotifyIcon icon = new();
        notifyIconDesigner.Initialize(icon);
        notifyIconDesigner.ActionLists.Count.Should().Be(1);
    }
}
