// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public class DesignerVerbToolStripMenuItemTests
{
    [Fact]
    public void Constructor_InitializesTextProperty()
    {
        DesignerVerb verb = new("TestVerb", (sender, e) => { });
        DesignerVerbToolStripMenuItem item = new(verb);

        item.Text.Should().Be("TestVerb");
        item.Enabled.Should().BeTrue();
        item.Checked.Should().BeFalse();
    }

    [Fact]
    public void RefreshItem_UpdatesProperties()
    {
        DesignerVerb verb = new("TestVerb", (sender, e) => { })
        {
            Enabled = false,
            Checked = true
        };

        DesignerVerbToolStripMenuItem item = new(verb);
        item.RefreshItem();

        item.Enabled.Should().BeFalse();
        item.Checked.Should().BeTrue();
    }

    [Fact]
    public void OnClick_InvokesDesignerVerb()
    {
        bool invoked = false;
        DesignerVerb verb = new("TestVerb", (sender, e) => invoked = true);
        DesignerVerbToolStripMenuItem item = new(verb);
        item.PerformClick();

        invoked.Should().BeTrue();
    }
}
