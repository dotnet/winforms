// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DotNet.RemoteExecutor;

namespace System.Windows.Forms.PropertyGridInternal.Tests;

public class GridToolTipTests : IDisposable
{
    private const int ToolTipStandardPart = 1;
    private const int ToolTipStandardNormalState = 1;
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

    [WinFormsFact]
    public void GridToolTip_OnHandleCreated_DarkMode_UsesDarkModeExplorerTheme()
    {
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke(() =>
        {
            if (SystemInformation.HighContrast)
            {
                return;
            }

            Application.SetColorMode(SystemColorMode.Dark);

            using Button control = new();
            using GridToolTip toolTip = new([control]);
            using Form form = new();
            form.Controls.Add(control);
            form.Controls.Add(toolTip);

            form.Show();

            using PInvoke.OpenThemeDataScope actualTheme = new(toolTip.HWND, "TOOLTIP");
            actualTheme.IsNull.Should().BeFalse();

            using Control expectedThemeHost = new();
            _ = expectedThemeHost.Handle;
            PInvoke.SetWindowTheme(expectedThemeHost.HWND, "DarkMode_Explorer", null).Should().Be(HRESULT.S_OK);

            using PInvoke.OpenThemeDataScope expectedTheme = new(expectedThemeHost.HWND, "TOOLTIP");
            expectedTheme.IsNull.Should().BeFalse();

            PInvoke.GetThemeColor(
                actualTheme,
                ToolTipStandardPart,
                ToolTipStandardNormalState,
                THEME_PROPERTY_SYMBOL_ID.TMT_TEXTCOLOR,
                out COLORREF actualTextColor).Should().Be(HRESULT.S_OK);
            PInvoke.GetThemeColor(
                expectedTheme,
                ToolTipStandardPart,
                ToolTipStandardNormalState,
                THEME_PROPERTY_SYMBOL_ID.TMT_TEXTCOLOR,
                out COLORREF expectedTextColor).Should().Be(HRESULT.S_OK);

            actualTextColor.Should().Be(expectedTextColor);
        });

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }
}
