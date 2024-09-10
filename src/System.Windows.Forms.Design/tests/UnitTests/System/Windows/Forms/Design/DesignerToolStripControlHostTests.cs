// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class DesignerToolStripControlHostTests
{
    [Fact]
    public void Constructor_SetsMarginToPaddingEmpty()
    {
        using Control control = new();

        using DesignerToolStripControlHost host = new(control);

        host.Margin.Should().Be(Padding.Empty);
        host.Control.Should().NotBeNull();
        host.Control.Should().Be(control);
    }
}
