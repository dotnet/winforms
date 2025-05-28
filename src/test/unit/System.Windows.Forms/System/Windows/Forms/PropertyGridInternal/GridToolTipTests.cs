// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal.Tests;

public class GridToolTipTests : IDisposable
{
    private readonly Control[] _controls;
    private readonly GridToolTip _toolTip;

    public GridToolTipTests()
    {
        _controls = [new Button(), new TextBox()];
        _toolTip = new GridToolTip(_controls);
    }

    public void Dispose()
    {
        foreach (Control c in _controls)
        {
            c.Dispose();
        }

        _toolTip.Dispose();
    }

    [Fact]
    public void GridToolTip_SetAndGetValue()
    {
        _toolTip.ToolTip = "Test tooltip";
        _toolTip.ToolTip.Should().Be("Test tooltip");

        _toolTip.ToolTip = "Another tooltip";
        _toolTip.ToolTip.Should().Be("Another tooltip");
    }

    [Fact]
    public void GridToolTip_SetNullValue()
    {
        _toolTip.ToolTip = null;
        _toolTip.ToolTip.Should().BeNull();
    }

    [Fact]
    public void GridToolTip_SetEmptyString()
    {
        _toolTip.ToolTip = string.Empty;
        _toolTip.ToolTip.Should().BeEmpty();
    }

    [Fact]
    public void GridToolTip_SetTooLong_Truncates()
    {
        string longText = new('a', 1005);
        _toolTip.ToolTip = longText;
        _toolTip.ToolTip.Should().EndWith("...");
        _toolTip.ToolTip.Length.Should().Be(1003);

        string text = new('b', 1000);
        _toolTip.ToolTip = text;
        _toolTip.ToolTip.Should().NotEndWith("...");
        _toolTip.ToolTip.Length.Should().Be(1000);
    }

    [Fact]
    public void GridToolTip_Reset()
    {
        _toolTip.ToolTip = "abc";
        _toolTip.Reset();
        _toolTip.ToolTip.Should().Be("abc");
    }

    [Fact]
    public void GridToolTip_OnHandleCreated_CallsSetupToolTip()
    {
        using Form form = new();
        foreach (Control c in _controls)
        {
            form.Controls.Add(c);
        }

        form.Controls.Add(_toolTip);

        _toolTip.IsHandleCreated.Should().BeFalse();

        form.Show();
        _toolTip.IsHandleCreated.Should().BeTrue();
    }
}
