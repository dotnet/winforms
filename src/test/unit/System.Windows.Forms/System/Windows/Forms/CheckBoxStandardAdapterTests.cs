// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.Tests;

public class CheckBoxStandardAdapterTests : IDisposable
{
    private CheckBox? _checkBox;

    public void Dispose()
    {
        _checkBox?.Dispose();

        // Restore default color mode after dark mode tests.
        Application.SetColorMode(SystemColorMode.Classic);
    }

    private (CheckBoxStandardAdapter, CheckBox) CreateAdapter(Appearance appearance = Appearance.Normal, bool enabled = true)
    {
        _checkBox = new CheckBox
        {
            Appearance = appearance,
            Enabled = enabled,
            Width = 100,
            Height = 25
        };

        CheckBoxStandardAdapter adapter = new(_checkBox);

        return (adapter, _checkBox);
    }

    [WinFormsFact]
    public void CreateButtonAdapter_InClassicMode_ReturnsButtonStandardAdapter()
    {
        Application.SetColorMode(SystemColorMode.Classic);
        (CheckBoxStandardAdapter adapter, _) = CreateAdapter();

        ButtonBaseAdapter result = adapter.TestAccessor.Dynamic.CreateButtonAdapter();

        result.Should().BeOfType<ButtonStandardAdapter>();
    }

    [WinFormsFact]
    public void CreateButtonAdapter_InDarkMode_ReturnsButtonDarkModeAdapter()
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        Application.SetColorMode(SystemColorMode.Dark);
        (CheckBoxStandardAdapter adapter, _) = CreateAdapter();

        ButtonBaseAdapter result = adapter.TestAccessor.Dynamic.CreateButtonAdapter();

        result.Should().BeOfType<ButtonDarkModeAdapter>();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, CheckState.Unchecked)]
    [InlineData(Appearance.Button, CheckState.Checked)]
    [InlineData(Appearance.Button, CheckState.Indeterminate)]
    [InlineData(Appearance.Normal, CheckState.Unchecked)]
    [InlineData(Appearance.Normal, CheckState.Checked)]
    [InlineData(Appearance.Normal, CheckState.Indeterminate)]
    public void PaintUp_InClassicMode_DoesNotThrow(Appearance appearance, CheckState checkState)
    {
        Application.SetColorMode(SystemColorMode.Classic);
        (CheckBoxStandardAdapter adapter, CheckBox checkBox) = CreateAdapter(appearance);
        checkBox.CheckState = checkState;

        using Bitmap bitmap = new(checkBox.Width, checkBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, checkBox.ClientRectangle);

        Exception? exception = Record.Exception(() => adapter.PaintUp(e, checkState));

        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, CheckState.Unchecked)]
    [InlineData(Appearance.Button, CheckState.Checked)]
    [InlineData(Appearance.Button, CheckState.Indeterminate)]
    [InlineData(Appearance.Normal, CheckState.Unchecked)]
    [InlineData(Appearance.Normal, CheckState.Checked)]
    [InlineData(Appearance.Normal, CheckState.Indeterminate)]
    public void PaintUp_InDarkMode_DoesNotThrow(Appearance appearance, CheckState checkState)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        Application.SetColorMode(SystemColorMode.Dark);
        (CheckBoxStandardAdapter adapter, CheckBox checkBox) = CreateAdapter(appearance);
        checkBox.CheckState = checkState;

        using Bitmap bitmap = new(checkBox.Width, checkBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, checkBox.ClientRectangle);

        Exception? exception = Record.Exception(() => adapter.PaintUp(e, checkState));

        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, CheckState.Unchecked)]
    [InlineData(Appearance.Button, CheckState.Checked)]
    [InlineData(Appearance.Normal, CheckState.Unchecked)]
    [InlineData(Appearance.Normal, CheckState.Checked)]
    public void PaintDown_InDarkMode_DoesNotThrow(Appearance appearance, CheckState checkState)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        Application.SetColorMode(SystemColorMode.Dark);
        (CheckBoxStandardAdapter adapter, CheckBox checkBox) = CreateAdapter(appearance);
        checkBox.CheckState = checkState;

        using Bitmap bitmap = new(checkBox.Width, checkBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, checkBox.ClientRectangle);

        Exception? exception = Record.Exception(() => adapter.PaintDown(e, checkState));

        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, CheckState.Unchecked)]
    [InlineData(Appearance.Button, CheckState.Checked)]
    [InlineData(Appearance.Normal, CheckState.Unchecked)]
    [InlineData(Appearance.Normal, CheckState.Checked)]
    public void PaintOver_InDarkMode_DoesNotThrow(Appearance appearance, CheckState checkState)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        Application.SetColorMode(SystemColorMode.Dark);
        (CheckBoxStandardAdapter adapter, CheckBox checkBox) = CreateAdapter(appearance);
        checkBox.CheckState = checkState;

        using Bitmap bitmap = new(checkBox.Width, checkBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, checkBox.ClientRectangle);

        Exception? exception = Record.Exception(() => adapter.PaintOver(e, checkState));

        exception.Should().BeNull();
    }
}
