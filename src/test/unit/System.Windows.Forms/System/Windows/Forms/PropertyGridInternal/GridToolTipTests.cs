// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal.Tests;

public class GridToolTipTests : IDisposable
{
    private readonly Control[] _controls;
    private readonly GridToolTip _toolTip;
    private const int MaximumToolTipLength = 1000;

    public GridToolTipTests()
    {
        _controls = [new Button(), new TextBox()];
        _toolTip = new(_controls);
    }

    public void Dispose()
    {
        foreach (Control c in _controls)
        {
            c.Dispose();
        }

        _toolTip.Dispose();
    }

    [WinFormsFact]
    public void GridToolTip_SetAndGetValue()
    {
        _toolTip.ToolTip = "Test tooltip";

        _toolTip.ToolTip.Should().Be("Test tooltip");

        _toolTip.ToolTip = "Another tooltip";

        _toolTip.ToolTip.Should().Be("Another tooltip");
    }

    [WinFormsFact]
    public void GridToolTip_SetNullValue()
    {
        _toolTip.ToolTip = null;

        _toolTip.ToolTip.Should().BeNull();
    }

    [WinFormsFact]
    public void GridToolTip_SetEmptyString()
    {
        _toolTip.ToolTip = string.Empty;

        _toolTip.ToolTip.Should().BeEmpty();
    }

    [WinFormsFact]
    public void GridToolTip_SetTooLong_Truncates()
    {
        _toolTip.ToolTip = new('a', MaximumToolTipLength + 5);

        _toolTip.ToolTip.Should().EndWith("...");
        _toolTip.ToolTip.Length.Should().Be(MaximumToolTipLength + 3);

        _toolTip.ToolTip = new('b', MaximumToolTipLength);

        _toolTip.ToolTip.Should().NotEndWith("...");
        _toolTip.ToolTip.Length.Should().Be(MaximumToolTipLength);
    }

    [WinFormsFact]
    public void GridToolTip_Reset()
    {
        _toolTip.ToolTip = "abc";
        _toolTip.Reset();

        _toolTip.ToolTip.Should().Be("abc");
    }

    [WinFormsFact]
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
